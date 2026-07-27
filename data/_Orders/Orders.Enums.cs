namespace data._Orders
{
    // ============================================================================================
    // SİPARİŞ SİSTEMİ V1 — ENUM SÖZLÜĞÜ
    // KURAL: Enum değerleri byte olarak saklanır. Mevcut değerlerin ARASINA yeni değer EKLENMEZ,
    // yeni değerler her zaman SONA eklenir. Değer silinmez, yeniden adlandırılmaz.
    // (Veritabanındaki mevcut byte değerlerinin anlamı asla kaymamalıdır.)
    // ============================================================================================

    /// <summary>
    /// Sipariş durum makinesi. Hem Orders (üst sipariş, müşteri bazlı) hem SubOrders (mağaza bazlı
    /// alt sipariş) bu enum'ı kullanır. Asıl akış SubOrder seviyesinde yürür; Orders.Status,
    /// alt siparişlerin toplulaştırılmış (aggregate) özetidir ve servis katmanı tarafından türetilir.
    /// Geçerli geçişler servis katmanında doğrulanır; her geçiş OrderStatusHistory'e append-only yazılır.
    /// </summary>
    public enum OrderStatus : byte
    {
        /// <summary>Sipariş oluşturuldu, ödeme bekleniyor (3DS yönlendirmesi vb.).</summary>
        PendingPayment = 0,

        /// <summary>Ödeme başarıyla tahsil edildi (Auth+Capture tamam).</summary>
        Paid = 1,

        /// <summary>Satıcı siparişi onayladı.</summary>
        Confirmed = 2,

        /// <summary>Satıcı ürünü hazırlıyor / paketliyor.</summary>
        Preparing = 3,

        /// <summary>Kargoya verildi (takip numarası oluştu).</summary>
        Shipped = 4,

        /// <summary>Alıcıya teslim edildi.</summary>
        Delivered = 5,

        /// <summary>İptal edildi (gerekçe: OrderCancellationReason).</summary>
        Cancelled = 6,

        /// <summary>Tamamı iade edildi.</summary>
        Refunded = 7,

        /// <summary>Kısmi iade yapıldı (bazı kalemler iade edildi).</summary>
        PartiallyRefunded = 8,

        /// <summary>Ödeme denemesi başarısız oldu (yeni deneme yapılabilir veya sipariş düşer).</summary>
        PaymentFailed = 9
    }

    /// <summary>
    /// İptal gerekçesi. SubOrders.CancellationReason ve OrderStatusHistory kayıtlarında kullanılır.
    /// </summary>
    public enum OrderCancellationReason : byte
    {
        /// <summary>Belirtilmedi.</summary>
        Unspecified = 0,

        /// <summary>Müşteri talebiyle iptal.</summary>
        CustomerRequest = 1,

        /// <summary>Stok yetersizliği (satıcı karşılayamadı).</summary>
        OutOfStock = 2,

        /// <summary>Ödeme alınamadı / zaman aşımı.</summary>
        PaymentFailed = 3,

        /// <summary>Satıcı tarafından iptal (diğer nedenler).</summary>
        SellerCancelled = 4,

        /// <summary>Teslimat adresi geçersiz / ulaşılamıyor.</summary>
        AddressInvalid = 5,

        /// <summary>Dolandırıcılık şüphesi (platform iptali).</summary>
        FraudSuspicion = 6,

        /// <summary>Mükerrer sipariş.</summary>
        DuplicateOrder = 7,

        /// <summary>Diğer (OrderStatusHistory.Note alanına açıklama yazılır).</summary>
        Other = 8
    }

    /// <summary>
    /// Durum değişikliğini yapan aktör tipi. OrderStatusHistory'de "kim" sorusunun tipini belirtir;
    /// kişi bazlı iz OrderEntityBase.CreatedByUserId üzerindedir.
    /// </summary>
    public enum OrderActorType : byte
    {
        /// <summary>Sistem / BackgroundService (webhook, otomatik geçiş).</summary>
        System = 0,

        /// <summary>Müşteri (alıcı).</summary>
        Customer = 1,

        /// <summary>Satıcı (mağaza yetkilisi).</summary>
        Seller = 2,

        /// <summary>Platform yöneticisi (admin).</summary>
        Admin = 3
    }

    /// <summary>
    /// E-fatura / e-arşiv entegrasyon durumu (OrderInvoices).
    /// Entegrasyon kanalı StoreIntegration üzerinden çözülür.
    /// </summary>
    public enum EInvoiceStatus : byte
    {
        /// <summary>Henüz e-fatura oluşturulmadı (yalnızca dahili kayıt/PDF var).</summary>
        NotCreated = 0,

        /// <summary>Entegratöre gönderim kuyruğunda.</summary>
        Queued = 1,

        /// <summary>Entegratöre gönderildi, sonuç bekleniyor.</summary>
        Sent = 2,

        /// <summary>GİB / entegratör tarafından kabul edildi.</summary>
        Approved = 3,

        /// <summary>Reddedildi (ProviderResponseJson'da ham hata yanıtı tutulur).</summary>
        Rejected = 4,

        /// <summary>Fatura iptal edildi.</summary>
        Cancelled = 5
    }

    /// <summary>
    /// Checkout oturumu durumu (CheckoutSessions). Sepet → sipariş dönüşümünün
    /// idempotency ve kilit mekanizmasını taşır.
    /// </summary>
    public enum CheckoutSessionStatus : byte
    {
        /// <summary>Oturum açık; müşteri ödeme adımında. Sepet bu oturuma kilitlidir.</summary>
        Active = 0,

        /// <summary>Sipariş başarıyla oluşturuldu (OrderId dolu). Aynı IdempotencyKey ile ikinci sipariş oluşturulamaz.</summary>
        Completed = 1,

        /// <summary>ExpiresAtUtc geçti; BackgroundService oturumu düşürdü, sepet kilidi çözüldü.</summary>
        Expired = 2,

        /// <summary>Müşteri veya sistem oturumu iptal etti.</summary>
        Cancelled = 3
    }
}
