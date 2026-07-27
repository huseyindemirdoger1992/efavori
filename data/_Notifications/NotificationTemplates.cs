using data._Attribute;

namespace data._Notifications
{
    /// <summary>
    /// BİLDİRİM ŞABLONU. Her NotificationType için bir satır. Metinler burada DEĞİL,
    /// 10 dilli NotificationTemplateTranslations tablosunda tutulur (proje çeviri deseni).
    ///
    /// Şablonlar yer tutucu (placeholder) kullanır: {orderNumber}, {productName}, {amount}...
    /// Render sonucu, kullanıcıya gönderildiği anda Notifications satırına SNAPSHOT'lanır —
    /// şablon sonradan değişse bile geçmiş bildirim metni bozulmaz.
    /// </summary>
    public class NotificationTemplates : NotificationEntityBase
    {
        /// <summary>Bu şablonun karşılık geldiği bildirim tipi. Soft-delete filtreli TEKİL.</summary>
        public NotificationType NotificationType { get; set; }

        /// <summary>Makine-okur kod ("order_status_changed"). Yönetim ekranı ve loglama için.</summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>Bu tipin gönderileceği varsayılan kanallar (kullanıcı tercihi bunu daraltabilir).</summary>
        public NotificationChannel DefaultChannels { get; set; } = NotificationChannel.InApp;

        /// <summary>Varsayılan önem düzeyi.</summary>
        public NotificationPriority DefaultPriority { get; set; } = NotificationPriority.Normal;

        /// <summary>Kullanıcı bu tipi kapatabilir mi? false = zorunlu bildirim
        /// (güvenlik, yasal duyuru — abonelikten çıkılamaz).</summary>
        public bool IsOptOutAllowed { get; set; } = true;

        /// <summary>Şablon aktif mi? Pasif şablonla yeni bildirim üretilmez.</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Şablonda kullanılabilecek yer tutucuların listesi (JSON dizi).
        /// Yönetim ekranında editöre gösterilir ve render öncesi doğrulamada kullanılır.</summary>
        public string? AvailablePlaceholdersJson { get; set; }

        /// <summary>Aynı tipte bildirimlerin birleştirilme (gruplama) penceresi — dakika.
        /// Örn. 60 = "son 1 saatte 5 ürünün fiyatı düştü" tek bildirimde toplanır.
        /// Null = gruplama yok.</summary>
        public int? GroupingWindowMinutes { get; set; }
    }

    /// <summary>
    /// BİLDİRİM ŞABLONU ÇEVİRİSİ. Proje çeviri deseni: (ParentId, Language) bileşik TEKİL indeks,
    /// IsManuallyEdited AI ezme kilidi. Dil enum'ı mevcut data._Attribute.Language'tir —
    /// YENİDEN TANIMLANMAZ, referans alınır. Türkçe (Tr) kanonik kaynak dildir.
    /// </summary>
    public class NotificationTemplateTranslations : NotificationEntityBase
    {
        /// <summary>Bağlı şablon (NotificationTemplates.Id). Çeviri deseninin ParentId'si.</summary>
        public Guid ParentId { get; set; }

        /// <summary>Çeviri dili (mevcut data._Attribute.Language enum'ı; 10 dil).</summary>
        public Language Language { get; set; }

        /// <summary>Bildirim başlığı şablonu ("Siparişiniz kargoya verildi").</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Bildirim gövdesi şablonu. Yer tutucu içerir:
        /// "{orderNumber} numaralı siparişiniz {carrierName} ile yola çıktı."</summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>E-posta konusu (In-app başlığından farklı olabilir; daha uzun/açıklayıcı).</summary>
        public string? EmailSubject { get; set; }

        /// <summary>E-posta HTML gövdesi. Boşsa Body düz metin olarak kullanılır.</summary>
        public string? EmailBodyHtml { get; set; }

        /// <summary>Push bildirimi kısa metni (karakter sınırı nedeniyle ayrı tutulur).</summary>
        public string? PushBody { get; set; }

        /// <summary>SMS metni (160 karakter kısıtı nedeniyle ayrı tutulur).</summary>
        public string? SmsBody { get; set; }

        /// <summary>AI EZME KİLİDİ: true ise otomatik çeviri servisi bu satırın üzerine YAZAMAZ.
        /// İnsan eliyle düzenlenmiş çeviriler korunur (mevcut ProductTranslations deseni).</summary>
        public bool IsManuallyEdited { get; set; }
    }

