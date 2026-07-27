using System;
using data._Attribute; // Language, ContentSource enum'ları ve V3 sözlük referansları

namespace data._Products
{
    /// <summary>
    /// Ürünün TEKNİK ÖZELLİK DEĞERİ — Attribute System V3 sözlüğünün ürün
    /// tarafındaki karşılığı. Ürün sayfasındaki "Teknik Özellikler" tablosunun,
    /// filtrelerin (facet) ve karşılaştırmanın veri kaynağıdır.
    ///
    /// Değer saklama kuralı (AttributeDefinition.DataType'a göre TEK kolon dolar):
    ///  • SingleSelect / MultiSelect / Color → <see cref="AttributeOptionId"/>
    ///    (MultiSelect'te her seçim için AYRI satır açılır — IsMultiValued desteği)
    ///  • Text / MultilineText / RichText / Url / Email → <see cref="ValueText"/>
    ///  • Integer / Decimal → <see cref="ValueNumeric"/>
    ///  • Measurement → <see cref="ValueNumeric"/> + <see cref="UnitId"/> +
    ///    <see cref="ValueNumericInBaseUnit"/> (baz birime normalize kopya;
    ///    birimden bağımsız doğru sıralama/filtreleme sağlar — ör. 1.5 m ve
    ///    150 cm aynı baz mm değerinde buluşur)
    ///  • Boolean → <see cref="ValueBool"/>
    ///  • Date / DateTime → <see cref="ValueDate"/>
    ///  • Json → <see cref="ValueJson"/>
    /// </summary>
    public class ProductAttributeValues : ProductEntityBase
    {
        /// <summary>Bağlı ürün (Products.Id).</summary>
        public Guid ProductId { get; set; }

        /// <summary>Bağlı attribute tanımı (AttributeDefinition.Id).</summary>
        public Guid AttributeDefinitionId { get; set; }

        // ── Değer kolonları (DataType'a göre yalnızca ilgili olan dolar) ──────
        /// <summary>Seçilen kanonik option (AttributeOption.Id) — select/renk tipleri.</summary>
        public Guid? AttributeOptionId { get; set; }

        /// <summary>Serbest metin değeri.</summary>
        public string? ValueText { get; set; }

        /// <summary>
        /// Serbest METİN değerinin dili (yalnızca çevrilebilir metin değerlerinde
        /// dolu; null = dilden bağımsız değer, ör. model kodu). Option tabanlı
        /// değerler çevirisini AttributeOptionTranslations'tan alır, burada dil tutulmaz.
        /// </summary>
        public Language? ValueLanguage { get; set; }

        /// <summary>Girilen sayısal değer (girilen birimde).</summary>
        public decimal? ValueNumeric { get; set; }

        /// <summary>Measurement için seçilen birim (Unit.Id; ör. cm).</summary>
        public Guid? UnitId { get; set; }

        /// <summary>
        /// Sayısal değerin, attribute'ın baz birimine (AttributeDefinition.BaseUnitId)
        /// normalize edilmiş kopyası — birimler arası tutarlı filtre/sıralama için.
        /// </summary>
        public decimal? ValueNumericInBaseUnit { get; set; }

        /// <summary>Evet/Hayır değeri.</summary>
        public bool? ValueBool { get; set; }

        /// <summary>Tarih / tarih-saat değeri (UTC).</summary>
        public DateTime? ValueDate { get; set; }

        /// <summary>Yapılandırılmış JSON değeri.</summary>
        public string? ValueJson { get; set; }

        // ── Görünüm / yönetişim ──────────────────────────────────────────────
        /// <summary>Teknik özellikler tablosundaki sıralama (null = attribute varsayılanı).</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Ürün sayfasında görünürlük override'ı (null = kategori/attribute varsayılanı).</summary>
        public bool? IsVisibleOnProductPage { get; set; }

        /// <summary>Değerin kaynağı (Manual/Ai/Import — mevcut ContentSource enum'ı).</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;

        /// <summary>Satıcı/Admin elle düzenledi mi? (AI/Import ezme kilidi).</summary>
        public bool IsManuallyEdited { get; set; }
    }
}
