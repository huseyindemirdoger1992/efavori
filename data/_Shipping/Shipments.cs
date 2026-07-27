using data._Products;

namespace data._Shipping
{
    /// <summary>
    /// SEVKİYAT (PAKET). SubOrder bazlıdır ve bir alt sipariş BİRDEN ÇOK pakete bölünebilir
    /// (farklı depolardan çıkan kalemler, hacim/ağırlık sınırı, kısmi sevkiyat).
    /// Hangi kalemin hangi pakette olduğu ShipmentItems satırlarında tutulur.
    ///
    /// YÖN: Direction alanı sayesinde iade (Return) ve depo transferi (Transfer) sevkiyatları da
    /// bu tabloda izlenir — ayrı tablo açılmaz, takip altyapısı tek noktada toplanır.
    ///
    /// TAHMİNİ TESLİM ARALIĞI: EstimatedDeliveryFromUtc/ToUtc, sipariş anında
    /// Products.ShippingPreparationDayMin/Max + Carriers.AverageDeliveryDays kullanılarak
    /// hesaplanır ve SNAPSHOT'lanır (README §6); sonradan kargo firması tahmin gönderirse
    /// CarrierEstimatedDeliveryUtc alanına yazılır, snapshot bozulmaz.
    /// </summary>
    public class Shipments : ShippingEntityBase
    {
        /// <summary>Bağlı alt sipariş (SubOrders.Id). Transfer sevkiyatlarında null.</summary>
        public Guid? SubOrderId { get; set; }

        /// <summary>Üst sipariş izi (Orders.Id). DENORMALIZE: kaynak SubOrders.OrderId; müşteri kargo takibi için.</summary>
        public Guid? OrderId { get; set; }

        /// <summary>Sevk eden mağaza (Store.Id).</summary>
        public Guid StoreId { get; set; }

        /// <summary>Sevkiyat yönü.</summary>
        public ShipmentDirection Direction { get; set; } = ShipmentDirection.Outbound;

        /// <summary>Kargo firması (Carriers.Id).</summary>
        public Guid CarrierId { get; set; }

        /// <summary>Kullanılan satıcı kargo hesabı (StoreCarrierAccounts.Id). Platform anlaşmasıyla gönderilirse null.</summary>
        public Guid? StoreCarrierAccountId { get; set; }

        /// <summary>Çıkış deposu (WareHouse.Id). Hangi depodan sevk edildiğinin izi (Faz 4 bağı).</summary>
        public Guid? SourceWarehouseId { get; set; }

        /// <summary>Kaynak: depo transferi (Faz 4 StockTransfers.Id). Direction=Transfer iken dolu.</summary>
        public Guid? StockTransferId { get; set; }

        /// <summary>Kaynak: iade talebi (Faz 7 ReturnRequests.Id). Direction=Return iken dolu.
        /// FK Faz 7'de tanımlanır; şimdilik iz alanıdır.</summary>
        public Guid? ReturnRequestId { get; set; }

        /// <summary>Durum makinesi.</summary>
        public ShipmentStatus Status { get; set; } = ShipmentStatus.Created;

        /// <summary>Takip numarası (taşıyıcı barkodu).</summary>
        public string? TrackingNumber { get; set; }

        /// <summary>Hazır takip URL'i. Carriers.TrackingUrlTemplate doldurularak üretilir ve
        /// SNAPSHOT'lanır — firma şablonu sonradan değişse bile eski gönderi linki bozulmaz.</summary>
        public string? TrackingUrl { get; set; }

        /// <summary>Kargo etiketi dosyası (MediaItems.Id). PDF/ZPL barkod çıktısı.</summary>
        public Guid? LabelMediaItemId { get; set; }

        /// <summary>Paketin sıra numarası ("2/3" gösterimi için). Alt sipariş içinde 1'den başlar.</summary>
        public int PackageIndex { get; set; } = 1;

        /// <summary>Alt siparişteki toplam paket adedi. DENORMALIZE: kaynak Shipments satır sayısı.</summary>
        public int PackageCount { get; set; } = 1;

        // ---------------- PAKET ÖLÇÜLERİ ----------------

        /// <summary>Paket eni (cm).</summary>
        public decimal? WidthCm { get; set; }

        /// <summary>Paket boyu (cm).</summary>
        public decimal? LengthCm { get; set; }

        /// <summary>Paket yüksekliği (cm).</summary>
        public decimal? HeightCm { get; set; }

        /// <summary>Fiili ağırlık (kg).</summary>
        public decimal? WeightKg { get; set; }

