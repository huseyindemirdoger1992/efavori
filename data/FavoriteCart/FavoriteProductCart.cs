using data._Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data.FavoriteCart
{
    public class FavoriteProductCart
    {
        [Key]
        // Sepet kaydının benzersiz kimliği.
        public Guid Id { get; set; } = Guid.NewGuid();

        // Sepetin ait olduğu kullanıcı kimliği.
        public Guid UserId { get; set; }

        // Ürünü satan mağazanın benzersiz kimliği.
        public Guid StoreId { get; set; }

        // Ürünün benzersiz kimliği.
        public Guid ProductId { get; set; }

        // Ürün varyasyonunun benzersiz kimliği.
        public Guid? ProductVariantId { get; set; }

        // Ürünün SEO uyumlu URL adı.
        public string ProductSlug { get; set; }

        // Ürünün kısa açıklaması.
        public string ProductShortDescription { get; set; }

        // Ürünün ana görsel adresi.
        public string ProductImageUrl { get; set; }

        // Ürünün markası.
        public string BrandName { get; set; }

        // Ürünün kategorisi.
        public int CategoryName { get; set; }

        // Ürünün stok kodu.
        public string SKU { get; set; }

        // Ürünün barkod numarası.
        public string Barcode { get; set; }


        // Dolar (USD) Fiyatlandırmaları
        public decimal SalePriceUsd { get; set; }
        public decimal DiscountAmountUsd { get; set; }
        public decimal ShippingPriceUsd { get; set; }


        // TL (TRY) Fiyatlandırmaları
        public decimal SalePriceTry { get; set; }
        public decimal DiscountAmountTry { get; set; }
        public decimal ShippingPriceTry { get; set; }

        // Euro (EUR) Fiyatlandırmaları
        public decimal SalePriceEur { get; set; }
        public decimal DiscountAmountEur { get; set; }
        public decimal ShippingPriceEur { get; set; }

        // Manat (AZN) Fiyatlandırmaları
        public decimal SalePriceAzn { get; set; }
        public decimal DiscountAmountAzn { get; set; }
        public decimal ShippingPriceAzn { get; set; }

        // Kullanıcının satın almak istediği adet.
        public int Quantity { get; set; }

        // Ürüne uygulanan kupon kodu.
        public string CouponCode { get; set; }

        // Ürüne ait KDV oranı.
        public decimal VatRate { get; set; }

        // Ürünün ağırlık bilgisi.
        public decimal? Weight { get; set; }

        // Ürünün desi bilgisi.
        public decimal? Desi { get; set; }

        // Ürünün teslimat süresi bilgisi.
        public string DeliveryTimeText { get; set; }

        // Kullanıcının ürüne eklediği not.
        public string CustomerNote { get; set; }

        // Ürünün sepete eklendiği tarih.
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public IsDeleted? IsDeleted { get; set; } = new();

    }
}
