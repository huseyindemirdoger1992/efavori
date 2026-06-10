using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// CSV / XML / JSON / API içe aktarım profilleri.
    /// Satıcı, kaynak dosya kolonlarının sistem alanlarına nasıl eşleneceğini bir kez tanımlar
    /// (FieldMappingJson), sonraki aktarımlarda profili yeniden kullanır.
    /// Trendyol, Amazon, WooCommerce, N11 gibi hazır kaynak tipleri desteklenir.
    /// </summary>
    public class ProductImportProfiles
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; } // Profili oluşturan kullanıcı (Users.Id)
        public Guid StoreId { get; set; } // Bağlı mağaza (Store.Id)

        public string? Name { get; set; } // Profil adı (Örn: "Trendyol Aylık Aktarım")

        // "CSV", "XML", "JSON", "RestApi", "SoapApi", "Amazon", "Alibaba", "Ebay", "MercadoLibre",
        // "Jd", "Trendyol", "Hepsiburada", "N11", "CicekSepeti", "Etsy", "Shopify", "WooCommerce"
        public string? SourceType { get; set; } = "CSV";

        public string? FieldMappingJson { get; set; } // Kolon → alan eşleme tanımı (JSON)
        public string? Delimiter { get; set; } = ";"; // CSV ayıracı
        public string? Encoding { get; set; } = "UTF-8"; // Dosya karakter seti
        public string? SourceUrl { get; set; } // API/XML feed adresi (dosya dışı kaynaklarda)

        public Guid? DefaultWarehouseId { get; set; } // Aktarılan ürünlerin varsayılan deposu (Warehouse.Id)
        public int? DefaultCategoryId { get; set; } // Eşlenemeyen ürünler için varsayılan kategori (CategoriesTr.Id)

        public bool IsActive { get; set; } = true; // Profil kullanımda mı
        public DateTime? LastRunDate { get; set; } // Son çalıştırma tarihi
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
