using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace data
{
    public class Pricing
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; } = Guid.NewGuid();

        #region Fiyatlar (TL)
        [Column(TypeName = "decimal(18,2)")]
        // Ürünün Türk Lirası cinsinden normal satış fiyatı
        public decimal PriceTL { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        // Ürünün Türk Lirası cinsinden indirimli (kampanyalı) satış fiyatı
        public decimal? DiscountPriceTL { get; set; }
        #endregion

        #region Fiyatlar (USD)
        [Column(TypeName = "decimal(18,2)")]
        // Ürünün Amerikan Doları cinsinden normal satış fiyatı
        public decimal PriceUSD { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        // Ürünün Amerikan Doları cinsinden indirimli satış fiyatı
        public decimal? DiscountPriceUSD { get; set; }
        #endregion

        #region Fiyatlar (EUR)
        [Column(TypeName = "decimal(18,2)")]
        // Ürünün Euro cinsinden normal satış fiyatı
        public decimal PriceEUR { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        // Ürünün Euro cinsinden indirimli satış fiyatı
        public decimal? DiscountPriceEUR { get; set; }
        #endregion

        #region Fiyatlar (AZN)
        [Column(TypeName = "decimal(18,2)")]
        // Ürünün Azerbaycan Manatı cinsinden normal satış fiyatı
        public decimal PriceAZN { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        // Ürünün Azerbaycan Manatı cinsinden indirimli satış fiyatı
        public decimal? DiscountPriceAZN { get; set; }
        #endregion

        public DateTime PriceTime { get; set; }
    }
}
