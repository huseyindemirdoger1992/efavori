using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// SERBEST spesifikasyon satırlarının dil bazlı karşılıkları.
    /// YALNIZCA ProductSpecifications.AttributeId == null olan (CustomName + CustomValue)
    /// satırlar için gereklidir — tanımlı özellik satırları çevirisini
    /// ProductAttributeTranslations + ProductAttributeValueTranslations'tan alır.
    ///
    /// Örn: "Kutu İçeriği" → "TV, Kumanda, Duvar Aparatı"
    ///      en: "Box Contents" → "TV, Remote, Wall Mount"
    ///
    /// NOT: Sayısal/dil bağımsız CustomValue'larda ("55", "220V") çeviri satırı açmaya gerek
    /// yoktur; fallback base satırdır.
    ///
    /// BENZERSİZLİK (Fluent API): (SpecificationId, LanguageCode)
    /// </summary>
    public class ProductSpecificationTranslations
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid SpecificationId { get; set; } // Spesifikasyon satırı (ProductSpecifications.Id)

        // "tr","en","az","de","es","fr","hi","pt","ru","zh"
        public string LanguageCode { get; set; } = "tr";

        public string? CustomName { get; set; } // Serbest özellik adının bu dildeki karşılığı
        public string? CustomValue { get; set; } // Serbest değerin bu dildeki karşılığı

        // === İçerik Kaynağı / AI Yönetimi ===
        public string? ContentSource { get; set; } = "Human"; // "Human" | "Import" | "AiGenerated" | "AiTranslated"
        public bool? AiStatus { get; set; } // null → işlenmedi | true → başarılı | false → hata
        public string? AiErrorMessage { get; set; }
        public int AiRetryCount { get; set; } = 0;
        public DateTime? AiProcessedAt { get; set; }
        public bool IsManuallyEdited { get; set; } = false; // true → AI üzerine yazmaz

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
