using System;
using data.Owned;

namespace data._Users
{
    /// <summary>
    /// Kullanıcı/Kimlik modülündeki (data._Users) TÜM entity'lerin ortak taban sınıfı.
    /// Mevcut <see cref="data._Products.ProductEntityBase"/> /
    /// <see cref="data._Orders.OrderEntityBase"/> /
    /// <see cref="data._Notifications.NotificationEntityBase"/> deseniyle birebir aynıdır:
    ///
    ///  • Guid birincil anahtar istemci tarafında üretilir (client-side PK deseni).
    ///  • Tüm tarihler UTC'dir ve <c>...Utc</c> son eki taşır.
    ///  • Soft delete <see cref="data.Owned.IsDeleted"/> owned tipi ile yapılır.
    ///  • <see cref="RowVersion"/> SQL Server rowversion kolonuna eşlenir (iyimser eşzamanlılık).
    ///
    /// KONVANSİYON: Navigation property YASAKTIR. İlişkiler yalnızca FK (Guid) alanları
    /// üzerinden kurulur; FK, indeks, tekillik ve silme davranışları
    /// <see cref="_UserModelConfiguration"/> içinde Fluent API ile tanımlanır.
    /// </summary>
    public abstract class UserEntityBase
    {
        /// <summary>Birincil anahtar. İstemci tarafında üretilir.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Kaydın oluşturulma anı (UTC).</summary>
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>Kaydı oluşturan kullanıcı (Users.Id). Sistem üretimi kayıtlarda null.</summary>
        public Guid? CreatedByUserId { get; set; }

        /// <summary>Son güncelleme anı (UTC). Hiç güncellenmediyse null.</summary>
        public DateTime? UpdatedAtUtc { get; set; }

        /// <summary>Son güncelleyen kullanıcı (Users.Id).</summary>
        public Guid? UpdatedByUserId { get; set; }

        /// <summary>Soft delete durumu. Fiziksel silme yapılmaz.</summary>
        public IsDeleted IsDeleted { get; set; } = new();

        /// <summary>İyimser eşzamanlılık belirteci (SQL Server rowversion).</summary>
        public byte[]? RowVersion { get; set; }
    }
}
