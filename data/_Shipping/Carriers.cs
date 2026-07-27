using data._Products;

namespace data._Shipping
{
    /// <summary>
    /// KARGO FİRMALARI REFERANS TABLOSU (Yurtiçi, Aras, MNG, UPS, DHL...).
    /// Mevcut IntegrationPlatform / PaymentProviders deseninin kargo karşılığıdır:
    /// firmalar enum DEĞİL, veri olarak yönetilir — yeni firma eklemek deploy gerektirmez.
    /// </summary>
    public class Carriers : ShippingEntityBase
    {
        /// <summary>Makine-okur tekil kod ("yurtici", "aras", "mng", "ups"). Küçük harf.</summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>Görünen ad ("Yurtiçi Kargo").</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Takip URL şablonu. {trackingNumber} yer tutucusu takip numarasıyla değiştirilir.
        /// Örn: "https://kargotakip.example.com/sorgu?kod={trackingNumber}".</summary>
        public string? TrackingUrlTemplate { get; set; }

        /// <summary>Entegrasyon tipi. Manual dışındaki tiplerde etiket/durum otomasyonu çalışır.</summary>
        public CarrierIntegrationType IntegrationType { get; set; } = CarrierIntegrationType.Manual;

        /// <summary>Firma aktif mi? Pasif firmayla yeni sevkiyat oluşturulamaz; mevcutlar etkilenmez.</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Logo URL'i (checkout'ta kargo seçimi ekranında gösterilir).</summary>
        public string? LogoUrl { get; set; }

        /// <summary>Uluslararası gönderi destekliyor mu?</summary>
        public bool SupportsInternational { get; set; }

        /// <summary>Kapıda ödeme (COD) destekliyor mu?</summary>
        public bool SupportsCashOnDelivery { get; set; }

        /// <summary>Şubeden teslim (kargo noktası) destekliyor mu?</summary>
        public bool SupportsPickupPoint { get; set; }

        /// <summary>API üzerinden etiket (barkod) üretimi destekliyor mu?</summary>
        public bool SupportsLabelGeneration { get; set; }

        /// <summary>Desi hesabında kullanılacak bölen. TR standardı 3000'dir; firma bazında değişebilir.
        /// Desi = (En × Boy × Yükseklik cm) / DesiDivisor.</summary>
        public int DesiDivisor { get; set; } = 3000;

        /// <summary>Ortalama teslimat süresi (gün). Tahmini teslim aralığı hesabının girdisidir (README §6).</summary>
        public int AverageDeliveryDays { get; set; } = 3;

        /// <summary>Checkout'taki gösterim sırası.</summary>
        public int SortOrder { get; set; }
    }

    /// <summary>
    /// SATICININ KARGO FİRMASI HESABI. Satıcının kargo firmasıyla yaptığı anlaşmanın
    /// kimlik bilgilerini taşır (müşteri kodu, API anahtarı vb.).
    ///
    /// GİZLİLİK: Kimlik bilgileri mevcut ImportCredential DESENİNDE ŞİFRELİ saklanır —
    /// düz metin ASLA yazılmaz. Şifreleme/çözme uygulama katmanındaki mevcut sağlayıcı ile yapılır;
    /// bu tablo yalnızca şifreli metni (cipher text) ve algoritma sürümünü tutar.
    /// Ekranda gösterim için yalnızca maskelenmiş özet (MaskedAccountCode) kullanılır.
    /// </summary>
    public class StoreCarrierAccounts : ShippingEntityBase
    {
        /// <summary>Satıcı mağaza (Store.Id).</summary>
        public Guid StoreId { get; set; }

        /// <summary>Kargo firması (Carriers.Id).</summary>
        public Guid CarrierId { get; set; }

        /// <summary>Anlaşma adı ("Yurtiçi - Ana Sözleşme"). Yönetim ekranında ayırt etmek için.</summary>
        public string AccountName { get; set; } = string.Empty;

        /// <summary>ŞİFRELİ kimlik bilgisi paketi (JSON: müşteri kodu, kullanıcı adı, parola, API anahtarı...).
        /// ImportCredential deseninde saklanır; düz metin YAZILMAZ.</summary>
        public string EncryptedCredentials { get; set; } = string.Empty;

