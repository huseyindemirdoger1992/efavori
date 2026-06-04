using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ana ürün tablosu (entity). Hem basit (varyantsız) hem de varyantlı ürünlerin ortak gövdesini tutar.
    /// ProductCreateRequest DTO'su bu tabloya map'lenir. İlişkiler yalnızca ID iledir; collection navigation yoktur.
    /// </summary>
    public class Product
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; } // Ürünü ekleyen kullanıcının/satıcının ID bilgisi
        public Guid StoreId { get; set; } // Ürünün bağlı olduğu mağazanın ID bilgisi
        public int? CategoryId { get; set; } // Yerel kategori ID'si (CategoriesTr.Id -> int) (opsiyonel)

        public string Name { get; set; } = string.Empty; // Müşteriye görünen tam ürün adı (SEO başlığı)
        public string? Description { get; set; } // Kısa SEO açıklaması (meta description)
        public string? Content { get; set; } // Detaylı ürün açıklaması (HTML destekli)
        public string UniqueNameSlug { get; set; } = string.Empty; // URL için benzersiz slug
        public string? Brand { get; set; } // Marka adı (filtreleme ve pazaryeri eşleştirmesi için)
        public string? AiSeoKeywords { get; set; } // AI ile üretilen SEO anahtar kelimeleri

        public bool IsActive { get; set; } = true; // Ürün satışta mı (aktif/pasif)
        public bool IsFeatured { get; set; } = false; // Vitrinde öne çıkarılsın mı

        public string? SpecificationsJson { get; set; } // Varyant oluşturmayan serbest teknik özellikler (JSON metni)

        public bool ProductIsVariant { get; set; } = false; // Varyant sistemini açan bayrak

        // ---- Yalnızca basit ürün (ProductIsVariant = false) iken doldurulur ----
        public string? SKU { get; set; } // Basit ürünün stok kodu
        public string? Barcode { get; set; } // Basit ürünün GTIN/EAN/UPC barkodu
        public int StockQuantity { get; set; } // Basit ürünün stok adedi (tek depo senaryosu)
        public string? ImagePath { get; set; } // Basit ürünün ana görsel yolu/URL'si

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public DateTime? UpdatedAt { get; set; } // Son güncelleme tarihi (pazaryeri senkronu için)

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
