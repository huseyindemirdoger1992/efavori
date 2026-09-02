using System;

namespace data._BulkImportProducts
{
    /// <summary>
    /// İÇE AKTARIM YORUM SATIRI (review staging / ara kayıt). Kaynaktaki (Amazon,
    /// Trendyol, WooCommerce vb.) HER ürün yorumu için bir satır oluşur.
    ///
    /// Ürün satırı (<see cref="ImportRow"/>) ile 1:N ilişkidedir: bir ürünün onlarca
    /// yorumu olabilir, her biri ayrı ImportReviewRow kaydı olarak ayrıştırılır.
    /// Başarılı işlemde gerçek <see cref="data._Products.ProductReviews"/> kaydına
    /// dönüştürülür.
    ///
    /// Kaynak yorumun tam JSON'u <see cref="RawReviewJson"/> alanında saklanır;
    /// efavori şemasına normalize edilmiş hali <see cref="MappedReviewJson"/>'dadır.
    ///
    /// TASARIM GEREKÇESİ — Neden ImportRow içinde JSON tutulmadı:
    /// (a) Bir ürünün yüzlerce yorumu olabilir — tümünü tek bir JSON alanına
    ///     sıkıştırmak sorgulama, durum takibi ve yeniden denemeyi imkânsız kılar.
    /// (b) Her yorum kendi moderasyon + hata + oluşturulan kayıt izini taşımalı.
    /// (c) Satır bazlı ilerleme/sayaç ve checkpoint sistemi ürünlerle aynı
    ///     kalıpla çalışır (self-healing counter deseni).
    ///
    /// Konvansiyon: Navigation property İÇERMEZ — ilişkiler yalnızca ID üzerinden.
    /// FK'ler, indeksler ve silme davranışları _BulkImportModelConfiguration içinde.
    /// </summary>
    public class ImportReviewRow : BulkImportEntityBase
    {
        /// <summary>Bağlı içe aktarım işi (ImportJob.Id).</summary>
        public Guid ImportJobId { get; set; }

        /// <summary>
        /// Yorumun ait olduğu ürün satırı (ImportRow.Id).
        /// İçe aktarım akışında önce ürün satırı işlenir, ardından o satıra bağlı
        /// review satırları işlenir. Ürün satırı henüz işlenmemişse review'ler bekler.
        /// </summary>
        public Guid ImportRowId { get; set; }

        /// <summary>
        /// Kaynak platformdaki dış yorum kimliği (varsa; ör. Amazon review id,
        /// Trendyol yorum id). Aynı review'in tekrar import edilmesini önler.
        /// </summary>
        public string? ExternalReviewId { get; set; }

        /// <summary>Kaynak satır/dizin sırası (0 tabanlı) — sıralama/checkpoint için.</summary>
        public int SourceRowIndex { get; set; }

        // ── Kaynak veriden ayrıştırılan önizleme alanları ─────────────────────
        /// <summary>Kaynaktaki yorum sahibinin adı/takma adı (önizleme için).</summary>
        public string? SourceReviewerName { get; set; }

        /// <summary>Kaynaktaki yıldız/puan değeri (ör. 1..5; platform ölçeğine göre).</summary>
        public byte? SourceRating { get; set; }

        /// <summary>Kaynaktaki yorum başlığı (opsiyonel).</summary>
        public string? SourceTitle { get; set; }

        /// <summary>Kaynaktaki yorum metni kısaltılmış (önizleme; tam metin RawReviewJson'da).</summary>
        public string? SourceBodyPreview { get; set; }

        /// <summary>Kaynaktaki yorum tarihi (UTC; kaynak saat dilimi profilde belirtilir).</summary>
        public DateTime? SourceReviewDate { get; set; }

        /// <summary>
        /// Kaynak platformda "doğrulanmış satın alma" (verified purchase) işareti
        /// var mıydı? Amazon/Trendyol gibi platformlar bunu ayrı alan olarak döner.
        /// </summary>
        public bool? SourceIsVerifiedPurchase { get; set; }

        // ── Alt puanlar (kaynak platformdan gelen kırılım puanları — opsiyonel) ──
        /// <summary>Kalite alt puanı (kaynaktan gelen; 1..5 ölçeğine normalize).</summary>
        public byte? SourceQualityRating { get; set; }

        /// <summary>Kargo/teslimat alt puanı (kaynaktan gelen).</summary>
        public byte? SourceShippingRating { get; set; }

        /// <summary>Fiyat/performans alt puanı (kaynaktan gelen).</summary>
        public byte? SourceValueRating { get; set; }

        // ── Medya bilgisi (kaynak yorum görselleri/videoları) ─────────────────
        /// <summary>
        /// Kaynaktaki yorum medya URL'leri (JSON dizisi). Ör: Amazon yorum
        /// fotoğrafları, Trendyol yorum görselleri. İçe aktarım sırasında
        /// indirilip Media tablosuna kaydedilir, ProductReviewMedia kaydı oluşturulur.
        /// </summary>
        public string? SourceMediaUrlsJson { get; set; }

        /// <summary>Kaynaktaki medya sayısı (önizleme/istatistik için).</summary>
        public int SourceMediaCount { get; set; }

