using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Bir özelliğe ait değer (Örn: Renk -> "Kırmızı", Beden -> "XL").
    /// AttributeId ile bağlı olduğu ProductAttribute kaydına işaret eder.
    /// </summary>
    public class ProductAttributeValue
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; } // Değeri ekleyen kullanıcının/satıcının ID bilgisi
        public Guid StoreId { get; set; } // Değerin bağlı olduğu mağazanın ID bilgisi
        public Guid AttributeId { get; set; } // Değerin bağlı olduğu özelliğin ID'si

        public string Value { get; set; } = string.Empty; // Değer metni (Örn: "Kırmızı")
        public string? ColorHex { get; set; } // Renk tipi değerler için hex kodu (Örn: "#FF0000")
        public string? ImagePath { get; set; } // Görsel tabanlı değerler için swatch/görsel yolu
        public int DisplayOrder { get; set; } = 0; // Listelenme sırası

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
