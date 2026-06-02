using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data
{
    /// <summary>
    /// 1. ADIM — Ana ürün oluşturma isteği. SADECE Product tablosunun alanlarını taşır.
    /// İç içe varyant/fiyat listesi YOKTUR. Sunucu bu kaydı oluşturur ve dönen ProductId,
    /// sonraki adımların (varyant, fiyat, özellik) istek modellerinde ID olarak kullanılır.
    /// </summary>
    public class ProductCreateRequest
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; } // Ürünü ekleyen kullanıcının/satıcının ID bilgisi
        public Guid StoreId { get; set; } // Ürünün bağlı olduğu mağazanın ID bilgisi
        public Guid? CategoryId { get; set; } // Ürünün ait olduğu yerel kategori ID'si (opsiyonel)

        public string Name { get; set; } = string.Empty; // Ürünün müşteriye görünen tam adı (SEO başlığı, 50-60 karakter)
        public string? Description { get; set; } // Kısa SEO açıklaması (meta description, 140-160 karakter)
        public string? Content { get; set; } // Ürün sayfasındaki detaylı açıklama (HTML destekli)
        public string UniqueNameSlug { get; set; } = string.Empty; // URL için benzersiz slug (Örn: iphone-18-pro-max-256gb)
        public string? Brand { get; set; } // Marka adı (filtreleme ve pazaryeri eşleştirmesi için)
        public string? AiSeoKeywords { get; set; } // AI ile üretilen SEO anahtar kelimeleri

        public bool IsActive { get; set; } = true; // Ürün satışta mı (aktif/pasif durumu)
        public bool IsFeatured { get; set; } = false; // Ürün vitrinde/ana sayfada öne çıkarılsın mı

        public string? SpecificationsJson { get; set; } // Varyant oluşturmayan serbest teknik özellikler (hazır JSON metni)

        public bool ProductIsVariant { get; set; } = false; // Varyant sistemini açan checkbox değeri

        // ---- Sadece basit ürün (ProductIsVariant = false) iken doldurulur ----
        public string? SKU { get; set; } // Basit ürünün stok kodu (Stock Keeping Unit)
        public string? Barcode { get; set; } // Basit ürünün GTIN/EAN/UPC barkodu
        public int StockQuantity { get; set; } // Basit ürünün güncel stok adedi
        public string? ImagePath { get; set; } // Basit ürünün ana görsel yolu/URL'si
    }
}
