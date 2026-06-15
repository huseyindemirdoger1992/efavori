using data;
using data._Product;
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
        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(10);

        // AI retry üst limiti — bu sayıya ulaşan ürün tekrar denenmez
        private const int MaxAiRetry = 3;

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
            _logger.LogInformation("AllBackgroundServices başlatıldı (10 sn aralık — Kur + AI Ürün İçerik).");

            using var timer = new PeriodicTimer(Interval);

            try
            {
                while (await timer.WaitForNextTickAsync(durdurmaSinyali))
                {
                    // Her iki görevi paralel değil sıralı çalıştırıyoruz — DB baskısını düşük tutmak için
                    await FetchAndSaveExchangeRatesAsync(durdurmaSinyali);
                    await ProcessNextAiProductAsync(durdurmaSinyali);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Arka plan görevi durdurma sinyali aldı.");
            }

            _logger.LogInformation("AllBackgroundServices Durduruldu.");
        }

        // ──────────────────────────────────────────────
        //  DÖVİZ KURU
        // ──────────────────────────────────────────────
        private async Task FetchAndSaveExchangeRatesAsync(CancellationToken cancellationToken)
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
        private async Task ProcessNextAiProductAsync(CancellationToken cancellationToken)
        {
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
                                 && p.AiRetryCount < MaxAiRetry
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
                    product.AiContentStatus = product.AiRetryCount >= MaxAiRetry ? false : null;
                    product.AiErrorMessage = "AI servisi boş yanıt döndürdü.";
                    product.UpdatedAt = DateTime.UtcNow;

                    _logger.LogWarning(
                        "AI boş yanıt döndü → {Id} (Deneme: {Retry}/{Max})",
                        targetProductId, product.AiRetryCount, MaxAiRetry);
                }

                await writeDb.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI ürün içerik üretiminde beklenmeyen hata.");

                // Hata durumunda da retry sayacını artır
                try
                {
                    await IncrementRetryOnErrorAsync(ex.Message, cancellationToken);
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
        private async Task IncrementRetryOnErrorAsync(string errorMessage, CancellationToken cancellationToken)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

            var stuck = await db.Set<Products>()
                .Where(p => p.IsAiManaged == true
                         && p.AiContentStatus == null
                         && p.AiRetryCount < MaxAiRetry)
                .OrderBy(p => p.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (stuck == null) return;

            stuck.AiRetryCount += 1;
            stuck.AiErrorMessage = errorMessage.Length > 500 ? errorMessage[..500] : errorMessage;
            stuck.AiContentStatus = stuck.AiRetryCount >= MaxAiRetry ? false : null;
            stuck.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