        /// <summary>Hesaplanan desi = (En × Boy × Yükseklik) / Carriers.DesiDivisor.
        /// Ücret çözümünde fiili ağırlık ile desiden BÜYÜK olanı kullanılır (taşımacılık standardı).</summary>
        public decimal? Desi { get; set; }

        // ---------------- ÜCRET ----------------

        /// <summary>Sipariş anında ShippingRateRules'tan çözülen kargo ücreti (SNAPSHOT).</summary>
        public decimal CalculatedCost { get; set; }

        /// <summary>Kargo firmasının fiilen faturaladığı ücret. Sonradan gelir; CalculatedCost ile farkı
        /// satıcı/platform maliyet analizinin girdisidir.</summary>
        public decimal? ActualCost { get; set; }

        /// <summary>Ücretlerin para birimi.</summary>
        public CurrencyCode Currency { get; set; }

        /// <summary>Ücretsiz kargo uygulandı mı? (FreeShippingThreshold aşıldı.) Raporlama için.</summary>
        public bool IsFreeShipping { get; set; }

        /// <summary>Çözümde kullanılan ücret kuralı izi (ShippingRateRules.Id). Uyuşmazlıkta hangi kuralın
        /// uygulandığını gösterir; kural sonradan değişse bile iz kalır.</summary>
        public Guid? AppliedRateRuleId { get; set; }

        // ---------------- ZAMAN ÇİZELGESİ ----------------

        /// <summary>Kargo etiketinin üretildiği an (UTC).</summary>
        public DateTime? LabelCreatedAtUtc { get; set; }

        /// <summary>Taşıyıcının paketi teslim aldığı an (UTC). SubOrders.ShippedAtUtc ile hizalanır.</summary>
        public DateTime? ShippedAtUtc { get; set; }

        /// <summary>Teslim edildiği an (UTC). SubOrders.DeliveredAtUtc ve Faz 2 escrow hesabının girdisidir.</summary>
        public DateTime? DeliveredAtUtc { get; set; }

        /// <summary>Sipariş anında hesaplanan tahmini teslimat aralığı — başlangıç (UTC).</summary>
        public DateTime? EstimatedDeliveryFromUtc { get; set; }

        /// <summary>Sipariş anında hesaplanan tahmini teslimat aralığı — bitiş (UTC).</summary>
        public DateTime? EstimatedDeliveryToUtc { get; set; }

        /// <summary>Kargo firmasının sonradan bildirdiği tahmini teslimat (UTC). Snapshot'ı EZMEZ;
        /// müşteriye ikisinden güncel olan gösterilir.</summary>
        public DateTime? CarrierEstimatedDeliveryUtc { get; set; }

        // ---------------- TESLİMAT KANITI ----------------

        /// <summary>Teslimat kanıtı tipi.</summary>
        public DeliveryProofType ProofType { get; set; } = DeliveryProofType.None;

        /// <summary>Teslimatı teslim alan kişinin adı (taşıyıcıdan gelir).</summary>
        public string? DeliveredToName { get; set; }

        /// <summary>Teslimat kanıtı dosyası — imza görüntüsü veya teslimat fotoğrafı (MediaItems.Id).</summary>
        public Guid? ProofMediaItemId { get; set; }

        /// <summary>Teslimat anındaki konum bilgisi (enlem,boylam veya adres metni). Taşıyıcı sağlıyorsa.</summary>
        public string? DeliveryLocationInfo { get; set; }

        /// <summary>Teslim denemesi sayısı. KAYNAK: ShipmentTrackingEvents içindeki DeliveryAttemptFailed olayları.</summary>
        public int DeliveryAttemptCount { get; set; }

        /// <summary>Serbest not (satıcı/operasyon notu).</summary>
        public string? Note { get; set; }

        // ---------------- TAKİP SORGULAMA (poll) LEASE DESENİ ----------------

        /// <summary>Bir sonraki takip sorgulama zamanı (UTC). Poll tipi entegrasyonlarda
        /// BackgroundService bu alana göre sıraya girer; teslim edilenler taranmaz.</summary>
        public DateTime? NextPollAtUtc { get; set; }

        /// <summary>Son başarılı takip sorgulaması (UTC).</summary>
        public DateTime? LastPolledAtUtc { get; set; }

        /// <summary>Lease sahibi işleyici kimliği. Çok örnekli BackgroundService'te aynı gönderiyi
        /// iki işleyicinin sorgulamasını engeller.</summary>
        public string? LeasedBy { get; set; }

        /// <summary>Lease bitiş zamanı (UTC).</summary>
        public DateTime? LeasedUntilUtc { get; set; }

        /// <summary>Ardışık sorgulama hata sayısı. Eşiği aşınca poll aralığı uzatılır ve operasyona bildirilir.</summary>
        public int PollFailureCount { get; set; }

