using data.Owned;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data._Categories
{
    public class CategoriesArticle
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Üst kategori ID
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Aktif / Pasif
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Menüde gösterilsin mi?
        /// </summary>
        public bool ShowInMenu { get; set; } = true;


        /// <summary>
        /// Dil bilgileri ve SEO URL bilgilerini içeren Categories Owned sınıfı
        /// </summary>
        public Categories? Categories { get; set; } = new();

        /// <summary>
        /// Oluşturulma tarihi
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Güncellenme tarihi
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        public IsDeleted? IsDeleted { get; set; } = new();

    }
}
