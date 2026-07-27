using System;
using data.Owned; // mevcut ortak soft-delete owned tipi (data.Owned.IsDeleted)

namespace data._BulkImportProducts
{
    /// <summary>
    /// Toplu ürün içe aktarım sistemindeki tüm entity'lerin ortak tabanı.
    /// Product/Attribute sistemlerindeki EntityBase deseninin birebir aynısı:
    /// GUID birincil anahtar, denetim tarih/kullanıcı alanları, ortak soft-delete
    /// owned tipi ve iyimser eşzamanlılık (RowVersion).
    ///
    /// KONVANSİYON: Navigation property İÇERMEZ — ilişkiler yalnızca ID üzerinden
    /// kurulur; FK'ler, indeksler ve silme davranışları
    /// <see cref="_BulkImportModelConfiguration"/> içinde Fluent API ile tanımlanır.
    /// </summary>
    public abstract class BulkImportEntityBase
    {
        /// <summary>Birincil anahtar. İstemci tarafında üretilen GUID (mevcut konvansiyon).</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Kaydın oluşturulma anı (UTC).</summary>
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>Kaydı oluşturan kullanıcı (varsa).</summary>
        public Guid? CreatedByUserId { get; set; }

        /// <summary>Son güncelleme anı (UTC).</summary>
        public DateTime? UpdatedAtUtc { get; set; }

        /// <summary>Son güncelleyen kullanıcı (varsa).</summary>
        public Guid? UpdatedByUserId { get; set; }

        /// <summary>Soft-delete durumu (mevcut ortak owned tip).</summary>
        public IsDeleted IsDeleted { get; set; } = new();

        /// <summary>
        /// İyimser eşzamanlılık damgası (rowversion). Çok örnekli import
        /// BackgroundService'lerinde ve eşzamanlı düzenlemede yarış koşulunu önler.
        /// </summary>
        public byte[]? RowVersion { get; set; }
    }
}
