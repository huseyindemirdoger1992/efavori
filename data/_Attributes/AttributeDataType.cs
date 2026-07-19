using System;

namespace data._Attributes
{
    /// <summary>
    /// Bir attribute'ın veri tipini belirtir.
    /// Veritabanında int olarak saklanır. Yeni tip eklerken mevcut sayısal
    /// değerler ASLA değiştirilmez, sadece sona yeni değer eklenir.
    /// AttributeValue tablosundaki hangi tipli kolonun (ValueText, ValueNumber,
    /// ValueDate, ValueBool, ValueJson, MediaId, OptionId) kullanılacağını bu tip belirler.
    /// </summary>
    public enum AttributeDataType
    {
        // --- Metin Tipleri ---
        Text = 0,
        LongText = 1,
        RichText = 2,

        // --- Sayısal Tipler ---
        Integer = 10,
        Decimal = 11,
        Money = 12,
        Percentage = 13,

        // --- Mantıksal ---
        Boolean = 20,

        // --- Tarih / Zaman ---
        Date = 30,
        DateTime = 31,
        Time = 32,

        // --- Renk ---
        Color = 40,
        HexColor = 41,
        Rgb = 42,

        // --- Medya (MediaId üzerinden Media tablosuna bağlanır) ---
        Image = 50,
        MultipleImage = 51,
        Video = 52,
        File = 53,

        // --- İletişim / Bağlantı ---
        Url = 60,
        Email = 61,
        Phone = 62,

        // --- Seçim Tipleri (AttributeOption tablosunu kullanır) ---
        Dropdown = 70,
        MultipleSelect = 71,
        RadioButton = 72,
        Checkbox = 73,
        Tag = 74,
        TreeSelector = 75,
        DynamicList = 76,
        ApiBasedList = 77,

        // --- Yapısal ---
        Json = 80,
        KeyValue = 81,
        Range = 82,
        Slider = 83,

        // --- Ölçü Birimleri (AttributeUnit ile birlikte kullanılır) ---
        Weight = 90,
        Length = 91,
        Width = 92,
        Height = 93,
        Volume = 94,
        Area = 95,
        Dimension = 96,

        // --- Konum ---
        Gps = 100,

        // --- Kod / Kimlik ---
        Barcode = 110,
        QrCode = 111,
        Ean = 112,
        Upc = 113,
        Isbn = 114,
        Sku = 115,

        // --- Referans Tipleri (ValueText içine ilgili Guid/kod yazılır) ---
        BrandReference = 120,
        CategoryReference = 121,
        Country = 122,
        Currency = 123,
        Language = 124,
        Unit = 125
    }
}
