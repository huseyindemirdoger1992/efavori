using System;
using data._Attribute;
using data._Products;

namespace data._Users
{
    /// <summary>
    /// KULLANICI TERCİHLERİ — arayüz, bölge, akış ve iletişim ayarları.
    ///
    /// <see cref="Users"/> ile 1:1'dir. Satır YOKSA sistem varsayılanları geçerlidir;
    /// kayıt anında oluşturmak zorunlu değildir (lazy create).
    ///
    /// Bildirim tercihleri BURADA DEĞİLDİR: tip bazlı kanal seçimi zaten
    /// <c>data._Notifications.UserNotificationPreferences</c> tablosundadır. Burada
    /// yalnızca kanalların ana (master) açma/kapama anahtarları tutulur.
    /// </summary>
    public class UserSettings : UserEntityBase
    {
        /// <summary>Ayarların sahibi (Users.Id). 1:1 — soft-delete filtreli TEKİL indeks.</summary>
        public Guid UserId { get; set; }

        // ── Bölge / yerelleştirme ─────────────────────────────────────────────
        /// <summary>Arayüz dili (mevcut data._Attribute.Language enum'ı — 10 dil).</summary>
        public Language Language { get; set; } = Language.En;

        /// <summary>Tercih edilen para birimi (mevcut data._Products.CurrencyCode enum'ı).</summary>
        public CurrencyCode Currency { get; set; } = CurrencyCode.Usd;

        /// <summary>IANA saat dilimi ("Europe/Istanbul"). Tarih gösterimi ve sessiz saatler için.</summary>
        public string? TimeZoneId { get; set; }

        /// <summary>Vitrin/kargo hesabı için tercih edilen teslimat ülkesi (data._Locations.Country.Id).</summary>
        public int? PreferredCountryId { get; set; }

        /// <summary>Tarih biçimi tercihi ("dd.MM.yyyy"). Null = dilin varsayılanı.</summary>
        public string? DateFormat { get; set; }

        /// <summary>Ölçü birimi tercihi: true = metrik (kg/cm), false = imperial (lb/in).</summary>
        public bool UseMetricUnits { get; set; } = true;

        // ── Akış (feed) tercihleri ────────────────────────────────────────────
        /// <summary>Ana akış sıralama tercihi.</summary>
        public FeedSortPreference FeedSort { get; set; } = FeedSortPreference.Algorithmic;

        /// <summary>Akışta takip edilen mağazaların gönderileri gösterilsin mi?</summary>
        public bool ShowStorePostsInFeed { get; set; } = true;

        /// <summary>Akışta önerilen (takip edilmeyen) içerik gösterilsin mi?</summary>
        public bool ShowSuggestedContentInFeed { get; set; } = true;

        /// <summary>Hassas/rahatsız edici olabilecek içerik bulanıklaştırılsın mı?</summary>
        public bool BlurSensitiveContent { get; set; } = true;

        /// <summary>Videolar akışta otomatik oynatılsın mı?</summary>
        public bool AutoPlayVideos { get; set; } = true;

        /// <summary>Karanlık tema tercihi. Null = cihaz ayarını izle.</summary>
        public bool? PrefersDarkTheme { get; set; }

        /// <summary>Düşük veri modu (görselleri düşük çözünürlükte yükle).</summary>
        public bool DataSaverEnabled { get; set; }

        // ── Bildirim ANA anahtarları ──────────────────────────────────────────
        /// <summary>Uygulama içi bildirimler açık mı?</summary>
        public bool InAppNotificationsEnabled { get; set; } = true;

        /// <summary>E-posta bildirimleri açık mı?</summary>
        public bool EmailNotificationsEnabled { get; set; } = true;

        /// <summary>Mobil push bildirimleri açık mı?</summary>
        public bool PushNotificationsEnabled { get; set; } = true;

        /// <summary>SMS bildirimleri açık mı?</summary>
        public bool SmsNotificationsEnabled { get; set; }

        /// <summary>"Rahatsız etmeyin" başlangıç saati (0-23, kullanıcının yerel saati). Null = kısıt yok.</summary>
        public byte? QuietHoursStart { get; set; }

        /// <summary>"Rahatsız etmeyin" bitiş saati (0-23).</summary>
        public byte? QuietHoursEnd { get; set; }

        // ── Pazarlama / veri politikası (açık rıza — KVKK/GDPR) ───────────────
        /// <summary>Pazarlama e-postaları için açık rıza verildi mi?</summary>
        public bool MarketingEmailConsent { get; set; }

        /// <summary>Pazarlama SMS'leri için açık rıza verildi mi?</summary>
        public bool MarketingSmsConsent { get; set; }

        /// <summary>Rızanın verildiği/güncellendiği an (UTC) — yasal ispat için.</summary>
        public DateTime? MarketingConsentAtUtc { get; set; }

        /// <summary>Kişiselleştirilmiş reklam gösterimine izin veriliyor mu?</summary>
        public bool PersonalizedAdsEnabled { get; set; }

        /// <summary>Anonim kullanım/analitik verisi toplanmasına izin veriliyor mu?</summary>
        public bool AnalyticsCollectionAllowed { get; set; } = true;

        /// <summary>İçeriğin yapay zekâ eğitiminde kullanılmasına izin veriliyor mu?</summary>
        public bool AiTrainingAllowed { get; set; }
    }
}
