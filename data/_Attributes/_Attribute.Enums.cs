using System;

namespace data._Attribute
{
    // ════════════════════════════════════════════════════════════════════════
    //  efavori — Enterprise Marketplace Attribute System V3
    //  Enum kataloğu — dilden bağımsız, tinyint olarak saklanacak sabit değerler
    //  Not: Enum değerlerine ASLA arada değer eklemeyin; yalnızca sona ekleyin.
    //       Aksi halde veritabanındaki mevcut kayıtların anlamı değişir.
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Sistemin desteklediği 10 dil. Görünen tüm metinler için çeviri satırlarında
    /// bu değer saklanır. Kanonik (varsayılan) kaynak dil Türkçe'dir (Tr).
    /// </summary>
    public enum Language : byte
    {
        /// <summary>Türkçe — kanonik / kaynak dil.</summary>
        Tr = 1,
        /// <summary>İngilizce.</summary>
        En = 2,
        /// <summary>Azerbaycanca.</summary>
        Az = 3,
        /// <summary>Almanca.</summary>
        De = 4,
        /// <summary>İspanyolca.</summary>
        Es = 5,
        /// <summary>Fransızca.</summary>
        Fr = 6,
        /// <summary>Hintçe.</summary>
        Hi = 7,
        /// <summary>Portekizce.</summary>
        Pt = 8,
        /// <summary>Rusça.</summary>
        Ru = 9,
        /// <summary>Çince.</summary>
        Zh = 10
    }

    /// <summary>
    /// Attribute'ın veri tipi. Değerin nasıl saklanacağını, doğrulanacağını ve
    /// arama/filtreleme davranışını belirler. UI widget'ından bağımsızdır.
    /// </summary>
    public enum AttributeDataType : byte
    {
        /// <summary>Kısa serbest metin (ör. Model kodu).</summary>
        Text = 1,
        /// <summary>Çok satırlı serbest metin.</summary>
        MultilineText = 2,
        /// <summary>Zengin metin (HTML).</summary>
        RichText = 3,
        /// <summary>Tam sayı.</summary>
        Integer = 4,
        /// <summary>Ondalık sayı.</summary>
        Decimal = 5,
        /// <summary>Evet/Hayır.</summary>
        Boolean = 6,
        /// <summary>Tarih.</summary>
        Date = 7,
        /// <summary>Tarih + saat.</summary>
        DateTime = 8,
        /// <summary>Tek seçimli option (ör. İşletim Sistemi).</summary>
        SingleSelect = 9,
        /// <summary>Çok seçimli option (ör. Bağlantı: Wifi + Bluetooth + NFC).</summary>
        MultiSelect = 10,
        /// <summary>Ölçü değeri (sayı + birim, ör. 15.6 inç).</summary>
        Measurement = 11,
        /// <summary>Renk (hex + option).</summary>
        Color = 12,
        /// <summary>URL.</summary>
        Url = 13,
        /// <summary>E-posta.</summary>
        Email = 14,
        /// <summary>Yapılandırılmış JSON.</summary>
        Json = 15
    }

    /// <summary>
    /// Attribute'ın admin/satıcı arayüzünde hangi giriş bileşeni ile gösterileceği.
    /// Semantik <see cref="AttributeDataType"/>'dan ayrıdır (aynı tip farklı widget'larla sunulabilir).
    /// </summary>
    public enum AttributeInputType : byte
    {
        /// <summary>Tek satır metin kutusu.</summary>
        TextBox = 1,
        /// <summary>Çok satırlı metin alanı.</summary>
        TextArea = 2,
        /// <summary>Zengin metin editörü.</summary>
        RichTextEditor = 3,
        /// <summary>Sayı kutusu.</summary>
        NumberBox = 4,
        /// <summary>Açılır liste (tek seçim).</summary>
        Dropdown = 5,
        /// <summary>Radyo düğmeleri (tek seçim).</summary>
        RadioGroup = 6,
        /// <summary>Onay kutusu listesi (çok seçim).</summary>
        CheckboxList = 7,
        /// <summary>Çok seçimli açılır liste.</summary>
        MultiSelectDropdown = 8,
        /// <summary>Aç/Kapa anahtarı (boolean).</summary>
        Toggle = 9,
        /// <summary>Tarih seçici.</summary>
        DatePicker = 10,
        /// <summary>Tarih + saat seçici.</summary>
        DateTimePicker = 11,
        /// <summary>Renk seçici.</summary>
        ColorPicker = 12,
        /// <summary>Ölçü kutusu (değer + birim seçimi).</summary>
        MeasurementBox = 13,
        /// <summary>URL kutusu.</summary>
        UrlBox = 14
    }

    /// <summary>
    /// Bir kaydın hangi kaynaktan üretildiğini belirtir. Manuel düzenleme koruması
    /// ve izlenebilirlik için kullanılır.
    /// </summary>
    public enum ContentSource : byte
    {
        /// <summary>Sistem tarafından (seed/migrasyon) oluşturuldu.</summary>
        System = 1,
        /// <summary>Admin tarafından manuel oluşturuldu.</summary>
        Manual = 2,
        /// <summary>Yapay zekâ tarafından üretildi.</summary>
        Ai = 3,
        /// <summary>Dış platform aktarımından (import) geldi.</summary>
        Import = 4
    }

