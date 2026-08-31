using data._Products;

namespace data._Payments
{
    /// <summary>
    /// ÖDEME SAĞLAYICILARI REFERANS TABLOSU (iyzico, Stripe, PayPal, PayTR...).
    /// Mevcut IntegrationPlatform deseninin ödeme karşılığıdır: sağlayıcılar enum DEĞİL,
    /// veri olarak yönetilir — yeni sağlayıcı eklemek deploy gerektirmez.
    /// </summary>
    public class PaymentProviders : PaymentEntityBase
    {
        /// <summary>Makine-okur tekil kod ("iyzico", "stripe", "paytr"). Küçük harf, soft-delete filtreli tekil indeks.</summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>Görünen ad ("iyzico", "Stripe").</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Sağlayıcı aktif mi? Pasif sağlayıcıyla yeni işlem başlatılamaz; eski işlemler etkilenmez.</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Ödeme sayfası logo URL'i.</summary>
        public string? LogoUrl { get; set; }

        /// <summary>3D Secure destekliyor mu?</summary>
        public bool Supports3DSecure { get; set; } = true;

        /// <summary>API üzerinden iade destekliyor mu? Desteklemiyorsa iadeler manuel işaretlenir.</summary>
        public bool SupportsRefund { get; set; } = true;

        /// <summary>Kart saklama (tokenization) destekliyor mu? UserPaymentMethods yalnızca destekleyen sağlayıcılarla çalışır.</summary>
        public bool SupportsTokenization { get; set; } = true;

        /// <summary>Ödeme sayfasındaki gösterim sırası.</summary>
        public int SortOrder { get; set; }
    }

    /// <summary>
    /// ÖDEME İŞLEMLERİ. Order'a bağlıdır (ödeme üst sipariş seviyesinde alınır; mağazalara dağılım
    /// Faz 2 ledger'da yapılır). Bir siparişte birden çok deneme/işlem olabilir:
    /// başarısız denemeler, Auth→Capture zinciri, kısmi iadeler ayrı satırlardır.
    /// Ham sağlayıcı istek/yanıtları JSON olarak saklanır (denetim + uzlaştırma/reconciliation izi).
    /// Idempotency: IdempotencyKey global tekildir; sağlayıcıya giden her istek bu anahtarla gider,
    /// webhook tekrarları ProviderTransactionId üzerinden idempotent işlenir (README'ye bakınız).
    /// </summary>
    public class PaymentTransactions : PaymentEntityBase
    {
        /// <summary>Bağlı üst sipariş (data._Orders.Orders.Id).</summary>
        public Guid OrderId { get; set; }

        /// <summary>Zincir kaynağı işlem (PaymentTransactions.Id): Capture→Auth, Void→Auth, Refund/PartialRefund→Capture, Chargeback→Capture.</summary>
        public Guid? ParentTransactionId { get; set; }

        /// <summary>Ödeme sağlayıcısı (PaymentProviders.Id).</summary>
        public Guid PaymentProviderId { get; set; }

        /// <summary>Kullanılan kayıtlı kart (UserPaymentMethods.Id). Tek seferlik kart girişinde null.</summary>
        public Guid? UserPaymentMethodId { get; set; }

        /// <summary>İşlem tipi.</summary>
        public PaymentTransactionType TransactionType { get; set; }

        /// <summary>Durum makinesi.</summary>
        public PaymentTransactionStatus Status { get; set; } = PaymentTransactionStatus.Pending;

        /// <summary>İşlem tutarı (daima pozitif; yönü TransactionType belirler).</summary>
        public decimal Amount { get; set; }

        /// <summary>Para birimi.</summary>
        public CurrencyCode Currency { get; set; }

        /// <summary>Taksit sayısı (1 = tek çekim). TR pazarında kart taksiti için.</summary>
        public int InstallmentCount { get; set; } = 1;

        /// <summary>3D Secure durumu.</summary>
        public ThreeDSecureStatus ThreeDSStatus { get; set; } = ThreeDSecureStatus.NotRequired;

