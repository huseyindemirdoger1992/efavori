using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Varyantlı ürünlerin tek bir kombinasyonunu (Örn: "Kırmızı / XL") temsil eden tablo.
    /// ProductId ile ana ürüne bağlanır. Kombinasyonun hangi değerlerden oluştuğu ProductVariantValue'da tutulur.
    /// </summary>
    public class ProductVariant
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; } // Ürünü ekleyen kullanıcının/satıcının ID bilgisi
        public Guid StoreId { get; set; } // Ürünün bağlı olduğu mağazanın ID bilgisi
        public Guid ProductId { get; set; } // Varyantın bağlı olduğu ana ürünün ID'si

        public string? SKU { get; set; } // Varyanta özel stok kodu
        public string? Barcode { get; set; } // Varyanta özel GTIN/EAN/UPC barkodu (pazaryeri eşleştirme anahtarı)
        public int StockQuantity { get; set; } // Varyantın stok adedi (tek depo senaryosu)
        public string? ImagePath { get; set; } // Varyanta özel ana görsel yolu (renk bazlı görsel)
        public bool IsDefault { get; set; } // Varsayılan/öne çıkan kombinasyon mu
        public int DisplayOrder { get; set; } = 0; // Varyantların listelenme sırası
        public string? VariantTitle { get; set; } // İnsan okunabilir kombinasyon adı (Örn: "Kırmızı / XL")

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public DateTime? UpdatedAt { get; set; } // Son güncelleme tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
