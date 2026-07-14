using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ürün varyantları. Birleşik tek-varyant modeli gereği basit ürünlerde de
    /// IsDefault = true olan tek bir varyant kaydı bulunur.
    /// Fiyat (ProductPrices) ve stok (ProductStocks) daima varyant üzerinden tutulur.
    ///
    /// REVİZYON: Paket ölçüleri eklendi. Amazon/eBay ÜRÜN ölçüsü (item dimensions) ile
    /// PAKET ölçüsünü (package dimensions) ayrı gönderir — kargo/desi hesabı PAKET üzerinden yapılır.
    /// İçe aktarımda birim dönüşümü uygulama katmanında yapılır (inç → cm, lbs → kg);
    /// ham değerler ProductMarketplaceListings.RawSourceData içinde korunur.
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

        // === Fiziksel Özellikler — ÜRÜN ölçüleri (ürün sayfasında gösterilir) ===
        public decimal? WeightKg { get; set; } // Ürün ağırlığı (kg)
        public decimal? WidthCm { get; set; } // Ürün genişliği (cm)
        public decimal? HeightCm { get; set; } // Ürün yüksekliği (cm)
        public decimal? LengthCm { get; set; } // Ürün uzunluğu (cm)

        // === Fiziksel Özellikler — PAKET ölçüleri (kargo/desi hesabı BU değerlerle yapılır) ===
        // Null ise kargo hesabında ürün ölçülerine geri düşülür (fallback).
        public decimal? PackageWeightKg { get; set; } // Paket ağırlığı (kg)
        public decimal? PackageWidthCm { get; set; } // Paket genişliği (cm)
        public decimal? PackageHeightCm { get; set; } // Paket yüksekliği (cm)
        public decimal? PackageLengthCm { get; set; } // Paket uzunluğu (cm)

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public DateTime? UpdatedAt { get; set; } // Son güncelleme tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
