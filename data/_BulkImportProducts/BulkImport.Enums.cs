using System;

namespace data._BulkImportProducts
{
    // ════════════════════════════════════════════════════════════════════════
    //  efavori — Toplu Ürün İçe Aktarım Sistemi (Bulk Import V1)
    //  Enum kataloğu — dilden bağımsız, tinyint (byte) olarak saklanan sabitler.
    //  Not: Enum değerlerine ASLA arada değer eklemeyin; yalnızca SONA ekleyin.
    //
    //  Platform (Amazon/Trendyol/WooCommerce/CSV...) mevcut data._Attribute.
    //  IntegrationPlatform referans TABLOSUNDAN gelir; enum olarak sabitlenmez —
    //  yeni pazaryeri/format yayın gerektirmeden eklenebilsin diye.
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Kaynağın TEKNİK türü — verinin nasıl alınacağını belirler.
    /// (Hangi pazaryeri olduğu ayrı bir referanstır; bu yalnızca aktarım kanalıdır.)
    /// </summary>
    public enum ImportSourceType : byte
    {
        /// <summary>Kullanıcının yüklediği CSV dosyası (ör. WooCommerce/OpenCart dışa aktarımı).</summary>
        CsvFile = 1,
        /// <summary>Kullanıcının yüklediği Excel dosyası (XLSX/XLS).</summary>
        ExcelFile = 2,
        /// <summary>Kullanıcının yüklediği XML dosyası.</summary>
        XmlFile = 3,
        /// <summary>Kullanıcının yüklediği JSON dosyası.</summary>
        JsonFile = 4,
        /// <summary>Uzak XML/RSS ürün beslemesi (feed URL'i).</summary>
        XmlFeedUrl = 5,
        /// <summary>Uzak JSON beslemesi (feed URL'i).</summary>
        JsonFeedUrl = 6,
        /// <summary>Uzak CSV beslemesi (feed URL'i).</summary>
        CsvFeedUrl = 7,
        /// <summary>Pazaryeri/uygulama REST API'si (kimlik doğrulamalı).</summary>
        RestApi = 8,
        /// <summary>GraphQL API.</summary>
        GraphQlApi = 9,
        /// <summary>Sayfa kazıma (scraping) — tek ürün URL'i veya URL listesi.</summary>
        WebScrape = 10
    }

    /// <summary>Kimlik doğrulama yöntemi (API tabanlı kaynaklar için).</summary>
    public enum ImportAuthType : byte
    {
        /// <summary>Kimlik doğrulama yok (public feed/dosya).</summary>
        None = 1,
        /// <summary>API anahtarı (header veya query).</summary>
        ApiKey = 2,
        /// <summary>API anahtarı + gizli anahtar (key + secret; ör. Amazon SP-API, eBay).</summary>
        ApiKeySecret = 3,
        /// <summary>Bearer token.</summary>
        BearerToken = 4,
        /// <summary>OAuth 2.0 (access + refresh token).</summary>
        OAuth2 = 5,
        /// <summary>Temel yetkilendirme (kullanıcı adı + şifre).</summary>
        BasicAuth = 6,
        /// <summary>İmzalı istek (HMAC — ör. AliExpress/Taobao açık platform).</summary>
        SignedRequest = 7
    }

    /// <summary>İçe aktarımın kayıtları nasıl işleyeceği (oluştur/güncelle davranışı).</summary>
    public enum ImportMode : byte
    {
        /// <summary>Yalnızca yeni ürün oluştur; eşleşen varsa atla.</summary>
        CreateOnly = 1,
        /// <summary>Yalnızca mevcut ürünleri güncelle; eşleşme yoksa atla.</summary>
        UpdateOnly = 2,
        /// <summary>Varsa güncelle, yoksa oluştur (upsert).</summary>
        CreateOrUpdate = 3
    }

    /// <summary>
    /// Aynı ürün ikinci kez geldiğinde (eşleşme anahtarına göre) uygulanacak strateji.
    /// Eşleşme anahtarı için <see cref="ImportMatchKey"/> kullanılır.
    /// </summary>
    public enum DuplicateStrategy : byte
    {
        /// <summary>Yinelemeyi atla (mevcut kayda dokunma).</summary>
        Skip = 1,
        /// <summary>Mevcut kaydın üzerine tümüyle yaz.</summary>
        Overwrite = 2,
        /// <summary>Yalnızca boş/eksik alanları doldur (mevcut değerleri koru).</summary>
        FillEmptyOnly = 3,
        /// <summary>Her zaman yeni kayıt oluştur (yinelemeye izin ver).</summary>
        CreateNew = 4
    }

    /// <summary>Yineleme tespitinde kullanılacak eşleşme anahtarı.</summary>
    public enum ImportMatchKey : byte
    {
        /// <summary>Barkod/GTIN (EAN/UPC) ile eşleştir.</summary>
        Gtin = 1,
        /// <summary>SKU ile eşleştir.</summary>
        Sku = 2,
        /// <summary>Üretici parça numarası (MPN) ile eşleştir.</summary>
        Mpn = 3,
        /// <summary>Kaynak platformdaki dış ürün kimliği ile eşleştir.</summary>
        ExternalId = 4,
        /// <summary>Ürün adı (normalize) ile eşleştir — en zayıf, dikkatli kullanılmalı.</summary>
        NormalizedName = 5
    }

