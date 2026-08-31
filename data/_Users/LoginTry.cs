using System;

namespace data._Users
{
    /// <summary>
    /// GİRİŞ DENEMESİ KAYDI — güvenlik denetimi ve brute-force analizi.
    ///
    /// YÜKSEK HACİMLİ, SALT-EKLEME (append-only) tablosudur:
    ///  • Soft delete YOKTUR — güvenlik kaydı silinmez.
    ///  • RowVersion YOKTUR — kayıt hiç güncellenmez.
    ///  • Saklama süresi dolan satırlar arka plan servisi tarafından toplu silinir
    ///    (<c>AllBackgroundServicesFrequencyRate</c> ile yapılandırılır).
    ///
    /// GİZLİLİK: <see cref="AttemptedIdentifier"/> alanına ASLA parola veya parola
    /// parçası yazılmaz; yalnızca denenen e-posta/telefon tutulur.
    /// </summary>
    public class LoginTry
    {
        /// <summary>Birincil anahtar.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Eşleşen kullanıcı (Users.Id). Kullanıcı bulunamadıysa null.</summary>
        public Guid? UserId { get; set; }

        /// <summary>Denenen kimlik (e-posta veya telefon). Parola BURAYA YAZILMAZ.</summary>
        public string? AttemptedIdentifier { get; set; }

        /// <summary>Deneme anı (UTC).</summary>
        public DateTime AttemptedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>Deneme başarılı mı?</summary>
        public bool IsSuccessful { get; set; }

        /// <summary>Başarısızlık nedeni ("InvalidPassword", "LockedOut", "UserNotFound", "MfaRequired").</summary>
        public string? FailureReason { get; set; }

        /// <summary>İstek IP adresi (IPv6 dâhil).</summary>
        public string? IpAddress { get; set; }

        /// <summary>Ham User-Agent bilgisi.</summary>
        public string? UserAgent { get; set; }

        /// <summary>Platform ("Web", "Android", "iOS").</summary>
        public string? Platform { get; set; }

        /// <summary>Tarayıcı adı ve sürümü.</summary>
        public string? Browser { get; set; }

        /// <summary>Cihaz parmak izi — "yeni cihazdan giriş" bildirimi için.</summary>
        public string? DeviceFingerprint { get; set; }

        /// <summary>IP'den çözümlenen ülke kodu (ISO 3166-1 alpha-2).</summary>
        public string? CountryCode { get; set; }

        /// <summary>İşlemin yapıldığı URL yolu.</summary>
        public string? RequestPath { get; set; }
    }
}
