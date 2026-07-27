using System;

namespace data._Products
{
    /// <summary>
    /// ÇOKLU PARA BİRİMİ fiyat tablosu (4 para birimi: USD, TRY, EUR, AZN).
    ///
    /// Kapsam kuralı:
    ///  • <see cref="ProductVariantId"/> = null → ürünün GENEL fiyatı (varyant
    ///    override etmemişse tüm varyantlar için geçerli varsayılan).
    ///  • <see cref="ProductVariantId"/> dolu  → o varyanta ÖZEL fiyat (önceliklidir).
    ///
    /// Tekillik: (ProductId, ProductVariantId, Currency) benzersizdir — aynı kapsam
    /// için aynı para biriminde ikinci satır açılamaz.
    ///
    /// Kur çevrimi: Satıcı yalnızca bir para biriminde fiyat girip diğerlerini
    /// sisteme bıraktığında, BackgroundService mevcut <see cref="MoneyExchangeRate"/>
    /// tablosundan çevirir ve <see cref="IsAutoConverted"/> = true işaretler;
    /// satıcı o para biriminde elle fiyat girerse bayrak false'a döner ve otomatik
    /// çevrim o satırın üzerine bir daha yazmaz.
    /// </summary>
    public class ProductPrices : ProductEntityBase
    {
        /// <summary>Bağlı ürün (Products.Id).</summary>
        public Guid ProductId { get; set; }

        /// <summary>Bağlı varyant (ProductVariants.Id — null = ürün geneli fiyat).</summary>
        public Guid? ProductVariantId { get; set; }

        /// <summary>Para birimi (Usd / Try / Eur / Azn).</summary>
        public CurrencyCode Currency { get; set; }

        /// <summary>
        /// Liste (üstü çizili / piyasa) fiyatı — KDV dâhil.
        /// Null = liste fiyatı gösterilmez, yalnızca satış fiyatı vardır.
        /// </summary>
        public decimal? ListPrice { get; set; }

        /// <summary>Güncel satış fiyatı — KDV dâhil, müşteri bu fiyatı öder.</summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// İndirimli kampanya fiyatı (opsiyonel). Yalnızca
        /// DiscountStartDate–DiscountEndDate aralığında geçerlidir; aralık dışında
        /// SalePrice uygulanır.
        /// </summary>
        public decimal? DiscountedPrice { get; set; }

        /// <summary>İndirim başlangıcı (UTC). Null = indirim tanımlı değil.</summary>
        public DateTime? DiscountStartDate { get; set; }

        /// <summary>İndirim bitişi (UTC). Null = süresiz indirim.</summary>
        public DateTime? DiscountEndDate { get; set; }

        /// <summary>
        /// Bu satır kur üzerinden OTOMATİK mi üretildi?
        /// true = MoneyExchangeRate ile çevrildi (kur güncellenince yeniden hesaplanabilir);
        /// false = satıcı elle girdi (otomatik çevrim ASLA üzerine yazmaz).
        /// </summary>
        public bool IsAutoConverted { get; set; }

        /// <summary>Otomatik çevrimde kaynak alınan para birimi (izlenebilirlik).</summary>
        public CurrencyCode? ConvertedFromCurrency { get; set; }

        /// <summary>Otomatik çevrimde kullanılan kur değeri (izlenebilirlik).</summary>
        public decimal? ConversionRateUsed { get; set; }

        /// <summary>Fiyat satırı aktif mi? (false = bu para biriminde satışa kapalı).</summary>
        public bool IsActive { get; set; } = true;
    }
}
