using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Entegre platform tanımları. Admin yönetimli, sistem geneli referans tablosudur.
    ///
    /// BÖLGESEL PAZAR MODELİ (REVİZYON):
    /// Her bölgesel pazar AYRI SATIRDIR: "amazon-us", "amazon-de", "amazon-com-tr",
    /// "mercadolibre-mx", "mercadolibre-br" gibi. Böylece:
    ///   - MarketplaceCategoryMappings ve MarketplaceAttributeMappings doğal olarak
    ///     bölgeye + dile özgü olur (amazon-de eşlemeleri Almanca, amazon-us İngilizce).
    ///   - ProductMarketplaceListings hangi bölgesel pazara bağlı olduğunu tek Id ile bilir.
    ///   - Varsayılan dil/para birimi platform satırından okunur.
    /// </summary>
    public class Marketplaces
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? Name { get; set; } // Platform adı (Örn: Amazon Almanya)
        public string? Code { get; set; } // Teknik kod (Örn: amazon-de, ebay-us, aliexpress, temu, mercadolibre-br) — entegrasyon katmanında anahtar
        public string? ApiBaseUrl { get; set; } // API kök adresi
        public Guid? LogoMediaId { get; set; } // Platform logosu (Media.Id)

        // === Bölge / Dil / Para Birimi (REVİZYON) ===
        public string? CountryCode { get; set; } // Pazarın ülkesi (ISO 3166-1 alpha-2, Örn: "DE") — null = küresel platform
        public string? DefaultLanguageCode { get; set; } = "en"; // Pazarın varsayılan içerik dili (Örn: amazon-de → "de")
        public string? DefaultCurrency { get; set; } = "USD"; // Pazarın varsayılan para birimi (Örn: amazon-de → "EUR")

        public bool IsActive { get; set; } = true; // Entegrasyon aktif mi

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
