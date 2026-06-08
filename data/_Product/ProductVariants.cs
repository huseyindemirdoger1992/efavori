using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Varyantlı ürünlerde her kombinasyonu ayrı SKU olarak saklar.
    /// Örn: "Kırmızı + XL" = 1 satır, "Mavi + M" = 1 satır.
    /// Standart ürünlerde bu tabloda kayıt olmaz.
    /// Varyant görselleri: ItemGallery tablosunda ItemType="VariantGallery", ItemId=VariantId şeklinde tutulur.
    /// </summary>
    public class ProductVariants
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; }  // Bağlı olduğu ana ürün
        public Guid UserId { get; set; }     // Satıcı ID bilgisi

        // === SKU & Barkod ===
        public string? SKU { get; set; }     // Varyanta özgü stok kodu
        public string? Barcode { get; set; } // Varyanta özgü barkod / GTIN

        // === Fiziksel Özellikler (Varyanta özel override) ===
        public decimal? Weight { get; set; }  // Ağırlık (gram) — null ise üst üründen alınır
        public decimal? Width { get; set; }   // Genişlik (cm)
        public decimal? Height { get; set; }  // Yükseklik (cm)
        public decimal? Length { get; set; }  // Uzunluk / Derinlik (cm)

        // === Durum ===
        public bool IsActive { get; set; } = true; // Varyant aktif mi

        // === Sıralama ===
        public int SortOrder { get; set; } = 0; // Görüntülenme sırası

        // === Dış Platform Referansı ===
        public string? ExternalVariantId { get; set; } // Dış platformdaki varyant ID'si

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
