using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ürün varyantları. Birleşik tek-varyant modeli gereği basit ürünlerde de
    /// IsDefault = true olan tek bir varyant kaydı bulunur.
    /// Fiyat (ProductPrices) ve stok (ProductStocks) daima varyant üzerinden tutulur.
    /// </summary>
    public class ProductVariants
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Bağlı olduğu ürün (Products.Id)

        // === Kimlik / Tanımlayıcı Alanlar (Marketplace entegrasyon standartları) ===
        public string? Sku { get; set; } // Stok kodu (Örn: KZK-KRM-S)
        public string? Barcode { get; set; } // Barkod
        public string? Gtin { get; set; } // Global Trade Item Number
        public string? Upc { get; set; } // Universal Product Code
        public string? Ean { get; set; } // European Article Number
        public string? Isbn { get; set; } // Kitaplar için ISBN
        public string? Mpn { get; set; } // Manufacturer Part Number (Üretici parça numarası)

        // === Durum ===
        public bool IsDefault { get; set; } = false; // Varsayılan varyant mı (basit üründe tek kayıt true)
        public bool IsActive { get; set; } = true; // Varyant satışta mı
        public int DisplayOrder { get; set; } = 0; // Listeleme sırası

        // === Fiziksel Özellikler (kargo/desi hesabı ShippingProfile bölen değerleri ile yapılır) ===
        public decimal? WeightKg { get; set; } // Ağırlık (kg)
        public decimal? WidthCm { get; set; } // Genişlik (cm)
        public decimal? HeightCm { get; set; } // Yükseklik (cm)
        public decimal? LengthCm { get; set; } // Uzunluk (cm)

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public DateTime? UpdatedAt { get; set; } // Son güncelleme tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
