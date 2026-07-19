using System;

namespace data._Attribute
{
    /// <summary>
    /// Merkezî attribute TANIMI — sistemin "sözlüğü". Bir attribute (ör. RAM, Renk,
    /// Ekran Boyutu) tüm sistemde YALNIZCA BİR KEZ tanımlanır; sonra Category,
    /// Template, Import, Export, Filter, Search ve AI tarafından tekrar kullanılır.
    ///
    /// Not: Sınıf adı "Attribute" YERİNE "AttributeDefinition"dır; çünkü "Attribute"
    /// .NET'te <see cref="System.Attribute"/> ile çakışır ve karışıklık yaratır.
    ///
    /// Merkezî felsefe: Satıcı yeni attribute OLUŞTURAMAZ — bu tablo yalnızca sistem
    /// (Admin/AI) tarafından yönetilir; bu yüzden burada satıcı/mağaza sahipliği yoktur.
    /// </summary>
    public class AttributeDefinition : AttributeEntityBase
    {
        /// <summary>
        /// Dilden bağımsız, GLOBAL TEKİL kanonik kod (ör. "ram", "color",
        /// "screen_size"). Dedup ve tüm eşleştirmelerin çapasıdır.
        /// Aynı kavramın farklı bağlamları için ayrıştırıcı ekleyin
        /// (ör. giyim bedeni → "size_clothing", ekran boyutu → "size_screen").
        /// </summary>
        public string CanonicalCode { get; set; } = string.Empty;

        /// <summary>
        /// Kanonik adın normalize edilmiş biçimi (küçük harf, diyakritiksiz).
        /// AI/Import dedup'ında hızlı arama için indekslidir.
        /// </summary>
        public string NormalizedName { get; set; } = string.Empty;

        /// <summary>Varsayılan (Türkçe) görünen ad — hızlı okuma için satır içi.</summary>
        public string CanonicalName { get; set; } = string.Empty;

        /// <summary>Bağlı görsel grup (opsiyonel).</summary>
        public Guid? AttributeGroupId { get; set; }

        /// <summary>Veri tipi (saklama/doğrulama davranışı).</summary>
        public AttributeDataType DataType { get; set; } = AttributeDataType.Text;

        /// <summary>Arayüz giriş bileşeni (widget).</summary>
        public AttributeInputType InputType { get; set; } = AttributeInputType.TextBox;

        // ── Ölçü (Measurement) attribute'ları için ──────────────────────────
        /// <summary>Ölçü attribute'ında izin verilen birim grubu (ör. Uzunluk).</summary>
        public Guid? UnitGroupId { get; set; }

        /// <summary>Normalize değeri saklamak için baz/temel birim (ör. mm).</summary>
        public Guid? BaseUnitId { get; set; }

        // ── Davranış varsayılanları (Category/Template ile override edilebilir) ─
        /// <summary>Birden fazla değer/option seçilebilir mi?</summary>
        public bool IsMultiValued { get; set; }

        /// <summary>Varsayılan olarak zorunlu mu?</summary>
        public bool IsRequiredByDefault { get; set; }

        /// <summary>
        /// Varsayılan olarak varyant üretebilir mi? (ör. giyimde Renk = true).
        /// Kategori bazında override edilebilir (mobilyada Renk yalnızca bilgi).
        /// </summary>
        public bool IsVariantByDefault { get; set; }

        /// <summary>Varsayılan olarak filtrelenebilir (facet) mi?</summary>
        public bool IsFilterableByDefault { get; set; }

        /// <summary>Varsayılan olarak arama motoru alanı mı?</summary>
        public bool IsSearchableByDefault { get; set; }

        /// <summary>Varsayılan olarak karşılaştırma (compare) alanı mı?</summary>
        public bool IsComparableByDefault { get; set; }

        /// <summary>Varsayılan olarak ürün sayfasında görünür mü?</summary>
        public bool IsVisibleOnProductPageByDefault { get; set; } = true;

        /// <summary>Öne çıkan/başlık özelliği mi? (ör. Laptop için CPU/RAM).</summary>
        public bool IsKeyAttribute { get; set; }

        // ── Doğrulama (Validation) kuralları ────────────────────────────────
        /// <summary>Regex doğrulama deseni (opsiyonel).</summary>
        public string? RegexPattern { get; set; }

        /// <summary>Sayısal minimum (Integer/Decimal/Measurement).</summary>
        public decimal? MinNumericValue { get; set; }

        /// <summary>Sayısal maksimum.</summary>
        public decimal? MaxNumericValue { get; set; }

