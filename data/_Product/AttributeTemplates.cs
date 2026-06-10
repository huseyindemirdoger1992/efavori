using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// SİSTEM ÖZELLİK ŞABLONLARI (yalnızca admin yönetimli).
    /// VERSİYONLAMA MODELİ:
    ///   - Her sürüm ayrı bir satırdır (immutable). Yayınlanan sürüm asla değiştirilmez.
    ///   - Aynı şablonun tüm sürümleri ortak TemplateGroupId değerini paylaşır.
    ///   - Yeni sürüm = aynı TemplateGroupId, Version + 1, yeni AttributeTemplateItems seti.
    ///   - Ürünler Products.AttributeTemplateId ile SÜRÜME sabitlenir; böylece şablon
    ///     güncellendiğinde mevcut ürünlerin veri bütünlüğü bozulmaz.
    ///   - Yeni ürünler her zaman grubun IsPublished = true olan en yüksek sürümünü kullanır.
    /// </summary>
    public class AttributeTemplates
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid TemplateGroupId { get; set; } = Guid.NewGuid(); // Sürüm grubu kimliği (tüm sürümlerde aynı)
        public int Version { get; set; } = 1; // Sürüm numarası (1, 2, 3...)

        public string? Name { get; set; } // Şablon adı (Örn: Televizyon)
        public string? Description { get; set; } // Şablon açıklaması
        public string? VersionNotes { get; set; } // Bu sürümde yapılan değişikliklerin notu

        public bool IsPublished { get; set; } = false; // Yayında mı (yeni ürünlerde kullanılabilir mi)
        public bool IsActive { get; set; } = true; // Grup genelinde aktiflik (false → yeni ürünlere kapalı)

        public Guid? CreatedByUserId { get; set; } // Şablonu oluşturan admin (Users.Id)
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public DateTime? PublishedAt { get; set; } // Yayınlanma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
