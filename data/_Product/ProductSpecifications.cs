using data._Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Product
{
    /// <summary>
    /// Teknik özellikleri key-value mantığıyla saklar.
    /// Varyant oluşturmayan, bilgi amaçlı özellikler burada tutulur.
    /// Örn: "Malzeme" → "Pamuk", "Garanti Süresi" → "2 Yıl", "Menşei" → "Türkiye"
    /// Gruplandırılabilir yapıda (GroupName ile).
    /// </summary>
    public class ProductSpecifications
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; }  // Ürün ID bilgisi
        public Guid UserId { get; set; }     // Satıcı ID bilgisi

        // === Özellik Bilgileri ===
        public string? GroupName { get; set; }  // Grup adı (Genel, Teknik, Fiziksel vb.)
        public string? Key { get; set; }        // Özellik adı (Malzeme, Garanti Süresi, Menşei)
        public string? Value { get; set; }      // Özellik değeri (Pamuk, 2 Yıl, Türkiye)

        public int SortOrder { get; set; } = 0; // Görüntülenme sırası

        // === Dış Platform Referansı ===
        public string? ExternalKey { get; set; } // Dış platformdaki özellik adı karşılığı

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
