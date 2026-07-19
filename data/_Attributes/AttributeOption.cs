using data.Owned;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Attributes
{
    /// <summary>
    /// Dropdown, MultipleSelect, RadioButton, Checkbox, Tag ve TreeSelector
    /// tipli attribute'ların seçilebilir değerlerini tutar.
    /// Değerler merkezi ve çok dillidir; satıcı serbest metin değil,
    /// bu tablodaki normalize seçeneklerden birini seçer. Bu sayede
    /// faceted filter'lar ("Renk: Kırmızı (1.245 ürün)") tutarlı çalışır.
    /// </summary>
    public class AttributeOption
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Bağlı olduğu attribute ID'si (AttributeDefinition tablosu).
        /// </summary>
        public Guid AttributeId { get; set; }

        /// <summary>
        /// TreeSelector tipi için üst seçenek ID'si. Düz listelerde null.
        /// Örn: "Ayakkabı Numarası > EU > 42"
        /// </summary>
        public Guid? ParentOptionId { get; set; }

        /// <summary>
        /// Dilden bağımsız kanonik değer kodu. Örn: "red", "xl", "8gb".
        /// Import eşleştirme, ElasticSearch keyword ve URL filtrelerinde bu kod kullanılır.
        /// Aynı attribute içinde unique olmalıdır.
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Seçeneğin 10 dildeki görünen adı.
        /// </summary>
        public LangText? Name { get; set; } = new();

        /// <summary>
        /// Renk tipli attribute'larda swatch olarak gösterilecek HEX kodu. Örn: "#FF0000"
        /// </summary>
        public string? HexColor { get; set; }

        /// <summary>
        /// Seçeneğe ait görsel ID'si (Media tablosu). Örn: kumaş deseni, desen görseli.
        /// </summary>
        public Guid? MediaId { get; set; }

        /// <summary>
        /// Sayısal karşılık. Örn: beden "42" → 42. Sıralı filtrelerde
        /// (numara, kapasite) alfabetik değil sayısal sıralama için kullanılır.
        /// </summary>
        public decimal? NumericValue { get; set; }

        /// <summary>
        /// Seçeneklerin gösterim sırası.
        /// </summary>
        public int SortOrder { get; set; } = 0;

        /// <summary>Aktif / Pasif. Pasif seçenek yeni ürünlerde seçilemez, eski değerler korunur.</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Bu seçenek AI tarafından mı oluşturuldu?
        /// </summary>
        public bool? CreatedByAi { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
