using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Yerel bir özelliğin (ProductAttribute) belirli bir pazaryerindeki karşılığına eşleştirilmesi.
    /// Çoğunlukla kategori bazlıdır (CategoryId opsiyonel). CSV import/export bu eşleştirmeden yürür.
    /// </summary>
    public class MarketplaceAttributeMapping
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; } // Eşleştirmeyi yöneten kullanıcının/satıcının ID bilgisi
        public Guid StoreId { get; set; } // Özelliğin bağlı olduğu mağazanın ID bilgisi
        public Guid MarketplaceId { get; set; } // Hedef pazaryerinin ID'si
        public Guid AttributeId { get; set; } // Yerel özelliğin (ProductAttribute) ID'si
        public int? CategoryId { get; set; } // Eşleştirmenin geçerli olduğu yerel kategori (opsiyonel)

        public string? ExternalAttributeId { get; set; } // Pazaryerindeki özellik ID'si
        public string? ExternalAttributeName { get; set; } // Pazaryerindeki özellik adı
        public bool IsRequired { get; set; } = false; // Pazaryeri bu özelliği zorunlu kılıyor mu

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
