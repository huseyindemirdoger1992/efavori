using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Sistem kategorilerinin platform kategorilerine eşlenmesi.
    /// Örn: CategoriesTr "Televizyon" ↔ Trendyol "TV & Görüntü Sistemleri" (id: 1234).
    /// </summary>
    public class MarketplaceCategoryMappings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid MarketplaceId { get; set; } // Platform (Marketplaces.Id)
        public int CategoryId { get; set; } // Sistem kategorisi (CategoriesTr.Id — int tip)

        public string? ExternalCategoryCode { get; set; } // Platformdaki kategori kodu/ID'si
        public string? ExternalCategoryName { get; set; } // Platformdaki kategori adı

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    }
}
