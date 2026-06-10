using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Bir ürünün hangi özellikleri, hangi rollerde kullandığını tanımlar.
    /// VARYANT GÖRSEL YÖNETİMİNİN KALBİ BURASIDIR:
    ///   IsVariantAttribute      → bu özellik varyant kombinasyonu üretir (Örn: Renk, Beden)
    ///   IsImageVariantAttribute → bu özelliğin DEĞERLERİNE görsel grubu atanır (Örn: yalnızca Renk)
    /// Bu seçim ÜRÜN BAZINDA yapılır: bir üründe Renk görsel varyantı iken,
    /// başka bir üründe Beden görsel varyantı olabilir.
    /// </summary>
    public class ProductAttributeMappings
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } // Ürün (Products.Id)
        public Guid AttributeId { get; set; } // Özellik (ProductAttributes.Id)

        public bool IsVariantAttribute { get; set; } = false; // Varyant üreten özellik mi (Renk, Beden vb.)
        public bool IsImageVariantAttribute { get; set; } = false; // Görsel grubu bu özelliğin değerlerine mi bağlanacak
        public bool IsRequired { get; set; } = false; // Ürün kaydında zorunlu mu

        public Guid? SourceTemplateId { get; set; } // Özellik bir şablondan geldiyse şablon sürümü (AttributeTemplates.Id), satıcı manuel eklediyse null

        public int DisplayOrder { get; set; } = 0; // Ürün detayında gösterim sırası

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    }
}
