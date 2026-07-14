using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Özellik DEĞERLERİNİN dil bazlı karşılıkları.
    /// Örn: "Kırmızı" → en:"Red", de:"Rot", es:"Rojo".
    /// Varyant seçim kutuları, filtreler ve dışa aktarım feed'leri değer metnini BU tablodan okur.
    /// (ColorHex gibi dil bağımsız alanlar base tabloda kalır.)
    ///
    /// FALLBACK KURALI: İlgili dilde satır yoksa ProductAttributeValues.Value gösterilir.
    ///
    /// BENZERSİZLİK (Fluent API): (AttributeValueId, LanguageCode)
    /// SAHİPLİK: ProductAttributeValues ile aynıdır.
    /// </summary>
    public class ProductAttributeValueTranslations
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid AttributeValueId { get; set; } // Özellik değeri (ProductAttributeValues.Id)

        // "tr","en","az","de","es","fr","hi","pt","ru","zh"
        public string LanguageCode { get; set; } = "tr";

        public string? Value { get; set; } // Değerin bu dildeki metni (Örn: Red)

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
