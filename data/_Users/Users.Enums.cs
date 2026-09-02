using System;

namespace data._Users
{
    // ════════════════════════════════════════════════════════════════════════
    //  efavori — Kimlik / Profil / Gizlilik Modülü (Identity V2)
    //  Enum kataloğu — dilden bağımsız, tinyint (byte) olarak saklanan sabitler.
    //  KURAL: Enum değerlerinin ARASINA yeni değer EKLENMEZ, yalnızca SONA eklenir.
    //  Değer silinmez, yeniden adlandırılmaz (veritabanındaki geçmiş kayıtlar bozulur).
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Hesabın yaşam döngüsü durumu. Eski <c>bool? IsActive</c> alanının yerini alır:
    /// "aktif değil" ile "yasaklandı", "kullanıcı kendi kapattı" ve "aktivasyon bekliyor"
    /// tamamen farklı iş kurallarıdır ve tek bir boolean ile modellenemez.
    /// </summary>
    public enum UserAccountStatus : byte
    {
        /// <summary>Kayıt oluşturuldu, e-posta/telefon doğrulaması bekleniyor. Giriş yapamaz.</summary>
        PendingActivation = 1,

        /// <summary>Aktif, tüm özellikleri kullanabilir.</summary>
        Active = 2,

        /// <summary>Yönetim tarafından geçici olarak askıya alındı. Giriş yapabilir ama içerik üretemez.</summary>
        Suspended = 3,

        /// <summary>Kullanıcı hesabını kendi isteğiyle dondurdu. Tekrar giriş yaparak canlandırabilir.</summary>
        Deactivated = 4,

        /// <summary>Kalıcı olarak yasaklandı. Giriş yapamaz, yeni hesap açması engellenir.</summary>
        Banned = 5,

        /// <summary>Kullanıcı silme talebinde bulundu; yasal saklama süresi sonunda anonimleştirilecek.</summary>
        PendingDeletion = 6,

        /// <summary>Kapatıldı ve kişisel verileri anonimleştirildi (GDPR/KVKK "unutulma hakkı").</summary>
        Closed = 7
    }

    /// <summary>
    /// Kullanıcının platformdaki temel rolü. Eski <c>string UsersType</c> alanının yerini alır.
    /// Satıcılık ayrı bir eksendir — bkz. <see cref="VendorCapability"/>.
    /// </summary>
    public enum UserType : byte
    {
        /// <summary>Standart son kullanıcı alışveriş/satış yapabilir. Mağaza açabilir ve ürün satabilir. Sosyal medyayı kullanabilir. Yani tüm temel kullanıcı yetkilerine sahiptir.</summary>
        Customer = 1,

        /// <summary>Yönetici / moderatör / içerik denetleyici. Kısacası tüm yetkilere sahiptir.(Admin)</summary>
        SuperAdmin = 2
    }

    /// <summary>
    /// Kullanıcının satıcı (vendor) olma yetkisi. Eski <c>bool? IsActiveVendorStatu</c>
    /// alanının yerini alır; başvuru/onay akışını modelleyebilmek için enum'dır.
    /// </summary>
    public enum VendorCapability : byte
    {
        /// <summary>Satıcı değil, başvuru da yok.</summary>
        None = 1,

        /// <summary>Satıcı başvurusu yapıldı, belge/inceleme bekliyor.</summary>
        Pending = 2,

        /// <summary>Onaylandı — mağaza açabilir ve ürün satabilir.</summary>
        Approved = 3,

        /// <summary>Yetki geçici olarak askıya alındı (mağazaları da pasifleşir).</summary>
        Suspended = 4,

        /// <summary>Başvuru reddedildi.</summary>
        Rejected = 5
    }

    /// <summary>
    /// Profilin genel görünürlüğü. Facebook/Instagram "hesap gizliliği" karşılığıdır.
    /// Gönderi bazlı görünürlükten (data._Shares.PostVisibility) BAĞIMSIZDIR:
    /// profil kapalıysa gönderi Public olsa bile yabancıya gösterilmez.
    /// </summary>
    public enum ProfileVisibility : byte
    {
        /// <summary>Herkese açık — giriş yapmamış ziyaretçiler de görebilir.</summary>
        Public = 1,

        /// <summary>Yalnızca takipçiler görebilir.</summary>
        FollowersOnly = 2,

        /// <summary>Yalnızca arkadaşlar görebilir.</summary>
        FriendsOnly = 3,

        /// <summary>Kapalı — yalnızca kullanıcının kendisi.</summary>
        Private = 4
    }