    /// <summary>
    /// BİLDİRİM. Kullanıcı bazlıdır ve uygulama içi (in-app) kaydın kendisidir:
    /// okunma durumu, hedef link ve render edilmiş metin burada tutulur.
    ///
    /// KANAL GÖNDERİMİ AYRI TABLODADIR (NotificationDeliveries) — gerekçe README §7:
    /// bir bildirim 3 kanaldan gidiyorsa 3 ayrı gönderim durumu, 3 ayrı deneme sayacı ve
    /// 3 ayrı lease gerekir; bunları tek satıra sığdırmak kanal başına kolon çoğaltmayı
    /// (EmailStatus, PushStatus, SmsStatus...) ve yeni kanalda şema değişikliğini zorunlu kılardı.
    ///
    /// METİN SNAPSHOT'TIR: Title/Body, şablondan render edilip buraya YAZILIR. Şablon veya
    /// çeviri sonradan değişse bile kullanıcının gördüğü geçmiş bildirim değişmez.
    /// </summary>
    public class Notifications : NotificationEntityBase
    {
        /// <summary>Bildirimin alıcısı (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>Bildirim tipi.</summary>
        public NotificationType NotificationType { get; set; }

        /// <summary>Kullanılan şablon izi (NotificationTemplates.Id).</summary>
        public Guid? NotificationTemplateId { get; set; }

        /// <summary>Bildirimin üretildiği dil (kullanıcının o anki tercih dili).
        /// Mevcut data._Attribute.Language enum'ı.</summary>
        public Language Language { get; set; }

        /// <summary>RENDER EDİLMİŞ başlık (snapshot).</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>RENDER EDİLMİŞ gövde (snapshot).</summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>Önem düzeyi.</summary>
        public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

        /// <summary>Tıklandığında gidilecek uygulama içi bağlantı ("/siparislerim/EF-2026-000123").</summary>
        public string? TargetUrl { get; set; }

        /// <summary>Bildirimde gösterilecek küçük görsel (MediaItems.Id) — ürün görseli, mağaza logosu.</summary>
        public Guid? ImageMediaItemId { get; set; }

        /// <summary>Talep edilen kanallar (bayrak). Kullanıcı tercihleri uygulandıktan SONRAKİ hâlidir;
        /// her bayrak için bir NotificationDeliveries satırı açılır.</summary>
        public NotificationChannel RequestedChannels { get; set; } = NotificationChannel.InApp;

        /// <summary>Okundu mu? (yalnızca in-app kanalı ilgilendirir)</summary>
        public bool IsRead { get; set; }

        /// <summary>Okunma anı (UTC).</summary>
        public DateTime? ReadAtUtc { get; set; }

        /// <summary>Kullanıcı bildirimi listeden gizledi mi? (soft-delete'ten farklıdır: kayıt durur, listede görünmez)</summary>
        public bool IsArchived { get; set; }

        // ---------------- KAYNAK OLAY REFERANSLARI ----------------

        /// <summary>Kaynak: sipariş (Orders.Id).</summary>
        public Guid? OrderId { get; set; }

        /// <summary>Kaynak: alt sipariş (SubOrders.Id).</summary>
        public Guid? SubOrderId { get; set; }

        /// <summary>Kaynak: ürün (Products.Id).</summary>
        public Guid? ProductId { get; set; }

        /// <summary>Kaynak: mağaza (Store.Id) — satıcıya giden bildirimlerde.</summary>
        public Guid? StoreId { get; set; }

        /// <summary>Kaynak: sevkiyat (Shipments.Id).</summary>
        public Guid? ShipmentId { get; set; }

        /// <summary>Kaynak: fiyat alarmı (Faz 3 PriceAlerts.Id).</summary>
        public Guid? PriceAlertId { get; set; }

        /// <summary>Genel amaçlı kaynak kimliği — yukarıdakilerin hiçbirine uymayan tipler için
        /// (iade talebi, ihtilaf, hakediş...). Tip bilgisi NotificationType'tan çıkarılır.</summary>
        public Guid? RelatedEntityId { get; set; }

        /// <summary>Render'da kullanılan yer tutucu değerleri (JSON). Şablon değişirse yeniden
        /// render edebilmek ve hata ayıklamak için saklanır.</summary>
        public string? PlaceholderDataJson { get; set; }

        /// <summary>Global tekil idempotency anahtarı. Deterministik üretilir
        /// (örn. "ntf:orderstatus:{subOrderId}:{status}", "ntf:pricealert:{alertId}:{yyyyMMdd}");
        /// aynı olayın çift bildirim üretmesi DB seviyesinde engellenir.</summary>
        public string IdempotencyKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// KANAL BAZLI GÖNDERİM KAYDI — KUYRUK TABLOSU.
    /// Bir Notifications satırı için, talep edilen her kanala bir satır açılır.
    /// Çok örnekli BackgroundService lease deseni taşır (mevcut AiGenerationJob / ImportJob deseni).
    ///
    /// E-POSTA KUYRUĞU: Channel=Email satırları e-posta gönderim kuyruğunun kendisidir.
    /// Mevcut EmailHistory ile ilişki README §8'de açıklanmıştır (EmailHistory kaldırılmaz;
    /// bu tablo kuyruk, o tablo gönderim arşivi olarak konumlanır).
    /// </summary>
    public class NotificationDeliveries : NotificationEntityBase
    {
        /// <summary>Bağlı bildirim (Notifications.Id).</summary>
        public Guid NotificationId { get; set; }

