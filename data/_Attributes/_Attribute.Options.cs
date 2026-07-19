using System;

namespace data._Attribute
{
    /// <summary>
    /// Bir attribute'ın seçilebilir KANONİK değeri (ör. Renk → Siyah; RAM → 16;
    /// CPU → Intel Core Ultra 7). Merkezî sözlüğün "değer" tarafıdır; ürünün
    /// gerçek attribute değeri (ProductAttributeValues) bu option'a ID ile bağlanır.
    ///
    /// Ağaç (tree) desteği: <see cref="ParentOptionId"/> ile hiyerarşik option'lar
    /// (ör. Renk → Maviler → Lacivert) modellenebilir.
    /// </summary>
    public class AttributeOption : AttributeEntityBase
    {
        /// <summary>Bağlı attribute.</summary>
        public Guid AttributeDefinitionId { get; set; }

        /// <summary>Üst option (ağaç yapısı için, opsiyonel).</summary>
        public Guid? ParentOptionId { get; set; }

        /// <summary>
        /// Attribute içinde tekil kanonik kod (ör. "black", "16", "intel_core_ultra_7").
        /// (AttributeDefinitionId, CanonicalCode) benzersizdir.
        /// </summary>
        public string CanonicalCode { get; set; } = string.Empty;

        /// <summary>Normalize edilmiş değer (dedup/arama için indeksli).</summary>
        public string NormalizedValue { get; set; } = string.Empty;

        /// <summary>Varsayılan (Türkçe) görünen değer.</summary>
        public string CanonicalValue { get; set; } = string.Empty;

        /// <summary>Renk option'ları için hex kod (ör. "#000000").</summary>
        public string? ColorHex { get; set; }

        /// <summary>Option için görsel/örnek (swatch) URL'i.</summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Sayısal option'lar için karşılaştırılabilir sayısal değer
        /// (ör. RAM "16" → 16). Doğru sayısal sıralama/filtreleme sağlar.
        /// </summary>
        public decimal? NumericValue { get; set; }

        /// <summary>Sıralama önceliği.</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Ağaç derinliği (denormalize; sorgu kolaylığı için).</summary>
        public int Level { get; set; }

        /// <summary>
        /// Materialize edilmiş yol (denormalize; ağaç sorguları için, opsiyonel).
        /// Ör: "renkler/maviler/lacivert".
        /// </summary>
        public string? PathCodes { get; set; }

        /// <summary>Option aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Yapay zekâ izlenebilirlik + manuel koruma bilgisi.</summary>
        public AiMetadata Ai { get; set; } = new();
    }

    /// <summary>
    /// <see cref="AttributeOption"/> için dile özgü metinler.
    /// (AttributeOptionId, Language) benzersizdir.
    /// </summary>
    public class AttributeOptionTranslation : AttributeEntityBase
    {
        /// <summary>Bağlı option.</summary>
        public Guid AttributeOptionId { get; set; }

        /// <summary>Dil.</summary>
        public Language Language { get; set; }

        /// <summary>Bu dildeki görünen değer (ör. "Siyah" / "Black" / "Schwarz").</summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>Kaydın kaynağı.</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;

        /// <summary>Admin bu çeviriyi elle düzenledi mi? (AI/Import ezme kilidi).</summary>
        public bool IsManuallyEdited { get; set; }
    }

    /// <summary>Option için insana dönük alternatif görünen ad (alias).</summary>
    public class AttributeOptionAlias : AttributeEntityBase
    {
        /// <summary>Bağlı option.</summary>
        public Guid AttributeOptionId { get; set; }

        /// <summary>Alias dili (null = dilden bağımsız).</summary>
        public Language? Language { get; set; }

        /// <summary>Alias metni.</summary>
        public string Alias { get; set; } = string.Empty;

        /// <summary>Normalize edilmiş alias (indeksli).</summary>
        public string NormalizedAlias { get; set; } = string.Empty;

        /// <summary>Kaydın kaynağı.</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;
    }

    /// <summary>
    /// Option için makine eşleştirme/dedup tokenı (synonym).
    /// Ör: "Gray", "Grey", "Gri" → aynı option. AI aynı option'ı ikinci kez üretmez.
    /// (AttributeDefinitionId, NormalizedToken) attribute kapsamında tekildir.
    /// </summary>
    public class AttributeOptionSynonym : AttributeEntityBase
    {
        /// <summary>Bağlı option.</summary>
        public Guid AttributeOptionId { get; set; }

        /// <summary>
        /// Denormalize attribute referansı — dedup'ı attribute kapsamında
        /// (option bazında değil) hızlı kısıtlamak için tutulur.
        /// </summary>
        public Guid AttributeDefinitionId { get; set; }

        /// <summary>Ham token metni.</summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>Normalize edilmiş token (indeksli).</summary>
        public string NormalizedToken { get; set; } = string.Empty;

        /// <summary>Token dili (null = dilden bağımsız).</summary>
        public Language? Language { get; set; }

        /// <summary>Kaynak.</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;

        /// <summary>AI güven skoru (0..1).</summary>
        public decimal? Confidence { get; set; }
    }
}
