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

        [Required]
        // Ürünün müşteriye görünen tam adı (Örn: Apple iPhone 15 Pro)
        public string Name { get; set; } = string.Empty;

        [Required]
        // URL yapısı için kullanılan temizlenmiş isim (Örn: apple-iphone-15-pro)
        public string Slug { get; set; } = string.Empty;

        // Ürün listeleme sayfalarında görünecek olan kısa tanıtım metni
        public string? Description { get; set; }

        // Ürün sayfasındaki detaylı teknik özellikler ve uzun metin (HTML destekli)
        public string? Content { get; set; }

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

        // --- SEO Özellikleri ---
        // Arama motorları için sayfa başlığı (Title Tag)
        public string? MetaTitle { get; set; }
        // Arama motorları için sayfa özeti (Meta Description)
        public string? MetaDescription { get; set; }

        // --- Metadata ---
        // Ürünün sisteme ilk kaydedilme tarihi (UTC formatında)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Ürün bilgilerinin son güncellenme tarihi
        public DateTime? UpdatedAt { get; set; }
    }
}