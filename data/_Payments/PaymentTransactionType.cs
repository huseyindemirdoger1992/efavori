namespace data._Payments
{
    // ============================================================================================
    // ÖDEME VE HAKEDİŞ SİSTEMİ V1 — ENUM SÖZLÜĞÜ
    // KURAL: Enum değerleri byte olarak saklanır. Mevcut değerlerin ARASINA yeni değer EKLENMEZ,
    // yeni değerler her zaman SONA eklenir. Değer silinmez, yeniden adlandırılmaz.
    // (Veritabanındaki mevcut byte değerlerinin anlamı asla kaymamalıdır.)
    // NOT: Ödeme sağlayıcıları (iyzico/Stripe/PayPal/PayTR...) enum DEĞİLDİR;
    // IntegrationPlatform benzeri PaymentProviders referans tablosunda tutulur.
    // ============================================================================================

    /// <summary>
    /// Ödeme işlem tipi. Bir siparişte birden çok işlem olabilir; Capture/Void/Refund işlemleri
    /// ParentTransactionId ile kaynak işleme (Auth veya Capture) bağlanır.
    /// </summary>
    public enum PaymentTransactionType : byte
    {
        /// <summary>Ön otorizasyon (tutar bloke edilir, tahsil edilmez).</summary>
        Auth = 0,

        /// <summary>Tahsilat (Auth sonrası çekim; direkt satışta Auth+Capture tek işlemde olabilir).</summary>
        Capture = 1,

        /// <summary>Otorizasyon iptali (Capture öncesi blokenin çözülmesi).</summary>
        Void = 2,

        /// <summary>Tam iade.</summary>
        Refund = 3,

        /// <summary>Kısmi iade.</summary>
        PartialRefund = 4,

        /// <summary>Ters ibraz (banka kaynaklı geri çekim; Refunds ve ledger'a ceza girişi üretir).</summary>
        Chargeback = 5
    }

    /// <summary>
    /// Ödeme işlemi durum makinesi. Geçişler: Pending → Processing → Succeeded/Failed;
    /// Pending/Processing → Cancelled. Webhook/poll sonuçları bu alanı günceller;
    /// ham sağlayıcı yanıtı daima RawResponseJson'a yazılır.
    /// </summary>
    public enum PaymentTransactionStatus : byte
    {
        /// <summary>İşlem oluşturuldu, sağlayıcıya henüz gönderilmedi veya 3DS yönlendirmesi bekleniyor.</summary>
        Pending = 0,

        /// <summary>Sağlayıcıya gönderildi, sonuç bekleniyor.</summary>
        Processing = 1,

        /// <summary>Başarıyla tamamlandı.</summary>
        Succeeded = 2,

        /// <summary>Başarısız (FailureCode/FailureMessage doldurulur).</summary>
        Failed = 3,

        /// <summary>Sonuçlanmadan iptal edildi (kullanıcı vazgeçti / zaman aşımı).</summary>
        Cancelled = 4
    }

    /// <summary>3D Secure doğrulama durumu.</summary>
    public enum ThreeDSecureStatus : byte
    {
        /// <summary>3DS gerekmedi (non-3DS akış).</summary>
        NotRequired = 0,

        /// <summary>Müşteri 3DS doğrulama sayfasında, sonuç bekleniyor.</summary>
        Pending = 1,

        /// <summary>3DS doğrulaması başarılı.</summary>
        Authenticated = 2,

        /// <summary>3DS doğrulaması başarısız.</summary>
        Failed = 3
    }

    /// <summary>
    /// Satıcı defteri giriş tipi. İşaret kuralı Amount alanının kendisindedir (± ondadır);
    /// buradaki açıklamalar tipik yönü belirtir. Bakiye SAKLANMAZ, defterden türetilir.
    /// </summary>
    public enum LedgerEntryType : byte
    {
        /// <summary>Alt sipariş tahsilatı (+). Kaynak: SubOrderId. Escrow: ReleaseAfterUtc dolana kadar bloke.</summary>
        SubOrderCollection = 0,

        /// <summary>Platform komisyonu (−). Kaynak: SubOrderId (SubOrders.CommissionTotal).</summary>
        Commission = 1,

        /// <summary>İade (−). Kaynak: RefundId.</summary>
        Refund = 2,

        /// <summary>Ceza / kesinti (−). Örn. geç kargolama cezası.</summary>
        Penalty = 3,

        /// <summary>Manuel düzeltme (±). Admin kaynaklı; Description zorunlu tutulmalıdır.</summary>
        ManualAdjustment = 4,

        /// <summary>Platform destekli kupon telafisi (+). Kupon maliyetini platform üstlendiyse satıcıya iade edilen pay (Faz 6 Coupons.CostBearer bağlantısı).</summary>
        CouponCompensation = 5,

        /// <summary>Ters ibraz kesintisi (−). Kaynak: PaymentTransactionId (Chargeback tipli işlem).</summary>
        Chargeback = 6
    }

    /// <summary>Satıcı hakediş ödemesi durumu.</summary>
    public enum PayoutStatus : byte
    {
        /// <summary>Dönem kapandı, ödeme oluşturuldu; işlenmeyi bekliyor.</summary>
        Pending = 0,

        /// <summary>Banka transferi işleniyor (BackgroundService lease aldı).</summary>
        Processing = 1,

        /// <summary>Ödendi (PaidAtUtc + dekont ReceiptMediaItemId doldurulur).</summary>
        Paid = 2,

        /// <summary>Başarısız (FailureMessage doldurulur; NextRetryAtUtc ile yeniden denenir).</summary>
        Failed = 3
    }

    /// <summary>İade nedeni (Refunds.Reason). Faz 7 ReturnRequests ayrıntılı iade akışını taşır; buradaki neden finansal kayıt içindir.</summary>
    public enum RefundReason : byte
    {
        /// <summary>Belirtilmedi.</summary>
        Unspecified = 0,

        /// <summary>Ürün iadesi sonucu (Faz 7 ReturnRequests kaynaklı).</summary>
        ProductReturned = 1,

        /// <summary>Sipariş iptali (kargolanmadan).</summary>
        OrderCancelled = 2,

        /// <summary>Teslim edilemedi.</summary>
        NotDelivered = 3,

        /// <summary>Hasarlı/kusurlu ürün.</summary>
        DamagedProduct = 4,

        /// <summary>Yanlış ürün gönderildi.</summary>
        WrongProduct = 5,

        /// <summary>İhtilaf kararı (Faz 7 Disputes admin hükmü).</summary>
        DisputeDecision = 6,

        /// <summary>İyi niyet / müşteri memnuniyeti jesti.</summary>
        GoodwillGesture = 7,

        /// <summary>Diğer (Note alanına açıklama yazılır).</summary>
        Other = 8
    }

    /// <summary>İade kaydı durum makinesi. Finansal yürütmeyi izler; PaymentTransactions (Refund tipi) ile senkron ilerler.</summary>
    public enum RefundStatus : byte
    {
        /// <summary>İade kararı verildi, ödeme sağlayıcısına henüz gönderilmedi.</summary>
        Pending = 0,

        /// <summary>Sağlayıcıda işleniyor.</summary>
        Processing = 1,

        /// <summary>Tamamlandı (para müşteriye döndü; ledger − girişi yazıldı).</summary>
        Completed = 2,

        /// <summary>Başarısız (manuel müdahale gerekir).</summary>
        Failed = 3
    }
}
