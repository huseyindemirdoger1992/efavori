using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Sistem özelliklerinin platform özelliklerine eşlenmesi
    /// (Amazon Attribute Mapping, eBay Item Specifics, Trendyol Attributes vb.).
    /// İçeri/dışarı aktarımda veri kaybını önler: "renk" ↔ Trendyol "color" (id: 47) gibi.
    /// Eşleme kategoriye özel olabilir (platformlar kategori bazlı özellik ister).
    /// </summary>
    public class MarketplaceAttributeMappings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid MarketplaceId { get; set; } // Platform (Marketplaces.Id)
        public Guid AttributeId { get; set; } // Sistem özelliği (ProductAttributes.Id)
        public int? CategoryId { get; set; } // Eşleme kategoriye özelse (CategoriesTr.Id), null = genel

        public string? ExternalAttributeCode { get; set; } // Platformdaki özellik kodu/ID'si
        public string? ExternalAttributeName { get; set; } // Platformdaki özellik adı
        public string? ValueMappingJson { get; set; } // Değer eşleme sözlüğü (JSON): {"kirmizi":"Red","bordo":"Maroon"}

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    }
}
