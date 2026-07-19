using System;

namespace data._Attribute
{
    /// <summary>
    /// Birim grubu / ölçüm ailesi (ör. Ağırlık, Uzunluk, Veri, Bellek, Ekran).
    /// Aynı gruptaki tüm birimler ortak bir baz birime dönüştürülebilir; böylece
    /// normalize edilmiş değer saklanıp doğru karşılaştırma/filtreleme yapılır.
    /// </summary>
    public class UnitGroup : AttributeEntityBase
    {
        /// <summary>Dilden bağımsız tekil kod (ör. "weight", "data_storage").</summary>
        public string CanonicalCode { get; set; } = string.Empty;

        /// <summary>Varsayılan (Türkçe) görünen ad.</summary>
        public string CanonicalName { get; set; } = string.Empty;

        /// <summary>Fiziksel boyut / ölçüm ailesi.</summary>
        public UnitDimension Dimension { get; set; } = UnitDimension.Custom;

        /// <summary>
        /// Grubun baz/temel birimi (normalize saklama için). Ör: Ağırlık → gram.
        /// (Dairesel FK olduğundan silme davranışı NoAction'dır.)
        /// </summary>
        public Guid? BaseUnitId { get; set; }

        /// <summary>Sıralama önceliği.</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Grup aktif mi?</summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary><see cref="UnitGroup"/> için dile özgü metinler.</summary>
    public class UnitGroupTranslation : AttributeEntityBase
    {
        /// <summary>Bağlı birim grubu.</summary>
        public Guid UnitGroupId { get; set; }

        /// <summary>Dil.</summary>
        public Language Language { get; set; }

        /// <summary>Bu dildeki grup adı.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Kaynak.</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;

        /// <summary>Manuel düzenleme kilidi.</summary>
        public bool IsManuallyEdited { get; set; }
    }

    /// <summary>
    /// Tek bir ölçü birimi (ör. kg, g, mm, cm, GB, MB, °C).
    /// Baz birime dönüşüm için çarpan ve (gerekirse) ofset taşır.
    /// </summary>
    public class Unit : AttributeEntityBase
    {
        /// <summary>Bağlı birim grubu.</summary>
        public Guid UnitGroupId { get; set; }

        /// <summary>Grup içinde tekil kod (ör. "kg", "gb").</summary>
        public string CanonicalCode { get; set; } = string.Empty;

        /// <summary>Görünen sembol (ör. "kg", "GB", "cm").</summary>
        public string Symbol { get; set; } = string.Empty;

        /// <summary>Varsayılan (Türkçe) görünen ad (ör. "Kilogram").</summary>
        public string CanonicalName { get; set; } = string.Empty;

        /// <summary>
        /// Baz birime çevirme çarpanı. Normalize değer = (değer * Çarpan) + Ofset.
        /// Ör: gram baz iken kg için çarpan 1000.
        /// </summary>
        public decimal ConversionFactorToBase { get; set; } = 1m;

        /// <summary>
        /// Baz birime çevirmede ofset (çoğunlukla 0; sıcaklık gibi durumlarda gerekir).
        /// </summary>
        public decimal ConversionOffset { get; set; } = 0m;

        /// <summary>Bu birim grubun baz birimi mi?</summary>
        public bool IsBaseUnit { get; set; }

        /// <summary>Sıralama önceliği.</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Birim aktif mi?</summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary><see cref="Unit"/> için dile özgü metinler.</summary>
    public class UnitTranslation : AttributeEntityBase
    {
        /// <summary>Bağlı birim.</summary>
        public Guid UnitId { get; set; }

        /// <summary>Dil.</summary>
        public Language Language { get; set; }

        /// <summary>Bu dildeki birim adı (tekil, ör. "Kilogram").</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Bu dildeki çoğul ad (opsiyonel, ör. "Kilograms").</summary>
        public string? PluralName { get; set; }

        /// <summary>Kaynak.</summary>
        public ContentSource Source { get; set; } = ContentSource.Manual;

        /// <summary>Manuel düzenleme kilidi.</summary>
        public bool IsManuallyEdited { get; set; }
    }
}
