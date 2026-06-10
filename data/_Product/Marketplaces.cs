using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Entegre platform tanımları (Amazon, Trendyol, Hepsiburada, eBay, Shopify vb.).
    /// Admin yönetimli, sistem geneli referans tablosudur.
    /// </summary>
    public class Marketplaces
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? Name { get; set; } // Platform adı (Örn: Trendyol)
        public string? Code { get; set; } // Teknik kod (Örn: trendyol, amazon, n11) — entegrasyon katmanında anahtar
        public string? ApiBaseUrl { get; set; } // API kök adresi
        public Guid? LogoMediaId { get; set; } // Platform logosu (Media.Id)

        public bool IsActive { get; set; } = true; // Entegrasyon aktif mi

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
