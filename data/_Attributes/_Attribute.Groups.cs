using System;

namespace data._Attribute
{
    /// <summary>
    /// Attribute'ları arayüzde mantıksal başlıklar altında toplayan grup
    /// (ör. "Teknik Özellikler", "Boyutlar", "Bağlantı", "Genel").
    /// Template'ten ayrıdır: Group görsel/mantıksal kümeleme; Template ise
    /// kategoriye atanan yeniden kullanılabilir attribute kümesidir.
    /// </summary>
    public class AttributeGroup : AttributeEntityBase
    {
        /// <summary>
        /// Dilden bağımsız, tekil ve kararlı kod (ör. "technical_specs").
        /// Kod tabanında/mapping'de referans olarak kullanılır.
        /// </summary>
        public string CanonicalCode { get; set; } = string.Empty;

        /// <summary>Varsayılan (Türkçe) görünen ad — hızlı okuma için satır içi tutulur.</summary>
        public string CanonicalName { get; set; } = string.Empty;

        /// <summary>Sıralama önceliği (küçük = önce).</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Font Awesome / Bootstrap Icons ikon sınıfı (ör. "fa fa-cogs").</summary>
        public string? IconCssClass { get; set; }

        /// <summary>Grup aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Yapay zekâ izlenebilirlik + manuel koruma bilgisi.</summary>
        public AiMetadata Ai { get; set; } = new();
    }

    /// <summary>
    /// <see cref="AttributeGroup"/> için dile özgü metinler.
    /// (AttributeGroupId, Language) benzersizdir.
    /// </summary>
    public class AttributeGroupTranslation : AttributeEntityBase
    {
        /// <summary>Bağlı grup.</summary>
        public Guid AttributeGroupId { get; set; }

        /// <summary>Dil.</summary>
        public Language Language { get; set; }

        /// <summary>Bu dildeki grup adı.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Bu dildeki açıklama (opsiyonel).</summary>
        public string? Description { get; set; }

        /// <summary>Kaydın kaynağı (manuel/AI/import).</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;

        /// <summary>Admin bu çeviriyi elle düzenledi mi? (AI/Import ezme kilidi).</summary>
        public bool IsManuallyEdited { get; set; }
    }
}
