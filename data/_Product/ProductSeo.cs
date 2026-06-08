using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ürüne ait SEO alanlarını saklar.
    /// Her ürün için tek bir SEO kaydı bulunur.
    /// Slug (URL-friendly isim) Türkçe karakter dönüşümlü oluşturulur.
    /// </summary>
    public class ProductSeo
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; }  // Ürün ID bilgisi
        public Guid UserId { get; set; }     // Satıcı ID bilgisi

        // === SEO Alanları ===
        public string? Slug { get; set; }            // URL-dostu ürün adı (ornek-urun-adi)
        public string? MetaTitle { get; set; }       // Sayfa başlığı (max 60 karakter önerilir)
        public string? MetaDescription { get; set; } // Meta açıklama (max 160 karakter önerilir)
        public string? MetaKeywords { get; set; }    // Meta anahtar kelimeler (virgülle ayrılmış)
        public string? CanonicalUrl { get; set; }    // Canonical URL (opsiyonel)

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
