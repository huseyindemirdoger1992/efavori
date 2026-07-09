using api.tr.AI;
using data;
using data._Product;
using data._Shared;
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
        //  AYARLAR — sıklık ve limit değerleri buradan yönetilir.
        //  Her görev KENDİ aralığında, birbirinden bağımsız çalışır.
        //  İstediğiniz süreyi doğrudan buraya yazın.
        // ════════════════════════════════════════════════════════════

        // Döviz kuru çekme sıklığı
        private static readonly TimeSpan ExchangeRateInterval = TimeSpan.FromMinutes(1);

        // AI ürün içerik üretimi — tetiklenme sıklığı
        private static readonly TimeSpan AiProductInterval = TimeSpan.FromSeconds(10);

        // AI ürün çevirisi — tetiklenme sıklığı (her 10 sn'de 1 ürün)
        private static readonly TimeSpan AiTranslationInterval = TimeSpan.FromSeconds(10);

        // Hedef çeviri dili
        private const string TargetTranslationLanguage = "en";

        // AI retry üst limiti (bu sayıya ulaşan kayıt tekrar denenmez)
        private const int MaxAiRetry = 3;

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
            _logger.LogInformation("AllBackgroundServices başlatıldı (Kur + AI Ürün İçerik + AI Çeviri).");

            // Her görev kendi bağımsız döngüsünde çalışır — biri diğerini bekletmez, kilitlenme olmaz.
            var exchangeRateLoop = RunLoopAsync(
                "Döviz Kuru",
                ExchangeRateInterval,
                FetchAndSaveExchangeRatesAsync,
                durdurmaSinyali);

            var aiProductLoop = RunLoopAsync(
                "AI Ürün İçerik",
                AiProductInterval,
                ProcessNextAiProductAsync,
                durdurmaSinyali);

            var aiTranslationLoop = RunLoopAsync(
                "AI Çeviri",
                AiTranslationInterval,
                ProcessNextAiTranslationAsync,
                durdurmaSinyali);

            await Task.WhenAll(exchangeRateLoop, aiProductLoop, aiTranslationLoop);

            _logger.LogInformation("AllBackgroundServices Durduruldu.");
        }

        /// <summary>
        /// Verilen görevi kendi aralığında sonsuz döngüde çalıştırır.
        /// Bir görevdeki gecikme/hata diğer görevleri ETKİLEMEZ.
        /// </summary>
        private async Task RunLoopAsync(
            string taskName,
            TimeSpan interval,
            Func<CancellationToken, Task> job,
            CancellationToken durdurmaSinyali)
        {
            using var timer = new PeriodicTimer(interval);

            try
            {
                while (await timer.WaitForNextTickAsync(durdurmaSinyali))
                {
                    await job(durdurmaSinyali);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("{Task} görevi durdurma sinyali aldı.", taskName);
            }
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

        // ──────────────────────────────────────────────
        //  AI ÜRÜN ÇEVİRİSİ  (tr → en, Tick başına maks 1 ürün)
        //
        //  Akış:
        //   1) ProductTranslations tablosunda "tr" olup, aynı ProductId için
        //      henüz "en" kaydı bulunmayan (veya hata alıp retry hakkı kalan)
        //      bir kayıt bul.
        //   2) tr içerikleri AI ile en'e çevir.
        //   3) Çeviriyi AYNI ProductId ile yeni bir "en" ProductTranslations
        //      kaydı olarak yaz (veya var olan hatalı en kaydını güncelle).
        // ──────────────────────────────────────────────
        private async Task ProcessNextAiTranslationAsync(CancellationToken cancellationToken)
        {
            Guid? sourceTranslationId = null;

            try
            {
                // ── 1) Sıradaki tr kaydını bul (kısa ömürlü read context) ──
                Guid productId = Guid.Empty;
                string? srcName = null, srcShort = null, srcFull = null, srcTags = null;
                Guid? existingEnId = null;   // varsa güncellenecek hatalı en kaydı

                await using (var readDb = await _dbFactory.CreateDbContextAsync(cancellationToken))
                {
                    // Aynı ProductId için zaten başarılı bir "en" kaydı olanları hariç tut.
                    var candidate = await readDb.Set<ProductTranslations>()
                        .AsNoTracking()
                        .Where(t => t.LanguageCode == "tr"
                                 && (t.IsDeleted == null || t.IsDeleted.IsDeletedStatu == false)
                                 // Bu ürün için başarılı bir en kaydı YOK
                                 && !readDb.Set<ProductTranslations>().Any(e =>
                                        e.ProductId == t.ProductId
                                     && e.LanguageCode == TargetTranslationLanguage
                                     && (e.IsDeleted == null || e.IsDeleted.IsDeletedStatu == false)
                                     && (e.AiTranslationStatus == true || e.IsManuallyEdited == true))
                                 // Retry limitini aşmış (kalıcı hatalı) en kaydı da YOK
                                 && !readDb.Set<ProductTranslations>().Any(e =>
                                        e.ProductId == t.ProductId
                                     && e.LanguageCode == TargetTranslationLanguage
                                     && (e.IsDeleted == null || e.IsDeleted.IsDeletedStatu == false)
                                     && e.AiRetryCount >= MaxAiRetry))
                        .OrderBy(t => t.CreatedAt)          // FIFO
                        .Select(t => new
                        {
                            t.Id,
                            t.ProductId,
                            t.Name,
                            t.ShortDescription,
                            t.FullDescription,
                            t.Tags
                        })
                        .FirstOrDefaultAsync(cancellationToken);

                    if (candidate == null) return;          // Çevrilecek iş yok, sessizce çık

                    sourceTranslationId = candidate.Id;
                    productId = candidate.ProductId;
                    srcName = candidate.Name;
                    srcShort = candidate.ShortDescription;
                    srcFull = candidate.FullDescription;
                    srcTags = candidate.Tags;

                    // Bu ürüne ait, hata almış (retry hakkı kalan) mevcut en kaydı var mı?
                    existingEnId = await readDb.Set<ProductTranslations>()
                        .Where(e => e.ProductId == productId
                                 && e.LanguageCode == TargetTranslationLanguage
                                 && (e.IsDeleted == null || e.IsDeleted.IsDeletedStatu == false))
                        .OrderBy(e => e.CreatedAt)
                        .Select(e => (Guid?)e.Id)
                        .FirstOrDefaultAsync(cancellationToken);
                }

                _logger.LogInformation(
                    "AI çeviri başlıyor → ProductId: {Pid} (tr kaynak: {Tid}) → {Lang}",
                    productId, sourceTranslationId, TargetTranslationLanguage);

                // ── 2) Scoped servis üzerinden AI çeviri çağrısı ──
                using var scope = _scopeFactory.CreateScope();
                var translator = scope.ServiceProvider.GetRequiredService<AIProductTranslationGeneratorTurkishToEnglish>();

                var translated = await translator.TranslateAsync(
                    TargetTranslationLanguage,
                    srcName, srcShort, srcFull, srcTags);

                // ── 3) Sonucu kaydet (kısa ömürlü write context) ──
                await using var writeDb = await _dbFactory.CreateDbContextAsync(cancellationToken);

                // Var olan en kaydını çek, yoksa yeni oluştur (AYNI ProductId).
                ProductTranslations? enRecord = null;
                if (existingEnId.HasValue)
                {
                    enRecord = await writeDb.Set<ProductTranslations>()
                        .FirstOrDefaultAsync(e => e.Id == existingEnId.Value, cancellationToken);
                }

                bool isNew = enRecord == null;
                if (isNew)
                {
                    enRecord = new ProductTranslations
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,                 // ← tr ürünüyle AYNI ProductId
                        LanguageCode = TargetTranslationLanguage,
                        CreatedAt = DateTime.UtcNow
                    };
                }

                if (translated != null)
                {
                    // Satıcı elle düzenlediyse üzerine YAZMA (güvenlik kontrolü)
                    if (!isNew && enRecord!.IsManuallyEdited)
                    {
                        _logger.LogInformation("en kaydı elle düzenlenmiş, atlanıyor → {Pid}", productId);
                        return;
                    }

                    enRecord!.Name = translated.Name;
                    enRecord.ShortDescription = translated.ShortDescription;
                    enRecord.FullDescription = translated.FullDescription;
                    enRecord.Tags = translated.Tags;

                    enRecord.IsAiTranslated = true;
                    enRecord.AiTranslationStatus = true;        // Başarılı
                    enRecord.AiErrorMessage = null;
                    enRecord.AiProcessedAt = DateTime.UtcNow;
                    enRecord.UpdatedAt = DateTime.UtcNow;

                    if (isNew) writeDb.Set<ProductTranslations>().Add(enRecord);

                    _logger.LogInformation("AI çeviri başarıyla yazıldı → ProductId: {Pid}", productId);
                }
                else
                {
                    // AI null döndü — başarısız
                    enRecord!.AiRetryCount += 1;
                    enRecord.IsAiTranslated = false;
                    enRecord.AiTranslationStatus = enRecord.AiRetryCount >= MaxAiRetry ? false : null;
                    enRecord.AiErrorMessage = "AI çeviri servisi boş yanıt döndürdü.";
                    enRecord.UpdatedAt = DateTime.UtcNow;

                    if (isNew) writeDb.Set<ProductTranslations>().Add(enRecord);

                    _logger.LogWarning(
                        "AI çeviri boş yanıt döndü → ProductId: {Pid} (Deneme: {Retry}/{Max})",
                        productId, enRecord.AiRetryCount, MaxAiRetry);
                }

                await writeDb.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI çeviride beklenmeyen hata.");
            }
        }
    }
}