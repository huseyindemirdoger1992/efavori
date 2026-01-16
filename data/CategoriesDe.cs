using System.ComponentModel.DataAnnotations;

namespace data
{
    public class CategoriesDe
    {
        [Key]
        public int Id { get; set; } // Veritabanının oluşturacağı otomatik artan ID
        public string? Name { get; set; } // Kategori adı (Örn: Korkuluklar)

        public string? DepartmentName { get; set; } // En üst grup adı (Örn: Antikalar)

        public int? ExternalId { get; set; } // CSV'den gelen KategoriDeğeri (Örn: 162925)

        public int? ParentCategoryId { get; set; } // Varsa üst kategorinin Id'si, yoksa null
    }
}