    /// <summary>
    /// Bir gizlilik ayarının hedef kitlesi ("kimler yapabilir?").
    /// <c>UserPrivacySettings</c> içindeki tüm "WhoCan..." alanları bu tipi kullanır.
    /// </summary>
    public enum PrivacyAudience : byte
    {
        /// <summary>Herkes.</summary>
        Everyone = 1,

        /// <summary>Yalnızca takipçiler.</summary>
        FollowersOnly = 2,

        /// <summary>Yalnızca arkadaşlar.</summary>
        FriendsOnly = 3,

        /// <summary>Arkadaşların arkadaşları.</summary>
        FriendsOfFriends = 4,

        /// <summary>Hiç kimse (özellik tümüyle kapalı).</summary>
        NoOne = 5
    }

    /// <summary>
    /// Doğrulama (mavi tik / kurumsal rozet) durumu. Kullanıcı ve mağaza için ortaktır.
    /// DİKKAT: Bu, "doğrulanmış satın alma" (ProductReviews.IsVerifiedPurchase) ile
    /// KARIŞTIRILMAMALIDIR — o, sipariş geçmişinden türetilen ayrı bir kavramdır.
    /// </summary>
    public enum VerificationStatus : byte
    {
        /// <summary>Doğrulanmamış.</summary>
        None = 1,

        /// <summary>Başvuru yapıldı, inceleme bekliyor.</summary>
        Pending = 2,

        /// <summary>Doğrulandı — rozet gösterilir.</summary>
        Verified = 3,

        /// <summary>Başvuru reddedildi.</summary>
        Rejected = 4,

        /// <summary>Daha önce verilmiş rozet geri alındı.</summary>
        Revoked = 5
    }

    /// <summary>
    /// İki faktörlü doğrulama yöntemi.
    /// </summary>
    public enum TwoFactorMethod : byte
    {
        /// <summary>Kapalı.</summary>
        None = 1,

        /// <summary>E-posta ile tek kullanımlık kod.</summary>
        Email = 2,

        /// <summary>SMS ile tek kullanımlık kod.</summary>
        Sms = 3,

        /// <summary>TOTP uygulaması (Google/Microsoft Authenticator).</summary>
        AuthenticatorApp = 4,

        /// <summary>Donanım güvenlik anahtarı (WebAuthn/FIDO2).</summary>
        SecurityKey = 5
    }

    /// <summary>
    /// Ana sayfa akışının (feed) sıralama tercihi.
    /// </summary>
    public enum FeedSortPreference : byte
    {
        /// <summary>Algoritmik — ilgi/etkileşime göre.</summary>
        Algorithmic = 1,

        /// <summary>Kronolojik — en yeniden eskiye.</summary>
        MostRecent = 2,

        /// <summary>Yalnızca arkadaşlar.</summary>
        FriendsOnly = 3,

        /// <summary>Yalnızca takip edilen mağazalar.</summary>
        FollowedStoresOnly = 4
    }

    /// <summary>
    /// Adres türü. Eski <c>string AddressType</c> alanının yerini alır.
    /// </summary>
    public enum AddressType : byte
    {
        /// <summary>Bireysel adres.</summary>
        Individual = 1,

        /// <summary>Kurumsal adres (fatura için vergi bilgileri gerekir).</summary>
        Corporate = 2
    }

    /// <summary>
    /// KİŞİSEL/HASSAS VERİ — cinsiyet.
    ///
    /// Alan NULLABLE'dır ve doldurulması ZORUNLU DEĞİLDİR. Varsayılan olarak yalnızca
    /// kullanıcının kendisine görünür (UserPrivacySettings.ShowGender = false).
    /// KVKK/GDPR kapsamında özel nitelikli veri sayılabileceğinden:
    ///  • Hedefleme/reklam amacıyla kullanılmadan önce açık rıza alınmalıdır,
    ///  • Veri dışa aktarma ve silme taleplerinde kapsama dâhil edilmelidir,
    ///  • Herkese açık API yanıtlarında varsayılan olarak maskelenmelidir.
    /// </summary>
    public enum Gender : byte
    {
        /// <summary>Belirtilmedi.</summary>
        Unspecified = 1,

        /// <summary>Kadın.</summary>
        Female = 2,

        /// <summary>Erkek.</summary>
        Male = 3,

        /// <summary>Diğer / özel.</summary>
        Other = 4,

        /// <summary>Kullanıcı belirtmek istemiyor.</summary>
        PreferNotToSay = 5
    }
}
