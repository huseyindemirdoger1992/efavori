using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Bir ürünün/varyantın belirli bir pazaryerindeki ilan (listing) eşleştirmesi.
    /// Harici ID, harici SKU/barkod, pazaryeri kategorisi ve senkronizasyon durumunu tutar.
    /// VariantId = null -> ürün düzeyi ilan; VariantId dolu -> varyant düzeyi ilan.
    /// </summary>
    public class ProductMarketplaceMapping
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; } // Eşleştirmeyi yöneten kullanıcının/satıcının ID bilgisi
        public Guid StoreId { get; set; } // Ürünün bağlı olduğu mağazanın ID bilgisi
        public Guid ProductId { get; set; } // İlanın ait olduğu ana ürünün ID'si (her zaman dolu)
        public Guid? VariantId { get; set; } // İlanın ait olduğu varyantın ID'si (ürün düzeyinde null)
        public Guid MarketplaceId { get; set; } // İlanın yayınlandığı pazaryerinin ID'si

        public string? ExternalProductId { get; set; } // Pazaryerindeki ürün ID'si
        public string? ExternalVariantId { get; set; } // Pazaryerindeki varyant ID'si
        public string? ExternalSku { get; set; } // Pazaryerine gönderilen/eşleşen SKU
        public string? ExternalBarcode { get; set; } // Pazaryerine gönderilen/eşleşen barkod
        public string? MarketplaceCategoryId { get; set; } // Pazaryerindeki kategori ID'si
        public string? MarketplaceUrl { get; set; } // Yayındaki ilan adresi

        public string SyncStatus { get; set; } = "Pending"; // Senkron durumu: "Pending" | "Synced" | "Error" | "Disabled"
        public string? SyncErrorMessage { get; set; } // Hata durumunda son hata mesajı
        public DateTime? LastSyncedAt { get; set; } // Son başarılı senkron tarihi
        public bool IsListed { get; set; } = false; // Şu an pazaryerinde yayında mı

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public DateTime? UpdatedAt { get; set; } // Son güncelleme tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
