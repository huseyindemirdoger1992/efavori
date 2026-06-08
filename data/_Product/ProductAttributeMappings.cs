using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Bir ürünün hangi attribute'ları kullanacağını belirler.
    /// Örn: Tişört → Renk + Beden
    ///      Laptop → RAM + İşlemci + SSD
    /// Bu tablo olmadan "bu ürün hangi özellikleri kullanıyor?" sorusu cevapsız kalır.
    /// </summary>
    public class ProductAttributeMappings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; }    // Ürün ID bilgisi
        public Guid AttributeId { get; set; }  // Attribute tanımı ID bilgisi (ProductAttributes.Id)

        // === Ürün bazlı attribute davranışı ===
        // true ise bu attribute varyant oluşturur (Renk+Beden gibi)
        // false ise sadece bilgi amaçlı kullanılır (ek özellik)
        public bool IsVariantAttribute { get; set; } = true;

        public int SortOrder { get; set; } = 0; // Ürün sayfasında gösterim sırası

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
