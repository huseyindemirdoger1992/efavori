namespace data._Shipping
{
    // ============================================================================================
    // KARGO VE TESLİMAT SİSTEMİ V1 — ENUM SÖZLÜĞÜ
    // KURAL: Enum değerleri byte olarak saklanır. Mevcut değerlerin ARASINA yeni değer EKLENMEZ,
    // yeni değerler her zaman SONA eklenir. Değer silinmez, yeniden adlandırılmaz.
    // NOT: Kargo firmaları enum DEĞİLDİR; Carriers referans tablosunda tutulur
    // (mevcut IntegrationPlatform / PaymentProviders deseni).
    // ============================================================================================

    /// <summary>
    /// Kargo firmasıyla teknik entegrasyon tipi (Carriers.IntegrationType).
    /// Etiket üretimi ve takip güncellemesinin nasıl yürüyeceğini belirler.
    /// </summary>
    public enum CarrierIntegrationType : byte
    {
        /// <summary>Entegrasyon yok; takip numarası satıcı tarafından elle girilir, durum elle güncellenir.</summary>
        Manual = 0,

        /// <summary>REST API; etiket üretimi ve durum sorgulaması API üzerinden yapılır.</summary>
        RestApi = 1,

        /// <summary>SOAP/XML servis (TR'de bazı kargo firmaları hâlâ bu protokolü kullanır).</summary>
        SoapApi = 2,

        /// <summary>Yalnızca webhook: firma durum değişimini bize bildirir, biz sorgulamayız.</summary>
        WebhookOnly = 3,

        /// <summary>FTP/SFTP dosya alışverişi (toplu gönderi listesi + durum dosyası).</summary>
        FileTransfer = 4
    }

    /// <summary>
    /// Sevkiyat yönü (Shipments.Direction).
    /// İade sevkiyatları da bu tabloda tutulur; Faz 7 ReturnRequests bu yöne bağlanır.
    /// </summary>
    public enum ShipmentDirection : byte
    {
        /// <summary>Satıcıdan alıcıya (normal sipariş sevkiyatı).</summary>
        Outbound = 0,

        /// <summary>Alıcıdan satıcıya (iade sevkiyatı). Faz 7 ReturnRequests.Id ile ilişkilendirilir.</summary>
        Return = 1,

        /// <summary>Depolar arası transfer sevkiyatı (Faz 4 StockTransfers.Id ile ilişkilendirilir).</summary>
        Transfer = 2
    }

    /// <summary>
    /// Sevkiyat durum makinesi (Shipments.Status).
    /// Taşıyıcıdan gelen ham durum kodları ShipmentTrackingEvents'e yazılır ve
    /// servis katmanı tarafından bu normalize durumlara eşlenir (Carriers bazlı eşleme tablosu).
    /// </summary>
    public enum ShipmentStatus : byte
    {
        /// <summary>Paket oluşturuldu, etiket henüz üretilmedi.</summary>
        Created = 0,

        /// <summary>Kargo etiketi üretildi, teslim alınmayı bekliyor.</summary>
        LabelCreated = 1,

        /// <summary>Taşıyıcı paketi satıcıdan teslim aldı.</summary>
        PickedUp = 2,

        /// <summary>Yolda (transfer merkezleri arasında hareket ediyor).</summary>
        InTransit = 3,

        /// <summary>Dağıtıma çıktı (son teslimat aracında).</summary>
        OutForDelivery = 4,

        /// <summary>Alıcıya teslim edildi.</summary>
        Delivered = 5,

        /// <summary>Teslim denemesi başarısız (alıcı yok, adres bulunamadı); yeniden denenecek.</summary>
        DeliveryFailed = 6,

        /// <summary>Satıcıya geri dönüyor (teslim edilemedi veya alıcı reddetti).</summary>
        ReturnedToSender = 7,

        /// <summary>Kayıp/hasar bildirimi açıldı; taşıyıcı tazmin süreci işliyor.</summary>
        Lost = 8,

        /// <summary>Sevkiyat iptal edildi (etiket iptali; paket hiç teslim alınmadı).</summary>
        Cancelled = 9,

        /// <summary>Şubeden teslim alınmayı bekliyor (kargo noktası teslimatı).</summary>
        AwaitingPickup = 10
    }

    /// <summary>
    /// Takip olayı tipi (ShipmentTrackingEvents.EventType).
    /// Taşıyıcının ham kodu ayrıca RawStatusCode alanında saklanır; bu alan normalize edilmiş halidir.
    /// </summary>
    public enum TrackingEventType : byte
    {
        /// <summary>Bilinmeyen/eşlenemeyen durum kodu (RawStatusCode incelenmelidir).</summary>
        Unknown = 0,

        /// <summary>Gönderi kaydı oluşturuldu.</summary>
        Registered = 1,

        /// <summary>Satıcıdan teslim alındı.</summary>
        PickedUp = 2,

        /// <summary>Transfer merkezine giriş.</summary>
        ArrivedAtFacility = 3,

        /// <summary>Transfer merkezinden çıkış.</summary>
        DepartedFacility = 4,

        /// <summary>Dağıtıma çıktı.</summary>
        OutForDelivery = 5,

        /// <summary>Teslim edildi.</summary>
        Delivered = 6,

        /// <summary>Teslim denemesi başarısız.</summary>
        DeliveryAttemptFailed = 7,

        /// <summary>Gümrük işlemlerinde bekliyor (uluslararası gönderi).</summary>
        CustomsHold = 8,

        /// <summary>Gönderici adresine iade edildi.</summary>
        ReturnedToSender = 9,

        /// <summary>Hasar bildirimi.</summary>
        Damaged = 10,

        /// <summary>Kayıp bildirimi.</summary>
        Lost = 11,

        /// <summary>Adres güncellendi / yönlendirildi.</summary>
        Redirected = 12
    }

    /// <summary>
    /// Kargo ücret kuralının hangi ölçüye göre uygulandığı (ShippingRateRules.RateBasis).
    /// </summary>
    public enum ShippingRateBasis : byte
    {
        /// <summary>Desi aralığına göre (TR kargo standardı: en×boy×yükseklik/3000).</summary>
        Desi = 0,

        /// <summary>Fiili ağırlık (kg) aralığına göre.</summary>
        Weight = 1,

        /// <summary>Sepet tutarı aralığına göre (örn. 500 TL altı 49.90 TL).</summary>
        OrderAmount = 2,

        /// <summary>Kalem adedi aralığına göre.</summary>
        ItemCount = 3,

        /// <summary>Sabit ücret (aralıktan bağımsız).</summary>
        Flat = 4
    }

    /// <summary>
    /// Kargo bölgesi kapsam satırının hangi coğrafi seviyeyi işaret ettiği (ShippingZoneAreas.ScopeType).
    /// Çözümde EN ÖZEL eşleşme kazanır: City &gt; State &gt; Country &gt; Region.
    /// </summary>
    public enum ShippingZoneScopeType : byte
    {
        /// <summary>Kıta/bölge kapsamı (Regions.Id).</summary>
        Region = 0,

        /// <summary>Ülke kapsamı (Country.Id).</summary>
        Country = 1,

        /// <summary>İl kapsamı (States.Id).</summary>
        State = 2,

        /// <summary>İlçe/şehir kapsamı (Cities.Id).</summary>
        City = 3,

        /// <summary>Posta kodu kapsamı (PostalCodeFrom/To aralığı).</summary>
        PostalCode = 4
    }

    /// <summary>Teslimat kanıtı tipi (Shipments.ProofType).</summary>
    public enum DeliveryProofType : byte
    {
        /// <summary>Kanıt yok.</summary>
        None = 0,

        /// <summary>Islak imza (kağıt/tablet).</summary>
        Signature = 1,

        /// <summary>Teslimat fotoğrafı (kapıya bırakma).</summary>
        Photo = 2,

        /// <summary>SMS/e-posta ile doğrulanan tek kullanımlık kod.</summary>
        OtpCode = 3,

        /// <summary>Kimlik kontrolü yapıldı.</summary>
        IdVerification = 4
    }
}