    /// <summary>İçe aktarım işinin (bir çalıştırma) yaşam döngüsü durumu.</summary>
    public enum ImportJobStatus : byte
    {
        /// <summary>Oluşturuldu, işlenmeyi bekliyor.</summary>
        Pending = 1,
        /// <summary>Bir worker tarafından kiralandı (çift işleme karşı).</summary>
        Leased = 2,
        /// <summary>Kaynak veri alınıyor (indirme/çekme).</summary>
        Fetching = 3,
        /// <summary>Satırlar ayrıştırılıyor (parse).</summary>
        Parsing = 4,
        /// <summary>Kullanıcının eşleştirme yapması bekleniyor (sütun/kategori mapping).</summary>
        AwaitingMapping = 5,
        /// <summary>Satırlar ürünlere dönüştürülüyor (import).</summary>
        Importing = 6,
        /// <summary>Tümü başarıyla tamamlandı.</summary>
        Completed = 7,
        /// <summary>Kısmen tamamlandı (bazı satırlar başarısız/atlandı).</summary>
        PartiallyCompleted = 8,
        /// <summary>Tümüyle başarısız.</summary>
        Failed = 9,
        /// <summary>Kullanıcı tarafından iptal edildi.</summary>
        Cancelled = 10
    }

    /// <summary>Tek bir içe aktarım satırının (staging kaydı) durumu.</summary>
    public enum ImportRowStatus : byte
    {
        /// <summary>Ayrıştırıldı, işlenmeyi bekliyor.</summary>
        Pending = 1,
        /// <summary>Alanlar eşlendi, aktarıma hazır.</summary>
        Mapped = 2,
        /// <summary>Ürün başarıyla oluşturuldu.</summary>
        Created = 3,
        /// <summary>Mevcut ürün güncellendi.</summary>
        Updated = 4,
        /// <summary>Yineleme olduğu için atlandı.</summary>
        SkippedDuplicate = 5,
        /// <summary>Geçersiz/eksik veri nedeniyle atlandı.</summary>
        SkippedInvalid = 6,
        /// <summary>Hata ile sonuçlandı (ErrorMessage dolu).</summary>
        Failed = 7,
        /// <summary>İnceleme gerekiyor (belirsiz eşleştirme/düşük güven).</summary>
        NeedsReview = 8
    }

    /// <summary>Bir alan eşleştirmesinin hedeflediği efavori ürün alanı türü.</summary>
    public enum ImportTargetField : byte
    {
        /// <summary>Ürün adı (çeviri).</summary>
        Name = 1,
        /// <summary>Kısa açıklama.</summary>
        ShortDescription = 2,
        /// <summary>Detaylı açıklama (HTML).</summary>
        Description = 3,
        /// <summary>Marka.</summary>
        Brand = 4,
        /// <summary>Kategori (yol/kod).</summary>
        Category = 5,
        /// <summary>SKU.</summary>
        Sku = 6,
        /// <summary>Barkod/GTIN.</summary>
        Gtin = 7,
        /// <summary>MPN.</summary>
        Mpn = 8,
        /// <summary>Fiyat.</summary>
        Price = 9,
        /// <summary>Liste (üstü çizili) fiyat.</summary>
        ListPrice = 10,
        /// <summary>Para birimi.</summary>
        Currency = 11,
        /// <summary>Stok adedi.</summary>
        StockQuantity = 12,
        /// <summary>Ağırlık.</summary>
        Weight = 13,
        /// <summary>Boyut (en/boy/yükseklik).</summary>
        Dimension = 14,
        /// <summary>Görsel URL'i.</summary>
        ImageUrl = 15,
        /// <summary>Varyant ekseni (renk/beden vb.).</summary>
        VariantAxis = 16,
        /// <summary>Teknik özellik (attribute) değeri.</summary>
        Attribute = 17,
        /// <summary>Dış ürün kimliği.</summary>
        ExternalId = 18,
        /// <summary>SEO slug.</summary>
        Slug = 19,
        /// <summary>Menşei ülke.</summary>
        CountryOfOrigin = 20,
        /// <summary>KDV oranı.</summary>
        VatRate = 21,
        /// <summary>Yok say (bu sütun/alan içe alınmaz).</summary>
        Ignore = 100
    }

    /// <summary>
    /// Bir alan değerine uygulanacak dönüşüm türü (ör. birim ayıklama, ondalık ayıracı,
    /// HTML temizleme). Ayrıntı yapılandırması TransformConfigJson içinde tutulur.
    /// </summary>
    public enum ImportTransformType : byte
    {
        /// <summary>Dönüşüm yok (ham değer).</summary>
        None = 1,
        /// <summary>Baş/son boşlukları kırp.</summary>
        Trim = 2,
        /// <summary>Ondalık ayıracını normalize et (virgül → nokta).</summary>
        NormalizeDecimal = 3,
        /// <summary>Para birimi sembolünü/kodunu ayıkla.</summary>
        StripCurrency = 4,
        /// <summary>Birim ekini ayıkla (ör. "16 GB" → "16").</summary>
        StripUnit = 5,
        /// <summary>HTML etiketlerini temizle.</summary>
        StripHtml = 6,
        /// <summary>Sabit çarpan uygula (ör. fiyata kâr marjı).</summary>
        Multiply = 7,
        /// <summary>Regex bul/değiştir.</summary>
        RegexReplace = 8,
        /// <summary>Değer eşlemesi uygula (ör. "red" → iç "black" option'ı; JSON sözlük).</summary>
        ValueMap = 9,
        /// <summary>Ayırıcıya göre böl (ör. çoklu görsel/kategori "|" ile ayrılmış).</summary>
        Split = 10
    }

    /// <summary>Kimlik/kredensiyel kaydının doğrulama durumu.</summary>
    public enum CredentialStatus : byte
    {
        /// <summary>Henüz test edilmedi.</summary>
        Untested = 1,
        /// <summary>Test edildi, geçerli.</summary>
        Valid = 2,
        /// <summary>Test edildi, geçersiz (kimlik hatası).</summary>
        Invalid = 3,
        /// <summary>Token süresi doldu (yenileme gerekli).</summary>
        Expired = 4
    }
}
