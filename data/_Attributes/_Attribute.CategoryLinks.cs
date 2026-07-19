using System;

namespace data._Attribute
{
    /// <summary>
    /// Bir attribute'ın belirli bir KATEGORİYE atanmış çözümlenmiş kaydı ve o
    /// kategoriye özgü override'ları. Attribute'lar kategoriye doğrudan, template
    /// üzerinden veya üst kategoriden miras (inheritance) yoluyla gelebilir.
    ///
    /// Miras + override kuralı: Üst kategoride tanımlı Brand/Warranty/Country gibi
    /// attribute'lar alt kategorilere otomatik aktarılır; alt kategori isterse
    /// override eder (<see cref="IsOverride"/>) veya bastırır (<see cref="IsSuppressed"/>).
    /// Çözümleme önceliği: doğrudan/override > template > miras.
    ///
    /// Not: CategoryId, ürün kategorisi CategoriesProduct (Guid PK) tablosuna işaret eder.
    /// (CategoryId, AttributeDefinitionId) benzersizdir.
    /// </summary>
    public class CategoryAttribute : AttributeEntityBase
    {
        /// <summary>Bağlı kategori (CategoriesProduct.Id — Guid).</summary>
        public Guid CategoryId { get; set; }

        /// <summary>Bağlı attribute.</summary>
        public Guid AttributeDefinitionId { get; set; }

        /// <summary>Bu atama bir template'ten geldiyse kaynak template.</summary>
        public Guid? SourceTemplateId { get; set; }

        // ── Miras (inheritance) durumu ──────────────────────────────────────
        /// <summary>Üst kategoriden miras yoluyla mı geldi?</summary>
        public bool IsInherited { get; set; }

        /// <summary>Miras kaynağı üst kategori (CategoriesProduct.Id — Guid).</summary>
        public Guid? InheritedFromCategoryId { get; set; }

        /// <summary>Miras/template ayarını bu kategoride override ediyor mu?</summary>
        public bool IsOverride { get; set; }

        /// <summary>Miras gelen attribute bu kategoride gizlensin (bastırılsın) mı?</summary>
        public bool IsSuppressed { get; set; }

        // ── Kategori bazlı davranış override'ları (null = attribute varsayılanı) ─
        /// <summary>Bu kategoride zorunlu mu? (null = varsayılan).</summary>
        public bool? IsRequired { get; set; }

        /// <summary>
        /// Bu kategoride varyant mı? (null = attribute varsayılanı).
        /// Ör: giyimde Renk = true (varyant), mobilyada Renk = false (yalnız bilgi).
        /// </summary>
        public bool? IsVariant { get; set; }

        /// <summary>Bu kategoride filtrelenebilir mi? (null = varsayılan).</summary>
        public bool? IsFilterable { get; set; }

        /// <summary>Bu kategoride aranabilir mi? (null = varsayılan).</summary>
        public bool? IsSearchable { get; set; }

        /// <summary>Bu kategoride karşılaştırılabilir mi? (null = varsayılan).</summary>
        public bool? IsComparable { get; set; }

        /// <summary>Bu kategoride ürün sayfasında görünür mü? (null = varsayılan).</summary>
        public bool? IsVisibleOnProductPage { get; set; }

        /// <summary>Bu kategorideki sıralama.</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Bu kategorideki görsel grup override'ı (opsiyonel).</summary>
        public Guid? AttributeGroupOverrideId { get; set; }

        /// <summary>Atama aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Yapay zekâ izlenebilirlik + manuel koruma bilgisi.</summary>
        public AiMetadata Ai { get; set; } = new();
    }
}
