using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Özellik (Attribute) adlarının dil bazlı karşılıkları.
    /// Örn: "Renk" → en:"Color", de:"Farbe", ru:"Цвет".
    /// Ürün detayı, filtre paneli ve dışa aktarım feed'leri özellik adını BU tablodan okur.
    ///
    /// FALLBACK KURALI: İlgili dilde satır yoksa ProductAttributes.Name gösterilir
    /// (base tablodaki ad, kaynak dildeki teknik referanstır — genelde Türkçe,
    /// içe aktarılan satıcı özelliklerinde kaynak dil olabilir).
    ///
    /// BENZERSİZLİK (Fluent API): (AttributeId, LanguageCode)
    /// SAHİPLİK: ProductAttributes ile aynıdır — sistem özelliğinin çevirisini admin,
    /// satıcı özel özelliğin çevirisini AI/satıcı yönetir.
    /// </summary>
    public class ProductAttributeTranslations
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid AttributeId { get; set; } // Özellik (ProductAttributes.Id)

        // "tr","en","az","de","es","fr","hi","pt","ru","zh"
        public string LanguageCode { get; set; } = "tr";

        public string? Name { get; set; } // Özelliğin bu dildeki adı (Örn: Color)
        public string? Description { get; set; } // Özelliğin bu dildeki açıklaması
        public string? Unit { get; set; } // Birimin bu dildeki karşılığı (Örn: "inç" → "inch") — null = ProductAttributes.Unit kullanılır

        // === İçerik Kaynağı / AI Yönetimi ===
        public string? ContentSource { get; set; } = "Human"; // "Human" | "Import" | "AiGenerated" | "AiTranslated"
        public bool? AiStatus { get; set; } // null → işlenmedi | true → başarılı | false → hata
        public string? AiErrorMessage { get; set; }
        public int AiRetryCount { get; set; } = 0;
        public DateTime? AiProcessedAt { get; set; }
        public bool IsManuallyEdited { get; set; } = false; // true → AI üzerine yazmaz

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
