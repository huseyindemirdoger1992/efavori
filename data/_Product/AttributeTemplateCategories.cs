using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Şablon - Kategori atama tablosu.
    /// TemplateGroupId üzerinden bağlanır (sürüm değil) — böylece yeni sürüm yayınlandığında
    /// kategori atamalarını tekrar yapmaya gerek kalmaz; kategori her zaman grubun
    /// yayındaki en güncel sürümünü sunar. Bir şablon birden fazla kategoriye atanabilir.
    /// </summary>
    public class AttributeTemplateCategories
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid TemplateGroupId { get; set; } // Şablon grubu (AttributeTemplates.TemplateGroupId)
        public int CategoryId { get; set; } // Kategori (CategoriesTr.Id — int tip)

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    }
}
