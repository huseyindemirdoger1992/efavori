using data._Products;

namespace data._Orders
{
    /// <summary>
    /// ÜST SİPARİŞ (müşteri bazlı). Müşteri tek checkout yapar; ödeme, kupon ve toplamların
    /// müşteriye bakan yüzü bu tablodadır. Mağaza bazlı bölünme SubOrders tablosundadır
    /// (Trendyol/Amazon split-order modeli). Kargo, durum akışı ve hakediş SubOrder seviyesindedir;
    /// buradaki Status alanı alt siparişlerden türetilen toplulaştırılmış özettir.
    /// Adresler sipariş anında DÜZ ALAN olarak snapshot'lanır; UserAddress FK'ları yalnızca izdir.
    /// </summary>
    public class Orders : OrderEntityBase
    {
        /// <summary>İnsan-okur, global tekil sipariş numarası. Örn: "EF-2026-000123".
        /// Üretim stratejisi: OrderNumberSequences tablosu + atomik UPDATE (README'ye bakınız).</summary>
        public string OrderNumber { get; set; } = string.Empty;

        /// <summary>Siparişi veren kullanıcı (Users.Id). MİSAFİR siparişte null'dır;
        /// bu durumda GuestEmail zorunlu, GuestPhone önerilir.</summary>
        public Guid? UserId { get; set; }

        /// <summary>Misafir siparişi e-posta adresi (UserId null iken zorunlu). Sipariş takibi bu e-posta ile yapılır.</summary>
        public string? GuestEmail { get; set; }

        /// <summary>Misafir siparişi telefon numarası.</summary>
        public string? GuestPhone { get; set; }

        /// <summary>Bu siparişi üreten checkout oturumu (CheckoutSessions.Id). Idempotency izi.</summary>
        public Guid? CheckoutSessionId { get; set; }

        /// <summary>Toplulaştırılmış sipariş durumu. KAYNAK: SubOrders.Status satırları;
        /// servis katmanı her SubOrder geçişinde yeniden türetir (denormalize).</summary>
        public OrderStatus Status { get; set; } = OrderStatus.PendingPayment;

        // ---------------- TUTAR DÖKÜMÜ (fiyatlar KDV DÂHİLDİR) ----------------

        /// <summary>Sipariş para birimi. Tüm tutar alanları bu para birimindedir (tek Currency + tek tutar seti).</summary>
        public CurrencyCode Currency { get; set; }

        /// <summary>Kalem toplamı (indirim öncesi, KDV dâhil). KAYNAK: OrderItems birim fiyat × adet toplamı.</summary>
        public decimal ItemsTotal { get; set; }

        /// <summary>Ürün/kampanya indirimleri toplamı.</summary>
        public decimal DiscountTotal { get; set; }

        /// <summary>Kupon indirimi toplamı.</summary>
        public decimal CouponTotal { get; set; }

        /// <summary>Kargo ücreti toplamı (tüm alt siparişlerin kargo toplamı).</summary>
        public decimal ShippingTotal { get; set; }

        /// <summary>Bilgi amaçlı KDV toplamı (fiyatlar KDV dâhil olduğundan GrandTotal'a AYRICA eklenmez).</summary>
        public decimal TaxTotal { get; set; }

        /// <summary>Ödenecek genel toplam = ItemsTotal − DiscountTotal − CouponTotal + ShippingTotal.</summary>
        public decimal GrandTotal { get; set; }

        /// <summary>Sipariş anındaki kur tablosu snapshot'ı (MoneyExchangeRate satırlarının JSON kopyası).
        /// Kur izlenebilirliği ve sonradan raporlama için; sipariş sonrası kur değişimlerinden etkilenmez.</summary>
        public string? ExchangeRateSnapshotJson { get; set; }

        // ---------------- TESLİMAT ADRESİ SNAPSHOT (düz alanlar) ----------------

        /// <summary>Kaynak adres izi (UserAddress.Id). Adres silinse/değişse bile aşağıdaki snapshot geçerlidir.</summary>
        public Guid? ShippingUserAddressId { get; set; }

        /// <summary>Teslimat: ad soyad.</summary>
        public string ShippingFullName { get; set; } = string.Empty;

        /// <summary>Teslimat: telefon.</summary>
        public string ShippingPhone { get; set; } = string.Empty;

        /// <summary>Teslimat: ülke adı (metin snapshot). İz için ShippingCountryId.</summary>
        public string ShippingCountryName { get; set; } = string.Empty;

        /// <summary>Teslimat: ülke izi (Country.Id).</summary>
        public int? ShippingCountryId { get; set; }

        /// <summary>Teslimat: il adı (metin snapshot). İz için ShippingStateId.</summary>
        public string? ShippingStateName { get; set; }

        /// <summary>Teslimat: il izi (States.Id).</summary>
        public int? ShippingStateId { get; set; }

        /// <summary>Teslimat: ilçe/şehir adı (metin snapshot). İz için ShippingCityId.</summary>
        public string? ShippingCityName { get; set; }

