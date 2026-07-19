using data.Owned;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Attributes
{
    /// <summary>
    /// Attribute değerlerinin tutulduğu merkezi EAV (Entity-Attribute-Value) tablosu.
    /// Sistemin en büyük tablosudur (on milyonlarca satır hedefi).
    ///
    /// KAPSAM (scope) MANTIĞI:
    /// Değer; ürün, varyant, SKU, mağaza, ülke, dil ve kanal boyutlarında tutulabilir.
    /// Null olan boyut "tümü için geçerli" demektir. Değer çözümlemesi en özelden
    /// en genele doğru yapılır (SKU > Varyant > Ürün, Store'lu > Store'suz,
    /// Dil'li > Dil'siz). Örn: LanguageCode = null → tüm diller için ortak değer
    /// (batarya kapasitesi gibi), LanguageCode = "en" → sadece İngilizce değer
    /// (metinsel malzeme adı gibi).
    ///
    /// TİPLİ KOLONLAR:
    /// DataType'a göre yalnızca ilgili kolon doldurulur, diğerleri null kalır.
    /// MultipleSelect için her seçilen seçenek ayrı satır olarak yazılır (SortOrder ile).
    /// </summary>
    public class AttributeValue
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Değerin ait olduğu attribute ID'si (AttributeDefinition tablosu).
        /// </summary>
        public Guid AttributeId { get; set; }

        // -------------------- KAPSAM (SCOPE) --------------------

        /// <summary>
        /// Ürün ID'si (Products tablosu). Varyant/SKU bazlı değerlerde de doldurulur
        /// (sorguların tek kolondan yapılabilmesi için).
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Varyant ID'si. Null ise değer ürün seviyesindedir.
        /// </summary>
        public Guid? VariantId { get; set; }

        /// <summary>
        /// SKU ID'si. Null ise değer varyant veya ürün seviyesindedir.
        /// </summary>
        public Guid? SkuId { get; set; }

        /// <summary>
        /// Mağaza ID'si (Store tablosu). Null ise tüm mağazalar için geçerlidir.
        /// Aynı ürünü satan farklı satıcıların farklı değer girebildiği durumlar için.
        /// </summary>
        public Guid? StoreId { get; set; }

        /// <summary>
        /// ISO ülke kodu ("TR", "DE"...). Null ise tüm ülkeler için geçerlidir.
        /// Ülkeye göre değişen değerler için (voltaj, fiş tipi, sertifika vb.).
        /// </summary>
        public string? CountryCode { get; set; }

        /// <summary>
        /// Dil kodu ("tr", "en"...). Null ise dilden bağımsız değerdir.
        /// Sadece metinsel, çevrilebilir değerlerde doldurulur.
        /// </summary>
        public string? LanguageCode { get; set; }

        /// <summary>
        /// Satış kanalı kodu ("web", "mobile", "google_merchant", "amazon"...).
        /// Null ise tüm kanallar için geçerlidir.
        /// </summary>
        public string? Channel { get; set; }

        // -------------------- TİPLİ DEĞER KOLONLARI --------------------

        /// <summary>
        /// Metin tabanlı tüm tipler için değer (Text, LongText, RichText, Url,
        /// Email, Phone, Barcode, Ean, Gps "lat,lng", referans Guid'leri vb.).
        /// </summary>
        public string? ValueText { get; set; }

        /// <summary>
        /// Sayısal tipler için değer (Integer, Decimal, Money, Percentage,
        /// Weight, Length, Range vb.). Birim tipli değerler her zaman
        /// base birime normalize edilerek yazılır (filtre/sıralama tutarlılığı için).
        /// </summary>
        public decimal? ValueNumber { get; set; }

        /// <summary>
        /// Date, DateTime, Time tipleri için değer (UTC).
        /// </summary>
        public DateTime? ValueDate { get; set; }

        /// <summary>
        /// Boolean tipi için değer.
        /// </summary>
        public bool? ValueBool { get; set; }

        /// <summary>
        /// Json, KeyValue, Dimension ({"w":10,"h":20,"d":5}), Rgb gibi
        /// yapısal tipler için JSON değeri.
        /// </summary>
        public string? ValueJson { get; set; }

        /// <summary>
        /// Seçim tipli attribute'larda seçilen seçenek ID'si (AttributeOption tablosu).
        /// MultipleSelect'te her seçenek için ayrı AttributeValue satırı açılır.
        /// </summary>
        public Guid? OptionId { get; set; }

        /// <summary>
        /// Image, Video, File tipleri için medya ID'si (Media tablosu).
        /// MultipleImage'da her görsel için ayrı satır açılır.
        /// </summary>
        public Guid? MediaId { get; set; }

        /// <summary>
        /// Girilen değerin birimi (AttributeUnit tablosu). ValueNumber base birime
        /// normalize edilmişse bile, satıcının girdiği orijinal birim burada tutulur.
        /// </summary>
        public Guid? UnitId { get; set; }

        /// <summary>
        /// Çoklu değer satırlarının (MultipleSelect, MultipleImage, Tag) sırası.
        /// </summary>
        public int SortOrder { get; set; } = 0;

        // -------------------- KAYNAK / İZLENEBİLİRLİK --------------------

        /// <summary>
        /// Değerin kaynağı: "Manual", "Import", "Ai", "System".
        /// </summary>
        public string? Source { get; set; } = "Manual";

        /// <summary>
        /// Import sırasında dış platformdan gelen ham değer. Eşleştirme/normalizasyon
        /// hatalarında geri dönüş ve denetim için saklanır. Hiçbir platform verisi kaybolmaz.
        /// </summary>
        public string? RawImportValue { get; set; }

        /// <summary>
        /// Satıcı veya admin bu değeri elle düzenledi mi? true ise AI ve Import
        /// bu değerin üzerine asla yazamaz. (IsManuallyEdited koruma deseni)
        /// </summary>
        public bool IsManuallyEdited { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
