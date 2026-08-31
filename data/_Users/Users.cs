using System;

namespace data._Users
{
    /// <summary>
    /// KULLANICI — KİMLİK VE HESAP YAŞAM DÖNGÜSÜ ÇEKİRDEĞİ.
    ///
    /// Bu tablo bilinçli olarak DARDIR. Yalnızca "bu hesap kimdir, hangi durumdadır ve
    /// giriş yapabilir mi?" sorularını yanıtlar. Her istekte okunan sıcak (hot) tablodur;
    /// bu yüzden nadiren değişen ve nadiren okunan alanlar buraya KONULMAZ.
    ///
    /// SORUMLULUK AYRIMI (§5):
    ///  • Sosyal profil (username, bio, avatar, sayaçlar) → <see cref="UserProfiles"/>
    ///  • Dil/para birimi/akış/bildirim tercihleri        → <see cref="UserSettings"/>
    ///  • Gizlilik ("kimler yapabilir")                    → <see cref="UserPrivacySettings"/>
    ///  • Parola, MFA, kilitlenme, güvenlik damgası        → <see cref="UserSecurity"/>
    ///  • Ödeme yöntemi (PCI DSS)                          → data._Payments.UserPaymentMethods
    ///  • Adres                                            → <see cref="UserAddress"/>
    ///
    /// GÜVENLİK: Parola HASH'i dâhil hiçbir kimlik doğrulama sırrı bu tabloda TUTULMAZ.
    /// Eski <c>Users.Password</c> alanı kaldırılmıştır (bkz. <see cref="UserSecurity"/>).
    /// </summary>
    public class Users : UserEntityBase
    {
        // ── Hesap durumu ──────────────────────────────────────────────────────
        /// <summary>Hesabın yaşam döngüsü durumu. Eski <c>bool? IsActive</c> yerine geçer.</summary>
        public UserAccountStatus AccountStatus { get; set; } = UserAccountStatus.PendingActivation;

        /// <summary>Platformdaki temel rol. Eski <c>string UsersType</c> yerine geçer.</summary>
        public UserType UserType { get; set; } = UserType.Customer;

        /// <summary>Satıcı olma yetkisi. Eski <c>bool? IsActiveVendorStatu</c> yerine geçer.</summary>
        public VendorCapability VendorCapability { get; set; } = VendorCapability.None;

        /// <summary>Hesap durumunun en son değiştiği an (UTC) — askıya alma/yasaklama izi.</summary>
        public DateTime? AccountStatusChangedAtUtc { get; set; }

        /// <summary>Askıya alma/yasaklama gerekçesi (yalnızca yönetim panelinde görünür).</summary>
        public string? AccountStatusReason { get; set; }

        /// <summary>Geçici askıya almanın kendiliğinden biteceği an (UTC). Null = süresiz.</summary>
        public DateTime? SuspendedUntilUtc { get; set; }

        // ── Kimlik doğrulayıcılar (login identifier) ──────────────────────────
        /// <summary>
        /// Birincil e-posta adresi (giriş kimliği). Normalize edilmiş hâli
        /// <see cref="NormalizedEmail"/> alanındadır ve TEKİL indekslidir.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Büyük harfe/trim'e normalize edilmiş e-posta. Soft-delete filtreli TEKİL indeks
        /// buradadır — büyük/küçük harf farkıyla ikinci hesap açılması engellenir.
        /// </summary>
        public string NormalizedEmail { get; set; } = string.Empty;

        /// <summary>E-posta doğrulandı mı?</summary>
        public bool IsEmailConfirmed { get; set; }

        /// <summary>E-postanın doğrulandığı an (UTC).</summary>
        public DateTime? EmailConfirmedAtUtc { get; set; }

        /// <summary>Telefon ülke kodu (ör. "+90").</summary>
        public string? PhoneCountryCode { get; set; }

        /// <summary>Telefon numarası (ülke kodu hariç).</summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// E.164 biçiminde tam telefon numarası (ör. "+905321234567").
        /// Soft-delete filtreli TEKİL indeks buradadır (null'lar hariç).
        /// </summary>
        public string? NormalizedPhoneNumber { get; set; }

        /// <summary>Telefon doğrulandı mı?</summary>
        public bool IsPhoneConfirmed { get; set; }

        /// <summary>Telefonun doğrulandığı an (UTC).</summary>
        public DateTime? PhoneConfirmedAtUtc { get; set; }

        // ── Kayıt / aktivasyon ────────────────────────────────────────────────
        /// <summary>Sisteme ilk kayıt anı (UTC).</summary>
        public DateTime RegistrationDateUtc { get; set; } = DateTime.UtcNow;

        /// <summary>Kayıt sırasındaki IP adresi (dolandırıcılık analizi ve KVKK kaydı için).</summary>
        public string? RegistrationIpAddress { get; set; }

        /// <summary>Kullanım koşulları ve gizlilik sözleşmesi kabul edildi mi?</summary>
        public bool TermsAccepted { get; set; }

        /// <summary>Sözleşmelerin kabul edildiği an (UTC) — yasal ispat için zorunlu.</summary>
        public DateTime? TermsAcceptedAtUtc { get; set; }

        /// <summary>Kabul edilen sözleşme sürümü (ör. "2026-01-tr"). Sürüm değişince yeniden onay istenir.</summary>
        public string? TermsVersion { get; set; }

        /// <summary>Kullanıcıyı davet eden/sponsor olan kullanıcı (Users.Id). Eski e-posta tabanlı alanın FK'li karşılığı.</summary>
        public Guid? SponsorUserId { get; set; }

        // ── Kurumsal / çalışan bağlamı (mevcut iş kuralı korunmuştur) ─────────
        /// <summary>Kullanıcı bir iş istasyonu/çalışan grubuna bağlı mı? (grup kimliği).</summary>
        public Guid? WorkstationEmployeeGroupId { get; set; }

        /// <summary>Kullanıcı çalışan mı?</summary>
        public bool IsEmployee { get; set; }

        // ── Oturum izleri ─────────────────────────────────────────────────────
        /// <summary>Son başarılı giriş anı (UTC).</summary>
        public DateTime? LastLoginAtUtc { get; set; }

        /// <summary>Son başarılı girişteki IP adresi.</summary>
        public string? LastLoginIpAddress { get; set; }

        /// <summary>
        /// Son etkinlik anı (UTC) — "son görülme" gösteriminin kaynağıdır.
        /// Gösterilip gösterilmeyeceğine <c>UserPrivacySettings.ShowLastSeen</c> karar verir.
        /// </summary>
        public DateTime? LastSeenAtUtc { get; set; }

        /// <summary>
        /// Hareketsizlik denetimi: oturumun otomatik sonlandırılacağı süre (saniye).
        /// Null = sistem varsayılanı geçerlidir.
        /// </summary>
        public int? LogOutTimerSeconds { get; set; }

        // ── Silme / anonimleştirme (KVKK/GDPR) ────────────────────────────────
        /// <summary>Kullanıcının hesap silme talebini oluşturduğu an (UTC).</summary>
        public DateTime? DeletionRequestedAtUtc { get; set; }

        /// <summary>Yasal bekleme süresi sonrası verilerin anonimleştirileceği an (UTC).</summary>
        public DateTime? ScheduledAnonymizationAtUtc { get; set; }

        /// <summary>Kişisel veriler anonimleştirildi mi? true ise profil/adres alanları maskelenmiştir.</summary>
        public bool IsAnonymized { get; set; }
    }
}