        /// <summary>Teslimat: ilçe/şehir izi (Cities.Id).</summary>
        public int? ShippingCityId { get; set; }

        /// <summary>Teslimat: açık adres satırı.</summary>
        public string ShippingAddressLine { get; set; } = string.Empty;

        /// <summary>Teslimat: posta kodu.</summary>
        public string? ShippingPostalCode { get; set; }

        // ---------------- FATURA ADRESİ SNAPSHOT (düz alanlar) ----------------

        /// <summary>Kaynak fatura adresi izi (UserAddress.Id).</summary>
        public Guid? BillingUserAddressId { get; set; }

        /// <summary>Fatura: ad soyad / yetkili adı.</summary>
        public string BillingFullName { get; set; } = string.Empty;

        /// <summary>Fatura: telefon.</summary>
        public string? BillingPhone { get; set; }

        /// <summary>Fatura: ülke adı (metin snapshot).</summary>
        public string BillingCountryName { get; set; } = string.Empty;

        /// <summary>Fatura: il adı (metin snapshot).</summary>
        public string? BillingStateName { get; set; }

        /// <summary>Fatura: ilçe/şehir adı (metin snapshot).</summary>
        public string? BillingCityName { get; set; }

        /// <summary>Fatura: açık adres satırı.</summary>
        public string BillingAddressLine { get; set; } = string.Empty;

        /// <summary>Fatura: posta kodu.</summary>
        public string? BillingPostalCode { get; set; }

        /// <summary>Kurumsal fatura mı? true ise BillingCompanyName/BillingTaxNumber/BillingTaxOffice doldurulur.</summary>
        public bool BillingIsCorporate { get; set; }

        /// <summary>Kurumsal fatura: firma unvanı.</summary>
        public string? BillingCompanyName { get; set; }

        /// <summary>Kurumsal fatura: vergi numarası (bireyselde TCKN yazılabilir).</summary>
        public string? BillingTaxNumber { get; set; }

        /// <summary>Kurumsal fatura: vergi dairesi.</summary>
        public string? BillingTaxOffice { get; set; }

        /// <summary>Müşterinin sipariş notu.</summary>
        public string? CustomerNote { get; set; }
    }

    /// <summary>
    /// ALT SİPARİŞ (mağaza bazlı). Üst siparişin mağaza başına bölünmüş parçasıdır.
    /// Kargo, durum makinesi, faturalama (OrderInvoices) ve hakediş (Faz 2 SellerLedgerEntries)
    /// BU seviyede yürür. Tutar alanları yalnızca bu mağazanın kalemlerini kapsar.
    /// </summary>
    public class SubOrders : OrderEntityBase
    {
        /// <summary>Bağlı üst sipariş (Orders.Id).</summary>
        public Guid OrderId { get; set; }

        /// <summary>Satıcı mağaza (Store.Id).</summary>
        public Guid StoreId { get; set; }

        /// <summary>İnsan-okur alt sipariş numarası. Üst numaradan türetilir: "EF-2026-000123-1", "-2"...
        /// Aynı checkout içinde mağaza sırasına göre servis katmanında atanır.</summary>
        public string SubOrderNumber { get; set; } = string.Empty;

        /// <summary>Alt sipariş durum makinesi (asıl akış buradadır). Her geçiş OrderStatusHistory'e yazılır.</summary>
        public OrderStatus Status { get; set; } = OrderStatus.PendingPayment;

        // ---------------- TUTAR DÖKÜMÜ (yalnızca bu mağazanın kalemleri) ----------------

        /// <summary>Para birimi. Üst siparişle aynıdır; sorgu kolaylığı için denormalize tutulur.</summary>
        public CurrencyCode Currency { get; set; }

        /// <summary>Kalem toplamı (indirim öncesi, KDV dâhil).</summary>
        public decimal ItemsTotal { get; set; }

        /// <summary>Ürün/kampanya indirimleri toplamı.</summary>
        public decimal DiscountTotal { get; set; }

        /// <summary>Bu alt siparişe düşen kupon payı toplamı. KAYNAK: OrderItems.CouponAmount toplamı.</summary>
        public decimal CouponTotal { get; set; }

        /// <summary>Bu alt siparişin kargo ücreti.</summary>
        public decimal ShippingTotal { get; set; }

        /// <summary>Bilgi amaçlı KDV toplamı.</summary>
        public decimal TaxTotal { get; set; }

        /// <summary>Alt sipariş genel toplamı.</summary>
        public decimal GrandTotal { get; set; }

        /// <summary>Platform komisyon toplamı (KAYNAK: OrderItems.CommissionAmount toplamı; Faz 2 ledger girdisinin dayanağı).</summary>
        public decimal CommissionTotal { get; set; }

        // ---------------- AKIŞ ZAMAN DAMGALARI ----------------

