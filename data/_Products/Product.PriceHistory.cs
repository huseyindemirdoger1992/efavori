namespace data._Products
{
    /// <summary>
    /// FİYAT DEĞİŞİM GEÇMİŞİ (append-only). ProductPrices satırındaki HER anlamlı değişimde
    /// (ListPrice / SalePrice / DiscountedPrice / indirim tarih aralığı / IsActive) bir satır yazılır.
    /// Mevcut satırlar ASLA güncellenmez veya silinmez.
    ///
    /// KULLANIM ALANLARI:
    /// - AB Omnibus / TR mevzuat: "son 30 günün en düşük satış fiyatı" (bkz. ProductPriceDailySnapshot).
    /// - Fiyat düşüşü alarmı tetikleme (PriceAlerts).
    /// - Satıcı fiyat oynaklığı denetimi, ihtilaf kanıtı.
    ///
    /// NOT: Taban sınıf ProductEntityBase'tir; "değiştiren kullanıcı" bilgisi CreatedByUserId
    /// alanında tutulur, "değişim anı" CreatedAtUtc'dedir — ayrı alan AÇILMAZ.
    /// </summary>
    public class ProductPriceHistory : ProductEntityBase
    {
        /// <summary>Ürün (Products.Id).</summary>
        public Guid ProductId { get; set; }

        /// <summary>Varyant (ProductVariants.Id). Basit üründe IsDefault=true tek varyantın Id'sidir.</summary>
        public Guid VariantId { get; set; }

        /// <summary>Değişime konu fiyat satırı (ProductPrices.Id). Satır sonradan silinse bile geçmiş yaşar (iz alanı).</summary>
        public Guid? ProductPriceId { get; set; }

        /// <summary>Para birimi. Fiyat geçmişi para birimi bazında ayrı izlenir.</summary>
        public CurrencyCode Currency { get; set; }

        // ---------------- ESKİ → YENİ DEĞERLER ----------------
        // İlk oluşturmada (ChangeSource=InitialCreate) Old* alanları null'dır.

        /// <summary>Eski liste fiyatı (üstü çizili fiyat).</summary>
        public decimal? OldListPrice { get; set; }

        /// <summary>Yeni liste fiyatı.</summary>
        public decimal? NewListPrice { get; set; }

        /// <summary>Eski satış fiyatı.</summary>
        public decimal? OldSalePrice { get; set; }

        /// <summary>Yeni satış fiyatı. Omnibus "en düşük fiyat" hesabının temel girdisidir.</summary>
        public decimal? NewSalePrice { get; set; }

        /// <summary>Eski indirimli fiyat.</summary>
        public decimal? OldDiscountedPrice { get; set; }

        /// <summary>Yeni indirimli fiyat.</summary>
        public decimal? NewDiscountedPrice { get; set; }

        /// <summary>Eski indirim başlangıcı (UTC).</summary>
        public DateTime? OldDiscountStartUtc { get; set; }

        /// <summary>Yeni indirim başlangıcı (UTC).</summary>
        public DateTime? NewDiscountStartUtc { get; set; }

        /// <summary>Eski indirim bitişi (UTC).</summary>
        public DateTime? OldDiscountEndUtc { get; set; }

        /// <summary>Yeni indirim bitişi (UTC).</summary>
        public DateTime? NewDiscountEndUtc { get; set; }

        /// <summary>Eski aktiflik durumu.</summary>
        public bool? OldIsActive { get; set; }

        /// <summary>Yeni aktiflik durumu.</summary>
        public bool? NewIsActive { get; set; }

        // ---------------- DEĞİŞİM BAĞLAMI ----------------

        /// <summary>Değişim kaynağı.</summary>
        public PriceChangeSource ChangeSource { get; set; } = PriceChangeSource.Unknown;

        /// <summary>Kur dönüşümüyle üretilmiş fiyat mı? (ProductPrices.IsAutoConverted snapshot'ı).</summary>
        public bool IsAutoConverted { get; set; }

        /// <summary>Dönüşümde kullanılan kur değeri (MoneyExchangeRate snapshot'ı).
        /// Sonradan kur değişse bile o günkü hesabın nasıl yapıldığı izlenebilir.</summary>
        public decimal? UsedExchangeRate { get; set; }

        /// <summary>Kur satırı izi (MoneyExchangeRate.Id).</summary>
        public Guid? MoneyExchangeRateId { get; set; }

        /// <summary>Kaynak toplu içe aktarım satırı izi (data._BulkImportProducts ImportRow.Id).
        /// ChangeSource=Import iken doldurulur.</summary>
        public Guid? ImportRowId { get; set; }

        /// <summary>Kaynak kampanya izi (Faz 6 Campaigns.Id). ChangeSource=Campaign/CampaignExpired iken doldurulur.
        /// FK Faz 6'da tanımlanır; şimdilik iz alanıdır.</summary>
        public Guid? CampaignId { get; set; }

        /// <summary>Serbest açıklama ("Yılbaşı kampanyası", "Toplu zam %8").</summary>
        public string? Note { get; set; }
    }

    /// <summary>
    /// GÜNLÜK FİYAT ÖZETİ. ProductPriceHistory ham defterdir; milyonlarca satır arasında
    /// "son 30 günün en düşük satış fiyatı" sorgusunu her ürün sayfası açılışında çalıştırmak
    /// PAHALIDIR. Bu tablo (ProductId, VariantId, Currency, PriceDate) bazında günlük özet tutar;
    /// Omnibus sorgusu 30 satır tarayarak yanıtlanır.
    ///
    /// BESLEME: Gecelik BackgroundService (AllBackgroundServices içinde bir görev) bir önceki
    /// günün ProductPriceHistory satırlarını + gün sonundaki geçerli fiyatı işleyerek UPSERT eder.
    /// Fiyat değişmeyen günler için de satır yazılır (ClosePrice taşınır) — aksi halde "boşluklu"
    /// aralıklarda en düşük fiyat yanlış hesaplanır (bkz. README §5).
    ///
    /// İNDİRİM ETİKETİ GÖSTERİM KURALI (mevzuat):
    /// İndirim oranı, indirim öncesi fiyata göre DEĞİL, son 30 gündeki en düşük satış fiyatına
    /// (Min30DayPrice) göre gösterilir. Yani ekrandaki üstü çizili referans fiyat =
    /// MIN(ClosePrice) over last 30 days. Ürün 30 günden yeniyse kural uygulanmaz, bunun yerine
    /// satışa çıkış tarihinden itibaren hesaplanır ve etikette "yeni ürün" istisnası kullanılır.
    /// </summary>
    public class ProductPriceDailySnapshot : ProductEntityBase
    {
        /// <summary>Ürün (Products.Id).</summary>
        public Guid ProductId { get; set; }

        /// <summary>Varyant (ProductVariants.Id).</summary>
        public Guid VariantId { get; set; }

        /// <summary>Para birimi.</summary>
        public CurrencyCode Currency { get; set; }

        /// <summary>Özetin ait olduğu gün (UTC, saat bileşeni 00:00). Tekil indeksin parçasıdır.</summary>
        public DateTime PriceDate { get; set; }

        /// <summary>Gün içindeki en düşük geçerli satış fiyatı (indirim varsa indirimli fiyat dikkate alınır).</summary>
        public decimal MinPrice { get; set; }

        /// <summary>Gün içindeki en yüksek geçerli satış fiyatı.</summary>
        public decimal MaxPrice { get; set; }

        /// <summary>Gün sonundaki (23:59:59 UTC) geçerli satış fiyatı. Omnibus referans hesabının ana girdisi.</summary>
        public decimal ClosePrice { get; set; }

        /// <summary>Gün içinde fiyat gerçekten değişti mi? false = önceki günden taşınan dolgu satırı.
        /// (Dolgu satırları olmadan aralık sorgusu boşluklu olur; kaynak: ProductPriceHistory.)</summary>
        public bool HasChange { get; set; }

        /// <summary>Gün içindeki fiyat değişim adedi. KAYNAK: ProductPriceHistory satır sayısı (denormalize sayaç).</summary>
        public int ChangeCount { get; set; }
    }

    /// <summary>
    /// FİYAT DÜŞÜŞÜ ALARMI. Kullanıcı bir ürün/varyant için hedef fiyat belirler; fiyat bu seviyeye
    /// indiğinde Faz 6 Notifications'a PriceDropAlert olayı üretilir.
    ///
    /// CartsFavorite İLE İLİŞKİ: Favoriye eklemek alarm kurmak DEĞİLDİR (favori = ilgi, alarm = eşik).
    /// Ancak UI akışı favoriden alarm kurmayı önerir; bu bağ CartsFavoriteId iz alanıyla korunur.
    /// Favorideki ürünün fiyatı düştüğünde alarm kurmamış kullanıcıya da bildirim gönderilmek
    /// istenirse, tarama TargetPrice yerine "favoriye eklendiği andaki fiyat" ile yapılır —
    /// bunun için PriceAtCreation alanı doldurulur.
    ///
    /// TETİKLEME: Gecelik/periyodik BackgroundService, ProductPriceDailySnapshot (veya anlık
    /// ProductPrices) üzerinden Active alarmları tarar; eşleşenleri Triggered'a çeker ve
    /// Notifications kuyruğuna satır yazar (README §7).
    /// </summary>
    public class PriceAlerts : ProductEntityBase
    {
        /// <summary>Alarmı kuran kullanıcı (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>İzlenen ürün (Products.Id).</summary>
        public Guid ProductId { get; set; }

        /// <summary>İzlenen varyant (ProductVariants.Id). Null = ürünün HERHANGİ bir varyantı hedefe inerse tetiklenir.</summary>
        public Guid? VariantId { get; set; }

        /// <summary>Hedef fiyat: geçerli satış fiyatı bu değere EŞİT VEYA ALTINA inince tetiklenir.</summary>
        public decimal TargetPrice { get; set; }

        /// <summary>Hedef fiyatın para birimi. Karşılaştırma aynı para birimindeki fiyat satırıyla yapılır.</summary>
        public CurrencyCode Currency { get; set; }

        /// <summary>Alarmın kurulduğu andaki geçerli fiyat. "Favorideki ürün ucuzladı" bildirimlerinin
        /// referansı ve kullanıcıya "%X düştü" gösterimi için.</summary>
        public decimal? PriceAtCreation { get; set; }

        /// <summary>Alarm durumu.</summary>
        public PriceAlertStatus Status { get; set; } = PriceAlertStatus.Active;

        /// <summary>Tetiklendiği an (UTC).</summary>
        public DateTime? TriggeredAtUtc { get; set; }

        /// <summary>Tetiklendiğinde ölçülen fiyat (bildirim metninde gösterilir).</summary>
        public decimal? TriggeredPrice { get; set; }

        /// <summary>Üretilen bildirim izi (Faz 6 Notifications.Id). FK Faz 6'da tanımlanır.</summary>
        public Guid? NotificationId { get; set; }

        /// <summary>Alarmın kaynaklandığı favori kaydı (CartsFavorite.Id). Yalnızca iz.</summary>
        public Guid? CartsFavoriteId { get; set; }

        /// <summary>Tetiklendikten sonra alarm otomatik yeniden kurulsun mu?
        /// true ise bildirim sonrası Status tekrar Active'e alınır (fiyat yeniden yükselip düşerse yeniden bildirim).
        /// Bildirim spam'ini önlemek için servis katmanı asgari bekleme süresi uygular.</summary>
        public bool IsRecurring { get; set; }
    }
}
