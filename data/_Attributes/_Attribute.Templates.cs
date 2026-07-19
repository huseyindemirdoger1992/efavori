using System;

namespace data._Attribute
{
    /// <summary>
    /// Kategori sınıfına atanabilen, yeniden kullanılabilir ATTRIBUTE KÜMESİ şablonu
    /// (ör. "Laptop" template'i: RAM, CPU, GPU, SSD, Ekran...). 50.000 kategori için
    /// AI her seferinde sıfırdan üretmesin diye Gaming/Business/Ultrabook gibi benzer
    /// kategoriler AYNI template'i paylaşır.
    ///
    /// Önemli: Bu, mevcut data._Product.AttributeTemplates ile KARIŞTIRILMAMALIDIR.
    /// Oradaki şablon ürün-özellik şablonudur; buradaki ise kategori → attribute-kümesi
    /// katalog şablonudur (farklı namespace, farklı sorumluluk).
    ///
    /// Immutable evrim: Template değişince yeni sürüm oluşturmak için
    /// <see cref="Version"/> + <see cref="SupersededByTemplateId"/> kullanılır;
    /// geçmiş ürün verisi sessizce değişmez.
    /// </summary>
    public class AttributeTemplate : AttributeEntityBase
    {
        /// <summary>Dilden bağımsız tekil kod (ör. "laptop_v1").</summary>
        public string CanonicalCode { get; set; } = string.Empty;

        /// <summary>Varsayılan (Türkçe) görünen ad (ör. "Laptop Şablonu").</summary>
        public string CanonicalName { get; set; } = string.Empty;

        /// <summary>Açıklama (opsiyonel).</summary>
        public string? Description { get; set; }

        /// <summary>Sürüm numarası (immutable evrim izleme).</summary>
        public int Version { get; set; } = 1;

        /// <summary>Bu template'i devralan yeni sürüm (varsa).</summary>
        public Guid? SupersededByTemplateId { get; set; }

        /// <summary>Template aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Yapay zekâ izlenebilirlik + manuel koruma bilgisi.</summary>
        public AiMetadata Ai { get; set; } = new();
    }

    /// <summary><see cref="AttributeTemplate"/> için dile özgü metinler.</summary>
    public class AttributeTemplateTranslation : AttributeEntityBase
    {
        /// <summary>Bağlı template.</summary>
        public Guid AttributeTemplateId { get; set; }

        /// <summary>Dil.</summary>
        public Language Language { get; set; }

        /// <summary>Bu dildeki template adı.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Bu dildeki açıklama.</summary>
        public string? Description { get; set; }

        /// <summary>Kaynak.</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;

        /// <summary>Manuel düzenleme kilidi.</summary>
        public bool IsManuallyEdited { get; set; }
    }

    /// <summary>
    /// Template ↔ Attribute bağıntısı: bir template'in içerdiği attribute'lar ve
    /// bu template kapsamındaki override'ları.
    /// (AttributeTemplateId, AttributeDefinitionId) benzersizdir.
    /// </summary>
    public class TemplateAttribute : AttributeEntityBase
    {
        /// <summary>Bağlı template.</summary>
        public Guid AttributeTemplateId { get; set; }

        /// <summary>Bağlı attribute.</summary>
        public Guid AttributeDefinitionId { get; set; }

        /// <summary>Template içindeki sıralama.</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Zorunluluk override'ı (null = attribute varsayılanını kullan).</summary>
        public bool? IsRequiredOverride { get; set; }

        /// <summary>Varyant override'ı (null = attribute varsayılanını kullan).</summary>
        public bool? IsVariantOverride { get; set; }

        /// <summary>Filtrelenebilirlik override'ı (null = varsayılan).</summary>
        public bool? IsFilterableOverride { get; set; }

        /// <summary>Karşılaştırılabilirlik override'ı (null = varsayılan).</summary>
        public bool? IsComparableOverride { get; set; }

        /// <summary>Ürün sayfası görünürlüğü override'ı (null = varsayılan).</summary>
        public bool? IsVisibleOverride { get; set; }

        /// <summary>Görsel grup override'ı (null = attribute'ın kendi grubu).</summary>
        public Guid? AttributeGroupOverrideId { get; set; }

        /// <summary>Yapay zekâ izlenebilirlik + manuel koruma bilgisi.</summary>
        public AiMetadata Ai { get; set; } = new();
    }

    /// <summary>
    /// Template ↔ Category bağıntısı: bir template'in hangi kategoriler tarafından
    /// kullanıldığı. Not: CategoryId, ürün kategorisi CategoriesProduct (Guid PK) tablosuna işaret eder.
    /// (AttributeTemplateId, CategoryId) benzersizdir.
    /// </summary>
    public class TemplateCategory : AttributeEntityBase
    {
        /// <summary>Bağlı template.</summary>
        public Guid AttributeTemplateId { get; set; }

        /// <summary>Bağlı kategori (CategoriesProduct.Id — Guid).</summary>
        public Guid CategoryId { get; set; }

        /// <summary>Bu kategori için birincil template mi?</summary>
        public bool IsPrimary { get; set; } = true;

        /// <summary>Kaynak (özellikle AI önerisi için).</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;

        /// <summary>Yapay zekâ izlenebilirlik + manuel koruma bilgisi.</summary>
        public AiMetadata Ai { get; set; } = new();
    }
}