        /// <summary>Kargoya verildiği an (UTC). Status=Shipped geçişinde yazılır.</summary>
        public DateTime? ShippedAtUtc { get; set; }

        /// <summary>Teslim edildiği an (UTC). Status=Delivered geçişinde yazılır; Faz 2 escrow ReleaseAfterUtc hesabının girdisidir.</summary>
        public DateTime? DeliveredAtUtc { get; set; }

        /// <summary>İptal edildiği an (UTC).</summary>
        public DateTime? CancelledAtUtc { get; set; }

        /// <summary>İptal gerekçesi (Status=Cancelled iken).</summary>
        public OrderCancellationReason? CancellationReason { get; set; }

        /// <summary>İptal açıklama notu.</summary>
        public string? CancellationNote { get; set; }
    }

    /// <summary>
    /// SİPARİŞ KALEMİ. SubOrder'a bağlıdır. Ürün/varyant referansları yalnızca İZDİR;
    /// faturayı ve raporlamayı koruyan asıl veri SİPARİŞ ANI SNAPSHOT alanlarıdır.
    /// Ürün silinse, adı/fiyatı değişse bile bu satır değişmez.
    /// NOT: Mevcut owned CartProductSnapshot BİLEREK kullanılmamıştır — sipariş, 4 para birimli
    /// sepet kalıbını miras almaz; tek Currency + tek tutar seti taşır.
    /// </summary>
    public class OrderItems : OrderEntityBase
    {
        /// <summary>Bağlı alt sipariş (SubOrders.Id).</summary>
        public Guid SubOrderId { get; set; }

        /// <summary>Üst sipariş izi (Orders.Id). DENORMALIZE: müşteri bazlı kalem sorgularını tek join'e indirir; kaynak SubOrders.OrderId.</summary>
        public Guid OrderId { get; set; }

        /// <summary>Mağaza izi (Store.Id). DENORMALIZE: satıcı bazlı satış raporları için; kaynak SubOrders.StoreId.</summary>
        public Guid StoreId { get; set; }

        /// <summary>Ürün referansı (Products.Id). Yalnızca iz; snapshot alanları esastır.</summary>
        public Guid ProductId { get; set; }

        /// <summary>Varyant referansı (ProductVariants.Id). Basit üründe IsDefault=true tek varyantın Id'sidir.</summary>
        public Guid VariantId { get; set; }

        // ---------------- SİPARİŞ ANI SNAPSHOT ----------------

        /// <summary>Snapshot: sipariş anındaki ürün adı (müşterinin gördüğü dilde).</summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>Snapshot: SKU.</summary>
        public string? Sku { get; set; }

        /// <summary>Snapshot: barkod.</summary>
        public string? Barcode { get; set; }

        /// <summary>Snapshot: birincil görsel URL'i.</summary>
        public string? ImageUrl { get; set; }

        /// <summary>Snapshot: seçili varyant özeti ("Renk: Kırmızı, Beden: M"). Bilgi amaçlı serbest metin.</summary>
        public string? VariantSummary { get; set; }

        /// <summary>Sipariş adedi.</summary>
        public int Quantity { get; set; }

        /// <summary>Snapshot: birim satış fiyatı (KDV dâhil, indirim uygulanmış birim fiyat öncesi liste değeri için bkz. UnitListPrice).</summary>
        public decimal UnitPrice { get; set; }

        /// <summary>Snapshot: birim liste fiyatı (üstü çizili fiyat gösterimi/raporlama için).</summary>
        public decimal UnitListPrice { get; set; }

        /// <summary>Para birimi (satır bazında; üst siparişle aynıdır, snapshot bütünlüğü için tekrarlanır).</summary>
        public CurrencyCode Currency { get; set; }

        /// <summary>Snapshot: KDV oranı (yüzde, örn. 20.00). Fiyatlar KDV dâhildir; bilgi/fatura dökümü içindir.</summary>
        public decimal VatRate { get; set; }

        /// <summary>Satıra düşen ürün/kampanya indirimi toplam tutarı.</summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>Satıra düşen kupon payı (kupon toplamının satırlara dağıtımı; hakediş hesabında satıcı/platform payı Faz 2'de ayrışır).</summary>
        public decimal CouponAmount { get; set; }

        /// <summary>Snapshot: sipariş anındaki komisyon oranı (yüzde). KAYNAK: Faz 2 CommissionRates çözümü.</summary>
        public decimal CommissionRate { get; set; }

        /// <summary>Snapshot: satır komisyon tutarı = komisyona esas tutar × CommissionRate.</summary>
        public decimal CommissionAmount { get; set; }

        /// <summary>Satır toplamı = (UnitPrice × Quantity) − DiscountAmount − CouponAmount.</summary>
        public decimal LineTotal { get; set; }

        /// <summary>İade edilen adet. DENORMALIZE: kaynak Faz 2 Refunds / Faz 7 ReturnRequests; servis katmanı günceller.</summary>
        public int RefundedQuantity { get; set; }
    }
}
