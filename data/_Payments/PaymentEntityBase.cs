namespace data._Payments
{
    /// <summary>
    /// Ödeme ve Hakediş Sistemi V1 (data._Payments) modülündeki TÜM entity'lerin ortak taban sınıfı.
    /// Mevcut ProductEntityBase / OrderEntityBase deseni ile birebir aynıdır:
    /// - Guid PK istemci tarafında üretilir (Guid.NewGuid()).
    /// - Tüm tarihler UTC'dir ve ...Utc son eki taşır.
    /// - Soft delete data.Owned.IsDeleted owned tipi ile yapılır; fiziksel silme YOKTUR.
    /// - RowVersion, SQL Server rowversion kolonuna eşlenir ve iyimser eşzamanlılık için kullanılır.
    /// Navigation property YASAKTIR; ilişkiler yalnızca ID (FK) alanları ile kurulur.
    /// </summary>
    public abstract class PaymentEntityBase
    {
        /// <summary>Birincil anahtar. İstemci tarafında üretilir (client-side PK deseni).</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Kaydın oluşturulma zamanı (UTC).</summary>
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>Kaydı oluşturan kullanıcı (Users.Id). Sistem/BackgroundService/webhook üretimlerinde null olabilir.</summary>
        public Guid? CreatedByUserId { get; set; }

        /// <summary>Son güncelleme zamanı (UTC). Hiç güncellenmediyse null.</summary>
        public DateTime? UpdatedAtUtc { get; set; }

        /// <summary>Son güncelleyen kullanıcı (Users.Id).</summary>
        public Guid? UpdatedByUserId { get; set; }

        /// <summary>Soft delete durumu. Fiziksel silme yapılmaz; tekil indeksler SoftDeleteFilter ile filtrelenir.</summary>
        public data.Owned.IsDeleted IsDeleted { get; set; } = new();

        /// <summary>İyimser eşzamanlılık belirteci (SQL Server rowversion).</summary>
        public byte[]? RowVersion { get; set; }
    }
}