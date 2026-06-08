using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Attribute tanımlarını saklar.
    /// Örn: "Renk", "Beden", "RAM", "Depolama", "İşlemci" gibi özellik başlıkları.
    /// Sistem genelinde (UserId=null → global) veya satıcıya özel olabilir.
    /// </summary>
    public class ProductAttributes
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? UserId { get; set; }    // null ise sistem geneli (global), dolu ise satıcıya özel tanım

        public string? Name { get; set; }    // Attribute adı (Renk, Beden, RAM vb.)

        // === Attribute Tipi ===
        // "Color"    = Renk (renk kodu ile birlikte gösterilebilir)
        // "Size"     = Beden
        // "Custom"   = Özel tanımlı (satıcı tarafından oluşturulan)
        // "Select"   = Açılır liste seçimi
        // "Text"     = Serbest metin
        public string? AttributeType { get; set; } = "Custom";

        // === CSV Import Eşleşme ===
        public string? ExternalName { get; set; } // Dış platformdaki karşılığı (Trendyol: "Renk", Amazon: "Color" vb.)

        public int SortOrder { get; set; } = 0;   // Sıralama

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
