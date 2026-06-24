using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace data.Articles
{
    public class ArticlesCategoriesDe
    {
        [Key]
        public int Id { get; set; } // Veritabanının oluşturacağı otomatik artan ID
        public string? Name { get; set; } // Kategori adı (Örn: Korkuluklar)
        public int? ParentCategoryId { get; set; } // Varsa üst kategorinin Id'si, yoksa null
        public bool? IsDelete { get; set; } // Soft Delete için kullanılacak alan, true ise silinmiş, false ise aktif
    }
}
