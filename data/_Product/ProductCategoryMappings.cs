using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ürünün birden fazla kategoriye bağlanmasını sağlar.
    /// CategoriesTr tablosu ile ilişkilendirilir (int CategoryId).
    /// Bir ürün birden fazla kategoride listelenebilir.
    /// </summary>
    public class ProductCategoryMappings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; }   // Ürün ID bilgisi
        public int CategoryId { get; set; }   // Kategori ID bilgisi (CategoriesTr.Id — int)

        public bool IsPrimary { get; set; } = false; // Ana kategori mi (bir ürünün sadece 1 ana kategorisi olur)

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
