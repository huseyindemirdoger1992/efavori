using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Varyant kombinasyon tablosu. Her varyantın hangi özellik-değer ikililerinden
    /// oluştuğunu tutar. Örn: KZK-KRM-S varyantı için iki satır:
    ///   (VariantId, Renk, Kırmızı) ve (VariantId, Beden, S)
    /// </summary>
    public class ProductVariantValues
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid VariantId { get; set; } // Varyant (ProductVariants.Id)
        public Guid AttributeId { get; set; } // Özellik (ProductAttributes.Id) — sorgu kolaylığı için denormalize
        public Guid AttributeValueId { get; set; } // Özellik değeri (ProductAttributeValues.Id)
    }
}
