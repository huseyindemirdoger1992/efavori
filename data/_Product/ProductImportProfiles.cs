using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Trendyol / Amazon / WooCommerce / N11 gibi platformlardan
    /// CSV ile toplu ürün aktarımı için kolon eşleşme profillerini tutar.
    /// Her satıcı, her platform için kendi eşleşme profilini tanımlayabilir.
    /// </summary>
    public class ProductImportProfiles
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }     // Profili oluşturan satıcı
        public Guid StoreId { get; set; }    // Bağlı olduğu mağaza

        // === Profil Bilgileri ===
        public string? ProfileName { get; set; }     // Profil adı (Örn: "Trendyol Giyim İmportu")

        // === Kaynak Platform ===
        // "Trendyol", "Amazon", "WooCommerce", "N11", "HepsiBurada", "Custom"
        public string? SourcePlatform { get; set; }

        // === Kolon Eşleşmeleri (JSON formatında) ===
        // Örn: {"Name":"Ürün Adı", "SKU":"Stok Kodu", "Price":"Fiyat", "Barcode":"Barkod", ...}
        // CSV'deki kolon adı → sistemdeki alan adı eşleşmesini tutar
        public string? ColumnMappingsJson { get; set; }

        // === Attribute Eşleşmeleri (JSON formatında) ===
        // Örn: {"Renk":"Color", "Beden":"Size", "Malzeme":"Material"}
        // Dış platformdaki attribute adı → sistemdeki attribute adı eşleşmesi
        public string? AttributeMappingsJson { get; set; }

        // === Kategori Eşleşmeleri (JSON formatında) ===
        // Örn: {"1234":"GuidOfLocalCategory", "5678":"GuidOfLocalCategory2"}
        // Dış platformdaki kategori ID → sistemdeki kategori ID eşleşmesi
        public string? CategoryMappingsJson { get; set; }

        // === Varsayılan Değerler (JSON formatında) ===
        // CSV'de bulunmayan alanlar için varsayılan değerler
        // Örn: {"Currency":"TRY", "TaxRate":"20", "Status":"Draft"}
        public string? DefaultValuesJson { get; set; }

        // === Ayarlar ===
        public string? Delimiter { get; set; } = ",";     // CSV ayraç karakteri (virgül, noktalı virgül, tab)
        public string? Encoding { get; set; } = "UTF-8";   // Dosya kodlaması
        public bool SkipFirstRow { get; set; } = true;     // İlk satır başlık mı

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
