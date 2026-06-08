using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ürün veya varyant fiyat geçmişini saklar.
    /// Her fiyat değişikliği yeni satır olarak eklenir, eski satırlar silinmez.
    /// 4 para birimi (TRY, USD, EUR, AZN) aynı satırda bağımsız tutulur.
    /// Standart ürünlerde VariantId=null, varyantlılarda dolu olur.
    /// </summary>
    public class ProductPrices
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; }    // Ürün ID bilgisi
        public Guid? VariantId { get; set; }   // Varyant ID bilgisi (null ise standart ürün fiyatı)
        public Guid UserId { get; set; }       // Satıcı ID bilgisi

        // === TRY Fiyatları ===
        public decimal? PriceTRY { get; set; }            // TRY normal satış fiyatı
        public decimal? DiscountedPriceTRY { get; set; }  // TRY indirimli fiyat (boşsa indirim yok)
        public decimal? CostPriceTRY { get; set; }        // TRY maliyet fiyatı

        // === USD Fiyatları ===
        public decimal? PriceUSD { get; set; }            // USD normal satış fiyatı
        public decimal? DiscountedPriceUSD { get; set; }  // USD indirimli fiyat
        public decimal? CostPriceUSD { get; set; }        // USD maliyet fiyatı

        // === EUR Fiyatları ===
        public decimal? PriceEUR { get; set; }            // EUR normal satış fiyatı
        public decimal? DiscountedPriceEUR { get; set; }  // EUR indirimli fiyat
        public decimal? CostPriceEUR { get; set; }        // EUR maliyet fiyatı

        // === AZN Fiyatları ===
        public decimal? PriceAZN { get; set; }            // AZN normal satış fiyatı
        public decimal? DiscountedPriceAZN { get; set; }  // AZN indirimli fiyat
        public decimal? CostPriceAZN { get; set; }        // AZN maliyet fiyatı

        // === Geçmiş Takibi ===
        public DateTime? EffectiveFrom { get; set; } = DateTime.UtcNow;  // Bu fiyatın geçerlilik başlangıcı
        public DateTime? EffectiveTo { get; set; }                        // Geçerlilik bitişi (null = aktif fiyat)
        public string? ChangeNote { get; set; }                           // Değişiklik notu (opsiyonel)

        // === Dış Platform Referansı ===
        public string? ExternalPriceId { get; set; }    // Dış platformdaki fiyat referansı

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}