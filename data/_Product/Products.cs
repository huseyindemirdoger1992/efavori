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
        public Guid UserId { get; set; }
        public Guid StoreId { get; set; }
        public Guid? BrandId { get; set; }
        public Guid? AttributeTemplateId { get; set; }
        public Guid? ShippingProfileId { get; set; }

        // === Ürün Tipi ===
        public string? ProductType { get; set; } = "Simple";

        // === İçerik Alanları ===
        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? FullDescription { get; set; }
        public string? Tags { get; set; }

        // === Medya ===
        public Guid? CoverMediaId { get; set; }

        // === Harici Ürün (External) Alanları ===
        public string? ExternalUrl { get; set; }
        public string? ExternalButtonText { get; set; }

        // === Yayın Durumu ===
        public string? PublishStatus { get; set; } = "Draft";
        public bool? IsApprovedByAdmin { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public Guid? ApprovedByUserId { get; set; }
        public bool IsActive { get; set; } = true;

        // === AI İçerik Yönetimi ===
        // Satıcı ürün eklerken "AI Yönetimine İzin Ver" toggle'ını açarsa true olur.
        public bool? IsAiManaged { get; set; } = false; // Satıcı AI yönetimine izin veriyor mu

        // null → AI yönetimi yapılmadı (IsAiManaged=false) | true → AI tarafından başarıyla oluşturuldu | false → AI işlemi başarısız oldu
        public bool? AiContentStatus { get; set; } // AI işlem durumu

        // Satıcının orijinal girişi — AI kapatılırsa geri yüklenir
        public string? AiOriginalName { get; set; }
        public string? AiOriginalShortDescription { get; set; }
        public string? AiOriginalFullDescription { get; set; }
        public string? AiOriginalTags { get; set; }

        public string? AiErrorMessage { get; set; }          // Son hata mesajı (Failed durumunda)
        public int AiRetryCount { get; set; } = 0;           // Tekrar deneme sayısı
        public DateTime? AiProcessedAt { get; set; }         // AI'ın işlemi tamamladığı an

        // === Tarih Bilgileri ===
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // === Owned Type'lar ===
        public Meta? Meta { get; set; } = new();
        public InteractionCounts? Interaction { get; set; } = new();
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}