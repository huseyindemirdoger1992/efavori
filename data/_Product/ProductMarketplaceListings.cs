using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ürün/varyantın dış platformlardaki karşılığı ve senkronizasyon durumu.
    /// İçe aktarımda platformdan gelen HAM VERİ RawSourceData alanında JSON olarak saklanır —
    /// modelimizde karşılığı olmayan alanlar dahi VERİ KAYBI olmadan korunur ve
    /// ileride yeniden işlenebilir.
    ///
    /// REVİZYON EKLERİ:
    ///   ExternalParentId → Amazon parent ASIN / eBay varyasyon grubu / AliExpress ana ürün kimliği.
    ///     Varyantlı içe aktarımda ebeveyn-çocuk ilişkisi kayıpsız korunur.
    ///   LanguageCode     → dış listelemenin içerik dili (null = platform varsayılanı).
    ///     Aynı ürün amazon-us (en) ve amazon-de (de) satırlarıyla ayrı ayrı eşlenebilir.
    ///   CurrencyCode     → dış listelemenin para birimi (null = platform varsayılanı).
    ///   SyncDirection    → veri akış yönü.
    ///   RawSourceHash    → RawSourceData'nın hash'i; kaynak değişmediyse senkronizasyon atlanır.
    ///
    /// BENZERSİZLİK (Fluent API): (StoreId, MarketplaceId, ExternalProductId, VariantId)
    /// </summary>
    public class ProductMarketplaceListings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Ürün (Products.Id)
        public Guid? VariantId { get; set; } // Varyant bazlı eşleme gerekiyorsa (ProductVariants.Id)
        public Guid MarketplaceId { get; set; } // Bölgesel platform (Marketplaces.Id — Örn: amazon-de satırı)
        public Guid StoreId { get; set; } // Mağaza (Store.Id) — hangi mağazanın entegrasyonu

        public string? ExternalProductId { get; set; } // Platformdaki ürün ID'si (Amazon ASIN, eBay ItemID, Temu goodsId, ML MLB-kodu)
        public string? ExternalParentId { get; set; } // Platformdaki EBEVEYN kimlik (Amazon parent ASIN / varyasyon grubu) — varyant hiyerarşisi korunur
        public string? ExternalSku { get; set; } // Platformdaki SKU (merchantSku vb.)
        public string? ExternalUrl { get; set; } // Platformdaki ürün sayfası adresi
        public string? ExternalCategoryPath { get; set; } // Platformdaki kategori yolu (Örn: "Electronics > TV") — eşleme önerisi ve denetim için

        // === Dil / Para Birimi ===
        public string? LanguageCode { get; set; } // Dış listelemenin içerik dili — null = Marketplaces.DefaultLanguageCode
        public string? CurrencyCode { get; set; } // Dış listelemenin para birimi — null = Marketplaces.DefaultCurrency

        // === Senkronizasyon ===
        // "Import" → yalnızca içe alınır | "Export" → yalnızca dışa gönderilir | "TwoWay" → çift yönlü
        public string? SyncDirection { get; set; } = "Import";

        // "Pending", "Synced", "Error", "Paused"
        public string? SyncStatus { get; set; } = "Pending";
        public DateTime? LastSyncDate { get; set; } // Son başarılı senkronizasyon tarihi
        public string? LastErrorMessage { get; set; } // Son hata mesajı

        public string? RawSourceData { get; set; } // İçe aktarımda gelen ham kaynak verisi (JSON) — veri kaybı sigortası
        public string? RawSourceHash { get; set; } // RawSourceData hash'i — değişiklik tespiti (değişmediyse işlem atlanır)

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public DateTime? UpdatedAt { get; set; } // Son güncelleme tarihi
    }
}
