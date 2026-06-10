using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Varyant görsel grupları.
    /// Görseller varyant kombinasyonuna DEĞİL, görsel-varyant özelliğinin DEĞERİNE bağlanır:
    ///   Örn: Renk görsel varyantı ise → Kırmızı için 1 grup, Bordo için 1 grup, Siyah için 1 grup.
    ///   Kırmızı-S ve Kırmızı-M aynı (Kırmızı) görsel grubunu paylaşır.
    /// Görseller ItemGallery üzerinden bağlanır:
    ///   ItemGallery.ItemType = "VariantGallery", ItemGallery.ItemId = ProductImageVariantGroups.Id
    /// Bir gruba birden fazla görsel eklenebilir (ItemGallery zaten çoklu kayıt destekler).
    /// </summary>
    public class ProductImageVariantGroups
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Ürün (Products.Id)
        public Guid AttributeId { get; set; } // Görsel varyant özelliği (ProductAttributes.Id — Örn: Renk)
        public Guid AttributeValueId { get; set; } // Görselin bağlandığı değer (ProductAttributeValues.Id — Örn: Kırmızı)

        public Guid? CoverMediaId { get; set; } // Bu değerin kapak görseli (Media.Id) — listelemede gösterilir

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    }
}
