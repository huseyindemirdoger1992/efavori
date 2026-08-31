using System;
using data.Owned;

namespace data._Follows
{
    /// <summary>
    /// Sosyal graf modülündeki (data._Follows) TÜM entity'lerin ortak taban sınıfı.
    /// Mevcut modül taban sınıfı deseniyle (ProductEntityBase / OrderEntityBase) aynıdır.
    ///
    /// KONVANSİYON: Navigation property YASAKTIR; ilişkiler yalnızca FK üzerinden kurulur.
    /// FK, indeks, tekillik, CHECK kısıtı ve silme davranışları
    /// <see cref="_FollowModelConfiguration"/> içinde Fluent API ile tanımlanır.
    /// </summary>
    public abstract class FollowEntityBase
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
