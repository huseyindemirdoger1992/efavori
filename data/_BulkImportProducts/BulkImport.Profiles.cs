using System;

namespace data._BulkImportProducts
{
    /// <summary>
    /// KULLANICININ KENDİ İÇE AKTARIM AYARI (profil/preset). Her kullanıcı, bir kaynak
    /// için (ör. "Amazon US mağazam", "WooCommerce CSV şablonum") tekrar tekrar
    /// kullanabileceği bir profil oluşturur. Bir profil; kaynak türünü, platformu,
    /// varsayılan davranışları (mode/duplicate/currency/dil), kimlik bilgilerini
    /// (<see cref="ImportCredential"/>) ve alan eşleştirmelerini
    /// (<see cref="ImportFieldMapping"/>) bir arada tutar.
    ///
    /// Sahiplik: <see cref="UserId"/> zorunlu — profil kullanıcıya özeldir.
    /// <see cref="StoreId"/> opsiyonel: içe aktarılan ürünlerin hangi mağazaya
    /// yazılacağını belirtir (satıcının birden çok mağazası olabilir).
    /// </summary>
    public class ImportProfile : BulkImportEntityBase
    {
        /// <summary>Profilin sahibi kullanıcı (Users.Id) — zorunlu.</summary>
        public Guid UserId { get; set; }

        /// <summary>İçe aktarılan ürünlerin yazılacağı mağaza (Store.Id — opsiyonel).</summary>
        public Guid? StoreId { get; set; }

        /// <summary>Kullanıcının verdiği profil adı (ör. "Trendyol ana katalog").</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Açıklama / not (opsiyonel).</summary>
        public string? Description { get; set; }

        // ── Kaynak tanımı ─────────────────────────────────────────────────────
        /// <summary>
        /// Kaynak platform (data._Attribute.IntegrationPlatform.Id).
        /// Ör: amazon, aliexpress, ebay, alibaba, etsy, walmart, rakuten,
        /// mercado_libre, taobao, shopee, trendyol, hepsiburada, woocommerce,
        /// opencart, csv, json, xml. Enum yerine referans tablo — yenisi eklenebilir.
        /// </summary>
        public Guid IntegrationPlatformId { get; set; }

        /// <summary>Kaynağın teknik türü (CSV dosyası / REST API / XML feed ...).</summary>
        public ImportSourceType SourceType { get; set; }

        /// <summary>API/feed tabanlı kaynakların temel adresi (ör. API base URL).</summary>
        public string? SourceEndpointUrl { get; set; }

        /// <summary>Bu profilin kullandığı kimlik kaydı (ImportCredential.Id — API için).</summary>
        public Guid? ImportCredentialId { get; set; }

        // ── Varsayılan davranışlar (job oluşturulurken devralınır) ────────────
        /// <summary>Oluştur/güncelle davranışı.</summary>
        public ImportMode Mode { get; set; } = ImportMode.CreateOnly;

        /// <summary>Yineleme stratejisi.</summary>
        public DuplicateStrategy DuplicateStrategy { get; set; } = DuplicateStrategy.Skip;

        /// <summary>Yineleme tespitinde kullanılacak eşleşme anahtarı.</summary>
        public ImportMatchKey MatchKey { get; set; } = ImportMatchKey.Gtin;

        /// <summary>
        /// Kaynak verinin ana dili (ör. "en"). İçe aktarılan metinler bu dilde
        /// orijinal kabul edilir; diğer 10 dile AI çeviri akışıyla çevrilebilir.
        /// data._Attribute.Language enum'ına karşılık gelen kültür kodu tutulur.
        /// </summary>
        public string SourceLanguageCode { get; set; } = "en";

        /// <summary>
        /// Kaynak fiyatların para birimi (ör. "USD"). ProductPrices'a yazılırken
        /// baz alınır; diğer para birimleri MoneyExchangeRate ile türetilebilir.
        /// </summary>
        public string SourceCurrencyCode { get; set; } = "USD";

        /// <summary>
        /// Fiyata uygulanacak çarpan/kâr marjı (ör. 1.20 = %20 ekle). Null/1 = birebir.
        /// </summary>
        public decimal? PriceMultiplier { get; set; }

        /// <summary>İçe aktarılan ürünler taslak (Draft) mı yoksa yayına mı alınsın?</summary>
        public bool PublishAfterImport { get; set; }

        /// <summary>İçe aktarılan ürünler admin onayına mı düşsün (PendingReview)?</summary>
        public bool RequiresReviewAfterImport { get; set; } = true;

        // ── Dosya ayrıştırma varsayılanları (CSV/Excel için) ──────────────────
        /// <summary>CSV sütun ayıracı (ör. "," veya ";"). Null = otomatik algıla.</summary>
        public string? CsvDelimiter { get; set; }

