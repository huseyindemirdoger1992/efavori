using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data
{
    /// <summary>
    /// Fiyat oluşturma isteği. Tek başına gönderilir.
    /// VariantId = null -> ürün/taban fiyatı (basit ürün veya varyantlar için varsayılan).
    /// VariantId dolu     -> yalnızca o varyanta ait fiyat (taban fiyatı ezer).
    /// </summary>
    public class ProductPricingCreateRequest
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; } // Fiyatın ait olduğu ana ürünün ID'si (her zaman dolu)
        public Guid? VariantId { get; set; } // Fiyatın ait olduğu varyantın ID'si (basit/taban fiyatta null)

        public decimal PriceTL { get; set; } // Türk Lirası normal satış fiyatı
        public decimal? DiscountPriceTL { get; set; } // Türk Lirası indirimli (kampanyalı) fiyatı
        public decimal PriceUSD { get; set; } // Amerikan Doları normal satış fiyatı
        public decimal? DiscountPriceUSD { get; set; } // Amerikan Doları indirimli fiyatı
        public decimal PriceEUR { get; set; } // Euro normal satış fiyatı
        public decimal? DiscountPriceEUR { get; set; } // Euro indirimli fiyatı
        public decimal PriceAZN { get; set; } // Azerbaycan Manatı normal satış fiyatı
        public decimal? DiscountPriceAZN { get; set; } // Azerbaycan Manatı indirimli fiyatı
    }
}
