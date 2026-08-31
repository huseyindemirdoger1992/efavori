namespace data._Returns
{
    // ============================================================================================
    // İADE VE İHTİLAF SİSTEMİ V1 — ENUM SÖZLÜĞÜ
    // KURAL: Enum değerleri byte olarak saklanır. Mevcut değerlerin ARASINA yeni değer EKLENMEZ,
    // yeni değerler her zaman SONA eklenir. Değer silinmez, yeniden adlandırılmaz.
    // ============================================================================================

    /// <summary>
    /// İade nedeni (ReturnRequestItems.Reason). Kalem bazlıdır: aynı talepte bir ürün
    /// "beden uymadı", diğeri "hasarlı" olabilir.
    ///
    /// KUSUR AYRIMI: IsSellerFault özelliği taşıyan nedenlerde (hasarlı, yanlış ürün, eksik parça)
    /// iade kargo bedeli SATICIYA aittir; cayma hakkı kaynaklı nedenlerde politikaya göre değişir
    /// (bkz. ReturnShippingPayer).
    /// </summary>
    public enum ReturnReason : byte
    {
        /// <summary>Belirtilmedi.</summary>
        Unspecified = 0,

        /// <summary>Beden/ölçü uymadı.</summary>
        SizeMismatch = 1,

        /// <summary>Renk/görsel beklentiyi karşılamadı.</summary>
        ColorMismatch = 2,

        /// <summary>Ürün hasarlı/kırık geldi (SATICI KUSURU).</summary>
        DamagedProduct = 3,

        /// <summary>Yanlış ürün gönderildi (SATICI KUSURU).</summary>
        WrongProductSent = 4,

        /// <summary>Ürün açıklamasıyla uyuşmuyor (SATICI KUSURU).</summary>
        NotAsDescribed = 5,

        /// <summary>Ürün arızalı/çalışmıyor (SATICI KUSURU).</summary>
        Defective = 6,

        /// <summary>Kutuda eksik parça/aksesuar var (SATICI KUSURU).</summary>
        MissingParts = 7,

        /// <summary>Fikir değişikliği / cayma hakkı (satıcı kusuru YOK).</summary>
        ChangedMind = 8,

        /// <summary>Geç teslim edildi, ihtiyaç kalmadı.</summary>
        LateDelivery = 9,

        /// <summary>Daha uygun fiyat bulundu.</summary>
        BetterPriceFound = 10,

        /// <summary>Yanlışlıkla sipariş verildi.</summary>
        OrderedByMistake = 11,

        /// <summary>Sahte/taklit ürün şüphesi (SATICI KUSURU; platform incelemesi tetiklenir).</summary>
        CounterfeitSuspicion = 12,

        /// <summary>Diğer (Description alanına açıklama zorunlu).</summary>
        Other = 13
    }

    /// <summary>
    /// İade talebi durum makinesi (ReturnRequests.Status).
    ///
    /// AKIŞ:
    ///   Requested → SellerApproved → InTransit → Received → Inspected → Refunded
    ///   Requested → SellerRejected → (müşteri itiraz ederse) → Disputed
    ///   Inspected → Rejected  (ürün kabul edilebilir durumda değil)
    ///
    /// Her geçiş append-only ReturnStatusHistory'e yazılır. Geçerli geçişler servis katmanında
    /// doğrulanır; geriye dönük geçiş yalnızca ihtilaf kararıyla mümkündür.
    /// </summary>
    public enum ReturnRequestStatus : byte
    {
        /// <summary>Müşteri iade talebi oluşturdu; satıcı onayı bekleniyor.</summary>
        Requested = 0,

        /// <summary>Satıcı iadeyi onayladı; kargo etiketi/talimat üretilir.</summary>
        SellerApproved = 1,

        /// <summary>Satıcı iadeyi reddetti. Müşteri ihtilaf açabilir.</summary>
        SellerRejected = 2,

        /// <summary>Ürün iade kargosuna verildi; yolda (Faz 5 Shipments, Direction=Return).</summary>
        InTransit = 3,

        /// <summary>Ürün satıcı/depo tarafından teslim alındı; inceleme bekliyor.</summary>
        Received = 4,

        /// <summary>İnceleme tamamlandı; iade tutarı belirlendi.</summary>
        Inspected = 5,

        /// <summary>İade tamamlandı, para müşteriye döndü (Faz 2 Refunds).</summary>
        Refunded = 6,

        /// <summary>İnceleme sonucu reddedildi (ürün kullanılmış/hasarlı vb.); ürün müşteriye geri gönderilebilir.</summary>
        Rejected = 7,

        /// <summary>Müşteri talebi geri çekti.</summary>
        CancelledByCustomer = 8,

        /// <summary>İhtilafa taşındı; platform hakemliği bekleniyor (Disputes).</summary>
        Disputed = 9,

        /// <summary>İhtilaf kararıyla çözüldü; sonuç Disputes.Decision alanındadır.</summary>
        ResolvedByDispute = 10
    }

    /// <summary>İade/ihtilaf akışında işlemi yapan taraf. Faz 1 OrderActorType ile aynı mantık.</summary>
    public enum ReturnActorType : byte
    {
        /// <summary>Sistem / BackgroundService (otomatik onay, süre aşımı).</summary>
        System = 0,

        /// <summary>Müşteri (alıcı).</summary>
        Customer = 1,

        /// <summary>Satıcı.</summary>
        Seller = 2,

        /// <summary>Platform yöneticisi (hakem).</summary>
        Admin = 3,

        /// <summary>Kargo firması (webhook kaynaklı durum geçişi).</summary>
        Carrier = 4
    }

    /// <summary>İade kargo bedelini kimin üstlendiği (ReturnRequests.ShippingPayer).</summary>
    public enum ReturnShippingPayer : byte
    {
        /// <summary>Belirlenmedi (inceleme sonrası kesinleşir).</summary>
        Undetermined = 0,

        /// <summary>Müşteri öder (cayma hakkı kaynaklı iadeler, politikaya göre).</summary>
        Customer = 1,

        /// <summary>Satıcı öder (satıcı kusuru kaynaklı iadeler).</summary>
        Seller = 2,

        /// <summary>Platform öder (müşteri memnuniyeti jesti veya ihtilaf kararı).</summary>
        Platform = 3
    }

    /// <summary>İade edilen ürünün inceleme sonucu (ReturnRequestItems.InspectionResult).</summary>
    public enum ReturnInspectionResult : byte
    {
        /// <summary>Henüz incelenmedi.</summary>
        NotInspected = 0,

        /// <summary>Sıfır/satılabilir durumda — stoğa geri girer (Faz 4 ReturnIn hareketi).</summary>
        SellableAsNew = 1,

        /// <summary>Açılmış ama sağlam — indirimli/yenilenmiş olarak satılabilir.</summary>
        SellableAsOpenBox = 2,

        /// <summary>Hasarlı — stoğa girmez, fire yazılır (Faz 4 Damage hareketi).</summary>
        Damaged = 3,

        /// <summary>Eksik parça var — kısmi iade uygulanır.</summary>
        MissingParts = 4,

        /// <summary>Farklı/yanlış ürün gönderilmiş (müşteri kaynaklı) — iade reddedilir.</summary>
        WrongItemReturned = 5,

        /// <summary>Kullanım izi var — politikaya göre kısmi iade veya ret.</summary>
        UsedCondition = 6
    }

    /// <summary>
    /// İhtilaf durum makinesi (Disputes.Status).
    /// AliExpress modeli: satıcı reddi sonrası müşteri platform hakemliğine başvurur.
    /// </summary>
    public enum DisputeStatus : byte
    {
        /// <summary>İhtilaf açıldı; satıcının yanıtı bekleniyor.</summary>
        Opened = 0,

        /// <summary>Satıcı yanıt verdi; taraflar kanıt sunuyor.</summary>
        SellerResponded = 1,

        /// <summary>Kanıt toplama süresi doldu; platform incelemesinde.</summary>
        UnderReview = 2,

        /// <summary>Taraflar kendi aralarında anlaştı; hakemlik gerekmedi.</summary>
        SettledByParties = 3,

        /// <summary>Platform karar verdi (Decision alanı doludur).</summary>
        Resolved = 4,

        /// <summary>Müşteri ihtilafı geri çekti.</summary>
        WithdrawnByCustomer = 5,

        /// <summary>Süre aşımı nedeniyle otomatik kapatıldı (taraflardan yanıt gelmedi).</summary>
        ExpiredNoResponse = 6,

        /// <summary>Üst mercie taşındı (yasal süreç/ödeme sağlayıcısı chargeback'i).</summary>
        Escalated = 7
    }

    /// <summary>Platformun ihtilaf kararı (Disputes.Decision).</summary>
    public enum DisputeDecisionType : byte
    {
        /// <summary>Henüz karar verilmedi.</summary>
        Pending = 0,

        /// <summary>Alıcı lehine: tam iade yapılır.</summary>
        BuyerFavor = 1,

        /// <summary>Satıcı lehine: iade yapılmaz.</summary>
        SellerFavor = 2,

        /// <summary>Kısmi karar: belirlenen tutar iade edilir (DecidedRefundAmount).</summary>
        Partial = 3,

        /// <summary>Alıcı lehine ama ürün iadesi şartıyla (ürün geri gönderilmeden para dönmez).</summary>
        BuyerFavorWithReturn = 4
    }

    /// <summary>İhtilaf mesajını gönderen taraf (DisputeMessages.SenderType).</summary>
    public enum DisputeMessageSenderType : byte
    {
        /// <summary>Alıcı.</summary>
        Customer = 0,

        /// <summary>Satıcı.</summary>
        Seller = 1,

        /// <summary>Platform hakemi (admin).</summary>
        Admin = 2,

        /// <summary>Sistem mesajı (durum bildirimi, süre uyarısı).</summary>
        System = 3
    }

    /// <summary>İade politikası kapsam seviyesi (ReturnPolicies çözüm önceliği; README §6).</summary>
    public enum ReturnPolicyScopeType : byte
    {
        /// <summary>Global varsayılan (StoreId, CategoryId, CountryId hepsi null).</summary>
        Global = 0,

        /// <summary>Ülke bazlı (yasal asgari süreler ülkeye göre değişir).</summary>
        Country = 1,

        /// <summary>Kategori bazlı (gıda/hijyen ürünlerinde iade yok, elektronikte 14 gün...).</summary>
        Category = 2,

        /// <summary>Mağaza bazlı (satıcı daha uzun süre tanıyabilir).</summary>
        Store = 3,

        /// <summary>Mağaza + kategori bazlı (en özel kapsam).</summary>
        StoreCategory = 4
    }
}
