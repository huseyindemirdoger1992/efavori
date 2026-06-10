using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ürün SEO bilgileri. Products tablosundan ayrı tutulur — listeleme sorgularını
    /// şişirmez ve ileride dil bazlı SEO satırlarına genişletilebilir.
    /// </summary>
    public class ProductSeo
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Ürün (Products.Id)

        public string? Slug { get; set; } // SEO dostu URL (Örn: samsung-55-oled-tv) — benzersiz olmalı
        public string? SeoTitle { get; set; } // SEO başlık (meta title)
        public string? SeoDescription { get; set; } // SEO açıklama (meta description)
        public string? SeoKeywords { get; set; } // SEO anahtar kelimeler (virgülle ayrılmış)
        public string? CanonicalUrl { get; set; } // Canonical adres (kopya içerik önleme)
        public string? MetaJson { get; set; } // Ek meta veriler (JSON: OpenGraph, schema.org vb.)

        public string? LanguageCode { get; set; } = "tr"; // Dil kodu — ileride çoklu dil SEO satırları için

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public DateTime? UpdatedAt { get; set; } // Son güncelleme tarihi
    }
}