        /// <summary>Alıcı izi (Users.Id). DENORMALIZE: kaynak Notifications.UserId;
        /// kuyruk taramasında join'i önler.</summary>
        public Guid UserId { get; set; }

        /// <summary>Bu satırın kanalı. BAYRAK DEĞİL, TEK kanaldır (InApp veya Email veya Push...).</summary>
        public NotificationChannel Channel { get; set; }

        /// <summary>Gönderim durumu.</summary>
        public NotificationDeliveryStatus Status { get; set; } = NotificationDeliveryStatus.Pending;

        /// <summary>Hedef adres snapshot'ı: e-posta adresi, telefon numarası veya push token.
        /// Kullanıcı adresini sonradan değiştirse bile nereye gönderildiği izlenebilir kalır.</summary>
        public string? Destination { get; set; }

        /// <summary>Gönderildiği an (UTC).</summary>
        public DateTime? SentAtUtc { get; set; }

        /// <summary>Teslim doğrulandığı an (UTC) — sağlayıcı delivery receipt'i.</summary>
        public DateTime? DeliveredAtUtc { get; set; }

        /// <summary>Açılma/tıklanma anı (UTC) — e-posta ve push için etkileşim ölçümü.</summary>
        public DateTime? OpenedAtUtc { get; set; }

        /// <summary>Sağlayıcı tarafındaki mesaj kimliği (bounce/webhook eşlemesi için).</summary>
        public string? ProviderMessageId { get; set; }

        /// <summary>Sağlayıcıdan dönen ham yanıt (JSON). Hata ayıklama izi.</summary>
        public string? ProviderResponseJson { get; set; }

        /// <summary>Başarısızlık açıklaması.</summary>
        public string? FailureMessage { get; set; }

        // ---------------- LEASE DESENİ (çok örnekli BackgroundService) ----------------

        /// <summary>Lease sahibi işleyici kimliği (makine+instance). Null = sahipsiz.</summary>
        public string? LeasedBy { get; set; }

        /// <summary>Lease bitiş zamanı (UTC). Geçmişse lease düşmüş sayılır, başka işleyici alabilir.</summary>
        public DateTime? LeasedUntilUtc { get; set; }

        /// <summary>Deneme sayısı.</summary>
        public int AttemptCount { get; set; }

        /// <summary>Azami deneme. Aşılırsa PermanentlyFailed'a çekilir.</summary>
        public int MaxAttempts { get; set; } = 5;

        /// <summary>Bir sonraki deneme zamanı (UTC). Başarısızlık sonrası üstel geri çekilme ile kurulur.</summary>
        public DateTime? NextRetryAtUtc { get; set; }

        /// <summary>Zamanlanmış gönderim: bu andan önce gönderilmez.
        /// "Rahatsız etmeyin" saatleri ve gruplama penceresi için kullanılır.</summary>
        public DateTime? ScheduledForUtc { get; set; }

        /// <summary>Global tekil idempotency anahtarı. Deterministik: "{notificationIdempotencyKey}:{channel}";
        /// aynı bildirimin aynı kanaldan iki kez gönderilmesi DB seviyesinde engellenir.</summary>
        public string IdempotencyKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// KULLANICI BİLDİRİM TERCİHLERİ. Kullanıcı hangi tipi hangi kanaldan almak istediğini belirler.
    /// Satır YOKSA şablonun DefaultChannels değeri geçerlidir (kayıt açmaya gerek yok).
    /// NotificationTemplates.IsOptOutAllowed=false olan tipler bu tercihlerle KAPATILAMAZ.
    /// </summary>
    public class UserNotificationPreferences : NotificationEntityBase
    {
        /// <summary>Kullanıcı (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>Tercih edilen bildirim tipi.</summary>
        public NotificationType NotificationType { get; set; }

        /// <summary>Kullanıcının bu tip için açık bıraktığı kanallar (bayrak). None = tümüyle kapalı.</summary>
        public NotificationChannel EnabledChannels { get; set; }

        /// <summary>"Rahatsız etmeyin" başlangıç saati (kullanıcının yerel saatinde, 0-23).
        /// Null = kısıt yok. Bu aralıktaki gönderimler ScheduledForUtc ile ertelenir.</summary>
        public byte? QuietHoursStart { get; set; }

        /// <summary>"Rahatsız etmeyin" bitiş saati (0-23).</summary>
        public byte? QuietHoursEnd { get; set; }

        /// <summary>Kullanıcının IANA saat dilimi ("Europe/Istanbul"). Sessiz saat hesabı için.</summary>
        public string? TimeZoneId { get; set; }
    }
}