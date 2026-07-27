using data._Products;

namespace data._Promotions
{
    /// <summary>
    /// KUPON TANIMI. Kod, müşteri tarafından checkout'ta girilir.
    ///
    /// KAPSAM: Hiç CouponScopes satırı yoksa kupon GLOBAL'dir. Satır varsa yalnızca eşleşen
    /// kalemler indirime esas alınır (dışlama satırları dâhil etmeyi ezer).
    ///
    /// LİMİT KONTROLÜ: TotalUsageLimit ve PerUserUsageLimit, CouponUsages tablosundan türetilir;
    /// eşzamanlılık altında doğru çalışması için UsedCount denormalize sayacı koşullu UPDATE ile
    /// artırılır (README §5 — bu, modülün en kritik yarış koşuludur).
    ///
    /// HAKEDİŞ ETKİSİ: CostBearer alanı Faz 2 SellerLedgerEntries girişlerini belirler
    /// (bkz. CouponCostBearer XML doc).
    /// </summary>
    public class Coupons : PromotionEntityBase
    {
        /// <summary>Kupon kodu ("YAZ2026"). Soft-delete filtreli TEKİL. Büyük harfe normalize edilerek saklanır.</summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>Yönetim ekranındaki açıklayıcı ad ("Yaz indirimi - yeni üyeler").</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Müşteriye gösterilecek açıklama.</summary>
        public string? Description { get; set; }

        /// <summary>Kuponu tanımlayan mağaza (Store.Id). Null = PLATFORM kuponu.</summary>
        public Guid? OwnerStoreId { get; set; }

        /// <summary>İndirim tipi.</summary>
        public CouponType CouponType { get; set; }

        /// <summary>İndirim değeri. Percentage'ta yüzde (örn. 15.00), FixedAmount/ShippingDiscount'ta tutar.
        /// FreeShipping'te kullanılmaz.</summary>
        public decimal Value { get; set; }

        /// <summary>Tutarların para birimi. Percentage tipinde de doldurulur:
        /// MinOrderAmount ve MaxDiscountAmount bu para birimindedir.</summary>
        public CurrencyCode Currency { get; set; }

        /// <summary>Yüzde kuponlarda indirim tavanı ("%20, en fazla 100 TL"). Null = tavan yok.</summary>
        public decimal? MaxDiscountAmount { get; set; }

        /// <summary>Kuponun geçerli olması için asgari sepet tutarı. Null = alt sınır yok.</summary>
        public decimal? MinOrderAmount { get; set; }

        /// <summary>Kuponun geçerli olması için asgari kalem adedi. Null = sınır yok.</summary>
        public int? MinItemCount { get; set; }

        // ---------------- KULLANIM LİMİTLERİ ----------------

        /// <summary>Toplam kullanım limiti (tüm kullanıcılar). Null = sınırsız.</summary>
        public int? TotalUsageLimit { get; set; }

        /// <summary>Kullanıcı başına kullanım limiti. Null = sınırsız.</summary>
        public int? PerUserUsageLimit { get; set; }

        /// <summary>Kullanılmış adet. DENORMALIZE SAYAÇ; KAYNAK: CouponUsages satır sayısı.
        /// Limit kontrolü bu alan üzerinden koşullu UPDATE ile yapılır (README §5);
        /// gecelik mutabakat görevi CouponUsages ile karşılaştırır.</summary>
        public int UsedCount { get; set; }

        // ---------------- BÜTÇE ----------------

        /// <summary>Toplam indirim bütçesi tavanı. Bu tutara ulaşılınca kupon otomatik kapatılır.
        /// Null = bütçe tavanı yok.</summary>
        public decimal? BudgetCap { get; set; }

        /// <summary>Şimdiye kadar verilen toplam indirim. DENORMALIZE SAYAÇ; KAYNAK: CouponUsages.DiscountAmount toplamı.</summary>
        public decimal BudgetUsed { get; set; }

        // ---------------- GEÇERLİLİK ----------------

        /// <summary>Geçerlilik başlangıcı (UTC).</summary>
        public DateTime ValidFromUtc { get; set; }

        /// <summary>Geçerlilik sonu (UTC).</summary>
        public DateTime ValidToUtc { get; set; }

        /// <summary>Kupon aktif mi? Elle kapatma anahtarı; tarih aralığından bağımsızdır.</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Yalnızca hiç siparişi olmayan kullanıcılara mı geçerli? ("Yeni üyeye özel")</summary>
        public bool IsNewUserOnly { get; set; }

        /// <summary>Yalnızca belirli bir kullanıcıya özel kupon (Users.Id). Null = herkese açık.
        /// Kişiye özel telafi/jest kuponları için.</summary>
        public Guid? TargetUserId { get; set; }

        /// <summary>Başka kuponlarla birlikte kullanılabilir mi? false = sepette tek kupon.</summary>
        public bool IsStackable { get; set; }

