using System;

namespace data._BulkImportProducts
{
    /// <summary>
    /// İÇE AKTARIM SATIRI (staging / ara kayıt). Kaynaktaki HER ürün için bir satır
    /// oluşur. Ham veri önce buraya alınır, ayrıştırılır, eşleştirilir ve ancak
    /// başarılıysa gerçek <see cref="data._Products.Products"/> kaydına dönüştürülür.
    /// Bu ara katman sayesinde: hatalı satır tüm işi durdurmaz, kullanıcı içe aktarım
    /// öncesi önizleme/düzeltme yapabilir ve işlem idempotent tekrar denenebilir.
    ///
    /// <see cref="RawRowJson"/> ham kaynağı (CSV satırı/JSON nesnesi) olduğu gibi
    /// saklar; <see cref="MappedDataJson"/> ise alan eşleştirmesi sonrası efavori
    /// şemasına normalize edilmiş halidir. Başarıda <see cref="CreatedProductId"/>
    /// doldurulur — böylece satır ile üretilen ürün arasında iz kurulur (idempotency:
    /// aynı satır ikinci kez ürün yaratmaz).
    /// </summary>
    public class ImportRow : BulkImportEntityBase
    {
        /// <summary>Bağlı içe aktarım işi (ImportJob.Id).</summary>
        public Guid ImportJobId { get; set; }

        /// <summary>Denormalize profil referansı (sorgu kolaylığı).</summary>
        public Guid ImportProfileId { get; set; }

        /// <summary>Kaynaktaki satır sırası (0 tabanlı) — sıralama/checkpoint için.</summary>
        public int SourceRowIndex { get; set; }

        // ── Kaynak kimlik/eşleşme ─────────────────────────────────────────────
        /// <summary>Kaynak platformdaki dış ürün kimliği (varsa; ör. ASIN, item id).</summary>
        public string? ExternalProductId { get; set; }

        /// <summary>Kaynaktan gelen SKU (eşleşme/izleme için).</summary>
        public string? SourceSku { get; set; }

        /// <summary>Kaynaktan gelen barkod/GTIN.</summary>
        public string? SourceGtin { get; set; }

        /// <summary>Kaynaktan gelen ürün adı (önizleme/log için).</summary>
        public string? SourceName { get; set; }

        /// <summary>
        /// Satır tekilleştirme anahtarı (hash). MatchKey'e göre üretilir; aynı işte
        /// aynı ürünün iki kez işlenmesini önler. (ImportJobId, SourceKeyHash) tekildir.
        /// </summary>
        public string? SourceKeyHash { get; set; }

        // ── Veri ──────────────────────────────────────────────────────────────
        /// <summary>Ham kaynak veri (CSV satırı / JSON nesnesi / XML düğümü) — JSON.</summary>
        public string? RawRowJson { get; set; }

        /// <summary>Alan eşleştirmesi + dönüşüm sonrası efavori şemasına normalize veri — JSON.</summary>
        public string? MappedDataJson { get; set; }

        // ── Durum + sonuç ─────────────────────────────────────────────────────
        /// <summary>Satırın durumu.</summary>
        public ImportRowStatus Status { get; set; } = ImportRowStatus.Pending;

        /// <summary>
        /// Bu satırdan oluşturulan/güncellenen ürün (data._Products.Products.Id).
        /// Idempotency: doluysa satır zaten işlenmiştir, tekrar ürün yaratılmaz.
        /// </summary>
        public Guid? CreatedProductId { get; set; }

        /// <summary>Bu satırdan oluşturulan varyant (data._Products.ProductVariants.Id — varsa).</summary>
        public Guid? CreatedVariantId { get; set; }

        /// <summary>
        /// Yineleme durumunda eşleşen mevcut ürün (izleme için; DuplicateStrategy'e
        /// göre atlanmış/güncellenmiş olabilir).
        /// </summary>
        public Guid? MatchedExistingProductId { get; set; }

        /// <summary>Satır hatası (Status = Failed iken).</summary>
        public string? ErrorMessage { get; set; }

        /// <summary>Uyarılar (kısmi başarı; ör. "kategori eşleşmedi, varsayılana atandı") — JSON.</summary>
        public string? WarningsJson { get; set; }

        /// <summary>İşlenme anı (UTC).</summary>
        public DateTime? ProcessedAtUtc { get; set; }
    }

    /// <summary>
    /// SATIR OLAYI/LOGU (opsiyonel ayrıntılı iz). Bir satırın işlenmesi sırasında
    /// oluşan adımlar/uyarılar/hatalar için ince taneli kayıt. Kullanıcı içe aktarım
    /// raporunu incelerken "bu ürün neden atlandı?" sorusunu buradan yanıtlar.
    ///
    /// Not: Bu, GENEL bir sistem denetim günlüğü DEĞİLDİR; yalnızca içe aktarım
    /// akışının satır düzeyindeki teşhis kaydıdır (kim-ne-zaman-değiştirdi tutmaz).
    /// </summary>
    public class ImportRowLog : BulkImportEntityBase
    {
        /// <summary>Bağlı satır (ImportRow.Id).</summary>
        public Guid ImportRowId { get; set; }

        /// <summary>Denormalize iş referansı (işe göre toplu sorgu için).</summary>
        public Guid ImportJobId { get; set; }

        /// <summary>Olay/adım adı (ör. "Parse", "MapCategory", "ResolveBrand", "SaveProduct").</summary>
        public string Step { get; set; } = string.Empty;

        /// <summary>Seviye (ör. "Info", "Warning", "Error").</summary>
        public string Level { get; set; } = "Info";

        /// <summary>Mesaj metni.</summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>Ek ayrıntı (JSON — ör. eşleşmeyen alan adı, ham değer).</summary>
        public string? DetailJson { get; set; }

        /// <summary>Olay anı (UTC).</summary>
        public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    }
}
