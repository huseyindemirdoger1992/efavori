using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product.BulkWordPressProductImport
{
    /// <summary>
    /// SATIR BAZLI İLERLEME ve HATA RAPORU. Her CSV satırının aktarım durumunu tutar.
    /// IDEMPOTENT ve RESUMABLE çalışmanın temelidir:
    ///   - SourceExternalId (WooCommerce "Kimlik") benzersiz anahtardır → aynı satır iki kez
    ///     işlenmez, DUPLICATE ürün oluşmaz. Yeniden çalıştırmada "Imported" satırlar atlanır.
    ///   - RawRowJson, satırın HAM halini saklar (veri kaybı sigortası) → ileride yeniden işlenebilir.
    ///
    /// AŞAMA-12 HATA RAPORLAMA: Eksik kategori, bozuk görsel, hatalı fiyat, eksik/yinelenen SKU,
    /// geçersiz varyasyon gibi durumlar ErrorCode/ErrorMessage ile satır seviyesinde raporlanır.
    /// </summary>
    public class ProductImportRows
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid JobId { get; set; }    // Bağlı aktarım (ProductImportJobs.Id)
        public Guid UserId { get; set; }   // Sahip kullanıcı (Users.Id) — denormalize
        public Guid StoreId { get; set; }  // Sahip mağaza (Store.Id) — denormalize

        public int SourceRowIndex { get; set; }        // CSV içindeki sıfır-tabanlı satır indeksi (resume için)
        public string? SourceExternalId { get; set; }  // WooCommerce "Kimlik" — IDEMPOTENCY anahtarı
        public string? SourceSku { get; set; }         // Kaynak SKU (varsa)
        public string? ParentExternalId { get; set; }  // Varyasyon ise üst ürünün Kimlik'i ("Ebeveyn")

        // "simple" | "variable" | "variation" | "grouped" | "external"
        public string? SourceProductType { get; set; }

        // "Pending"   → henüz işlenmedi
        // "Imported"  → başarıyla aktarıldı
        // "Failed"    → hata ile sonuçlandı (ErrorCode/ErrorMessage dolu)
        // "Skipped"   → kapsam dışı / kullanıcı atladı
        // "Duplicate" → daha önce aktarılmış (idempotency engeli)
        public string? RowStatus { get; set; } = "Pending";

        // === Üretilen Kayıtlar (geri izleme + rollback için) ===
        public Guid? CreatedProductId { get; set; }    // Oluşan ürün (Products.Id)
        public Guid? CreatedVariantId { get; set; }    // Oluşan varyant (ProductVariants.Id)

        // === Hata Detayı (AŞAMA-12) ===
        // "MissingCategory" | "MissingBrand" | "BrokenImage" | "InvalidPrice" | "MissingSku"
        // "DuplicateSku" | "InvalidVariation" | "OrphanVariation" | "MediaProcessError" | "Unknown"
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }      // İnsan-okur hata açıklaması
        public string? WarningsJson { get; set; }      // Bloklamayan uyarılar (JSON) — Örn: "görsel bulunamadı, atlandı"

        public string? RawRowJson { get; set; }        // Satırın ham CSV verisi (JSON) — veri kaybı sigortası

        public DateTime? ProcessedAt { get; set; }     // İşlenme tarihi
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Kayıt tarihi
    }
}
