using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Bir varyantın hangi attribute değerlerinden oluştuğunu tutar.
    /// Örn: Varyant "Kırmızı-XL" → 2 satır:
    ///   → AttributeId=Renk,  AttributeValueId=Kırmızı
    ///   → AttributeId=Beden, AttributeValueId=XL
    /// Kartezyen çarpım sonrası her varyant kombinasyonu bu tabloda tanımlanır.
    /// </summary>
    public class ProductVariantAttributeMappings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid VariantId { get; set; }        // Varyant ID bilgisi (ProductVariants.Id)
        public Guid AttributeId { get; set; }      // Attribute tanımı ID bilgisi (ProductAttributes.Id)
        public Guid AttributeValueId { get; set; } // Attribute değeri ID bilgisi (ProductAttributeValues.Id)

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