    /// <summary>Yapay zekâ üretim işinin türü (BackgroundService ayrımı).</summary>
    public enum AiJobType : byte
    {
        /// <summary>SERVICE 1 — kategori yolunu analiz edip Attribute üretir.</summary>
        CategoryAttributeAnalysis = 1,
        /// <summary>SERVICE 2 — üretilmiş Attribute'lar için Option listesi üretir.</summary>
        OptionGeneration = 2,
        /// <summary>Attribute/Option için çok dilli çeviri üretir.</summary>
        TranslationGeneration = 3,
        /// <summary>Attribute/Option için alias & synonym (eşanlamlı) üretir.</summary>
        SynonymGeneration = 4,
        /// <summary>Kategoriye uygun Template eşleştirmesi/önerisi üretir.</summary>
        TemplateSuggestion = 5
    }

    /// <summary>Yapay zekâ üretim işinin yaşam döngüsü durumu.</summary>
    public enum AiJobStatus : byte
    {
        /// <summary>Kuyrukta, işlenmeyi bekliyor.</summary>
        Pending = 1,
        /// <summary>Bir worker tarafından kiralandı (lease) — çift işleme karşı.</summary>
        Leased = 2,
        /// <summary>İşleniyor.</summary>
        Processing = 3,
        /// <summary>Başarıyla tamamlandı.</summary>
        Completed = 4,
        /// <summary>Hata ile sonuçlandı (yeniden denenebilir).</summary>
        Failed = 5,
        /// <summary>Tamamlandı ancak admin onayı bekliyor.</summary>
        NeedsReview = 6,
        /// <summary>İptal edildi.</summary>
        Cancelled = 7,
        /// <summary>Zaten mevcut olduğu için atlandı (idempotent).</summary>
        Skipped = 8
    }

    /// <summary>Yapay zekâ üretilen kaydın onay durumu.</summary>
    public enum AiApprovalStatus : byte
    {
        /// <summary>Onay gerekmiyor (ör. manuel kayıt).</summary>
        NotRequired = 1,
        /// <summary>Onay bekliyor.</summary>
        PendingApproval = 2,
        /// <summary>Admin tarafından onaylandı.</summary>
        Approved = 3,
        /// <summary>Admin tarafından reddedildi.</summary>
        Rejected = 4
    }

    /// <summary>Yapay zekâ üretilen kaydın inceleme durumu.</summary>
    public enum AiReviewStatus : byte
    {
        /// <summary>Henüz incelenmedi.</summary>
        NotReviewed = 1,
        /// <summary>İnceleme sürecinde.</summary>
        InReview = 2,
        /// <summary>İncelendi.</summary>
        Reviewed = 3
    }

    /// <summary>Attribute bağımlılığında (dependency) koşul karşılaştırma operatörü.</summary>
    public enum DependencyOperator : byte
    {
        /// <summary>Eşittir.</summary>
        Equals = 1,
        /// <summary>Eşit değildir.</summary>
        NotEquals = 2,
        /// <summary>Listedekilerden biri.</summary>
        In = 3,
        /// <summary>Listedekilerden hiçbiri.</summary>
        NotIn = 4,
        /// <summary>Büyüktür.</summary>
        GreaterThan = 5,
        /// <summary>Büyük veya eşittir.</summary>
        GreaterOrEqual = 6,
        /// <summary>Küçüktür.</summary>
        LessThan = 7,
        /// <summary>Küçük veya eşittir.</summary>
        LessOrEqual = 8,
        /// <summary>Herhangi bir değere sahip (dolu).</summary>
        IsSet = 9,
        /// <summary>Boş (değeri yok).</summary>
        IsNotSet = 10,
        /// <summary>Metin içerir.</summary>
        Contains = 11
    }

    /// <summary>Bağımlılık koşulu sağlandığında hedef attribute'a uygulanacak eylem.</summary>
    public enum DependencyAction : byte
    {
        /// <summary>Hedef attribute'ı göster.</summary>
        Show = 1,
        /// <summary>Hedef attribute'ı gizle.</summary>
        Hide = 2,
        /// <summary>Hedef attribute'ı zorunlu yap.</summary>
        Require = 3,
        /// <summary>Hedef attribute'ı opsiyonel yap.</summary>
        MakeOptional = 4,
        /// <summary>Hedef attribute'ı etkinleştir.</summary>
        Enable = 5,
        /// <summary>Hedef attribute'ı devre dışı bırak.</summary>
        Disable = 6
    }

    /// <summary>Aynı koşul grubundaki bağımlılık koşullarının mantıksal birleşimi.</summary>
    public enum DependencyLogic : byte
    {
        /// <summary>Gruptaki tüm koşullar sağlanmalı.</summary>
        And = 1,
        /// <summary>Gruptaki koşullardan biri sağlanmalı.</summary>
        Or = 2
    }

