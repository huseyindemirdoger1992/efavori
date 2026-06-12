using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product.BulkWordPressProductImport
{
    /// <summary>
    /// BİRLEŞİK EŞLEŞME HAFIZASI. WooCommerce alan/kategori/marka/özellik değerlerinin
    /// sistemdeki karşılıklarını TEK tabloda, tip ayraçlı (MappingType) olarak saklar.
    ///
    /// "KATEGORİ EŞLEŞTİRME HAFIZASI" KURALI:
    ///   Bir eşleşme yapıldıktan ve onaylandıktan sonra (IsConfirmedByUser = true) aynı kaynak
    ///   değer tekrar kullanıcıya SORULMAZ. SourceKeyHash üzerinden hızlı sözlük araması yapılır.
    ///
    /// KAPSAM (JobId):
    ///   - JobId dolu  → yalnızca o aktarıma özel eşleşme.
    ///   - JobId null  → kullanıcı/mağaza genelinde KALICI eşleşme (sonraki aktarımlarda yeniden kullanılır).
    ///
    /// GÜVEN SEVİYESİ: ConfidenceLevel + ConfidenceScore ile her eşleşmenin ne kadar kesin
    /// olduğu işaretlenir; belirsiz olanlar (Uncertain/Unmapped) kullanıcı onayına düşer.
    /// </summary>
    public class ProductImportMappings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? JobId { get; set; }   // Bağlı aktarım (ProductImportJobs.Id) — null = kalıcı/global eşleşme
        public Guid UserId { get; set; }   // Sahip kullanıcı (Users.Id)
        public Guid StoreId { get; set; }  // Sahip mağaza (Store.Id)

        // "Field"          → CSV kolonu → sistem alanı     (Örn: "Normal fiyat" → Price)
        // "Category"       → WooCommerce kategori yolu → CategoriesTr.Id
        // "Brand"          → WooCommerce marka → Brands.Id
        // "Attribute"      → WooCommerce attribute başlığı → ProductAttributes.Id (bilgi dağarcığı)
        // "AttributeValue" → WooCommerce attribute değeri → ProductAttributeValues.Id
        // "StockStatus"    → instock/outofstock/onbackorder → sistem stok durumu
        // "TaxClass"       → vergi sınıfı eşlemesi
        public string? MappingType { get; set; }

        // === Kaynak (WooCommerce tarafı) ===
        public string? SourceKey { get; set; }      // Ham kaynak değer (kolon adı / kategori yolu / marka / attribute)
        public string? SourceKeyHash { get; set; }  // SourceKey'in normalize+hash hali — hızlı/benzersiz arama
        public string? SourceParentKey { get; set; } // Hiyerarşik kaynaklarda üst düğüm (kategori ağacı için)
        public int? SourceItemCount { get; set; }   // Bu kaynağa ait ürün adedi (kategori ağacı raporu: "65 ürün")

        // === Hedef (sistem tarafı) ===
        // "ProductField" | "CategoryId" | "BrandId" | "AttributeId" | "AttributeValueId" | "Literal"
        public string? TargetType { get; set; }
        public string? TargetValue { get; set; }    // Hedef değer (alan adı, Guid veya int — string olarak saklanır)
        public string? TargetDisplayName { get; set; } // UI'da gösterilecek hedef adı (Örn: "Uydu Ekipmanları")

        // === Güven / Onay ===
        // "Exact"    → kesin eşleşme (Örn: Weight → WeightKg)
        // "Likely"   → büyük olasılıkla (Örn: Normal fiyat → Price)
        // "Uncertain"→ belirsiz, kullanıcı onayı şart (Örn: Meta: custom_field_12)
        // "Unmapped" → henüz eşlenmedi
        public string? ConfidenceLevel { get; set; } = "Unmapped";
        public int ConfidenceScore { get; set; } = 0; // 0–100 güven puanı (otomatik öneri sıralaması)
        public bool IsConfirmedByUser { get; set; } = false; // Kullanıcı onayladı mı (true → tekrar sorulmaz)
        public bool CreateIfMissing { get; set; } = false;   // Hedef yoksa yeni oluşturulsun mu (marka/değer için)

        public string? SuggestionsJson { get; set; } // Akıllı öneri listesi (JSON) — UI'da kullanıcıya sunulan adaylar

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public DateTime? UpdatedAt { get; set; }                    // Son güncelleme tarihi

        public IsDeleted? IsDeleted { get; set; } = new();          // Silinme durumu (soft delete)
    }
}
