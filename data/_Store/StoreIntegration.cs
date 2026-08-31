using System;

namespace data._Store
{
    /// <summary>
    /// MAĞAZA ENTEGRASYON KİMLİK BİLGİLERİ (e-fatura, ön muhasebe, ERP).
    ///
    /// GÜVENLİK KURALI (§61): Bu tablodaki tüm <c>Encrypted*</c> alanlar uygulama
    /// katmanında ŞİFRELENEREK yazılır (zarf şifreleme — anahtar bir anahtar
    /// kasasında/Key Vault'ta tutulur, veritabanında DEĞİL). Veritabanı yedeği
    /// sızsa bile bu değerler tek başına kullanılamaz.
    ///
    /// Ortak <see cref="StoreEntityBase"/> desenine taşınmış, sağlayıcı adı serbest
    /// metin olarak korunmuş (entegratör listesi sık değiştiği için enum yapılmamıştır)
    /// ve bağlantı sağlık izleme alanları eklenmiştir.
    /// </summary>
    public class StoreIntegration : StoreEntityBase
    {
        /// <summary>Entegrasyonun ait olduğu mağaza (Store.Id).</summary>
        public Guid StoreId { get; set; }

        /// <summary>Mağaza sahibi (Users.Id) — denormalize iz.</summary>
        public Guid? OwnerUserId { get; set; }

        /// <summary>Entegratör adı ("Parasut", "Logo", "Uyumsoft", "Mikro").</summary>
        public string Provider { get; set; } = string.Empty;

        /// <summary>Entegrasyon türü ("Invoicing", "Accounting", "Erp", "Marketplace").</summary>
        public string? IntegrationType { get; set; }

        // ── Kimlik bilgileri — HEPSİ ŞİFRELİ SAKLANIR ─────────────────────────
        /// <summary>ŞİFRELİ API anahtarı.</summary>
        public string? EncryptedApiKey { get; set; }

        /// <summary>ŞİFRELİ API gizli anahtarı.</summary>
        public string? EncryptedApiSecret { get; set; }

        /// <summary>ŞİFRELİ kullanıcı adı (bazı entegratörler için zorunlu).</summary>
        public string? EncryptedUserName { get; set; }

        /// <summary>ŞİFRELİ parola.</summary>
        public string? EncryptedPassword { get; set; }

        /// <summary>ŞİFRELİ erişim token'ı (OAuth kullanan sağlayıcılar için).</summary>
        public string? EncryptedAccessToken { get; set; }

        /// <summary>ŞİFRELİ yenileme token'ı.</summary>
        public string? EncryptedRefreshToken { get; set; }

        /// <summary>Erişim token'ının geçerlilik bitişi (UTC).</summary>
        public DateTime? TokenExpiresAtUtc { get; set; }

        /// <summary>Şifrelemede kullanılan anahtar sürümü ("kv-2026-01") — anahtar rotasyonu için.</summary>
        public string? EncryptionKeyVersion { get; set; }

        // ── Operasyonel alanlar ───────────────────────────────────────────────
        /// <summary>Şube kodu / firma kimliği gibi sağlayıcıya özel değer.</summary>
        public string? CompanyCode { get; set; }

        /// <summary>Sağlayıcının API uç noktası (test/canlı ayrımı için).</summary>
        public string? BaseUrl { get; set; }

        /// <summary>Test (sandbox) ortamı mı?</summary>
        public bool IsSandbox { get; set; }

        /// <summary>Bu entegrasyon aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Aynı türde birden çok entegrasyon varsa varsayılan olan mı?</summary>
        public bool IsDefault { get; set; } = true;

        /// <summary>Son başarılı bağlantı anı (UTC).</summary>
        public DateTime? LastSuccessfulSyncAtUtc { get; set; }

        /// <summary>Son hata mesajı (izleme paneli için).</summary>
        public string? LastErrorMessage { get; set; }

        /// <summary>Ardışık hata sayısı. Eşiği aşınca entegrasyon otomatik pasifleştirilir.</summary>
        public int ConsecutiveFailureCount { get; set; }
    }
}