        /// <summary>İndirimli (kampanyalı) ürünlere de uygulanır mı? false = yalnızca tam fiyatlı kalemler.</summary>
        public bool AppliesToDiscountedItems { get; set; } = true;

        // ---------------- MALİYET PAYLAŞIMI ----------------

        /// <summary>İndirim maliyetini kim üstlenir.</summary>
        public CouponCostBearer CostBearer { get; set; } = CouponCostBearer.Platform;

        /// <summary>CostBearer=Shared iken platformun üstlendiği yüzde (örn. 50.00).</summary>
        public decimal? PlatformSharePercent { get; set; }
    }

    /// <summary>
    /// KUPON KAPSAM SATIRI. Kuponun hangi mağaza/kategori/ürün/varyant/markaya uygulanacağını
    /// belirler. Bir kupon birden çok kapsam satırı taşıyabilir (mantıksal VEYA ilişkisi).
    /// IsExcluded=true satırlar dışlamadır ve dâhil etmeyi EZER.
    /// </summary>
    public class CouponScopes : PromotionEntityBase
    {
        /// <summary>Bağlı kupon (Coupons.Id).</summary>
        public Guid CouponId { get; set; }

        /// <summary>Kapsam seviyesi.</summary>
        public CouponScopeType ScopeType { get; set; }

        /// <summary>Mağaza (Store.Id). ScopeType=Store iken dolu.</summary>
        public Guid? StoreId { get; set; }

        /// <summary>Kategori (CategoriesProduct.Id). ScopeType=Category iken dolu; alt kategoriler de kapsanır.</summary>
        public Guid? CategoryId { get; set; }

        /// <summary>Ürün (Products.Id). ScopeType=Product iken dolu.</summary>
        public Guid? ProductId { get; set; }

        /// <summary>Varyant (ProductVariants.Id). ScopeType=Variant iken dolu.</summary>
        public Guid? VariantId { get; set; }

        /// <summary>Marka (Brands.Id). ScopeType=Brand iken dolu.</summary>
        public Guid? BrandId { get; set; }

        /// <summary>true = bu kapsam kupondan DIŞLANIR. Dışlama, dâhil etme satırlarını ezer.</summary>
        public bool IsExcluded { get; set; }
    }

    /// <summary>
    /// KUPON KULLANIM KAYDI (append-only). Kupon bir siparişte kullanıldığında YENİ SATIR yazılır;
    /// satırlar ASLA güncellenmez. Limit kontrolünün doğruluk kaynağıdır
    /// (Coupons.UsedCount yalnızca hızlandırıcı denormalize sayaçtır).
    ///
    /// SİPARİŞ İPTALİ: Satır SİLİNMEZ. İptal/iade durumunda IsReverted=true işaretlenir ve
    /// Coupons.UsedCount koşullu UPDATE ile geri azaltılır — böylece kullanıcı kuponu
    /// yeniden kullanabilir ama kullanım denetim izi kaybolmaz.
    /// </summary>
    public class CouponUsages : PromotionEntityBase
    {
        /// <summary>Kullanılan kupon (Coupons.Id).</summary>
        public Guid CouponId { get; set; }

        /// <summary>Kuponu kullanan kullanıcı (Users.Id). Misafir siparişte null; ayrım GuestEmail ile yapılır.</summary>
        public Guid? UserId { get; set; }

        /// <summary>Misafir kullanımında e-posta (kullanıcı başına limit bu alanla uygulanır).</summary>
        public string? GuestEmail { get; set; }

        /// <summary>Kuponun uygulandığı sipariş (Orders.Id).</summary>
        public Guid OrderId { get; set; }

        /// <summary>Fiilen verilen indirim tutarı. Coupons.BudgetUsed'ın kaynağıdır.</summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>İndirimin para birimi.</summary>
        public CurrencyCode Currency { get; set; }

        /// <summary>Snapshot: kullanım anındaki maliyet üstlenici. Kupon sonradan değişse bile
        /// hakediş hesabı bozulmaz (Faz 2 ledger dayanağı).</summary>
        public CouponCostBearer CostBearerSnapshot { get; set; }

        /// <summary>Snapshot: platform payı yüzdesi (CostBearer=Shared iken).</summary>
        public decimal? PlatformSharePercentSnapshot { get; set; }

        /// <summary>Kullanım geri alındı mı? (sipariş iptali/tam iade). true iken limit hesabına GİRMEZ.</summary>
        public bool IsReverted { get; set; }

        /// <summary>Geri alınma anı (UTC).</summary>
        public DateTime? RevertedAtUtc { get; set; }

        /// <summary>Global tekil idempotency anahtarı. Deterministik: "cpnuse:{couponId}:{orderId}";
        /// aynı siparişte kuponun iki kez sayılması DB seviyesinde engellenir.</summary>
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}