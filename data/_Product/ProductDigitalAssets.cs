using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Dijital ürün dosyaları (ProductType = "Digital").
    /// Satın alma sonrası indirilebilir dosyalar ve indirme kuralları burada tutulur.
    /// </summary>
    public class ProductDigitalAssets
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Ürün (Products.Id)
        public Guid? VariantId { get; set; } // Dosya belirli bir varyanta özelse (ProductVariants.Id)

        public Guid MediaId { get; set; } // İndirilebilir dosya (Media.Id)
        public string? DisplayName { get; set; } // Müşteriye gösterilecek dosya adı

        public int? DownloadLimit { get; set; } // Maksimum indirme sayısı (null = sınırsız)
        public int? ExpirationDays { get; set; } // Satın alma sonrası geçerlilik süresi - gün (null = süresiz)
        public bool RequiresLicenseKey { get; set; } = false; // Lisans anahtarı üretilsin mi

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    }
}
