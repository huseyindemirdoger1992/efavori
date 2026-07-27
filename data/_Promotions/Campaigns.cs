using data._Products;

namespace data._Promotions
{
    /// <summary>
    /// ZAMANLI İNDİRİM KAMPANYASI ("Yaz İndirimi", "Black Friday").
    ///
    /// ProductPrices.DiscountedPrice İLE İLİŞKİSİ — TASARIM KARARI (README §6):
    /// Kampanya motoru fiyat satırını YAZAR (materyalize eder), sorgu anında hesaplamaz.
    /// Başlangıçta ProductPrices.DiscountedPrice + DiscountStart/EndUtc alanlarına kampanya fiyatı
    /// yazılır; bitişte geri alınır. Her iki yazım da Faz 3 ProductPriceHistory'e
    /// Campaign / CampaignExpired kaynağıyla iz düşer.
    ///
    /// Gerekçe özeti: ürün listeleme sorguları (milyonlarca satır, fiyat aralığı filtresi,
    /// fiyata göre sıralama) kampanya kurallarını sorgu anında çözemez — indeks kullanılamaz hale
    /// gelir. Ayrıca Omnibus mevzuatı fiyatın ne zaman neye düştüğünün defterde durmasını gerektirir.
    ///
    /// BÜTÇE: BudgetCap dolu ise, kampanyadan doğan toplam indirim (sipariş kalemlerinden hesaplanır)
    /// tavana ulaşınca Status=BudgetExhausted'a çekilir ve fiyatlar geri alınır.
    /// </summary>
    public class Campaigns : PromotionEntityBase
    {
        /// <summary>Kampanya adı ("Black Friday 2026").</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Makine-okur kod ("bf2026"). Soft-delete filtreli tekil.</summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>Açıklama.</summary>
        public string? Description { get; set; }

        /// <summary>Kampanyayı tanımlayan mağaza (Store.Id). Null = PLATFORM kampanyası.</summary>
        public Guid? OwnerStoreId { get; set; }

        /// <summary>İndirim değeri tipi.</summary>
        public CampaignDiscountType DiscountType { get; set; }

        /// <summary>İndirim değeri. Percentage'ta yüzde, FixedAmount'ta indirilecek tutar,
        /// FixedPrice'ta çekilecek sabit satış fiyatı.</summary>
        public decimal DiscountValue { get; set; }

        /// <summary>Tutarların para birimi. Percentage tipinde de doldurulur (tavan/bütçe için).</summary>
        public CurrencyCode Currency { get; set; }

        /// <summary>Yüzde kampanyalarda ürün başına indirim tavanı. Null = tavan yok.</summary>
        public decimal? MaxDiscountAmount { get; set; }

        /// <summary>Kampanya durumu.</summary>
        public CampaignStatus Status { get; set; } = CampaignStatus.Draft;

        /// <summary>Başlangıç (UTC). Bu anda BackgroundService fiyatları yazar.</summary>
        public DateTime StartsAtUtc { get; set; }

        /// <summary>Bitiş (UTC). Bu anda BackgroundService fiyatları geri alır.</summary>
        public DateTime EndsAtUtc { get; set; }

        /// <summary>Toplam indirim bütçesi tavanı. Null = tavan yok.</summary>
        public decimal? BudgetCap { get; set; }

        /// <summary>Şimdiye kadar verilen toplam indirim. DENORMALIZE SAYAÇ;
        /// KAYNAK: OrderItems.DiscountAmount (kampanyaya isabet eden pay); BackgroundService besler.</summary>
        public decimal BudgetUsed { get; set; }

        /// <summary>Vitrin görseli (MediaItems.Id) — kampanya sayfası/banner için.</summary>
        public Guid? BannerMediaItemId { get; set; }

        /// <summary>Kampanya sayfası bağlantısı (slug).</summary>
        public string? Slug { get; set; }

        /// <summary>Öncelik. Aynı ürüne birden çok kampanya isabet ederse küçük değer kazanır
        /// (kampanyalar BİRLEŞTİRİLMEZ; tek kampanya uygulanır — README §6).</summary>
        public int Priority { get; set; }

        /// <summary>Fiyat yazımının fiilen tamamlandığı an (UTC). Idempotency izi:
        /// dolu ise motor bu kampanyayı yeniden uygulamaz.</summary>
        public DateTime? PricesAppliedAtUtc { get; set; }

        /// <summary>Fiyatların geri alındığı an (UTC).</summary>
        public DateTime? PricesRevertedAtUtc { get; set; }

        /// <summary>Etkilenen fiyat satırı adedi. DENORMALIZE SAYAÇ; KAYNAK: motorun yazdığı ProductPrices satır sayısı.</summary>
        public int AffectedPriceRowCount { get; set; }

        // ---------------- LEASE DESENİ (fiyat yazma/geri alma kuyruğu) ----------------

        /// <summary>Lease sahibi işleyici kimliği. Çok örnekli BackgroundService'te aynı kampanyanın
        /// iki kez uygulanmasını engeller.</summary>
        public string? LeasedBy { get; set; }

        /// <summary>Lease bitiş zamanı (UTC).</summary>
        public DateTime? LeasedUntilUtc { get; set; }

        /// <summary>Deneme sayısı.</summary>
        public int AttemptCount { get; set; }

        /// <summary>Azami deneme.</summary>
        public int MaxAttempts { get; set; } = 3;

        /// <summary>Bir sonraki deneme zamanı (UTC).</summary>
        public DateTime? NextRetryAtUtc { get; set; }
    }

    /// <summary>
    /// KAMPANYA KAPSAM SATIRI. Kampanyanın hangi mağaza/kategori/ürün/varyant/markaya
    /// uygulanacağını belirler. Kapsam satırı YOKSA kampanya hiçbir ürüne uygulanmaz —
    /// kuponun aksine "boş kapsam = global" DEĞİLDİR, çünkü tüm katalogu yanlışlıkla
    /// indirime sokmak geri dönüşü pahalı bir hatadır.
    /// </summary>
    public class CampaignScopes : PromotionEntityBase
    {
        /// <summary>Bağlı kampanya (Campaigns.Id).</summary>
        public Guid CampaignId { get; set; }

        /// <summary>Kapsam seviyesi.</summary>
        public CampaignScopeType ScopeType { get; set; }

        /// <summary>Mağaza (Store.Id).</summary>
        public Guid? StoreId { get; set; }

        /// <summary>Kategori (CategoriesProduct.Id); alt kategoriler dâhil.</summary>
        public Guid? CategoryId { get; set; }

        /// <summary>Ürün (Products.Id).</summary>
        public Guid? ProductId { get; set; }

        /// <summary>Varyant (ProductVariants.Id).</summary>
        public Guid? VariantId { get; set; }

        /// <summary>Marka (Brands.Id).</summary>
        public Guid? BrandId { get; set; }

        /// <summary>true = bu kapsam kampanyadan DIŞLANIR; dâhil etmeyi ezer.</summary>
        public bool IsExcluded { get; set; }

        /// <summary>Bu kapsam satırına özel indirim değeri. Null = kampanyanın genel değeri kullanılır.
        /// ("Tüm elektronik %10, ama telefon kategorisi %15" gibi kuralları tek kampanyada ifade eder.)</summary>
        public decimal? OverrideDiscountValue { get; set; }
    }
}