        /// <summary>İlk satır başlık mı? (WooCommerce/OpenCart dışa aktarımları genelde başlıklı).</summary>
        public bool HasHeaderRow { get; set; } = true;

        /// <summary>Metin kodlaması (ör. "UTF-8"). Null = otomatik.</summary>
        public string? Encoding { get; set; }

        /// <summary>
        /// API/feed yanıtında ürün listesinin bulunduğu kök yol (ör. JSON "data.items",
        /// XML "channel/item"). Ayrıştırıcı buradan itibaren satırları okur.
        /// </summary>
        public string? RecordsRootPath { get; set; }

        /// <summary>Profil aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>En son bu profille içe aktarım yapıldığı an (UTC).</summary>
        public DateTime? LastUsedAtUtc { get; set; }
    }

    /// <summary>
    /// KULLANICIYA ÖZEL KİMLİK/KREDENSİYEL kaydı (API tabanlı kaynaklar için).
    /// Gizli değerler DAİMA ŞİFRELİ saklanır (mevcut projedeki Encrypted* deseni);
    /// bu tabloya asla düz metin anahtar yazılmaz. Bir profil bir kimlik kaydına
    /// <see cref="ImportProfile.ImportCredentialId"/> ile bağlanır; aynı kimlik
    /// birden çok profilde paylaşılabilir.
    ///
    /// GÜVENLİK NOTU: Alan adları "Encrypted..." önekiyle, uygulama katmanının bu
    /// değerleri şifreleyerek yazması gerektiğini vurgular. Şifreleme/çözme
    /// veritabanı katmanında DEĞİL, uygulama servis katmanında yapılır.
    /// </summary>
    public class ImportCredential : BulkImportEntityBase
    {
        /// <summary>Kimliğin sahibi kullanıcı (Users.Id) — zorunlu.</summary>
        public Guid UserId { get; set; }

        /// <summary>Bağlı platform (data._Attribute.IntegrationPlatform.Id).</summary>
        public Guid IntegrationPlatformId { get; set; }

        /// <summary>Kullanıcının verdiği takma ad (ör. "Amazon SP-API üretim").</summary>
        public string Alias { get; set; } = string.Empty;

        /// <summary>Kimlik doğrulama yöntemi.</summary>
        public ImportAuthType AuthType { get; set; } = ImportAuthType.None;

        // ── Şifreli gizli değerler (uygulama katmanında şifrelenir) ───────────
        /// <summary>Şifreli API anahtarı / client id.</summary>
        public string? EncryptedApiKey { get; set; }

        /// <summary>Şifreli gizli anahtar / client secret.</summary>
        public string? EncryptedApiSecret { get; set; }

        /// <summary>Şifreli kullanıcı adı (BasicAuth).</summary>
        public string? EncryptedUserName { get; set; }

        /// <summary>Şifreli şifre (BasicAuth).</summary>
        public string? EncryptedPassword { get; set; }

        /// <summary>Şifreli erişim token'ı (OAuth2/Bearer).</summary>
        public string? EncryptedAccessToken { get; set; }

        /// <summary>Şifreli yenileme token'ı (OAuth2).</summary>
        public string? EncryptedRefreshToken { get; set; }

        /// <summary>Access token bitiş anı (UTC) — yenileme zamanlaması için.</summary>
        public DateTime? TokenExpiresAtUtc { get; set; }

        // ── Platforma özgü ek parametreler ────────────────────────────────────
        /// <summary>
        /// Bölge/pazar (ör. Amazon "US"/"UK", eBay site kodu, MercadoLibre "MLA").
        /// </summary>
        public string? MarketplaceRegion { get; set; }

        /// <summary>Satıcı/mağaza kimliği (ör. Amazon Seller ID, eBay Store).</summary>
        public string? ExternalSellerId { get; set; }

        /// <summary>
        /// Platforma özgü ek yapılandırma (JSON). Ör: AliExpress app_key ek alanları,
        /// Shopee shop_id, Taobao imza parametreleri. Şemayı sabitlemeden esneklik sağlar.
        /// Not: Buraya düz metin sır YAZILMAZ; sırlar yukarıdaki Encrypted* alanlarındadır.
        /// </summary>
        public string? ExtraConfigJson { get; set; }

        /// <summary>Kimliğin doğrulama durumu (test sonucu).</summary>
        public CredentialStatus Status { get; set; } = CredentialStatus.Untested;

        /// <summary>En son doğrulama/test anı (UTC).</summary>
        public DateTime? LastValidatedAtUtc { get; set; }

        /// <summary>En son test hatası (varsa).</summary>
        public string? LastValidationError { get; set; }

        /// <summary>Kimlik aktif mi?</summary>
        public bool IsActive { get; set; } = true;
    }
}
