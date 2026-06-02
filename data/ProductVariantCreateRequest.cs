using data._Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data
{
    /// <summary>
    /// Tek bir varyant (kombinasyon) oluşturma isteği. Tek başına gönderilir.
    /// ProductId, daha önce oluşturulan ana ürünün ID'sidir.
    /// Sunucu dönen VariantId'yi hem ProductVariantValueCreateRequest hem PricingCreateRequest'te kullanır.
    /// Varyantın hangi değerlerden oluştuğu BU modelde DEĞİL, ayrı ProductVariantValue istekleriyle kurulur.
    /// </summary>
    public class ProductVariantCreateRequest
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; } // Ürünü ekleyen kullanıcının/satıcının ID bilgisi
        public Guid StoreId { get; set; } // Ürünün bağlı olduğu mağazanın ID bilgisi
        public Guid ProductId { get; set; } // Varyantın bağlı olduğu ana ürünün ID'si
        public string? SKU { get; set; } // Varyanta özel stok kodu
        public string? Barcode { get; set; } // Varyanta özel GTIN/EAN/UPC barkodu (pazaryeri eşleştirme anahtarı)
        public int StockQuantity { get; set; } // Varyantın güncel stok adedi
        public string? ImagePath { get; set; } // Varyanta özel ana görsel yolu (renk bazlı görsel için)
        public string? GalleryJson { get; set; } // Varyanta ait ek görsellerin hazır JSON dizisi
        public bool IsDefault { get; set; } // Bu varyant varsayılan/öne çıkan kombinasyon mu
        public int DisplayOrder { get; set; } = 0; // Varyantların listelenme sırası
        public string? VariantTitle { get; set; } // İnsan okunabilir kombinasyon adı (Örn: "Kırmızı / XL")
        public IsDeleted? IsDeleted { get; set; } = new(); // silinme durumu (soft delete için)

    }
}