        /// <summary>Sayısal artış adımı (ör. 0.5).</summary>
        public decimal? NumericStep { get; set; }

        /// <summary>Ondalık basamak sayısı.</summary>
        public int? DecimalScale { get; set; }

        /// <summary>Metin minimum uzunluğu.</summary>
        public int? MinLength { get; set; }

        /// <summary>Metin maksimum uzunluğu.</summary>
        public int? MaxLength { get; set; }

        /// <summary>Değer ürün/varyant kapsamında benzersiz mi olmalı?</summary>
        public bool IsUnique { get; set; }

        /// <summary>Varsayılan değer (opsiyonel).</summary>
        public string? DefaultValue { get; set; }

        /// <summary>Genel sıralama önceliği.</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Attribute aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Sistemsel/kritik attribute — silinemez/salt-okunur korunur.</summary>
        public bool IsSystemLocked { get; set; }

        /// <summary>Sürüm numarası (immutable evrim izleme için).</summary>
        public int Version { get; set; } = 1;

        /// <summary>Yapay zekâ izlenebilirlik + manuel koruma bilgisi.</summary>
        public AiMetadata Ai { get; set; } = new();
    }

    /// <summary>
    /// <see cref="AttributeDefinition"/> için dile özgü metinler.
    /// (AttributeDefinitionId, Language) benzersizdir.
    /// </summary>
    public class AttributeTranslation : AttributeEntityBase
    {
        /// <summary>Bağlı attribute.</summary>
        public Guid AttributeDefinitionId { get; set; }

        /// <summary>Dil.</summary>
        public Language Language { get; set; }

        /// <summary>Bu dildeki attribute adı (ör. RAM → "Bellek").</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Bu dildeki açıklama.</summary>
        public string? Description { get; set; }

        /// <summary>Bu dildeki yardım metni (form ipucu).</summary>
        public string? HelpText { get; set; }

        /// <summary>Bu dildeki placeholder metni.</summary>
        public string? Placeholder { get; set; }

        /// <summary>Kaydın kaynağı.</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;

        /// <summary>Admin bu çeviriyi elle düzenledi mi? (AI/Import ezme kilidi).</summary>
        public bool IsManuallyEdited { get; set; }
    }

    /// <summary>
    /// İNSANA DÖNÜK alternatif görünen ad (alias). Ör: RAM için TR'de "Bellek".
    /// Görüntüleme/eşleştirme kolaylığı sağlar. (Synonym'den farkı: alias görünen
    /// etikettir; synonym ise makine eşleştirmesi/dedup tokenıdır.)
    /// </summary>
    public class AttributeAlias : AttributeEntityBase
    {
        /// <summary>Bağlı attribute.</summary>
        public Guid AttributeDefinitionId { get; set; }

        /// <summary>Alias'ın dili (null = dilden bağımsız).</summary>
        public Language? Language { get; set; }

        /// <summary>Alias metni (görünen biçim).</summary>
        public string Alias { get; set; } = string.Empty;

        /// <summary>Alias'ın normalize edilmiş biçimi (indeksli).</summary>
        public string NormalizedAlias { get; set; } = string.Empty;

        /// <summary>Kaydın kaynağı.</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;
    }

    /// <summary>
    /// MAKİNE eşleştirme/dedup tokenı (synonym). Ör: RAM için "system memory",
    /// "main memory", "bellek", "memory". AI/Import bir attribute üretmeden önce
    /// ürettiği metnin normalize tokenını burada arar; eşleşirse YENİ attribute
    /// oluşturmaz, mevcut olanı kullanır.
    ///
    /// <see cref="NormalizedToken"/> GLOBAL TEKİLDİR: bir token yalnızca tek bir
    /// attribute'a işaret edebilir (aksi hâlde eşleştirme belirsizleşir).
    /// </summary>
    public class AttributeSynonym : AttributeEntityBase
    {
        /// <summary>Token'ın işaret ettiği attribute.</summary>
        public Guid AttributeDefinitionId { get; set; }

        /// <summary>Ham token metni.</summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>Normalize edilmiş token (global tekil, indeksli).</summary>
        public string NormalizedToken { get; set; } = string.Empty;

        /// <summary>Token'ın dili (null = dilden bağımsız).</summary>
        public Language? Language { get; set; }

        /// <summary>Kaynak (özellikle AI üretimi için).</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;

        /// <summary>AI eşleştirmesinde güven skoru (0..1).</summary>
        public decimal? Confidence { get; set; }
    }
}
