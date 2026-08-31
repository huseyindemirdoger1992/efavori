using System;
using data._Attribute;
using data._Products;
using data.Owned;

namespace data._Shares
{
    /// <summary>
    /// MAKALE / BLOG İÇERİĞİ — editoryal uzun içerik.
    ///
    /// <see cref="Posts"/> ile FARKI: Posts sosyal akışın kısa, etkileşim odaklı
    /// içeriğidir; Articles ise SEO odaklı, kategorili, uzun soluklu editoryal
    /// içeriktir (rehber, blog yazısı, kurumsal duyuru). İkisi ayrı tablolarda
    /// kalır çünkü sorgu desenleri, indeksleri ve yaşam döngüleri tamamen farklıdır.
    ///
    /// BU SÜRÜMDE DÜZELTİLENLER:
    ///  • <c>bool IsUser + Guid UserStoreId</c> sahiplik modeli, Posts ile aynı
    ///    tipli-FK modeline geçirildi (AuthorType + AuthorUserId + AuthorStoreId).
    ///  • <c>string CoverImage</c> fiziksel medya alanı KALDIRILDI; kapak artık
    ///    merkezî medyaya FK'dir (§72).
    ///  • <c>DateTime.Now</c> yerel saat varsayılanları UTC'ye çevrildi.
    ///  • Yayın durumu boolean yerine <see cref="PostStatus"/> enum'ıyla modellendi.
    /// </summary>
    public class Articles : SocialEntityBase
    {
        // ── Sahiplik ──────────────────────────────────────────────────────────
        /// <summary>Makale sahibinin türü (kullanıcı veya mağaza).</summary>
        public PostAuthorType AuthorType { get; set; } = PostAuthorType.User;

        /// <summary>Makaleyi yazan kullanıcı (Users.Id). AuthorType = User iken dolu.</summary>
        public Guid? AuthorUserId { get; set; }

        /// <summary>Makaleyi yayımlayan mağaza (Store.Id). AuthorType = Store iken dolu.</summary>
        public Guid? AuthorStoreId { get; set; }

        // ── Sınıflandırma ─────────────────────────────────────────────────────
        /// <summary>Makale kategorisi (CategoriesArticle.Id — int PK).</summary>
        public int? CategoriesArticleId { get; set; }

        /// <summary>İçeriğin dili (mevcut data._Attribute.Language enum'ı).</summary>
        public Language Language { get; set; } = Language.Tr;

        // ── İçerik ────────────────────────────────────────────────────────────
        /// <summary>Makale başlığı.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>SEO uyumlu URL parçası. (Slug, Language) çifti tekildir.</summary>
        public string Slug { get; set; } = string.Empty;

        /// <summary>Kısa özet — liste ve önizlemelerde gösterilir.</summary>
        public string? Summary { get; set; }

        /// <summary>Makale gövdesi (HTML veya Markdown).</summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>Kapak görseli (data._Galleries.Media.Id). Fiziksel yol/URL TUTULMAZ.</summary>
        public Guid? CoverMediaId { get; set; }

        /// <summary>Tahmini okuma süresi (dakika) — gövdeden hesaplanır.</summary>
        public int? ReadingTimeMinutes { get; set; }

        // ── Yayın ─────────────────────────────────────────────────────────────
        /// <summary>Yayın yaşam döngüsü durumu.</summary>
        public PostStatus Status { get; set; } = PostStatus.Draft;

        /// <summary>Yayın anı (UTC). Gelecek tarih + Status = Scheduled ile zamanlanır.</summary>
        public DateTime PublishAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>Öne çıkarılmış makale mi?</summary>
        public bool IsFeatured { get; set; }

        /// <summary>Yorum yapılabilir mi?</summary>
        public bool AllowComments { get; set; } = true;

        /// <summary>Moderasyon durumu (platform genelinde ortak enum).</summary>
        public ModerationStatus ModerationStatus { get; set; } = ModerationStatus.Approved;

        // ── SEO ve sayaçlar ───────────────────────────────────────────────────
        /// <summary>SEO meta verileri (mevcut ortak owned tip).</summary>
        public Meta? Meta { get; set; }

        /// <summary>Etkileşim sayaçları (ÖNBELLEK — gerçeğin kaynağı etkileşim tablolarıdır).</summary>
        public InteractionCounts Interaction { get; set; } = new();
    }
}