        /// <summary>Şifreleme algoritma/anahtar sürümü. Anahtar rotasyonunda eski kayıtları çözebilmek için.</summary>
        public string? EncryptionKeyVersion { get; set; }

        /// <summary>Gösterim için maskelenmiş hesap kodu ("****4821"). Şifre çözmeden ekranda gösterilir.</summary>
        public string? MaskedAccountCode { get; set; }

        /// <summary>Bu hesap aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Mağazanın varsayılan kargo hesabı mı? Mağaza başına en fazla bir tane true (servis katmanı korur).</summary>
        public bool IsDefault { get; set; }

        /// <summary>Anlaşmalı gönderi ücreti satıcıya mı faturalanır? true = satıcı kendi anlaşmasından öder,
        /// false = platform anlaşması kullanılır (kargo bedeli Faz 2 ledger'a farklı yansır).</summary>
        public bool IsSellerBilled { get; set; } = true;

        /// <summary>Son başarılı bağlantı testi (UTC). Kimlik bilgisi geçerliliği izlemesi için.</summary>
        public DateTime? LastVerifiedAtUtc { get; set; }

        /// <summary>Son bağlantı hatası mesajı (kimlik bilgisi bozulduysa satıcıya uyarı gösterilir).</summary>
        public string? LastErrorMessage { get; set; }
    }

    /// <summary>
    /// KARGO BÖLGESİ (BAŞLIK). Ücret kurallarının uygulanacağı coğrafi kümedir
    /// ("Yurt İçi", "Doğu Anadolu", "Avrupa", "Adalar"). Kapsamı ShippingZoneAreas satırları belirler.
    ///
    /// SAHİPLİK: StoreId null ise PLATFORM bölgesidir (tüm mağazalar kullanabilir);
    /// dolu ise yalnızca o mağazanın kendi tanımladığı bölgedir. Çözümde mağaza bölgesi
    /// platform bölgesine ÖNCELİKLİDİR (README §5).
    /// </summary>
    public class ShippingZones : ShippingEntityBase
    {
        /// <summary>Bölgeyi tanımlayan mağaza (Store.Id). Null = platform geneli bölge.</summary>
        public Guid? StoreId { get; set; }

        /// <summary>Bölge adı ("Yurt İçi", "Adalar ve Uzak Bölgeler").</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Makine-okur kod ("domestic", "islands"). Aynı sahip kapsamında tekildir.</summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>Açıklama.</summary>
        public string? Description { get; set; }

        /// <summary>Bölge aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Hiçbir bölge eşleşmezse kullanılacak yedek bölge mi?
        /// Sahip başına en fazla bir tane true olmalıdır; "teslimat yapılamıyor" hatasını önler.</summary>
        public bool IsFallback { get; set; }

        /// <summary>Çözüm sırası. Aynı özgüllükte birden çok bölge eşleşirse küçük değer kazanır.</summary>
        public int Priority { get; set; }
    }

    /// <summary>
    /// KARGO BÖLGESİ KAPSAM SATIRI. Bir bölge birden çok coğrafi kapsam satırı içerir
    /// (örn. "Doğu Anadolu" = 14 il satırı). Mevcut Regions/Country/States/Cities tablolarına
    /// ID ile referans verir — coğrafi veri BURADA TEKRARLANMAZ.
    ///
    /// ÇÖZÜM: Teslimat adresi için en ÖZEL eşleşme kazanır: City &gt; State &gt; Country &gt; Region.
    /// IsExcluded=true satırlar dışlamadır: "Türkiye dâhil, ama Gökçeada hariç" gibi kuralları
    /// tek bölgede ifade etmeyi sağlar; dışlama her zaman dâhil etmeyi ezer.
    /// </summary>
    public class ShippingZoneAreas : ShippingEntityBase
    {
        /// <summary>Bağlı bölge (ShippingZones.Id).</summary>
        public Guid ShippingZoneId { get; set; }

        /// <summary>Bu satırın işaret ettiği coğrafi seviye.</summary>
        public ShippingZoneScopeType ScopeType { get; set; }

        /// <summary>Kıta/bölge (Regions.Id). ScopeType=Region iken dolu.</summary>
        public Guid? RegionId { get; set; }

