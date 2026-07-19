using System;

namespace data._Attribute
{
    /// <summary>
    /// Attribute bağımlılığı (koşullu görünürlük/zorunluluk).
    /// Ör: "Has Bluetooth = false" ise "Bluetooth Version" gizlenir;
    ///     "Gender = Female" ise "Bra Size" gösterilir.
    ///
    /// Birden çok koşul <see cref="ConditionGroup"/> ile gruplanabilir ve grup içi
    /// mantık <see cref="GroupLogic"/> (AND/OR) ile belirlenir. Bağımlılık global
    /// (CategoryId null) veya belirli bir kategoriye özgü olabilir.
    /// </summary>
    public class AttributeDependency : AttributeEntityBase
    {
        /// <summary>Kapsam kategorisi (null = global; CategoriesProduct.Id — Guid).</summary>
        public Guid? CategoryId { get; set; }

        /// <summary>Koşulu tetikleyen (kontrol eden) attribute.</summary>
        public Guid SourceAttributeDefinitionId { get; set; }

        /// <summary>Karşılaştırma operatörü.</summary>
        public DependencyOperator Operator { get; set; } = DependencyOperator.Equals;

        /// <summary>Karşılaştırılan option (option tabanlı koşullarda).</summary>
        public Guid? ExpectedOptionId { get; set; }

        /// <summary>Karşılaştırılan skaler değer (sayı/metin/boolean koşullarında).</summary>
        public string? ExpectedValue { get; set; }

        /// <summary>Koşul sağlandığında etkilenen hedef attribute.</summary>
        public Guid TargetAttributeDefinitionId { get; set; }

        /// <summary>Uygulanacak eylem (göster/gizle/zorunlu yap...).</summary>
        public DependencyAction Action { get; set; } = DependencyAction.Show;

        /// <summary>Koşul grubu numarası (aynı grup birlikte değerlendirilir).</summary>
        public int ConditionGroup { get; set; }

        /// <summary>Grup içi mantıksal birleşim (AND/OR).</summary>
        public DependencyLogic GroupLogic { get; set; } = DependencyLogic.And;

        /// <summary>Sıralama önceliği.</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Kural aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Yapay zekâ izlenebilirlik + manuel koruma bilgisi.</summary>
        public AiMetadata Ai { get; set; } = new();
    }

    /// <summary>
    /// Yeniden kullanılabilir normalizasyon/transform kuralı. Import ve dedup
    /// sırasında dış değerleri kanonik biçime getirmek için kullanılır
    /// (ör. "16GB" → "16", "grey" → "gray", büyük/küçük harf düzeltme).
    /// AttributeMapping/OptionMapping bu kurallara ID ile bağlanabilir.
    /// </summary>
    public class NormalizationRule : AttributeEntityBase
    {
        /// <summary>Tekil kod (ör. "strip_gb_suffix").</summary>
        public string CanonicalCode { get; set; } = string.Empty;

        /// <summary>İnsan-okur ad.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Kural türü.</summary>
        public NormalizationRuleType RuleType { get; set; } = NormalizationRuleType.Lowercase;

        /// <summary>Desen/kaynak (regex, aranan değer vb.).</summary>
        public string? Pattern { get; set; }

        /// <summary>Değiştirilecek hedef (regex replace / map hedefi).</summary>
        public string? Replacement { get; set; }

        /// <summary>Uygulama sırası (küçük = önce).</summary>
        public int Priority { get; set; }

        /// <summary>Yalnızca belirli veri tipine uygulanır (null = tümü).</summary>
        public AttributeDataType? AppliesToDataType { get; set; }

        /// <summary>Açıklama.</summary>
        public string? Description { get; set; }

        /// <summary>Kural aktif mi?</summary>
        public bool IsActive { get; set; } = true;
    }
}
