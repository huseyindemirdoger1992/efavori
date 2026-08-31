using System;
using data.Owned;

namespace data._Chat
{
    /// <summary>
    /// Mesajlaşma modülündeki (data._Chat) TÜM entity'lerin ortak taban sınıfı.
    /// Mevcut modül taban sınıfı desenini izler.
    ///
    /// KONVANSİYON: Navigation property YASAKTIR; ilişkiler yalnızca FK üzerinden kurulur.
    /// </summary>
    public abstract class ChatEntityBase
    {
        /// <summary>Birincil anahtar. İstemci tarafında üretilir.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Kaydın oluşturulma anı (UTC).</summary>
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>Kaydı oluşturan kullanıcı (Users.Id).</summary>
        public Guid? CreatedByUserId { get; set; }

        /// <summary>Son güncelleme anı (UTC).</summary>
        public DateTime? UpdatedAtUtc { get; set; }

        /// <summary>Son güncelleyen kullanıcı (Users.Id).</summary>
        public Guid? UpdatedByUserId { get; set; }

        /// <summary>Soft delete durumu.</summary>
        public IsDeleted IsDeleted { get; set; } = new();

        /// <summary>İyimser eşzamanlılık belirteci (SQL Server rowversion).</summary>
        public byte[]? RowVersion { get; set; }
    }
}
