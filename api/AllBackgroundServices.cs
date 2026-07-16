using data;
using data._Product;
using data._Shared;
using data._System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace api
{
    public record ExchangeRateApiResponse(string Base_code, Dictionary<string, decimal> Rates);

    public class AllBackgroundServices : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IDbContextFactory<_ApplicationConnectionDb> _dbFactory;
        private readonly ILogger<AllBackgroundServices> _logger;

        private static readonly HttpClient _httpClient = new HttpClient();

        // ════════════════════════════════════════════════════════════
        //  AYARLAR — sıklık, izin ve limit değerleri artık VERİTABANINDAN
        //  (AllBackgroundServicesFrequencyRate, Id = 1) okunur.
        //  Ayarları AllBackgroundServicesTasks.razor sayfasından değiştirin.
        //
        //  Aşağıdaki sabitler yalnızca DB'de kayıt bulunamadığında veya
        //  değerler geçersiz olduğunda devreye giren GÜVENLİ VARSAYILANLARDIR.
        // ════════════════════════════════════════════════════════════

        // Ayar tablosu okunamadığında bir sonraki denemeye kadar beklenecek süre
        private static readonly TimeSpan SettingsReadFallbackInterval = TimeSpan.FromSeconds(10);

        // Minimum aralık koruması — 0/negatif değer CPU'yu kilitlemesin
        private static readonly TimeSpan MinimumInterval = TimeSpan.FromSeconds(1);

        // DB'den okunan varsayılan aralık (kayıt yoksa)
        private static readonly TimeSpan DefaultInterval = TimeSpan.FromSeconds(10);

        // AI retry üst limiti için varsayılan (DB'de yoksa)
        private const int DefaultMaxAiRetry = 3;

        // ════════════════════════════════════════════════════════════

        public AllBackgroundServices(
            IServiceScopeFactory scopeFactory,
            IDbContextFactory<_ApplicationConnectionDb> dbFactory,
            ILogger<AllBackgroundServices> logger)
        {
            _scopeFactory = scopeFactory;
            _dbFactory = dbFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken durdurmaSinyali)
        {
            _logger.LogInformation("AllBackgroundServices başlatıldı (Kur + AI Ürün İçerik).");

            // Her görev kendi bağımsız döngüsünde çalışır — biri diğerini bekletmez, kilitlenme olmaz.
            var exchangeRateLoop = RunLoopAsync(
                "Döviz Kuru",
                FetchAndSaveExchangeRatesAsync,
                durdurmaSinyali);

            var aiProductLoop = RunLoopAsync(
                "AI Ürün İçerik",
                ProcessNextAiProductAsync,
                durdurmaSinyali);

            await Task.WhenAll(exchangeRateLoop, aiProductLoop);

            _logger.LogInformation("AllBackgroundServices Durduruldu.");
        }

        /// <summary>
        /// Verilen görevi, her tick'te VERİTABANINDAN taze okunan ayarlara göre
        /// (izin + aralık) çalıştırır. Bir görevdeki gecikme/hata diğerlerini ETKİLEMEZ.
        /// Ayarlar canlı okunduğu için servis yeniden başlatmadan değişiklikler uygulanır.
        /// </summary>
        private async Task RunLoopAsync(
            string taskName,
            Func<AllBackgroundServicesFrequencyRate, CancellationToken, Task> job,
            CancellationToken durdurmaSinyali)
        {
            try
            {
                while (!durdurmaSinyali.IsCancellationRequested)
                {
                    // ── Her turda ayarları taze oku ──
                    var settings = await LoadSettingsAsync(durdurmaSinyali);

                    var (isEnabled, interval) = ResolveTaskConfig(taskName, settings);

                    // Bir sonraki tick'e kadar bekle
                    await Task.Delay(interval, durdurmaSinyali);

                    // İzin kapalıysa görevi çalıştırmadan sadece bekle
                    if (!isEnabled) continue;

                    await job(settings!, durdurmaSinyali);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("{Task} görevi durdurma sinyali aldı.", taskName);
            }
        }

        /// <summary>
        /// Görev adına göre ilgili izin + aralık değerini ayarlardan çözer.
        /// Ayar yoksa güvenli varsayılanlara düşer.
        /// </summary>
        private (bool isEnabled, TimeSpan interval) ResolveTaskConfig(
            string taskName,
            AllBackgroundServicesFrequencyRate? settings)
        {
            if (settings == null)
                return (false, SettingsReadFallbackInterval);

            return taskName switch
            {
                "Döviz Kuru" => (
                    settings.IsCurrencyFetchEnabled,
                    NormalizeInterval(settings.CurrencyFetchIntervalInSeconds)),

                "AI Ürün İçerik" => (
                    settings.IsAiContentGenerationEnabled,
                    NormalizeInterval(settings.AiContentGenerationIntervalInSeconds)),

                _ => (false, SettingsReadFallbackInterval)
            };
        }

        /// <summary>
        /// Saniye değerini güvenli TimeSpan'e çevirir; 0/negatif değerleri minimuma sabitler.
        /// </summary>
        private static TimeSpan NormalizeInterval(int seconds)
        {
            if (seconds <= 0) return DefaultInterval;
            var span = TimeSpan.FromSeconds(seconds);
            return span < MinimumInterval ? MinimumInterval : span;
        }

        /// <summary>
        /// Ayar tablosunun tek satırını (Id = 1) kısa ömürlü context ile okur.
        /// </summary>
        private async Task<AllBackgroundServicesFrequencyRate?> LoadSettingsAsync(CancellationToken cancellationToken)
        {
            try
            {
                await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

                return await db.Set<AllBackgroundServicesFrequencyRate>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == 1, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Arka plan servis ayarları okunamadı (Id = 1).");
                return null;
            }
        }

        // ──────────────────────────────────────────────
        //  DÖVİZ KURU
        // ──────────────────────────────────────────────
        private async Task FetchAndSaveExchangeRatesAsync(
            AllBackgroundServicesFrequencyRate settings,
            CancellationToken cancellationToken)
        {
            try
            {
                string apiUrl = "https://open.er-api.com/v6/latest/USD";

                var response = await _httpClient.GetFromJsonAsync<ExchangeRateApiResponse>(apiUrl, cancellationToken);

                if (response?.Rates != null)
                {
                    await using var context = await _dbFactory.CreateDbContextAsync(cancellationToken);

                    var newRate = new MoneyExchangeRate
                    {
                        Dolar_Usd = response.Rates.GetValueOrDefault("USD", 1m),
                        Euro_Eur = response.Rates.GetValueOrDefault("EUR", 0m),
                        Manat_Azn = response.Rates.GetValueOrDefault("AZN", 0m),
                        Lira_Tl = response.Rates.GetValueOrDefault("TRY", 0m),
                        CreatedAt = DateTime.Now
                    };

                    context.Set<MoneyExchangeRate>().Add(newRate);
                    await context.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation(
                        "Kurlar kaydedildi → EUR: {Eur} | AZN: {Azn} | TRY: {Try}",
                        newRate.Euro_Eur, newRate.Manat_Azn, newRate.Lira_Tl);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kur verileri çekilirken hata oluştu.");
            }
        }

        // ──────────────────────────────────────────────
        //  AI ÜRÜN İÇERİK ÜRETİMİ (Tick başına maks 1 ürün)
        // ──────────────────────────────────────────────
        private async Task ProcessNextAiProductAsync(
            AllBackgroundServicesFrequencyRate settings,
            CancellationToken cancellationToken)
        {
            // Retry limitini ayardan al (yoksa varsayılan)
            int maxAiRetry = settings.AiContentGenerationIntervalMaxAiRetry > 0
                ? settings.AiContentGenerationIntervalMaxAiRetry
                : DefaultMaxAiRetry;

            try
            {
                // ── 1) Sıradaki uygun ürünü bul (kısa ömürlü read context) ──
                Guid? targetProductId = null;
                string? productName = null;

                await using (var readDb = await _dbFactory.CreateDbContextAsync(cancellationToken))
                {
                    var candidate = await readDb.Set<Products>()
                        .AsNoTracking()
                        .Where(p => p.IsAiManaged == true
                                 && p.AiContentStatus == null
                                 && p.AiRetryCount < maxAiRetry
                                 && (p.IsDeleted == null || p.IsDeleted.IsDeletedStatu == false))
                        .OrderBy(p => p.CreatedAt)          // FIFO — en eski bekleyen önce
                        .Select(p => new { p.Id, p.Name })
                        .FirstOrDefaultAsync(cancellationToken);

                    if (candidate == null) return;          // Kuyrukta iş yok, sessizce çık

                    targetProductId = candidate.Id;
                    productName = candidate.Name;
                }

                _logger.LogInformation("AI içerik üretimi başlıyor → Ürün: {Name} ({Id})", productName, targetProductId);

                // ── 2) Scoped servis üzerinden AI çağrısı ──
                using var scope = _scopeFactory.CreateScope();
                var aiGenerator = scope.ServiceProvider.GetRequiredService<AIProductKnowledgeGenerator>();

                var seoData = await aiGenerator.GenerateSeoContentAsync(productName ?? "");

                // ── 3) Sonucu kaydet (kısa ömürlü write context) ──
                await using var writeDb = await _dbFactory.CreateDbContextAsync(cancellationToken);

                var product = await writeDb.Set<Products>()
                    .FirstOrDefaultAsync(p => p.Id == targetProductId, cancellationToken);

                if (product == null) return;

                if (seoData != null)
                {
                    // Orijinal değerleri yedekle (sadece ilk seferde — daha önce yedeklenmemişse)
                    if (string.IsNullOrEmpty(product.AiOriginalName))
                    {
                        product.AiOriginalName = product.Name;
                        product.AiOriginalShortDescription = product.ShortDescription;
                        product.AiOriginalFullDescription = product.FullDescription;
                        product.AiOriginalTags = product.Tags;
                    }

                    // AI çıktısını ürüne yaz
                    product.Name = seoData.Title;
                    product.ShortDescription = seoData.ShortDesc;
                    product.FullDescription = seoData.LongDesc;
                    product.Tags = seoData.Keywords;

                    product.AiContentStatus = true;         // Başarılı
                    product.AiErrorMessage = null;
                    product.AiProcessedAt = DateTime.UtcNow;
                    product.UpdatedAt = DateTime.UtcNow;

                    _logger.LogInformation("AI içerik başarıyla yazıldı → {Id}", targetProductId);
                }
                else
                {
                    // AI null döndü — başarısız
                    product.AiRetryCount += 1;
                    product.AiContentStatus = product.AiRetryCount >= maxAiRetry ? false : null;
                    product.AiErrorMessage = "AI servisi boş yanıt döndürdü.";
                    product.UpdatedAt = DateTime.UtcNow;

                    _logger.LogWarning(
                        "AI boş yanıt döndü → {Id} (Deneme: {Retry}/{Max})",
                        targetProductId, product.AiRetryCount, maxAiRetry);
                }

                await writeDb.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI ürün içerik üretiminde beklenmeyen hata.");

                // Hata durumunda da retry sayacını artır
                try
                {
                    await IncrementRetryOnErrorAsync(ex.Message, maxAiRetry, cancellationToken);
                }
                catch (Exception innerEx)
                {
                    _logger.LogError(innerEx, "Retry sayacı güncellenirken ikincil hata.");
                }
            }
        }

        /// <summary>
        /// Exception fırladığında ilgili ürünün retry sayacını artırır.
        /// targetProductId scope dışında kaybolduğu için aynı sorguyu tekrarlar.
        /// </summary>
        private async Task IncrementRetryOnErrorAsync(string errorMessage, int maxAiRetry, CancellationToken cancellationToken)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

            var stuck = await db.Set<Products>()
                .Where(p => p.IsAiManaged == true
                         && p.AiContentStatus == null
                         && p.AiRetryCount < maxAiRetry)
                .OrderBy(p => p.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (stuck == null) return;

            stuck.AiRetryCount += 1;
            stuck.AiErrorMessage = errorMessage.Length > 500 ? errorMessage[..500] : errorMessage;
            stuck.AiContentStatus = stuck.AiRetryCount >= maxAiRetry ? false : null;
            stuck.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync(cancellationToken);
        }
    }
}