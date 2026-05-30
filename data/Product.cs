using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data
{
    public class Product
    {
        [Key]
        // Ürünün benzersiz kimlik numarası (Primary Key)
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        // Ürünü sisteme ekleyen personelin veya satıcının ID'si
        public Guid UserId { get; set; }

        [Required]
        // Ürünün fiziksel veya sanal olarak bağlı olduğu mağazanın ID'si
        public Guid StoreId { get; set; }


        // --- SEO Özellikleri ---
        // AI ile oluşturulan SEO uyumlu başlık, kısa açıklama ve uzun açıklamanın durumları

        [Required]
        // Ürünün müşteriye görünen tam adı (Örn: Apple iPhone 18 Pro Max 256 GB Titanium)
        public string Name { get; set; } = string.Empty; // Title uzunluğu → 50-60 karakter
        public string? Description { get; set; } // Description → 140-160 karakter
        public string? Content { get; set; } // Ürün sayfasında detaylı açıklama olarak kullanılabilir (HTML destekli)

        [Required]
        // <link rel = "canonical" href="https://efavori.com/iphone-18-pro-max-256gb-titanium">
        public string UniqueNameSlug { get; set; } = string.Empty;

        public string? AiSeoKeywords { get; set; }
        public bool? AiSeoKeywordsIsOk { get; set; }


        // Stok takibi için kullanılan özel ürün barkodu veya kodu (Stock Keeping Unit)
        public string? SKU { get; set; }

        // Mevcut depodaki güncel stok adedi
        public int StockQuantity { get; set; } = 0;

        // Ürünün satışta olup olmadığını belirleyen aktiflik durumu
        public bool IsActive { get; set; } = true;

        // Ürünün ana sayfada veya vitrinde öne çıkarılma durumu
        public bool IsFeatured { get; set; } = false;

        // Ürünün ana görselinin dosya yolu veya URL adresi
        public string? ImagePath { get; set; }


        // Ürünün sisteme ilk kaydedilme tarihi (UTC formatında)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Ürün bilgilerinin son güncellenme tarihi
        public DateTime? UpdatedAt { get; set; }

        public InteractionCounts? Interaction { get; set; } = new();
        public IsDeleted? IsDeleted { get; set; } = new();
    }
}