    /// <summary>Dış platform eşleştirmesinin yönü (import / export).</summary>
    public enum MappingDirection : byte
    {
        /// <summary>Yalnızca içeri aktarım (dış → efavori).</summary>
        Import = 1,
        /// <summary>Yalnızca dışa aktarım (efavori → dış).</summary>
        Export = 2,
        /// <summary>Her iki yön.</summary>
        Bidirectional = 3
    }

    /// <summary>Bir attribute'ın dış platform açısından zorunluluk seviyesi.</summary>
    public enum PlatformRequirementLevel : byte
    {
        /// <summary>Opsiyonel.</summary>
        Optional = 1,
        /// <summary>Önerilen.</summary>
        Recommended = 2,
        /// <summary>Zorunlu (aktarım için gerekli).</summary>
        Required = 3
    }

    /// <summary>Birim grubunun fiziksel boyutu / ölçüm ailesi.</summary>
    public enum UnitDimension : byte
    {
        /// <summary>Ağırlık (kg, g...).</summary>
        Weight = 1,
        /// <summary>Uzunluk (m, cm, mm...).</summary>
        Length = 2,
        /// <summary>Alan (m², cm²...).</summary>
        Area = 3,
        /// <summary>Hacim (l, ml...).</summary>
        Volume = 4,
        /// <summary>Veri depolama (GB, MB, TB...).</summary>
        DataStorage = 5,
        /// <summary>Güç (W, kW...).</summary>
        Power = 6,
        /// <summary>Frekans (Hz, GHz...).</summary>
        Frequency = 7,
        /// <summary>Sıcaklık (°C, K, °F...).</summary>
        Temperature = 8,
        /// <summary>Basınç (bar, Pa...).</summary>
        Pressure = 9,
        /// <summary>Bellek boyutu (RAM: GB...).</summary>
        MemorySize = 10,
        /// <summary>Ekran boyutu (inç, cm).</summary>
        ScreenSize = 11,
        /// <summary>Zaman/süre (sa, dk, sn).</summary>
        Time = 12,
        /// <summary>Hız (km/h...).</summary>
        Speed = 13,
        /// <summary>Gerilim (V).</summary>
        Voltage = 14,
        /// <summary>Enerji/kapasite (mAh, Wh...).</summary>
        Energy = 15,
        /// <summary>Adet/sayı.</summary>
        Count = 16,
        /// <summary>Özel/tanımsız.</summary>
        Custom = 100
    }

    /// <summary>
    /// Normalizasyon/transform kuralının türü. Import ve dedup sırasında metni
    /// kanonik hâle getirmek için kullanılır.
    /// </summary>
    public enum NormalizationRuleType : byte
    {
        /// <summary>Küçük harfe çevir.</summary>
        Lowercase = 1,
        /// <summary>Baş/son boşlukları kırp.</summary>
        TrimWhitespace = 2,
        /// <summary>İç boşlukları tek boşluğa indir.</summary>
        CollapseWhitespace = 3,
        /// <summary>Diyakritikleri kaldır (ör. ş→s, ç→c).</summary>
        RemoveDiacritics = 4,
        /// <summary>Noktalama işaretlerini kaldır.</summary>
        RemovePunctuation = 5,
        /// <summary>Regex ile bul/değiştir.</summary>
        RegexReplace = 6,
        /// <summary>Birim ekini ayır (ör. "16GB" → "16").</summary>
        StripUnit = 7,
        /// <summary>Değeri sabit bir eşlemeye göre çevir (ör. "grey" → "gray").</summary>
        MapValue = 8,
        /// <summary>Özel/kod tarafından işlenen kural.</summary>
        Custom = 100
    }

    /// <summary><see cref="Language"/> enum'ı için kültür kodu yardımcıları.</summary>
    public static class LanguageExtensions
    {
        /// <summary>İki harfli ISO kültür kodunu döndürür (ör. Language.Tr → "tr").</summary>
        public static string ToCultureCode(this Language language) => language switch
        {
            Language.Tr => "tr",
            Language.En => "en",
            Language.Az => "az",
            Language.De => "de",
            Language.Es => "es",
            Language.Fr => "fr",
            Language.Hi => "hi",
            Language.Pt => "pt",
            Language.Ru => "ru",
            Language.Zh => "zh",
            _ => "tr"
        };

        /// <summary>Kültür kodundan <see cref="Language"/> çözer; bilinmiyorsa Tr döner.</summary>
        public static Language FromCultureCode(string? code) => (code?.Trim().ToLowerInvariant()) switch
        {
            "tr" => Language.Tr,
            "en" => Language.En,
            "az" => Language.Az,
            "de" => Language.De,
            "es" => Language.Es,
            "fr" => Language.Fr,
            "hi" => Language.Hi,
            "pt" => Language.Pt,
            "ru" => Language.Ru,
            "zh" => Language.Zh,
            _ => Language.Tr
        };
    }
}
