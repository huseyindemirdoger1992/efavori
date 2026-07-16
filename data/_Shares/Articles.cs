using data.Owned;
using System;
using System.Collections.Generic;

namespace data._Shares
{
    public class Articles
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Makalenin ait olduğu kategori bilgisi. (Kategori tablosu ile ilişkilendirilecek)
        /// </summary>
        public int CategoriesArticleId { get; set; }

        /// <summary>
        /// Makale kullanıcıya mı yoksa sisteme mi ait?
        /// </summary>
        public bool IsUser { get; set; }

        /// <summary>
        /// Makalenin ait olduğu mağaza veya kullanıcı.
        /// </summary>
        public Guid UserStoreId { get; set; }

        /// <summary>
        /// Makale başlığı
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// SEO dostu URL
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Kısa açıklama
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Makale içeriği (HTML / Markdown)
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Kapak görseli
        /// </summary>
        public string CoverImage { get; set; }

        /// <summary>
        /// Yayın tarihi
        /// </summary>
        public DateTime PublishDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Oluşturulma tarihi
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Son güncellenme tarihi
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Yayında mı?
        /// </summary>
        public bool IsPublished { get; set; } = true;

        /// <summary>
        /// Öne çıkarılmış makale
        /// </summary>
        public bool IsFeatured { get; set; }

        /// <summary>
        /// Yorum yapılabilir mi?
        /// </summary>
        public bool AllowComments { get; set; } = true;

        public Meta? Meta { get; set; } = new();

        public InteractionCounts? Interaction { get; set; } = new();

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}