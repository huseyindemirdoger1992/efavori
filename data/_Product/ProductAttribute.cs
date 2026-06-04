using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Dinamik özellik tanımı (Örn: "Renk", "Beden", "Hafıza"). Mağaza bazlıdır.
    /// Hem varyant üretiminde hem de filtrelemede kullanılabilir.
    /// </summary>
    public class ProductAttribute
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; } // Özelliği ekleyen kullanıcının/satıcının ID bilgisi
        public Guid StoreId { get; set; } // Özelliğin bağlı olduğu mağazanın ID bilgisi

        public string Name { get; set; } = string.Empty; // Özellik adı (Örn: "Renk")
        public string? Code { get; set; } // Özellik teknik kodu (Örn: "color")
        public string DisplayType { get; set; } = "text"; // Gösterim tipi: "color" | "text" | "image"
        public bool IsVariantDefining { get; set; } = true; // Varyant üretiminde kullanılıyor mu
        public bool IsFilterable { get; set; } = true; // Filtre panelinde kullanılsın mı
        public int DisplayOrder { get; set; } = 0; // Listelenme sırası

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
