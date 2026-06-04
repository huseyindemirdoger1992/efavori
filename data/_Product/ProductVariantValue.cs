using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Bir varyantı bir özellik-değere bağlayan ara (junction) tablo: varyant ↔ değer eşleştirmesi.
    /// "Kırmızı / XL" varyantı için iki satır olur (Renk=Kırmızı, Beden=XL).
    /// </summary>
    public class ProductVariantValue
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; } // Ürünü ekleyen kullanıcının/satıcının ID bilgisi
        public Guid StoreId { get; set; } // Ürünün bağlı olduğu mağazanın ID bilgisi
        public Guid VariantId { get; set; } // Bağlanan varyantın ID'si
        public Guid AttributeId { get; set; } // İlgili özelliğin ID'si (denormalize, sorgu kolaylığı için)
        public Guid AttributeValueId { get; set; } // Atanan değerin ID'si

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
