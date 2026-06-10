using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Marka tanımları. UserId/StoreId null ise sistem geneli (admin) markasıdır,
    /// dolu ise yalnızca ilgili satıcıya/mağazaya özeldir.
    /// </summary>
    public class Brands
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? UserId { get; set; } // Null = sistem markası, dolu = satıcıya özel marka
        public Guid? StoreId { get; set; } // Null = sistem markası, dolu = mağazaya özel marka

        public string? Name { get; set; } // Marka adı (Örn: Samsung)
        public string? Slug { get; set; } // SEO dostu marka kodu (Örn: samsung)
        public string? Description { get; set; } // Marka açıklaması
        public Guid? LogoMediaId { get; set; } // Marka logosu (Media.Id)
        public string? WebsiteUrl { get; set; } // Marka resmi web sitesi

        public bool IsActive { get; set; } = true; // Marka aktif mi
        public bool IsApprovedByAdmin { get; set; } = false; // Satıcı markası admin onayından geçti mi

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
