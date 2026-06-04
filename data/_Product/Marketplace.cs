using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Desteklenen pazaryeri tanımı (Trendyol, Amazon, Hepsiburada, N11, Etsy, eBay, WooCommerce, Shopify...).
    /// Yeni pazaryeri eklemek şema değil veri değişikliğidir. IsInternational, desi bölenini etkiler.
    /// </summary>
    public class Marketplace
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty; // Pazaryeri adı (Örn: "Trendyol")
        public string? Code { get; set; } // Pazaryeri kısa kodu (Örn: "trendyol")
        public string? ApiBaseUrl { get; set; } // Entegrasyon API kök adresi
        public bool IsInternational { get; set; } = false; // Yurtdışı pazaryeri mi (desi bölenini etkiler)
        public int? DefaultDesiDivisor { get; set; } // Pazaryerine özel desi böleni (varsa ürün ayarını ezer)
        public bool IsActive { get; set; } = true; // Pazaryeri entegrasyonu aktif mi

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
