using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ürün/varyantın dış platformlardaki karşılığı ve senkronizasyon durumu.
    /// İçe aktarımda platformdan gelen HAM VERİ RawSourceData alanında JSON olarak saklanır —
    /// böylece modelimizde karşılığı olmayan alanlar dahi VERİ KAYBI olmadan korunur
    /// ve ileride yeniden işlenebilir.
    /// </summary>
    public class ProductMarketplaceListings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Ürün (Products.Id)
        public Guid? VariantId { get; set; } // Varyant bazlı eşleme gerekiyorsa (ProductVariants.Id)
        public Guid MarketplaceId { get; set; } // Platform (Marketplaces.Id)
        public Guid StoreId { get; set; } // Mağaza (Store.Id) — hangi mağazanın entegrasyonu

        public string? ExternalProductId { get; set; } // Platformdaki ürün ID'si (Örn: Amazon ASIN, Trendyol contentId)
        public string? ExternalSku { get; set; } // Platformdaki SKU (merchantSku vb.)
        public string? ExternalUrl { get; set; } // Platformdaki ürün sayfası adresi

        // "Pending", "Synced", "Error", "Paused"
        public string? SyncStatus { get; set; } = "Pending";
        public DateTime? LastSyncDate { get; set; } // Son başarılı senkronizasyon tarihi
        public string? LastErrorMessage { get; set; } // Son hata mesajı

        public string? RawSourceData { get; set; } // İçe aktarımda gelen ham kaynak verisi (JSON) — veri kaybı sigortası

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    }
}
