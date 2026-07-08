using data;
using data._Product;
using data.Articles;
using data._Shared;
using api.tr.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
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
        //  AYARLAR — tüm sıklık ve limit değerleri buradan yönetilir.
        //  Başka hiçbir yerde bu değerleri elle değiştirmeyin.
        // ════════════════════════════════════════════════════════════

        // Ana döngü tetiklenme sıklığı (döviz kuru + AI ürün içerik burada kontrol edilir)
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

        // AI ürün içerik üretimi — retry üst limiti (bu sayıya ulaşan ürün tekrar denenmez)
        private const int MaxAiRetry = 3;

        // AI makale içerik üretimi — tetiklenme sıklığı ve retry üst limiti
        private static readonly TimeSpan ArticleAiInterval = TimeSpan.FromMinutes(30);
        private const int MaxArticleAiRetry = 3;

        // Her makale üretiminden sonra kuyruğa eklenecek yeni başlıkta ZORUNLU olarak yer alacak telefon numarası.
        // Numarayı değiştirmek isterseniz SADECE burayı güncelleyin — başka hiçbir yerde sabit numara yoktur.
        private const string ContactPhoneNumber = "+905302342580";

        // Yeni başlık üretiminde dönülecek bölgeler (Düzce merkez + ilçeleri)
        private static readonly string[] DuzceBolgeleri =
        {
            "Düzce Merkez", "Akçakoca", "Cumayeri", "Çilimli", "Gölyaka", "Gümüşova", "Kaynaşlı", "Yığılca"
        };

        // Yeni başlık üretiminde dönülecek hizmet kategorileri
        private static readonly string[] HizmetKategorileri =
        {
            "Boya ve badana",
            "Kırım ve yıkım işleri",
            "Tamirat ve tadilat",
            "Su tesisatı",
            "Su kaçağı tespiti ve tamiri"
        };

        // ════════════════════════════════════════════════════════════

        private DateTime _lastArticleAiRunUtc = DateTime.MinValue;

        // Bölge/hizmet kombinasyonlarını sırayla (round-robin) dolaşmak için sayaç.
        // Rastgele seçim yerine sıralı dönüş kullanılıyor ki tüm kombinasyonlar dengeli şekilde kapsansın.
        private int _titleRotationCounter = 0;

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
            _logger.LogInformation("AllBackgroundServices başlatıldı (10 sn aralık — Kur + AI Ürün İçerik + AI Makale İçerik).");

            using var timer = new PeriodicTimer(Interval);

            try
            {
                while (await timer.WaitForNextTickAsync(durdurmaSinyali))
                {
                    // Görevleri paralel değil sıralı çalıştırıyoruz — DB baskısını düşük tutmak için
                    await FetchAndSaveExchangeRatesAsync(durdurmaSinyali);
                    await ProcessNextAiProductAsync(durdurmaSinyali);

                    // 10 sn'lik ana döngüyü bozmadan, 30 dakikada bir makale üretimini tetikle
                    if (DateTime.UtcNow - _lastArticleAiRunUtc >= ArticleAiInterval)
                    {
                        _lastArticleAiRunUtc = DateTime.UtcNow;
                        await ProcessNextAiArticleAsync(durdurmaSinyali);
                    }
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

        // ──────────────────────────────────────────────
        //  AI MAKALE İÇERİK ÜRETİMİ (30 dk'da 1 başlık)
        // ──────────────────────────────────────────────
        private async Task ProcessNextAiArticleAsync(CancellationToken cancellationToken)
        {
            try
            {
                // ── 1) Sıradaki uygun başlığı bul ──
                Guid? targetTitleId = null;
                string? titleText = null;

                await using (var readDb = await _dbFactory.CreateDbContextAsync(cancellationToken))
                {
                    var candidate = await readDb.Set<AiTitlesForArticle>()
                        .AsNoTracking()
                        .Where(t => (t.AiIsOk == false || t.AiIsOk == null)
                                 && t.AiRetryCount < MaxArticleAiRetry
                                 && (t.IsDeleted == null || t.IsDeleted.IsDeletedStatu == false))
                        .OrderBy(t => t.CreateDate)          // FIFO — en eski bekleyen önce
                        .Select(t => new { t.Id, t.Title })
                        .FirstOrDefaultAsync(cancellationToken);

                    if (candidate == null) return;           // Kuyrukta iş yok, sessizce çık

                    targetTitleId = candidate.Id;
                    titleText = candidate.Title;
                }

                _logger.LogInformation("AI makale üretimi başlıyor → Başlık: {Title} ({Id})", titleText, targetTitleId);

                // ── 2) Scoped servis üzerinden AI çağrısı ──
                using var scope = _scopeFactory.CreateScope();
                var aiCreator = scope.ServiceProvider.GetRequiredService<AIPostContentCreator>();

                var result = await aiCreator.GenerateArticleContentAsync(titleText ?? "", cancellationToken);

                // ── 3) Sonucu kaydet ──
                await using var writeDb = await _dbFactory.CreateDbContextAsync(cancellationToken);

                var titleEntity = await writeDb.Set<AiTitlesForArticle>()
                    .FirstOrDefaultAsync(t => t.Id == targetTitleId, cancellationToken);

                if (titleEntity == null) return;

                if (result.Success && result.Data != null)
                {
                    var seoData = result.Data;
                    string slug = GenerateSlug(seoData.Title);

                    var article = new Article
                    {
                        Id = Guid.NewGuid(),
                        IsUser = null,                       // AI tarafından üretildi
                        UserStoreId = null,
                        CategoriId = null,
                        FeaturedImage = null,
                        Title = seoData.Title,
                        ShotDescription = seoData.ShortDescription,
                        ArticleLognDescription = seoData.ContentHtml,
                        SourceAiTitleId = titleEntity.Id,
                        Meta = new Meta
                        {
                            MetaTitle = seoData.MetaTitle,
                            MetaDescription = seoData.MetaDescription,
                            FocusKeywords = seoData.FocusKeywords,
                            // NOT: Domain/route yapınız _Viewer.cs'teki canonical mantığıyla aynı olmalı — burada geçici bir varsayım kullanıldı, gerekirse güncelleyin.
                            CanonicalUrl = $"https://efavori.com/makale/{slug}",
                            OgType = "article",
                            RobotsIndex = "index, follow"
                        },
                        Interaction = new InteractionCounts
                        {
                            ViewCount = 0,
                            ShareCount = 0,
                            RecommendCount = 0
                        },
                        IsDeleted = new IsDeleted()
                    };

                    writeDb.Set<Article>().Add(article);

                    titleEntity.AiIsOk = true;               // Başarılı
                    titleEntity.AiErrorMessage = null;
                    titleEntity.AiProcessedAt = DateTime.UtcNow;

                    _logger.LogInformation("AI makale başarıyla üretildi ve kaydedildi → {ArticleId} (Kaynak başlık: {TitleId})", article.Id, targetTitleId);

                    // ── 4) Kuyruğu besle: yeni bir başlık üret ve ekle (kuyruk kendi kendini besler) ──
                    await GenerateAndQueueNextTitleAsync(aiCreator, writeDb, cancellationToken);
                }
                else
                {
                    // Artık GERÇEK sebep elimizde — genel/anlamsız bir mesaj yerine bunu kaydediyoruz
                    string reason = result.FailureReason ?? "Bilinmeyen hata (sebep döndürülmedi).";

                    titleEntity.AiRetryCount += 1;
                    titleEntity.AiIsOk = titleEntity.AiRetryCount >= MaxArticleAiRetry ? false : null;
                    titleEntity.AiErrorMessage = reason.Length > 500 ? reason[..500] : reason;
                    titleEntity.AiProcessedAt = DateTime.UtcNow;

                    _logger.LogWarning(
                        "AI makale üretimi başarısız → {Id} (Deneme: {Retry}/{Max}) | Sebep: {Reason}",
                        targetTitleId, titleEntity.AiRetryCount, MaxArticleAiRetry, reason);
                }

                await writeDb.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI makale üretiminde beklenmeyen hata.");

                try
                {
                    await IncrementArticleRetryOnErrorAsync(ex.Message, cancellationToken);
                }
                catch (Exception innerEx)
                {
                    _logger.LogError(innerEx, "Makale retry sayacı güncellenirken ikincil hata.");
                }
            }
        }

        /// <summary>
        /// Exception fırladığında ilgili başlığın retry sayacını artırır.
        /// </summary>
        private async Task IncrementArticleRetryOnErrorAsync(string errorMessage, CancellationToken cancellationToken)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

            var stuck = await db.Set<AiTitlesForArticle>()
                .Where(t => (t.AiIsOk == false || t.AiIsOk == null)
                         && t.AiRetryCount < MaxArticleAiRetry)
                .OrderBy(t => t.CreateDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (stuck == null) return;

            stuck.AiRetryCount += 1;
            stuck.AiErrorMessage = errorMessage.Length > 500 ? errorMessage[..500] : errorMessage;
            stuck.AiIsOk = stuck.AiRetryCount >= MaxArticleAiRetry ? false : null;
            stuck.AiProcessedAt = DateTime.UtcNow;

            await db.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Bir makale başarıyla üretildikten hemen sonra çağrılır. AI'dan (telefon numarası İÇERMEYEN)
        /// ham bir konu başlığı ister, ardından ContactPhoneNumber'ı deterministik olarak ekleyip
        /// AiTitlesForArticle kuyruğuna yeni bir kayıt olarak yazar. Bu adım en iyi çaba (best-effort)
        /// mantığıyla çalışır — başarısız olursa sadece loglanır, ana makale kaydını etkilemez.
        /// </summary>
        private async Task GenerateAndQueueNextTitleAsync(
            AIPostContentCreator aiCreator,
            _ApplicationConnectionDb writeDb,
            CancellationToken cancellationToken)
        {
            try
            {
                var (district, category) = GetNextRotation();

                var titleResult = await aiCreator.GenerateNextArticleTitleAsync(district, category, cancellationToken);

                if (!titleResult.Success || string.IsNullOrWhiteSpace(titleResult.Title))
                {
                    _logger.LogWarning(
                        "Yeni başlık üretilemedi, kuyruk beslenemedi → Bölge: {District} | Hizmet: {Category} | Sebep: {Reason}",
                        district, category, titleResult.FailureReason ?? "bilinmiyor");
                    return;
                }

                string finalTitle = BuildTitleWithPhoneNumber(titleResult.Title, ContactPhoneNumber);

                // Aynı başlık zaten kuyrukta/kayıtlıysa tekrar ekleme
                bool alreadyExists = await writeDb.Set<AiTitlesForArticle>()
                    .AnyAsync(t => t.Title == finalTitle, cancellationToken);

                if (alreadyExists)
                {
                    _logger.LogInformation("Üretilen başlık zaten mevcut, atlandı → {Title}", finalTitle);
                    return;
                }

                var newTitleEntity = new AiTitlesForArticle
                {
                    Id = Guid.NewGuid(),
                    Title = finalTitle,
                    CreateDate = DateTime.UtcNow,
                    AiIsOk = null,               // Yeni kuyruk kaydı — henüz işlenmedi
                    AiRetryCount = 0,
                    IsDeleted = new IsDeleted()
                };

                writeDb.Set<AiTitlesForArticle>().Add(newTitleEntity);

                _logger.LogInformation("Kuyruğa yeni başlık eklendi → {Title} (Bölge: {District} | Hizmet: {Category})", finalTitle, district, category);
            }
            catch (Exception ex)
            {
                // Kuyruk besleme adımı başarısız olsa bile ana makale kaydı zarar görmemeli
                _logger.LogError(ex, "Kuyruğa yeni başlık eklenirken beklenmeyen hata.");
            }
        }

        /// <summary>
        /// AI'nın ürettiği ham başlıktan olası telefon benzeri dizeleri temizler,
        /// ardından ZORUNLU telefon numarasını deterministik ve garanti şekilde ekler.
        /// Bu sayede numaranın eksik, yanlış veya farklı formatta gelme riski tamamen ortadan kalkar.
        /// </summary>
        private static string BuildTitleWithPhoneNumber(string rawTitle, string phoneNumber)
        {
            // AI yanlışlıkla bir telefon/rakam dizisi eklemiş olabilir — bunları temizle
            string cleaned = Regex.Replace(rawTitle, @"(\+?\d[\d\s\-\(\)]{6,}\d)", "").Trim();
            cleaned = Regex.Replace(cleaned, @"\s{2,}", " ").Trim(' ', '-', '|', ',');

            if (string.IsNullOrWhiteSpace(cleaned))
            {
                cleaned = rawTitle.Trim(); // temizlik her şeyi silmiş olursa orijinali kullan
            }

            return $"{cleaned} | İletişim: {phoneNumber}";
        }

        /// <summary>
        /// Bölge/hizmet kombinasyonları arasında sırayla (round-robin) dolaşır.
        /// Tüm kombinasyonlar tükenmeden aynı kombinasyon tekrar seçilmez.
        /// </summary>
        private (string district, string category) GetNextRotation()
        {
            int totalCombinations = DuzceBolgeleri.Length * HizmetKategorileri.Length;
            int index = _titleRotationCounter % totalCombinations;

            int districtIndex = index % DuzceBolgeleri.Length;
            int categoryIndex = (index / DuzceBolgeleri.Length) % HizmetKategorileri.Length;

            _titleRotationCounter++;

            return (DuzceBolgeleri[districtIndex], HizmetKategorileri[categoryIndex]);
        }

        // Türkçe karakterleri normalize ederek SEO dostu slug üretir
        private static string GenerateSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return Guid.NewGuid().ToString("N");

            var map = new Dictionary<char, char>
            {
                {'ç','c'}, {'Ç','c'}, {'ğ','g'}, {'Ğ','g'}, {'ı','i'}, {'İ','i'},
                {'ö','o'}, {'Ö','o'}, {'ş','s'}, {'Ş','s'}, {'ü','u'}, {'Ü','u'}
            };

            var sb = new StringBuilder();
            foreach (var ch in input.ToLowerInvariant())
            {
                char c = map.TryGetValue(ch, out var mapped) ? mapped : ch;
                if (char.IsLetterOrDigit(c)) sb.Append(c);
                else sb.Append('-');
            }

            string slug = Regex.Replace(sb.ToString(), "-{2,}", "-").Trim('-');
            return string.IsNullOrEmpty(slug) ? Guid.NewGuid().ToString("N") : slug;
        }
    }
}