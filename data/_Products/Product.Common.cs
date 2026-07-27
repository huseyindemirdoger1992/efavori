using System;
using data.Owned; // mevcut ortak soft-delete owned tipi (data.Owned.IsDeleted)

namespace data._Products
{
    /// <summary>
    /// Ürün sistemindeki tüm entity'lerin ortak tabanı.
    /// Attribute System V3'teki <see cref="data._Attribute.AttributeEntityBase"/>
    /// ile birebir aynı deseni izler: GUID birincil anahtar, denetim tarih/kullanıcı
    /// alanları, ortak soft-delete owned tipi ve iyimser eşzamanlılık (RowVersion).
    ///
    /// KONVANSİYON: Navigation property İÇERMEZ — ilişkiler yalnızca ID üzerinden
    /// kurulur; FK'ler, indeksler ve silme davranışları
    /// <see cref="_ProductModelConfiguration"/> içinde Fluent API ile tanımlanır.
    /// </summary>
    public abstract class ProductEntityBase
    {
        /// <summary>Birincil anahtar. İstemci tarafında üretilen GUID (mevcut proje konvansiyonu).</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Kaydın oluşturulma anı (UTC).</summary>
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>Kaydı oluşturan kullanıcı (varsa). Sistem/AI kayıtlarında null olabilir.</summary>
        public Guid? CreatedByUserId { get; set; }

        /// <summary>Son güncelleme anı (UTC).</summary>
        public DateTime? UpdatedAtUtc { get; set; }

        /// <summary>Son güncelleyen kullanıcı (varsa).</summary>
        public Guid? UpdatedByUserId { get; set; }

        /// <summary>Soft-delete durumu (mevcut ortak owned tip).</summary>
        public IsDeleted IsDeleted { get; set; } = new();

        /// <summary>
        /// İyimser eşzamanlılık damgası (rowversion). Blazor Server'da aynı kaydı
        /// açan iki admin/satıcının birbirini ezmesini ve BackgroundService yarış
        /// koşullarını önler.
        /// </summary>
        public byte[]? RowVersion { get; set; }
    }
}
