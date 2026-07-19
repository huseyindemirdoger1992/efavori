using data.Owned;
using System;
using System.ComponentModel.DataAnnotations;

namespace data._Attributes
{
    /// <summary>
    /// Ölçü birimi kütüphanesi. Weight, Length, Volume, Area, Data, Power gibi
    /// birim gruplarını ve grup içi dönüşüm katsayılarını tutar.
    /// Satıcı "2.5 kg" girdiğinde değer base birime (gr → 2500) çevrilerek
    /// AttributeValue.ValueNumber'a yazılır; böylece farklı birimlerle girilen
    /// değerler filtre ve sıralamada tutarlı karşılaştırılır.
    /// </summary>
    public class AttributeUnit
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Birim kodu. Örn: "gr", "kg", "cm", "inch", "mah", "w", "gb".
        /// Sistem genelinde unique olmalıdır.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Birim grubu. Örn: "Weight", "Length", "Volume", "Area",
        /// "Data", "Power", "Frequency". Aynı gruptaki birimler birbirine çevrilebilir.
        /// </summary>
        public string UnitGroup { get; set; } = string.Empty;

        /// <summary>
        /// Birimin 10 dildeki görünen adı. Örn: Tr "Gram", En "Gram", Ru "Грамм".
        /// </summary>
        public LangText? Name { get; set; } = new();

        /// <summary>
        /// Değerin yanında gösterilecek kısa sembol. Örn: "g", "cm", "mAh".
        /// </summary>
        public string? Symbol { get; set; }

        /// <summary>
        /// Bu birim, grubunun base birimi mi? Her grupta yalnızca bir base birim olur.
        /// </summary>
        public bool IsBase { get; set; } = false;

        /// <summary>
        /// Base birime çevrim katsayısı. Değer × katsayı = base değer.
        /// Örn: kg → 1000 (base: gr), inch → 2.54 (base: cm), base birimde 1.
        /// </summary>
        public decimal ConversionFactorToBase { get; set; } = 1;

        /// <summary>
        /// Birim seçim listelerindeki gösterim sırası.
        /// </summary>
        public int SortOrder { get; set; } = 0;

        /// <summary>Aktif / Pasif</summary>
        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        public IsDeleted? IsDeleted { get; set; } = new();
    }
}
