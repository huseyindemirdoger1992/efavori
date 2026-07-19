using data.Owned;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Attributes
{
    /// <summary>
    /// Attribute'ların ürün detay sayfasında ve satıcı formunda gruplanmasını sağlar.
    /// Örn: "Genel Bilgiler", "Teknik Özellikler", "Batarya", "Kamera", "Ekran",
    /// "Bağlantılar", "Boyutlar", "Paket İçeriği", "Garanti".
    /// Gruplar globaldir; bir attribute'ın hangi kategoride hangi grupta görüneceği
    /// AttributeCategoryJoint.GroupId üzerinden belirlenir.
    /// </summary>
    public class AttributeGroup
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Sistem genelinde benzersiz teknik kod. Örn: "technical_specs", "battery".
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Grubun 10 dildeki görünen adı.
        /// </summary>
        public LangText? Name { get; set; } = new();

        /// <summary>
        /// Grubun 10 dildeki açıklaması (opsiyonel, admin paneli için).
        /// </summary>
        public LangText? Description { get; set; } = new();

        /// <summary>
        /// Grup başlığında gösterilecek Font Awesome ikon sınıfı. Örn: "fa fa-battery-full"
        /// </summary>
        public string? IconCss { get; set; }

        /// <summary>
        /// Grupların birbirine göre gösterim sırası.
        /// </summary>
        public int SortOrder { get; set; } = 0;

        /// <summary>Aktif / Pasif</summary>
        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