        /// <summary>Global tekil idempotency anahtarı. Deterministik: "shp:{subOrderId}:{packageIndex}";
        /// aynı paketin çift oluşturulması DB seviyesinde engellenir.</summary>
        public string IdempotencyKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// SEVKİYAT KALEMİ. Bir alt sipariş birden çok pakete bölündüğünde hangi kalemin hangi
    /// pakette, kaç adet gittiğini tutar. Kısmi sevkiyatta kalemin bir kısmı bir pakette,
    /// kalanı sonraki pakette olabilir.
    ///
    /// DOĞRULAMA: Bir OrderItem için tüm sevkiyat satırlarındaki Quantity toplamı,
    /// OrderItems.Quantity değerini AŞAMAZ (servis katmanı doğrular).
    /// </summary>
    public class ShipmentItems : ShippingEntityBase
    {
        /// <summary>Bağlı sevkiyat (Shipments.Id).</summary>
        public Guid ShipmentId { get; set; }

        /// <summary>Sevk edilen sipariş kalemi (OrderItems.Id). Transfer sevkiyatlarında null.</summary>
        public Guid? OrderItemId { get; set; }

        /// <summary>Varyant izi (ProductVariants.Id). DENORMALIZE: kaynak OrderItems.VariantId;
        /// toplama listesi (picking list) için join'siz erişim.</summary>
        public Guid VariantId { get; set; }

        /// <summary>Bu paketteki adet.</summary>
        public int Quantity { get; set; }

        /// <summary>Snapshot: ürün adı (paket içerik listesi/irsaliye çıktısı için).</summary>
        public string? ProductNameSnapshot { get; set; }

        /// <summary>Snapshot: SKU.</summary>
        public string? SkuSnapshot { get; set; }
    }

    /// <summary>
    /// SEVKİYAT TAKİP OLAYLARI (append-only). Taşıyıcıdan webhook veya poll ile gelen her
    /// durum bildirimi buraya YENİ SATIR olarak yazılır; satırlar ASLA güncellenmez veya silinmez.
    ///
    /// İDEMPOTENCY: Aynı olay hem webhook hem poll ile gelebilir veya webhook birden çok kez
    /// tekrarlanabilir. IdempotencyKey deterministik üretilir
    /// ("trk:{shipmentId}:{carrierEventId}" veya carrierEventId yoksa
    /// "trk:{shipmentId}:{rawStatusCode}:{occurredAt:yyyyMMddHHmm}") ve tekil indeksle korunur.
    ///
    /// Shipments.Status bu olaylardan TÜRETİLİR: servis katmanı, olayın EventType değerini
    /// normalize durum makinesine eşler ve yalnızca ileri yönlü geçişleri uygular
    /// (geç gelen eski olay, Delivered durumunu InTransit'e geri çekemez).
    /// </summary>
    public class ShipmentTrackingEvents : ShippingEntityBase
    {
        /// <summary>Bağlı sevkiyat (Shipments.Id).</summary>
        public Guid ShipmentId { get; set; }

        /// <summary>Normalize olay tipi.</summary>
        public TrackingEventType EventType { get; set; } = TrackingEventType.Unknown;

        /// <summary>Olayın taşıyıcı sisteminde gerçekleştiği an (UTC).
        /// DİKKAT: Bu, kaydın oluşturulma anı (CreatedAtUtc) DEĞİLDİR; sıralama bu alana göre yapılır.</summary>
        public DateTime OccurredAtUtc { get; set; }

        /// <summary>Taşıyıcının ham durum kodu. EventType=Unknown ise eşleme tablosuna eklenmesi gereken koddur.</summary>
        public string? RawStatusCode { get; set; }

        /// <summary>Taşıyıcının insan-okur açıklaması ("Şubeye teslim edildi").</summary>
        public string? Description { get; set; }

        /// <summary>Olayın gerçekleştiği yer (şube/şehir metni).</summary>
        public string? LocationName { get; set; }

        /// <summary>Taşıyıcı tarafındaki olay kimliği (varsa). İdempotency anahtarının parçasıdır.</summary>
        public string? CarrierEventId { get; set; }

        /// <summary>Olayın kaynağı webhook mu? false = poll ile çekildi. Hata ayıklama ve
        /// entegrasyon sağlığı raporu için.</summary>
        public bool IsFromWebhook { get; set; }

        /// <summary>Taşıyıcıdan gelen ham yanıt (JSON/XML). Uyuşmazlıkta tek kanıt kaynağıdır.</summary>
        public string? RawPayload { get; set; }

        /// <summary>Global tekil idempotency anahtarı (yukarıdaki XML doc'ta üretim kuralı).</summary>
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}