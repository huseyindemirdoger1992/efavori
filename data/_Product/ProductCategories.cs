using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ürün - Kategori çoklu ilişki tablosu.
    /// Bir ürün birden fazla kategoriye bağlanabilir; IsPrimary = true olan kayıt ana kategoridir.
    /// </summary>
    public class ProductCategories
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Ürün (Products.Id)
        public int CategoryId { get; set; } // Kategori (CategoriesTr.Id — int tip)

        public bool IsPrimary { get; set; } = false; // Ana kategori mi (ürün başına yalnızca 1 adet true olmalı)

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    }
}
