using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ana ürün tablosu. Tüm ürün tipleri (basit, varyantlı, dijital, hizmet, paket, harici)
    /// bu tablo üzerinden yönetilir. Basit ürünler dahi en az bir ProductVariants kaydına sahiptir
    /// (birleşik tek-varyant modeli) — böylece fiyat/stok her zaman varyant üzerinden okunur.
    /// </summary>
    public class Products
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // === İlişkiler (ID bazlı, navigation property kullanılmaz) ===
        public Guid UserId { get; set; } // Ürün sahibi kullanıcı (Users.Id)
        public Guid StoreId { get; set; } // Ürün sahibi mağaza (Store.Id)
        public Guid? BrandId { get; set; } // Marka (Brands.Id)
        public Guid? AttributeTemplateId { get; set; } // Ürünün oluşturulduğu özellik şablonu SÜRÜMÜ (AttributeTemplates.Id) — versiyon sabitlenir, şablon güncellense bile ürün verisi bozulmaz
        public Guid? ShippingProfileId { get; set; } // Kargo/desi profili (ShippingProfile.Id)

        // === Ürün Tipi ===
        // "Simple", "Variant", "Digital", "Service", "Bundle", "External"
        // Yeni tipler için enum yerine string tercih edildi (genişletilebilirlik)
        public string? ProductType { get; set; } = "Simple";

        // === İçerik Alanları ===
        public string? Name { get; set; } // Ürün adı
        public string? ShortDescription { get; set; } // Kısa açıklama
        public string? FullDescription { get; set; } // Detaylı açıklama (HTML destekli)
        public string? Tags { get; set; } // Etiketler (virgülle ayrılmış)

        // === Medya ===
        public Guid? CoverMediaId { get; set; } // Kapak görseli (Media.Id)
        // Galeri görselleri: ItemGallery (ItemType = "ProductGallery", ItemId = Products.Id)
        // Varyant görselleri: ItemGallery (ItemType = "VariantGallery", ItemId = ProductImageVariantGroups.Id)

        // === Harici Ürün (External) Alanları ===
        public string? ExternalUrl { get; set; } // Harici ürün bağlantısı
        public string? ExternalButtonText { get; set; } // Harici ürün buton metni (Örn: "Satıcı sitesinde gör")

        // === Yayın Durumu ===
        // "Draft", "PendingApproval", "Published", "Rejected", "Archived"
        public string? PublishStatus { get; set; } = "Draft";
        public bool? IsApprovedByAdmin { get; set; } // Admin onay durumu
        public DateTime? ApprovedDate { get; set; } // Admin onay tarihi
        public Guid? ApprovedByUserId { get; set; } // Onaylayan admin (Users.Id)
        public bool IsActive { get; set; } = true; // Satıcı tarafından aktif/pasif

        // === Tarih Bilgileri ===
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public DateTime? UpdatedAt { get; set; } // Son güncelleme tarihi

        // === Owned Type'lar ===
        public Meta? Meta { get; set; } = new();
        public InteractionCounts? Interaction { get; set; } = new();
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
