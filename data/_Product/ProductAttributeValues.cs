using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Özelliklerin önceden tanımlı değerleri (Örn: Renk → Kırmızı, Bordo, Siyah).
    /// Sahiplik kuralı ProductAttributes ile aynıdır:
    ///   UserId/StoreId null → sistem değeri, dolu → yalnızca o satıcıda görünen özel değer.
    /// Böylece sistem özelliği olan "Renk"e bir satıcı kendi özel rengini ekleyebilir
    /// ama bu değer diğer satıcılara yansımaz.
    /// </summary>
    public class ProductAttributeValues
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid AttributeId { get; set; } // Bağlı olduğu özellik (ProductAttributes.Id)

        public Guid? UserId { get; set; } // Null = sistem değeri, dolu = satıcıya özel değer
        public Guid? StoreId { get; set; } // Null = sistem değeri, dolu = mağazaya özel değer

        public string? Value { get; set; } // Değer metni (Örn: Kırmızı, 16 GB, 55")
        public string? Code { get; set; } // Teknik kod (Örn: kirmizi) — entegrasyon eşlemede kullanılır
        public string? ColorHex { get; set; } // Renk özellikleri için HEX kodu (Örn: #FF0000) — UI renk kutucuğu
        public int DisplayOrder { get; set; } = 0; // Listeleme sırası
        public bool IsActive { get; set; } = true; // Değer kullanımda mı

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
