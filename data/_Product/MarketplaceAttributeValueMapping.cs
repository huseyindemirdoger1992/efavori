using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Yerel bir özellik değerinin (ProductAttributeValue) belirli bir pazaryerindeki karşılığına eşleştirilmesi.
    /// Örn: yerel "Kırmızı" -> Trendyol renk değeri ID'si. Değer düzeyi eşleştirme için kullanılır.
    /// </summary>
    public class MarketplaceAttributeValueMapping
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; } // Eşleştirmeyi yöneten kullanıcının/satıcının ID bilgisi
        public Guid StoreId { get; set; } // Değerin bağlı olduğu mağazanın ID bilgisi
        public Guid MarketplaceId { get; set; } // Hedef pazaryerinin ID'si
        public Guid AttributeValueId { get; set; } // Yerel değerin (ProductAttributeValue) ID'si

        public string? ExternalAttributeValueId { get; set; } // Pazaryerindeki değer ID'si
        public string? ExternalAttributeValueName { get; set; } // Pazaryerindeki değer adı

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
