using data.Owned;
using System;

namespace data._Shares
{
    public class Posts
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gönderi kullanıcıya mı yoksa sisteme mi ait?
        /// </summary>
        public bool IsUser { get; set; }

        /// <summary>
        /// Gönderinin ait olduğu mağaza veya kullanıcı kimliği.
        /// </summary>
        public Guid UserStoreId { get; set; }

        /// <summary>
        /// Gönderi başlığı.
        /// (İsteğe bağlı, uzun paylaşımlar için kullanılabilir.)
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gönderi metni.
        /// HTML, Markdown veya düz metin olarak saklanabilir.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gönderinin kapak veya önizleme görseli.
        /// </summary>
        public string? CoverImage { get; set; }

        /// <summary>
        /// SEO uyumlu URL.
        /// Sadece herkese açık gönderiler için kullanılabilir.
        /// </summary>
        public string? Slug { get; set; }

        /// <summary>
        /// Oluşturulma tarihi.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Son güncellenme tarihi.
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Yayın tarihi.
        /// Zamanlanmış paylaşımlar için kullanılabilir.
        /// </summary>
        public DateTime PublishDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gönderi yayında mı?
        /// </summary>
        public bool IsPublished { get; set; } = true;

        /// <summary>
        /// Gönderi öne çıkarılmış mı?
        /// </summary>
        public bool IsFeatured { get; set; }

        /// <summary>
        /// Gönderiye yorum yapılabilir mi?
        /// </summary>
        public bool AllowComments { get; set; } = true;

        /// <summary>
        /// Gönderinin SEO bilgileri.
        /// </summary>
        public Meta? Meta { get; set; } = new();

        /// <summary>
        /// Beğeni, yorum, paylaşım ve görüntülenme sayaçları.
        /// </summary>
        public InteractionCounts? Interaction { get; set; } = new();

        /// <summary>
        /// Soft Delete bilgisi.
        /// </summary>
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}