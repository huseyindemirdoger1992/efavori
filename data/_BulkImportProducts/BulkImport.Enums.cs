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
        Cancelled = 10,

        // ── Yorum içe aktarımı için eklenen durumlar ────────────────────────
        /// <summary>Ürünler tamamlandı, yorumlar içe aktarılıyor.</summary>
        ImportingReviews = 11,
        /// <summary>Yorum medyaları (fotoğraf/video) indiriliyor.</summary>
        DownloadingReviewMedia = 12
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

        // ── Yorum/Puanlama İçe Aktarım Alanları (Review Import V1) ──────────
        /// <summary>Yorum kök yolu (JSON/XML'de yorumların bulunduğu dizi yolu).</summary>
        ReviewRootPath = 22,
        /// <summary>Yorum dış kimliği (kaynak platformdaki review id).</summary>
        ReviewExternalId = 23,
        /// <summary>Yorum yıldız puanı (1..5 veya platformun ölçeğinde).</summary>
        ReviewRating = 24,
        /// <summary>Yorum başlığı.</summary>
        ReviewTitle = 25,
        /// <summary>Yorum metni (gövde).</summary>
        ReviewBody = 26,
        /// <summary>Yorum yazarı adı/takma adı.</summary>
        ReviewAuthorName = 27,
        /// <summary>Yorum yazarı dış kimliği (kaynak customer id).</summary>
        ReviewAuthorExternalId = 28,
        /// <summary>Yorum tarihi.</summary>
        ReviewDate = 29,
        /// <summary>Yorum doğrulanmış satın alma bayrağı.</summary>
        ReviewIsVerifiedPurchase = 30,
        /// <summary>Yorum medya URL'si (fotoğraf/video).</summary>
        ReviewMediaUrl = 31,
        /// <summary>Yorum faydalı oy sayısı.</summary>
        ReviewHelpfulVoteCount = 32,
        /// <summary>Yorum toplam oy sayısı.</summary>
        ReviewTotalVoteCount = 33,
        /// <summary>Yorum kalite alt puanı.</summary>
        ReviewQualityRating = 34,
        /// <summary>Yorum kargo alt puanı.</summary>
        ReviewShippingRating = 35,
        /// <summary>Yorum fiyat/performans alt puanı.</summary>
        ReviewValueRating = 36,
        /// <summary>Yorumun hangi varyanta ait olduğu (kaynak varyant etiketi).</summary>
        ReviewVariantLabel = 37,
        /// <summary>Yorum yazarı profil resmi URL'si.</summary>
        ReviewAuthorAvatarUrl = 38,
        /// <summary>Ürün genel puan ortalaması (özet — ProductRatingSummary için).</summary>
        ReviewAverageRating = 39,
        /// <summary>Ürün toplam yorum sayısı (özet — ProductRatingSummary için).</summary>
        ReviewTotalCount = 40,

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
        Split = 10,

        // ── Yorum puanı dönüşümleri ─────────────────────────────────────────
        /// <summary>
        /// Puan ölçeğini normalize et (ör. 10'luk → 5'lik). Config: {"sourceMax":10,"targetMax":5}.
        /// Amazon 5'lik, bazı platformlar 10'luk veya 100'lük ölçek kullanır.
        /// </summary>
        NormalizeRatingScale = 11,
        /// <summary>
        /// Tarih biçimini parse et (kaynak platformun tarih formatı farklı olabilir).
        /// Config: {"sourceFormat":"dd.MM.yyyy","sourceTimeZone":"Europe/Istanbul"}.
        /// </summary>
        ParseDate = 12
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

    // ════════════════════════════════════════════════════════════════════════
    //  Yorum İçe Aktarım Davranışları (Review Import V1)
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// İçe aktarılan yorumların nasıl işleneceğini belirleyen davranış politikası.
    /// ImportProfile'da <see cref="ImportProfile.ReviewImportBehavior"/> ile ayarlanır;
    /// job oluşturulurken profilden kopyalanır.
    /// </summary>
    public enum ImportReviewBehavior : byte
    {
        /// <summary>Yorum içe aktarımı yapılmasın (yalnızca ürünler).</summary>
        Skip = 1,
        /// <summary>Yorumları içe aktar; hepsi doğrudan yayına alınsın (Approved).</summary>
        ImportAndPublish = 2,
        /// <summary>Yorumları içe aktar; tümü moderasyon kuyruğuna düşsün (Pending).</summary>
        ImportAsPending = 3,
        /// <summary>
        /// Yorumları içe aktar; yalnızca doğrulanmış satın alma (verified purchase)
        /// olanlar yayına alınsın, diğerleri Pending. Amazon/Trendyol akışlarında
        /// güvenilir filtre olarak kullanılır.
        /// </summary>
        ImportVerifiedOnly = 4
    }

    /// <summary>
    /// İçe aktarılan yorumun yıldız puanının tutarlılığının nasıl sağlanacağı.
    /// Farklı platformlar farklı ölçekler kullanır (Amazon: 1-5, bazı siteler: 1-10,
    /// Trustpilot: 1-5 ama ondalıklı). Bu enum, dönüşüm stratejisini belirler.
    /// </summary>
    public enum ReviewRatingScaleMode : byte
    {
        /// <summary>Dönüşüm yapma — kaynak puan olduğu gibi (efavori 1-5 ölçeğinde kabul et).</summary>
        AsIs = 1,
        /// <summary>
        /// Kaynak ölçekten efavori 1-5 ölçeğine orantılı dönüşüm.
        /// Profile SourceRatingScaleMax tanımlanır (ör. 10); formül: round(source * 5 / max).
        /// </summary>
        Proportional = 2,
        /// <summary>
        /// Alan eşleştirmesindeki ValueMap dönüşümü ile birebir eşleme.
        /// Ör: {"A":"5","B":"4","C":"3","D":"2","E":"1"} (harf bazlı sistemler).
        /// </summary>
        ValueMap = 3
    }

    /// <summary>
    /// Aynı dış yorum kimliği (ExternalReviewId) tekrar geldiğinde uygulanacak
    /// strateji. Ürün düzeyindeki DuplicateStrategy'den bağımsızdır çünkü
    /// yorum tekilleştirmesi farklı kurallar gerektirir.
    /// </summary>
    public enum ReviewDuplicateStrategy : byte
    {
        /// <summary>Aynı ExternalReviewId varsa atla (mevcut yoruma dokunma).</summary>
        Skip = 1,
        /// <summary>Aynı ExternalReviewId varsa mevcut yorumu güncelle (puan/metin değişmiş olabilir).</summary>
        Update = 2,
        /// <summary>Tekilleştirme yapma — her zaman yeni yorum oluştur (dikkatli kullanılmalı).</summary>
        CreateNew = 3
    }
}