using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Şablon sürümü ile sistem özellikleri arasındaki bağlantı.
    /// Yalnızca SİSTEM özellikleri (ProductAttributes.UserId == null) eklenebilir —
    /// satıcı özel özellikleri şablonlara ASLA dahil edilmez (uygulama katmanında doğrulanır).
    /// </summary>
    public class AttributeTemplateItems
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid TemplateId { get; set; } // Şablon SÜRÜMÜ (AttributeTemplates.Id)
        public Guid AttributeId { get; set; } // Sistem özelliği (ProductAttributes.Id)

        public bool IsRequired { get; set; } = false; // Bu şablonda zorunlu mu
        public bool IsVariantSuggested { get; set; } = false; // Varyant özelliği olarak önerilsin mi (Örn: Renk)

        public int DisplayOrder { get; set; } = 0; // Şablondaki gösterim sırası
    }
}
