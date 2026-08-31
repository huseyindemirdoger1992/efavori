using System;

namespace data._Users
{
    /// <summary>
    /// GÖNDERİLEN E-POSTA ARŞİVİ.
    ///
    /// Bu tablo bir KUYRUK DEĞİLDİR — gönderim kuyruğu
    /// <c>data._Notifications.NotificationDeliveries</c> (Channel = Email) tablosudur.
    /// Burası, gönderilmiş e-postanın değişmez (immutable) arşiv kaydıdır:
    /// destek talebi geldiğinde "kullanıcıya tam olarak ne yazdık?" sorusunu yanıtlar.
    ///
    /// SOFT DELETE UYGULANMAZ (§40): arşiv kaydı silinmez.
    /// </summary>
    public class EmailHistory
    {
        /// <summary>Birincil anahtar.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Alıcı kullanıcı (Users.Id). Sistem/dış adreslere gönderimde null olabilir.</summary>
        public Guid? UserId { get; set; }

        /// <summary>Bu arşivi üreten bildirim gönderimi (NotificationDeliveries.Id). Manuel gönderimde null.</summary>
        public Guid? NotificationDeliveryId { get; set; }

        /// <summary>Gönderen adres.</summary>
        public string FromAddress { get; set; } = string.Empty;

        /// <summary>Alıcı adres.</summary>
        public string ToAddress { get; set; } = string.Empty;

        /// <summary>Bilgi (CC) adresleri — virgülle ayrılmış.</summary>
        public string? CcAddresses { get; set; }

        /// <summary>Gizli bilgi (BCC) adresleri — virgülle ayrılmış.</summary>
        public string? BccAddresses { get; set; }

        /// <summary>Konu satırı.</summary>
        public string Subject { get; set; } = string.Empty;

        /// <summary>Gönderilen gövde (HTML veya düz metin).</summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>Ek dosyalar (data._Galleries.Media.Id listesi, JSON dizi).</summary>
        public string? AttachmentMediaIdsJson { get; set; }

        /// <summary>Dağıtık izleme kimliği (log korelasyonu için).</summary>
        public string? TraceId { get; set; }

        /// <summary>Sağlayıcı tarafındaki mesaj kimliği (bounce/webhook eşlemesi).</summary>
        public string? ProviderMessageId { get; set; }

        /// <summary>Gönderim anı (UTC).</summary>
        public DateTime SentAtUtc { get; set; } = DateTime.UtcNow;
    }
}
