using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Bir kategoriye hangi özelliklerin bağlı olduğunu belirleyen ara tablo (kategori bazlı attribute).
    /// CategoryId yerel kategoriyi (CategoriesTr.Id -> int), AttributeId ise özelliği işaret eder.
    /// </summary>
    public class CategoryAttribute
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; } // Eşleştirmeyi ekleyen kullanıcının/satıcının ID bilgisi
        public Guid StoreId { get; set; } // Özelliklerin bağlı olduğu mağazanın ID bilgisi
        public int CategoryId { get; set; } // Yerel kategori ID'si (CategoriesTr.Id -> int)
        public Guid AttributeId { get; set; } // Kategoriye bağlanan özelliğin ID'si

        public bool IsRequired { get; set; } = false; // Bu kategoride özellik zorunlu mu
        public bool IsFilterable { get; set; } = true; // Bu kategoride filtrede kullanılsın mı
        public bool IsVariantDefining { get; set; } = false; // Bu kategoride varyant üretiminde kullanılsın mı
        public int DisplayOrder { get; set; } = 0; // Listelenme sırası

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
