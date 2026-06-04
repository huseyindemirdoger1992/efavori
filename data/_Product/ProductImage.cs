using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// İlişkisel ürün/varyant görsel tablosu (GalleryJson'ın ölçeklenebilir alternatifi).
    /// İsteğe bağlı olarak mevcut Media tablosuna MediaId ile bağlanır ya da doğrudan ImageUrl tutar.
    /// VariantId = null -> ürün geneli görsel; VariantId dolu -> o varyanta özel görsel.
    /// </summary>
    public class ProductImage
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; } // Görseli ekleyen kullanıcının/satıcının ID bilgisi
        public Guid StoreId { get; set; } // Ürünün bağlı olduğu mağazanın ID bilgisi
        public Guid ProductId { get; set; } // Görselin ait olduğu ana ürünün ID'si (her zaman dolu)
        public Guid? VariantId { get; set; } // Görselin ait olduğu varyantın ID'si (ürün geneli görselde null)
        public Guid? MediaId { get; set; } // Mevcut Media tablosundaki kaydın ID'si (opsiyonel)

        public string? ImageUrl { get; set; } // Doğrudan görsel URL'si (Media kullanılmıyorsa)
        public string? AltText { get; set; } // SEO/erişilebilirlik alternatif metni
        public bool IsCover { get; set; } = false; // Kapak (ana) görsel mi
        public int DisplayOrder { get; set; } = 0; // Görsel sıralaması

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