        /// <summary>Ülke (Country.Id). ScopeType=Country iken dolu.</summary>
        public Guid? CountryId { get; set; }

        /// <summary>İl (States.Id). ScopeType=State iken dolu.</summary>
        public Guid? StateId { get; set; }

        /// <summary>İlçe/şehir (Cities.Id). ScopeType=City iken dolu.</summary>
        public Guid? CityId { get; set; }

        /// <summary>Posta kodu aralığı başlangıcı. ScopeType=PostalCode iken dolu.</summary>
        public string? PostalCodeFrom { get; set; }

        /// <summary>Posta kodu aralığı sonu.</summary>
        public string? PostalCodeTo { get; set; }

        /// <summary>true = bu kapsam bölgeden DIŞLANIR. Dışlama, dâhil etme satırlarını ezer.</summary>
        public bool IsExcluded { get; set; }
    }

    /// <summary>
    /// KARGO ÜCRET KURALI. (Carrier, Zone, ölçü aralığı) üçlüsüne karşılık gelen fiyatı tanımlar.
    ///
    /// ÜCRETSİZ KARGO: FreeShippingThreshold dolu ve alt sipariş tutarı bu değeri geçiyorsa
    /// ücret 0 uygulanır. Eşik, kuralın kendi para birimindedir.
    ///
    /// TARİHÇE: Fiyat değişimi = eski satıra ValidToUtc yazılır + YENİ satır açılır
    /// (CommissionRates ile aynı felsefe). Sipariş anında çözülen ücret SubOrders.ShippingTotal'a
    /// ve Shipments.CalculatedCost alanına SNAPSHOT'lanır; bu tablo yalnızca çözüm kaynağıdır.
    /// </summary>
    public class ShippingRateRules : ShippingEntityBase
    {
        /// <summary>Kuralı tanımlayan mağaza (Store.Id). Null = platform varsayılan kuralı.</summary>
        public Guid? StoreId { get; set; }

        /// <summary>Kargo firması (Carriers.Id). Null = firmadan bağımsız kural.</summary>
        public Guid? CarrierId { get; set; }

        /// <summary>Kargo bölgesi (ShippingZones.Id).</summary>
        public Guid ShippingZoneId { get; set; }

        /// <summary>Kuralın hangi ölçüye göre uygulandığı.</summary>
        public ShippingRateBasis RateBasis { get; set; } = ShippingRateBasis.Desi;

        /// <summary>Ölçü aralığı alt sınırı (dâhil). Desi/kg/tutar/adet — RateBasis'e göre yorumlanır.
        /// RateBasis=Flat iken 0 bırakılır.</summary>
        public decimal RangeFrom { get; set; }

        /// <summary>Ölçü aralığı üst sınırı (hariç). Null = üst sınırsız ("30 desi ve üzeri").</summary>
        public decimal? RangeTo { get; set; }

        /// <summary>Bu aralık için taban ücret.</summary>
        public decimal Price { get; set; }

        /// <summary>Aralık üzerinde birim başına ek ücret (örn. "10 desiden sonra her desi +5 TL").
        /// Null/0 = ek ücret yok. Hesap: Price + (ölçü − RangeFrom) × PricePerExtraUnit.</summary>
        public decimal? PricePerExtraUnit { get; set; }

        /// <summary>Ücretin para birimi.</summary>
        public CurrencyCode Currency { get; set; }

        /// <summary>Ücretsiz kargo eşiği: alt sipariş tutarı bu değeri geçerse ücret 0 olur.
        /// Null = ücretsiz kargo yok.</summary>
        public decimal? FreeShippingThreshold { get; set; }

        /// <summary>Kural aktif mi?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Geçerlilik başlangıcı (UTC).</summary>
        public DateTime ValidFromUtc { get; set; }

        /// <summary>Geçerlilik sonu (UTC). Null = halen geçerli.</summary>
        public DateTime? ValidToUtc { get; set; }

        /// <summary>Aynı aralıkta birden çok kural eşleşirse küçük değer kazanır.</summary>
        public int Priority { get; set; }

        /// <summary>Admin açıklaması.</summary>
        public string? Note { get; set; }
    }
}