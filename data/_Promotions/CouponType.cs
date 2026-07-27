namespace data._Promotions
{
    // ============================================================================================
    // KAMPANYA VE KUPON SİSTEMİ V1 — ENUM SÖZLÜĞÜ
    // KURAL: Enum değerleri byte olarak saklanır. Mevcut değerlerin ARASINA yeni değer EKLENMEZ,
    // yeni değerler her zaman SONA eklenir. Değer silinmez, yeniden adlandırılmaz.
    // ============================================================================================

    /// <summary>
    /// Kupon indirim tipi (Coupons.CouponType).
    /// </summary>
    public enum CouponType : byte
    {
        /// <summary>Yüzde indirim (Value = yüzde; MaxDiscountAmount ile tavanlanabilir).</summary>
        Percentage = 0,

        /// <summary>Sabit tutar indirimi (Value = tutar, Currency alanındaki para biriminde).</summary>
        FixedAmount = 1,

        /// <summary>Kargo bedava (kargo ücreti sıfırlanır; Value kullanılmaz).</summary>
        FreeShipping = 2,

        /// <summary>Sabit tutar kargo indirimi (kargo ücretinden Value kadar düşülür, sıfırın altına inmez).</summary>
        ShippingDiscount = 3
    }

    /// <summary>
    /// Kupon kapsam satırının hangi seviyeyi işaret ettiği (CouponScopes.ScopeType).
    ///
    /// ÇÖZÜM MANTIĞI: Kuponun hiç kapsam satırı yoksa GLOBAL'dir (tüm sepete uygulanır).
    /// Kapsam satırı varsa yalnızca eşleşen kalemler indirime esas alınır.
    /// IsExcluded=true satırlar dışlamadır ve dâhil etmeyi EZER
    /// ("tüm elektronik, ama X markası hariç" gibi kuralları tek kuponda ifade eder).
    /// </summary>
    public enum CouponScopeType : byte
    {
        /// <summary>Mağaza kapsamı (Store.Id).</summary>
        Store = 0,

        /// <summary>Kategori kapsamı (CategoriesProduct.Id). Alt kategoriler de dâhildir (hiyerarşik).</summary>
        Category = 1,

        /// <summary>Ürün kapsamı (Products.Id).</summary>
        Product = 2,

        /// <summary>Varyant kapsamı (ProductVariants.Id).</summary>
        Variant = 3,

        /// <summary>Marka kapsamı (Brands.Id).</summary>
        Brand = 4
    }

    /// <summary>
    /// Kupon maliyetini kimin üstlendiği (Coupons.CostBearer).
    /// FAZ 2 HAKEDİŞ ETKİSİ:
    /// - Platform: İndirim tutarı satıcının hakedişinden DÜŞÜLMEZ; satıcı tam tutarı alır.
    ///   Ledger'a CouponCompensation (+) girişi yazılır (bkz. LedgerEntryType).
    /// - Store: İndirim satıcının cebinden çıkar; ledger'a ek giriş yazılmaz, tahsilat zaten düşüktür.
    /// - Shared: Yüzde paylaşımına göre kısmi CouponCompensation girişi yazılır.
    /// </summary>
    public enum CouponCostBearer : byte
    {
        /// <summary>Maliyeti platform üstlenir (satıcıya telafi edilir).</summary>
        Platform = 0,

        /// <summary>Maliyeti satıcı üstlenir.</summary>
        Store = 1,

        /// <summary>Maliyet paylaşılır (PlatformSharePercent alanına göre).</summary>
        Shared = 2
    }

    /// <summary>Kampanya indirim değeri tipi (Campaigns.DiscountValueType).</summary>
    public enum CampaignDiscountType : byte
    {
        /// <summary>Yüzde indirim.</summary>
        Percentage = 0,

        /// <summary>Sabit tutar indirimi.</summary>
        FixedAmount = 1,

        /// <summary>Sabit satış fiyatına çekme (ör. "hepsi 99 TL").</summary>
        FixedPrice = 2
    }

    /// <summary>
    /// Kampanya durum makinesi (Campaigns.Status).
    /// Geçişler BackgroundService tarafından tarih alanlarına göre otomatik yürütülür.
    /// </summary>
    public enum CampaignStatus : byte
    {
        /// <summary>Taslak; henüz yayınlanmadı, fiyatlara etki etmez.</summary>
        Draft = 0,

        /// <summary>Yayınlandı, başlangıç tarihi bekleniyor.</summary>
        Scheduled = 1,

        /// <summary>Yürürlükte; fiyatlara etki ediyor.</summary>
        Active = 2,

        /// <summary>Süresi doldu; indirimler geri alındı.</summary>
        Expired = 3,

        /// <summary>Elle durduruldu (süre dolmadan); indirimler geri alındı.</summary>
        Cancelled = 4,

        /// <summary>Bütçe tavanı doldu; otomatik durduruldu.</summary>
        BudgetExhausted = 5
    }

    /// <summary>Kampanya kapsam satırı seviyesi (CampaignScopes.ScopeType). CouponScopeType ile aynı mantık.</summary>
    public enum CampaignScopeType : byte
    {
        /// <summary>Mağaza kapsamı (Store.Id).</summary>
        Store = 0,

        /// <summary>Kategori kapsamı (CategoriesProduct.Id); alt kategoriler dâhil.</summary>
        Category = 1,

        /// <summary>Ürün kapsamı (Products.Id).</summary>
        Product = 2,

        /// <summary>Varyant kapsamı (ProductVariants.Id).</summary>
        Variant = 3,

        /// <summary>Marka kapsamı (Brands.Id).</summary>
        Brand = 4
    }
}