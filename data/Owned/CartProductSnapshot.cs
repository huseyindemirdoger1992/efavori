using Microsoft.EntityFrameworkCore;

namespace data.Owned
{
    /// <summary>
    /// SEPET ÜRÜN ANLIK GÖRÜNTÜSÜ (snapshot) — sepete eklendiği andaki ürün bilgileri.
    ///
    /// Sepet, sipariş kadar katı bir değişmezlik (immutability) gerektirmez; ancak
    /// ürün adı/fiyatı değiştiğinde kullanıcıya "fiyat güncellendi" uyarısı
    /// gösterebilmek için eklenme anındaki değerler saklanır.
    ///
    /// MEDYA KURALI (§72): Burada fiziksel görsel URL'i TUTULMAZ. Sepet satırı
    /// görselini <c>Products</c> → <c>MediaItems</c> üzerinden çözer; eski
    /// <c>ProductImageUrl</c> alanı kaldırılmış, yerine merkezî medya kimliği
    /// (<see cref="ProductImageMediaId"/>) konmuştur.
    ///
    /// NULLABLE UYUMU (§52): Tüm metin alanları nullable'dır; parasal alanlar
    /// varsayılan 0'dır.
    /// </summary>
    [Owned]
    public class CartProductSnapshot
    {
        /// <summary>Eklenme anındaki ürün adı.</summary>
        public string? ProductName { get; set; }

        /// <summary>Eklenme anındaki kısa açıklama.</summary>
        public string? ProductShortDescription { get; set; }

        /// <summary>Ürün görselinin merkezî medya kimliği (data._Galleries.Media.Id).</summary>
        public Guid? ProductImageMediaId { get; set; }

        /// <summary>Eklenme anındaki marka adı.</summary>
        public string? BrandName { get; set; }

        /// <summary>Eklenme anındaki birincil kategori adı.</summary>
        public string? CategoryName { get; set; }

        /// <summary>Stok kodu.</summary>
        public string? Sku { get; set; }

        /// <summary>Barkod / GTIN.</summary>
        public string? Barcode { get; set; }

        // ── Fiyatlar (4 para birimi — mevcut çoklu para birimi deseni) ────────
        /// <summary>Satış fiyatı (USD).</summary>
        public decimal SalePriceUsd { get; set; }

        /// <summary>İndirim tutarı (USD).</summary>
        public decimal DiscountAmountUsd { get; set; }

        /// <summary>Kargo ücreti (USD).</summary>
        public decimal ShippingPriceUsd { get; set; }

        /// <summary>Satış fiyatı (TRY).</summary>
        public decimal SalePriceTry { get; set; }

        /// <summary>İndirim tutarı (TRY).</summary>
        public decimal DiscountAmountTry { get; set; }

        /// <summary>Kargo ücreti (TRY).</summary>
        public decimal ShippingPriceTry { get; set; }

        /// <summary>Satış fiyatı (EUR).</summary>
        public decimal SalePriceEur { get; set; }

        /// <summary>İndirim tutarı (EUR).</summary>
        public decimal DiscountAmountEur { get; set; }

        /// <summary>Kargo ücreti (EUR).</summary>
        public decimal ShippingPriceEur { get; set; }

        /// <summary>Satış fiyatı (AZN).</summary>
        public decimal SalePriceAzn { get; set; }

        /// <summary>İndirim tutarı (AZN).</summary>
        public decimal DiscountAmountAzn { get; set; }

        /// <summary>Kargo ücreti (AZN).</summary>
        public decimal ShippingPriceAzn { get; set; }

        // ── Sepet satırı ──────────────────────────────────────────────────────
        /// <summary>Adet.</summary>
        public int Quantity { get; set; } = 1;

        /// <summary>Uygulanan kupon kodu.</summary>
        public string? CouponCode { get; set; }

        /// <summary>KDV oranı (yüzde).</summary>
        public decimal VatRate { get; set; }

        /// <summary>Ağırlık.</summary>
        public decimal? Weight { get; set; }

        /// <summary>Desi (hacimsel ağırlık).</summary>
        public decimal? Desi { get; set; }

        /// <summary>Tahmini teslimat metni ("2-4 iş günü").</summary>
        public string? DeliveryTimeText { get; set; }

        /// <summary>Müşteri notu.</summary>
        public string? CustomerNote { get; set; }
    }
}
