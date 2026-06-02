using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data
{
    /// <summary>
    /// Bir varyantı bir özellik-değere bağlayan istek (varyant ↔ değer eşleştirmesi).
    /// Tek başına gönderilir; her varyant için, her varyant-belirleyici özellikten bir tane atılır.
    /// Örn: "Kırmızı / XL" varyantı için iki ayrı istek (Renk=Kırmızı, Beden=XL).
    /// </summary>
    public class ProductVariantValueCreateRequest
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid VariantId { get; set; } // Bağlanacak varyantın ID'si
        public Guid AttributeId { get; set; } // İlgili özelliğin ID'si (denormalize, sorgu kolaylığı için)
        public Guid AttributeValueId { get; set; } // Atanan değerin ID'si
    }
}
