using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Attribute seçeneklerini saklar.
    /// Örn: Renk → "Kırmızı", "Mavi", "Siyah"
    ///      Beden → "S", "M", "L", "XL"
    ///      RAM → "8GB", "16GB", "32GB"
    /// </summary>
    public class ProductAttributeValues
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid AttributeId { get; set; }  // Bağlı olduğu attribute tanımı (ProductAttributes.Id)
        public Guid? UserId { get; set; }       // null ise global değer, dolu ise satıcıya özel

        public string? Value { get; set; }      // Değer metni (Kırmızı, XL, 16GB vb.)
        public string? ColorCode { get; set; }  // Renk tipi attribute'lar için HEX kodu (#FF0000 vb.)
        public string? ImageUrl { get; set; }   // Değere ait ikon/görsel URL'i (opsiyonel)

        // === CSV Import Eşleşme ===
        public string? ExternalValue { get; set; } // Dış platformdaki karşılığı

        public int SortOrder { get; set; } = 0;    // Sıralama

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
