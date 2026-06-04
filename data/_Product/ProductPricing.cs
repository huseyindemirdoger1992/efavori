using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Fiyat tablosu. VariantId = null -> ürün/taban fiyatı; VariantId dolu -> o varyanta özel fiyat (tabanı ezer).
    /// Mevcut kültüre uyum için para birimleri sabit kolon olarak tutulmuştur.
    /// (Alternatif: her satır bir para birimini temsil eden satır-bazlı model — yeni para birimi eklemek sadece veri olur.)
    /// </summary>
    public class ProductPricing
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; } // Fiyatı ekleyen kullanıcının/satıcının ID bilgisi
        public Guid StoreId { get; set; } // Ürünün bağlı olduğu mağazanın ID bilgisi
        public Guid ProductId { get; set; } // Fiyatın ait olduğu ana ürünün ID'si (her zaman dolu)
        public Guid? VariantId { get; set; } // Fiyatın ait olduğu varyantın ID'si (basit/taban fiyatta null)

        public decimal PriceTL { get; set; } // Türk Lirası normal satış fiyatı
        public decimal? DiscountPriceTL { get; set; } // Türk Lirası indirimli fiyatı
        public decimal PriceUSD { get; set; } // Amerikan Doları normal satış fiyatı
        public decimal? DiscountPriceUSD { get; set; } // Amerikan Doları indirimli fiyatı
        public decimal PriceEUR { get; set; } // Euro normal satış fiyatı
        public decimal? DiscountPriceEUR { get; set; } // Euro indirimli fiyatı
        public decimal PriceAZN { get; set; } // Azerbaycan Manatı normal satış fiyatı
        public decimal? DiscountPriceAZN { get; set; } // Azerbaycan Manatı indirimli fiyatı

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public DateTime? UpdatedAt { get; set; } // Son güncelleme tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
