using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace data.Owned
{
    [Owned]
    public class CartProductSnapshot
    {
        public string ProductShortDescription { get; set; }
        public string ProductImageUrl { get; set; }

        public string BrandName { get; set; }
        public int CategoryName { get; set; }

        public string SKU { get; set; }
        public string Barcode { get; set; }

        public decimal SalePriceUsd { get; set; }
        public decimal DiscountAmountUsd { get; set; }
        public decimal ShippingPriceUsd { get; set; }

        public decimal SalePriceTry { get; set; }
        public decimal DiscountAmountTry { get; set; }
        public decimal ShippingPriceTry { get; set; }

        public decimal SalePriceEur { get; set; }
        public decimal DiscountAmountEur { get; set; }
        public decimal ShippingPriceEur { get; set; }

        public decimal SalePriceAzn { get; set; }
        public decimal DiscountAmountAzn { get; set; }
        public decimal ShippingPriceAzn { get; set; }

        public int Quantity { get; set; }
        public string CouponCode { get; set; }

        public decimal VatRate { get; set; }

        public decimal? Weight { get; set; }
        public decimal? Desi { get; set; }

        public string DeliveryTimeText { get; set; }
        public string CustomerNote { get; set; }

    }
}
