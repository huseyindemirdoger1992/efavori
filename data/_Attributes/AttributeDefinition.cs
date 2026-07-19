using data.Owned;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Attributes
{
    /// <summary>
    /// Sistemdeki tüm ürün özelliklerinin (attribute) merkezi tanım tablosu.
    /// Bir attribute bir kez tanımlanır, AttributeCategoryJoint üzerinden
    /// birden fazla kategoriye bağlanır. Hiçbir kategoriye özel kod yazılmaz;
    /// Admin panelden kod yazmadan yeni attribute oluşturulabilir.
    /// Değerler AttributeValue tablosunda tutulur (EAV yaklaşımı).
    /// </summary>
    public class AttributeDefinition
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Sistem genelinde benzersiz teknik kod (İngilizce, snake_case).
        /// Örn: "screen_size", "battery_capacity_mah", "ram_type".
        /// Import/Export, API, ElasticSearch ve AI eşleştirmelerinde
        /// dilden bağımsız anahtar olarak bu kod kullanılır. Unique index atılmalıdır.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Attribute'ın veri tipi (Text, Decimal, Dropdown, Weight vb.).
        /// AttributeValue tablosunda hangi tipli kolonun dolacağını belirler.
        /// </summary>
        public AttributeDataType DataType { get; set; } = AttributeDataType.Text;

        // -------------------- ÇOK DİLLİ METİNLER --------------------

        /// <summary>
        /// Attribute'ın 10 dildeki görünen adı. (Kolonlar: Name_Tr, Name_En ...)
        /// </summary>
        public LangText? Name { get; set; } = new();

        /// <summary>
        /// Attribute'ın 10 dildeki detaylı açıklaması.
        /// </summary>
        public LangText? Description { get; set; } = new();

        /// <summary>
        /// Form alanının üzerinde gösterilecek ipucu metni (10 dil).
        /// </summary>
        public LangText? Tooltip { get; set; } = new();

        /// <summary>
        /// Input içinde gösterilecek placeholder metni (10 dil).
        /// </summary>
        public LangText? Placeholder { get; set; } = new();

        /// <summary>
        /// Alanın altında gösterilecek yardım metni (10 dil).
        /// </summary>
        public LangText? HelpText { get; set; } = new();

        // -------------------- VALIDATION --------------------

        /// <summary>
        /// Varsayılan değer. Tip ne olursa olsun string olarak saklanır,
        /// DataType'a göre parse edilir.
        /// </summary>
        public string? DefaultValue { get; set; }

        /// <summary>
        /// Değer doğrulama için regex deseni. Null ise regex kontrolü yapılmaz.
        /// </summary>
        public string? ValidationRegex { get; set; }

        /// <summary>
        /// Sayısal tipler için minimum değer, metin tipleri için minimum uzunluk.
        /// </summary>
        public decimal? MinValue { get; set; }

        /// <summary>
        /// Sayısal tipler için maksimum değer, metin tipleri için maksimum uzunluk.
        /// </summary>
        public decimal? MaxValue { get; set; }

        /// <summary>
        /// Decimal/Money/Percentage tipleri için ondalık hassasiyet. Örn: 2 → 12.50
        /// </summary>
        public int? DecimalPrecision { get; set; }

        // -------------------- BİRİM --------------------

        /// <summary>
        /// Varsayılan ölçü birimi ID'si (AttributeUnit tablosu). Örn: "gr", "cm", "mAh".
        /// Weight/Length/Volume gibi tiplerde satıcı farklı birim seçerse
        /// değer base birime çevrilerek normalize edilir.
        /// </summary>
        public Guid? UnitId { get; set; }

        /// <summary>
        /// Birim grubu kodu (Weight, Length, Volume, Area, Data, Power...).
        /// Satıcıya sadece bu gruptaki birimler gösterilir.
        /// </summary>
        public string? UnitGroup { get; set; }

        // -------------------- DAVRANIŞ BAYRAKLARI --------------------

        /// <summary>Kategori bağında override edilmediyse varsayılan zorunluluk durumu.</summary>
        public bool IsRequiredDefault { get; set; } = false;

        /// <summary>Arama motorunda (ElasticSearch/Azure Search/OpenSearch) indexlensin mi?</summary>
        public bool IsSearchable { get; set; } = false;

        /// <summary>Kategori sayfasında faceted filter olarak gösterilsin mi?</summary>
        public bool IsFilterable { get; set; } = false;

        /// <summary>Listelemede sıralama kriteri olarak kullanılabilir mi?</summary>
        public bool IsSortable { get; set; } = false;

        /// <summary>Ürün karşılaştırma tablosunda gösterilsin mi?</summary>
        public bool IsComparable { get; set; } = false;

        /// <summary>Ürün detay sayfasında (specification tablosunda) görünsün mü?</summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>Satıcı bu alanı düzenleyebilir mi?</summary>
        public bool IsEditable { get; set; } = true;

        /// <summary>Salt okunur mu? (Sistem/AI/Import tarafından doldurulur, satıcı sadece görür)</summary>
        public bool IsReadOnly { get; set; } = false;

        /// <summary>Formda tamamen gizli mi? (Sadece sistem içi kullanılır)</summary>
        public bool IsHidden { get; set; } = false;

        /// <summary>AI bu attribute için otomatik değer üretebilir mi?</summary>
        public bool UseInAi { get; set; } = true;

        /// <summary>Dış platform importlarında eşleştirilebilir mi?</summary>
        public bool UseInImport { get; set; } = true;

        /// <summary>Export ve feed'lerde (Google Merchant, Facebook Catalog) kullanılsın mı?</summary>
        public bool UseInExport { get; set; } = true;

        /// <summary>SEO meta/başlık/description üretiminde kullanılsın mı?</summary>
        public bool UseInSeo { get; set; } = false;

        /// <summary>Varyant ekseni olarak kullanılabilir mi? (Renk, Beden, RAM gibi)</summary>
        public bool UseInVariant { get; set; } = false;

        /// <summary>Ürün seviyesinde kullanılabilir mi?</summary>
        public bool UseInProduct { get; set; } = true;

        /// <summary>Üst kategoriye bağlandığında alt kategorilere miras kalabilir mi?</summary>
        public bool IsInheritable { get; set; } = true;

        /// <summary>Aktif / Pasif</summary>
        public bool IsActive { get; set; } = true;

        // -------------------- SIRALAMA / AI --------------------

        /// <summary>
        /// Kategori bağında override edilmediyse varsayılan gösterim sırası.
        /// </summary>
        public int SortOrder { get; set; } = 0;

        /// <summary>
        /// Bu attribute AI tarafından mı oluşturuldu? (Kategori analizi çıktısı)
        /// </summary>
        public bool? CreatedByAi { get; set; } = false;

        /// <summary>
        /// Admin bu tanımı elle düzenledi mi? true ise AI bu tanımın üzerine
        /// asla yazamaz. (ProductTranslations.IsManuallyEdited deseni)
        /// </summary>
        public bool IsManuallyEdited { get; set; } = false;

        // -------------------- SİSTEM --------------------

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
