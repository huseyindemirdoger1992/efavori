using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Özellik (Attribute) tanımları — tamamen dinamik özellik sistemi.
    /// KRİTİK SAHİPLİK KURALI:
    ///   UserId = null ve StoreId = null → SİSTEM özelliği (yalnızca admin oluşturur/düzenler, şablonlarda kullanılabilir)
    ///   UserId/StoreId dolu             → SATICIYA ÖZEL özellik (yalnızca o satıcının ürünlerinde görünür,
    ///                                      asla şablonlara eklenmez, diğer satıcılara gösterilmez)
    /// </summary>
    public class ProductAttributes
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? UserId { get; set; } // Null = sistem özelliği, dolu = satıcıya özel
        public Guid? StoreId { get; set; } // Null = sistem özelliği, dolu = mağazaya özel

        public string? Name { get; set; } // Özellik adı (Örn: Renk, RAM, Ekran Boyutu)
        public string? Code { get; set; } // Teknik kod / slug (Örn: renk, ram) — entegrasyon eşlemede kullanılır
        public string? Description { get; set; } // Özellik açıklaması

        // Giriş tipi: "Select", "MultiSelect", "Text", "Number", "Bool", "Date"
        public string? InputType { get; set; } = "Select";

        public string? Unit { get; set; } // Birim (Örn: GB, inç, Hz) — Number tipinde kullanışlı
        public bool IsFilterable { get; set; } = false; // Ürün listelerinde filtre olarak gösterilsin mi
        public bool IsActive { get; set; } = true; // Özellik kullanımda mı
        public int DisplayOrder { get; set; } = 0; // Listeleme sırası

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

        public IsDeleted? IsDeleted { get; set; } = new(); // Silinme durumu (soft delete)
    }
}
