using System;

namespace data._Users
{
    /// <summary>
    /// KİMLİK DOĞRULAMA SIRLARI VE GÜVENLİK DURUMU.
    ///
    /// <see cref="Users"/> ile 1:1'dir. AYRI TABLO OLMASININ GEREKÇESİ GÜVENLİKTİR:
    /// profil/feed sorguları <c>SELECT *</c> ile parola hash'ini yanlışlıkla çekemez,
    /// veritabanı seviyesinde bu tabloya erişim ayrı bir role kısıtlanabilir ve
    /// denetim (audit) kuralları yalnızca burada uygulanır.
    ///
    /// KESİN KURALLAR:
    ///  • Parola ASLA düz metin (plain text) saklanmaz. <see cref="PasswordHash"/>
    ///    yalnızca yavaş ve tuzlanmış bir KDF çıktısıdır (Argon2id veya PBKDF2-SHA256,
    ///    en az 210.000 iterasyon). Algoritma ve parametreler
    ///    <see cref="PasswordHashAlgorithm"/> alanında saklanır ki ileride
    ///    parametre yükseltmesi yapılabilsin.
    ///  • MFA gizli anahtarı (<see cref="TwoFactorSecret"/>) uygulama katmanında
    ///    şifrelenmiş (envelope encryption) olarak yazılır; DB'de düz durmaz.
    ///  • Bu tablo hiçbir zaman API yanıtına serileştirilmez.
    /// </summary>
    public class UserSecurity : UserEntityBase
    {
        /// <summary>Kaydın sahibi (Users.Id). 1:1 — soft-delete filtreli TEKİL indeks.</summary>
        public Guid UserId { get; set; }

        // ── Parola ────────────────────────────────────────────────────────────
        /// <summary>Tuzlanmış ve yavaş KDF ile üretilmiş parola özeti. DÜZ METİN DEĞİLDİR.</summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Kullanılan algoritma ve parametreleri ("argon2id$v=19$m=65536,t=3,p=4").
        /// Parametreler yükseltildiğinde eski hash'ler girişte sessizce yeniden üretilir.
        /// </summary>
        public string? PasswordHashAlgorithm { get; set; }

        /// <summary>Parolanın en son değiştirildiği an (UTC).</summary>
        public DateTime? PasswordChangedAtUtc { get; set; }

        /// <summary>Kullanıcının bir sonraki girişte parola değiştirmesi zorunlu mu?</summary>
        public bool MustChangePassword { get; set; }

        /// <summary>
        /// GÜVENLİK DAMGASI. Parola değişimi, MFA değişimi veya "tüm oturumları kapat"
        /// işleminde yenilenir; mevcut tüm token/çerezler böylece geçersizleşir.
        /// </summary>
        public string SecurityStamp { get; set; } = Guid.NewGuid().ToString("N");

        // ── Parola sıfırlama / aktivasyon (kod DEĞİL, token hash'i) ───────────
        /// <summary>
        /// Parola sıfırlama token'ının SHA-256 özeti. Token'ın kendisi yalnızca
        /// e-posta ile gönderilir, DB'de tutulmaz (sızıntıda kullanılamaz).
        /// </summary>
        public string? PasswordResetTokenHash { get; set; }

        /// <summary>Parola sıfırlama token'ının geçerlilik bitişi (UTC).</summary>
        public DateTime? PasswordResetTokenExpiresAtUtc { get; set; }

        /// <summary>Hesap aktivasyon token'ının SHA-256 özeti.</summary>
        public string? ActivationTokenHash { get; set; }

        /// <summary>Aktivasyon token'ının geçerlilik bitişi (UTC).</summary>
        public DateTime? ActivationTokenExpiresAtUtc { get; set; }

        /// <summary>E-posta değişikliği doğrulama token'ının SHA-256 özeti.</summary>
        public string? EmailChangeTokenHash { get; set; }

        /// <summary>Onay bekleyen yeni e-posta adresi (onaylanana kadar Users.Email değişmez).</summary>
        public string? PendingEmail { get; set; }

        /// <summary>Telefon doğrulama kodunun SHA-256 özeti.</summary>
        public string? PhoneVerificationCodeHash { get; set; }

        /// <summary>Telefon doğrulama kodunun geçerlilik bitişi (UTC).</summary>
        public DateTime? PhoneVerificationCodeExpiresAtUtc { get; set; }

        // ── İki faktörlü doğrulama ────────────────────────────────────────────
        /// <summary>MFA açık mı?</summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>Kullanılan MFA yöntemi.</summary>
        public TwoFactorMethod TwoFactorMethod { get; set; } = TwoFactorMethod.None;

        /// <summary>TOTP gizli anahtarı — UYGULAMA KATMANINDA ŞİFRELENMİŞ olarak yazılır.</summary>
        public string? TwoFactorSecret { get; set; }

        /// <summary>Kurtarma kodlarının hash'leri (JSON dizi). Her kod tek kullanımlıktır.</summary>
        public string? TwoFactorRecoveryCodeHashesJson { get; set; }

        /// <summary>MFA'nın etkinleştirildiği an (UTC).</summary>
        public DateTime? TwoFactorEnabledAtUtc { get; set; }

        // ── Kilitlenme (brute-force koruması) ─────────────────────────────────
        /// <summary>Ardışık başarısız giriş denemesi sayısı. Başarılı girişte sıfırlanır.</summary>
        public int AccessFailedCount { get; set; }

        /// <summary>Kilitlenme özelliği bu hesap için etkin mi?</summary>
        public bool LockoutEnabled { get; set; } = true;

        /// <summary>Kilidin kalkacağı an (UTC). Null veya geçmiş = kilitli değil.</summary>
        public DateTime? LockoutEndUtc { get; set; }

        /// <summary>Son başarısız giriş denemesi anı (UTC).</summary>
        public DateTime? LastFailedLoginAtUtc { get; set; }

        /// <summary>Son parola değişikliğinin yapıldığı IP adresi (şüpheli etkinlik analizi).</summary>
        public string? LastPasswordChangeIpAddress { get; set; }
    }
}