        // ── Fayda oyları (kaynak platformdan gelen toplam oy verileri) ────────
        /// <summary>Kaynaktaki "faydalı" (helpful) oy sayısı.</summary>
        public int? SourceHelpfulVoteCount { get; set; }

        /// <summary>Kaynaktaki toplam oy sayısı (helpful + not helpful).</summary>
        public int? SourceTotalVoteCount { get; set; }

        // ── Kaynak yorum sahibi bilgileri (anonim aktarım) ────────────────────
        /// <summary>
        /// Kaynaktaki yorum sahibi dış kimliği (ör. Amazon customer id).
        /// Sistemde Users kaydı OLUŞTURULMAZ; bu alan izleme/dedup içindir.
        /// </summary>
        public string? SourceReviewerExternalId { get; set; }

        /// <summary>Kaynaktaki yorum sahibinin profil resmi URL'si (opsiyonel).</summary>
        public string? SourceReviewerAvatarUrl { get; set; }

        // ── Kaynak varyant bilgisi ────────────────────────────────────────────
        /// <summary>
        /// Kaynaktaki yorumun hangi varyanta (renk/beden) ait olduğu (ör. "Siyah, XL").
        /// Eşleştirme sırasında efavori ProductVariants.Id'ye çözümlenir.
        /// </summary>
        public string? SourceVariantLabel { get; set; }

        // ── Ham + dönüştürülmüş veri ──────────────────────────────────────────
        /// <summary>
        /// Ham kaynak yorum verisi (JSON nesnesi — API yanıtı / CSV satırı / scrape
        /// çıktısı olduğu gibi). Hata ayıklama ve tekrar işleme için saklanır.
        /// </summary>
        public string? RawReviewJson { get; set; }

        /// <summary>
        /// Alan eşleştirmesi + dönüşüm sonrası efavori şemasına normalize edilmiş
        /// yorum verisi (JSON). Review servisinin doğrudan tüketebileceği formatta.
        /// </summary>
        public string? MappedReviewJson { get; set; }

        // ── Tekilleştirme ─────────────────────────────────────────────────────
        /// <summary>
        /// Yorum tekilleştirme anahtarı (hash). ExternalReviewId veya
        /// (SourceReviewerName + SourceBodyPreview hash) üretilir; aynı işte
        /// aynı yorum iki kez işlenmesin.
        /// </summary>
        public string? SourceKeyHash { get; set; }

        // ── Durum + sonuç ─────────────────────────────────────────────────────
        /// <summary>Satırın durumu (ürün satırlarıyla aynı enum).</summary>
        public ImportRowStatus Status { get; set; } = ImportRowStatus.Pending;

        /// <summary>
        /// Bu satırdan oluşturulan yorum kaydı (data._Products.ProductReviews.Id).
        /// Idempotency: doluysa satır zaten işlenmiştir, tekrar yorum yaratılmaz.
        /// </summary>
        public Guid? CreatedReviewId { get; set; }

        /// <summary>
        /// Ürün eşleştirmesi sonucu bulunan efavori ürün (data._Products.Products.Id).
        /// ImportRow.CreatedProductId veya ImportRow.MatchedExistingProductId'den türetilir.
        /// </summary>
        public Guid? TargetProductId { get; set; }

        /// <summary>
        /// Varyant eşleştirmesi sonucu bulunan efavori varyant
        /// (data._Products.ProductVariants.Id — opsiyonel).
        /// </summary>
        public Guid? TargetVariantId { get; set; }

        /// <summary>Satır hatası (Status = Failed iken).</summary>
        public string? ErrorMessage { get; set; }

        /// <summary>Uyarılar (JSON — ör. "puan ölçeği normalize edildi: 10→5").</summary>
        public string? WarningsJson { get; set; }

        /// <summary>İşlenme anı (UTC).</summary>
        public DateTime? ProcessedAtUtc { get; set; }
    }

    /// <summary>
    /// YORUM SATIRI OLAYI/LOGU — ImportRowLog'un review karşılığı.
    /// Her yorum satırının işlenmesi sırasında oluşan adım/uyarı/hata kayıtları.
    /// Kullanıcı "bu yorum neden atlandı?" sorusunu buradan yanıtlar.
    /// </summary>
    public class ImportReviewRowLog : BulkImportEntityBase
    {
        /// <summary>Bağlı yorum satırı (ImportReviewRow.Id).</summary>
        public Guid ImportReviewRowId { get; set; }

        /// <summary>Denormalize iş referansı (işe göre toplu sorgu için).</summary>
        public Guid ImportJobId { get; set; }

        /// <summary>Olay/adım adı (ör. "ParseReview", "MapRating", "DownloadMedia", "SaveReview").</summary>
        public string Step { get; set; } = string.Empty;

        /// <summary>Seviye (ör. "Info", "Warning", "Error").</summary>
        public string Level { get; set; } = "Info";

        /// <summary>Mesaj metni.</summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>Ek ayrıntı (JSON — ör. puan dönüşümü detayı, medya indirme hatası).</summary>
        public string? DetailJson { get; set; }

        /// <summary>Olay anı (UTC).</summary>
        public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    }
}