        /// <summary>Sağlayıcı tarafındaki işlem numarası (webhook eşlemesinin anahtarı).</summary>
        public string? ProviderTransactionId { get; set; }

        /// <summary>Sağlayıcı ek referans kodu (conversationId, basket no vb.).</summary>
        public string? ProviderReferenceCode { get; set; }

        /// <summary>Global tekil idempotency anahtarı. Deterministik üretilir (örn. "cap:{orderId}:{deneme}");
        /// aynı anahtarla ikinci satır DB seviyesinde engellenir.</summary>
        public string IdempotencyKey { get; set; } = string.Empty;

        /// <summary>Sağlayıcıya giden ham istek (JSON; PAN/CVV GİBİ HASSAS ALANLAR YAZILMADAN maskelenmiş hali).</summary>
        public string? RawRequestJson { get; set; }

        /// <summary>Sağlayıcıdan dönen ham yanıt (JSON). Uzlaştırma ve hata ayıklamanın tek kaynağı.</summary>
        public string? RawResponseJson { get; set; }

        /// <summary>Sağlayıcı hata kodu (Status=Failed iken).</summary>
        public string? FailureCode { get; set; }

        /// <summary>İnsan-okur hata mesajı.</summary>
        public string? FailureMessage { get; set; }

        /// <summary>İşlemin nihai sonuca ulaştığı an (UTC) — Succeeded/Failed/Cancelled.</summary>
        public DateTime? CompletedAtUtc { get; set; }
    }

    /// <summary>
    /// KAYITLI ÖDEME YÖNTEMLERİ — mevcut UserPayment tablosunun PCI-DSS uyumlu HALEFİ.
    /// KRİTİK GÜVENLİK KURALI: Bu tabloda PAN (kart numarası), CVV veya track verisi ASLA tutulmaz.
    /// Kart, ödeme sağlayıcısında (PSP) tokenize edilir; burada yalnızca token + gösterim
    /// meta verisi (marka, son 4 hane, son kullanma) saklanır.
    /// GEÇİŞ NOTU: Eski UserPayment'taki PAN verisi TAŞINMAZ (taşımak ihlali sürdürmek olur);
    /// kullanıcıdan yeniden kart kaydı istenir. Ayrıntılı geçiş planı README'dedir.
    /// </summary>
    public class UserPaymentMethods : PaymentEntityBase
    {
        /// <summary>Kart sahibi kullanıcı (Users.Id). Misafir kartı SAKLANMAZ.</summary>
        public Guid UserId { get; set; }

        /// <summary>Token'ı tutan ödeme sağlayıcısı (PaymentProviders.Id). Token yalnızca bu sağlayıcıda geçerlidir.</summary>
        public Guid PaymentProviderId { get; set; }

        /// <summary>PSP kart token'ı (örn. iyzico cardToken). PAN DEĞİLDİR; tek başına kartı ifşa etmez.</summary>
        public string ProviderToken { get; set; } = string.Empty;

        /// <summary>Sağlayıcı tarafındaki kullanıcı/kart cüzdanı referansı (örn. iyzico cardUserKey).</summary>
        public string? ProviderCustomerReference { get; set; }

        /// <summary>Kart markası gösterim metni ("Visa", "Mastercard", "Troy"). Enum değil: sağlayıcıdan geldiği gibi.</summary>
        public string CardBrand { get; set; } = string.Empty;

        /// <summary>Kartın son 4 hanesi (gösterim: "**** 1234").</summary>
        public string Last4 { get; set; } = string.Empty;

        /// <summary>Son kullanma ayı (1-12).</summary>
        public byte ExpiryMonth { get; set; }

        /// <summary>Son kullanma yılı (örn. 2029).</summary>
        public short ExpiryYear { get; set; }

        /// <summary>Kullanıcının karta verdiği takma ad ("İş kartım").</summary>
        public string? CardAlias { get; set; }

        /// <summary>Varsayılan ödeme yöntemi mi? Kullanıcı başına en fazla bir tane true olmalı (servis katmanı korur).</summary>
        public bool IsDefault { get; set; }
    }
}
