using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ürün SEO bilgileri — DİL BAŞINA BİR SATIR.
    /// BENZERSİZLİK (Fluent API):
    ///   (ProductId, LanguageCode) → ürün başına dil başına tek SEO satırı
    ///   (LanguageCode, Slug)      → slug her dil içinde benzersiz (aynı slug farklı dillerde kullanılabilir)
    /// Products tablosundan ayrı tutulur — listeleme sorgularını şişirmez.
    /// AI üretimi desteklenir; IsManuallyEdited = true satırlara AI dokunmaz.
    /// </summary>
    public class ProductSeo
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Ürün (Products.Id)

        // "tr","en","az","de","es","fr","hi","pt","ru","zh"
        public string LanguageCode { get; set; } = "tr";

        public string? Slug { get; set; } // SEO dostu URL (Örn: samsung-55-oled-tv) — dil içinde benzersiz
        public string? SeoTitle { get; set; } // SEO başlık (meta title)
        public string? SeoDescription { get; set; } // SEO açıklama (meta description)
        public string? SeoKeywords { get; set; } // SEO anahtar kelimeler (virgülle ayrılmış)
        public string? CanonicalUrl { get; set; } // Canonical adres (kopya içerik önleme)
        public string? MetaJson { get; set; } // Ek meta veriler (JSON: OpenGraph, schema.org vb.)

        // === İçerik Kaynağı / AI Yönetimi (ProductTranslations ile aynı desen) ===
        public string? ContentSource { get; set; } = "Human"; // "Human" | "Import" | "AiGenerated" | "AiTranslated"
        public bool? AiStatus { get; set; } // null → işlenmedi | true → başarılı | false → hata
        public string? AiErrorMessage { get; set; }
        public int AiRetryCount { get; set; } = 0;
        public DateTime? AiProcessedAt { get; set; }
        public bool IsManuallyEdited { get; set; } = false; // true → AI üzerine yazmaz

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public DateTime? UpdatedAt { get; set; } // Son güncelleme tarihi
    }
}
