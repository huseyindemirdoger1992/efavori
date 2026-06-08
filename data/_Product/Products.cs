using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Ürünün ana kaydını ve temel bilgilerini tutar.
    /// Hem standart (tek SKU) hem de varyantlı ürünler bu tabloda başlar.
    /// </summary>
    public class Products
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }   // Ürünü ekleyen satıcının ID bilgisi
        public Guid StoreId { get; set; }   // Ürünün bağlı olduğu mağazanın ID bilgisi

        // === Temel Bilgiler ===
        public string? Name { get; set; }            // Ürün adı
        public string? Description { get; set; }     // Ürün açıklaması (HTML destekli)
        public string? ShortDescription { get; set; } // Kısa açıklama / özet

        // === Ürün Tipi ===
        // "Standard" = tek SKU, varyant yok
        // "Variable" = varyantlı ürün (Renk+Beden gibi)
        public string? ProductType { get; set; } = "Standard";

        // === SKU & Barkod (Standart ürünler için doğrudan burada tutulur) ===
        public string? SKU { get; set; }              // Stok Kodu (standart ürünlerde burada, varyantlılarda ProductVariants'ta)
        public string? Barcode { get; set; }          // Barkod / GTIN / EAN / UPC
        public string? Brand { get; set; }            // Marka adı
        public string? Manufacturer { get; set; }     // Üretici firma

        // === Fiziksel Özellikler (Standart ürünler için) ===
        public decimal? Weight { get; set; }          // Ağırlık (gram)
        public decimal? Width { get; set; }           // Genişlik (cm)
        public decimal? Height { get; set; }          // Yükseklik (cm)
        public decimal? Length { get; set; }          // Uzunluk / Derinlik (cm)

        // === Durum ===
        // "Draft" = Taslak, "Active" = Yayında, "Passive" = Pasif, "PendingApproval" = Onay Bekliyor
        public string? Status { get; set; } = "Draft";

        // === Vergi ===
        public decimal? TaxRate { get; set; }         // KDV oranı (%)

        // === Dış Platform Referansları (CSV import sonrası eşleşme için) ===
        public string? ExternalId { get; set; }       // Dış platformdaki ürün ID'si (Trendyol, Amazon vb.)
        public string? ExternalPlatform { get; set; } // Kaynak platform adı (Trendyol, Amazon, WooCommerce, N11)

        // === İçe Aktarma ===
        public Guid? ImportProfileId { get; set; }    // Hangi import profili ile yüklendi (null ise manuel)
        public Guid? ImportBatchId { get; set; }      // Toplu yükleme işlem grubu ID'si

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
