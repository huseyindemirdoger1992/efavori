using data._Products;

namespace data._Returns
{
    /// <summary>
    /// İADE TALEBİ (BAŞLIK). SubOrder bazlıdır; hangi kalemlerden kaç adet iade edildiği
    /// ReturnRequestItems satırlarındadır.
    ///
    /// TASARIM NOTU — neden başlık + kalem (şartnamedeki "OrderItem bazlı" ifadesinden sapma):
    /// Müşteri tek seferde birden çok kalemi iade eder ve bunlar TEK KOLİDE geri gider.
    /// Talebi doğrudan OrderItem'a bağlamak: (a) tek iade kargosunu N talebe bölerdi
    /// (Faz 5 Shipments bire-bir bağlanamazdı), (b) tek ihtilafı N ihtilafa bölerdi,
    /// (c) müşteriye N ayrı durum akışı gösterirdi. Kısmi adet iadesi kalem satırında
    /// (ReturnRequestItems.Quantity) zaten desteklenmektedir.
    /// Faz 2 Refunds ise OrderItem bazlı kalır — finansal kayıt kalem düzeyindedir.
    /// </summary>
    public class ReturnRequests : ReturnEntityBase
    {
        /// <summary>İnsan-okur iade numarası ("IAD-2026-000123"). Faz 1 OrderNumberSequences ile
        /// aynı atomik UPDATE stratejisiyle üretilir.</summary>
        public string ReturnNumber { get; set; } = string.Empty;

        /// <summary>Bağlı alt sipariş (SubOrders.Id). İade daima tek mağazaya karşıdır.</summary>
        public Guid SubOrderId { get; set; }

        /// <summary>Üst sipariş izi (Orders.Id). DENORMALIZE: kaynak SubOrders.OrderId.</summary>
        public Guid OrderId { get; set; }

        /// <summary>Satıcı mağaza izi (Store.Id). DENORMALIZE: kaynak SubOrders.StoreId; satıcı paneli filtresi.</summary>
        public Guid StoreId { get; set; }

        /// <summary>Talebi açan kullanıcı (Users.Id). Misafir siparişte null.</summary>
        public Guid? UserId { get; set; }

        /// <summary>Misafir iadelerinde iletişim e-postası.</summary>
        public string? GuestEmail { get; set; }

        /// <summary>Durum makinesi.</summary>
        public ReturnRequestStatus Status { get; set; } = ReturnRequestStatus.Requested;

        /// <summary>Baskın iade nedeni (kalemlerden türetilir; listeleme ve raporlama kolaylığı için).
        /// DENORMALIZE: kaynak ReturnRequestItems.Reason.</summary>
        public ReturnReason PrimaryReason { get; set; } = ReturnReason.Unspecified;

        /// <summary>Müşterinin serbest açıklaması.</summary>
        public string? CustomerNote { get; set; }

        /// <summary>Satıcının onay/ret gerekçesi.</summary>
        public string? SellerNote { get; set; }

        /// <summary>Platform yöneticisinin notu (iç kullanım; müşteriye gösterilmez).</summary>
        public string? AdminNote { get; set; }

        // ---------------- TUTARLAR ----------------

        /// <summary>Talep edilen toplam iade tutarı. KAYNAK: ReturnRequestItems.RequestedAmount toplamı.</summary>
        public decimal RequestedAmount { get; set; }

        /// <summary>İnceleme sonrası onaylanan toplam iade tutarı. KAYNAK: ReturnRequestItems.ApprovedAmount toplamı.</summary>
        public decimal? ApprovedAmount { get; set; }

        /// <summary>Kargo bedelinin de iade edilip edilmeyeceği. Satıcı kusuru kaynaklı iadelerde
        /// müşterinin ödediği kargo ücreti de geri verilir.</summary>
        public bool RefundShippingCost { get; set; }

        /// <summary>Para birimi (alt siparişle aynı).</summary>
        public CurrencyCode Currency { get; set; }

        // ---------------- İADE KARGOSU ----------------

        /// <summary>İade kargo bedelini kim üstlenir.</summary>
        public ReturnShippingPayer ShippingPayer { get; set; } = ReturnShippingPayer.Undetermined;

        /// <summary>İade sevkiyatı (Faz 5 Shipments.Id, Direction=Return). Satıcı onayından sonra oluşturulur.</summary>
        public Guid? ReturnShipmentId { get; set; }

        /// <summary>Ürünün geri döneceği depo (WareHouse.Id). Faz 4 ReturnIn hareketinin hedefi.</summary>
        public Guid? TargetWarehouseId { get; set; }

        // ---------------- POLİTİKA SNAPSHOT ----------------

        /// <summary>Snapshot: siparişe uygulanan iade süresi (gün). Politika sonradan değişse bile
        /// bu talebin hak durumu tartışmasız kalır (README §6).</summary>
        public int AppliedReturnWindowDays { get; set; }

        /// <summary>Snapshot: çözümde kullanılan politika satırı izi (ReturnPolicies.Id).</summary>
        public Guid? AppliedReturnPolicyId { get; set; }

        /// <summary>İade hakkının son günü (UTC). Teslim tarihi + AppliedReturnWindowDays.</summary>
        public DateTime? ReturnDeadlineUtc { get; set; }

        // ---------------- ZAMAN ÇİZELGESİ ----------------

        /// <summary>Satıcının karar verdiği an (UTC).</summary>
        public DateTime? SellerRespondedAtUtc { get; set; }

        /// <summary>Satıcı yanıt vermezse otomatik onaylanacağı an (UTC).
        /// Süre dolduğunda BackgroundService talebi SellerApproved'a çeker — satıcı sessizliğiyle
        /// müşteriyi mağdur edemez.</summary>
        public DateTime? SellerResponseDeadlineUtc { get; set; }

        /// <summary>Ürünün teslim alındığı an (UTC).</summary>
        public DateTime? ReceivedAtUtc { get; set; }

        /// <summary>İncelemenin tamamlandığı an (UTC).</summary>
        public DateTime? InspectedAtUtc { get; set; }

        /// <summary>İadenin tamamlandığı an (UTC).</summary>
        public DateTime? CompletedAtUtc { get; set; }

        /// <summary>Bağlı ihtilaf (Disputes.Id). Talep ihtilafa taşındıysa dolu.</summary>
        public Guid? DisputeId { get; set; }

        /// <summary>Global tekil idempotency anahtarı. Deterministik: "ret:{subOrderId}:{istemciTalepId}";
        /// çift tıklamada iki iade talebi açılmasını DB seviyesinde engeller.</summary>
        public string IdempotencyKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// İADE TALEBİ KALEMİ. Hangi sipariş kaleminden kaç adet iade edildiğini, nedenini ve
    /// inceleme sonucunu tutar. Kısmi adet iadesi burada desteklenir.
    ///
    /// DOĞRULAMA: Bir OrderItem için tüm iade taleplerindeki Quantity toplamı,
    /// OrderItems.Quantity − OrderItems.RefundedQuantity değerini AŞAMAZ (servis katmanı doğrular).
    /// </summary>
    public class ReturnRequestItems : ReturnEntityBase
    {
        /// <summary>Bağlı iade talebi (ReturnRequests.Id).</summary>
        public Guid ReturnRequestId { get; set; }

        /// <summary>İade edilen sipariş kalemi (OrderItems.Id).</summary>
        public Guid OrderItemId { get; set; }

        /// <summary>Varyant izi (ProductVariants.Id). DENORMALIZE: kaynak OrderItems.VariantId;
        /// stok hareketi ve toplama listesi için join'siz erişim.</summary>
        public Guid VariantId { get; set; }

        /// <summary>Ürün izi (Products.Id). DENORMALIZE: kaynak OrderItems.ProductId; iade oranı raporları için.</summary>
        public Guid ProductId { get; set; }

        /// <summary>İade edilmek istenen adet.</summary>
        public int Quantity { get; set; }

        /// <summary>İnceleme sonrası kabul edilen adet. Null = henüz incelenmedi.</summary>
        public int? ApprovedQuantity { get; set; }

        /// <summary>Kalem bazlı iade nedeni.</summary>
        public ReturnReason Reason { get; set; }

        /// <summary>Müşterinin bu kaleme dair açıklaması.</summary>
        public string? Description { get; set; }

        /// <summary>Snapshot: ürün adı (talep listesinde göstermek için; ürün silinse bile okunur kalır).</summary>
        public string? ProductNameSnapshot { get; set; }

        /// <summary>Snapshot: SKU.</summary>
        public string? SkuSnapshot { get; set; }

        /// <summary>Talep edilen iade tutarı (birim fiyat × adet − orantılı indirim/kupon payı).</summary>
        public decimal RequestedAmount { get; set; }

        /// <summary>İnceleme sonrası onaylanan tutar. Kısmi kesinti uygulanabilir (eksik parça, kullanım izi).</summary>
        public decimal? ApprovedAmount { get; set; }

        /// <summary>Uygulanan kesinti tutarı ve gerekçesinin açıklaması.</summary>
        public string? DeductionNote { get; set; }

        /// <summary>Para birimi.</summary>
        public CurrencyCode Currency { get; set; }

        /// <summary>İnceleme sonucu. Faz 4 stok hareketini belirler:
        /// SellableAsNew/OpenBox → ReturnIn (OnHand +), Damaged → Damage (fire).</summary>
        public ReturnInspectionResult InspectionResult { get; set; } = ReturnInspectionResult.NotInspected;

        /// <summary>Üretilen stok hareketi izi (Faz 4 StockMovements.Id).</summary>
        public Guid? StockMovementId { get; set; }

        /// <summary>Üretilen finansal iade kaydı izi (Faz 2 Refunds.Id).</summary>
        public Guid? RefundId { get; set; }
    }

    /// <summary>
    /// İADE TALEBİ MEDYA EKİ. Müşterinin sunduğu hasar fotoğrafı/videosu veya satıcının
    /// inceleme sırasında çektiği kanıt görselleri. Bir talep/kalem birden çok ek taşıyabilir.
    ///
    /// İhtilafta kanıt olarak kullanıldığından silinmez (soft-delete dışında) ve
    /// yükleyen taraf bilgisi korunur.
    /// </summary>
    public class ReturnRequestMedia : ReturnEntityBase
    {
        /// <summary>Bağlı iade talebi (ReturnRequests.Id).</summary>
        public Guid ReturnRequestId { get; set; }

        /// <summary>Bağlı kalem (ReturnRequestItems.Id). Null = talebin geneline ait ek.</summary>
        public Guid? ReturnRequestItemId { get; set; }

        /// <summary>Medya dosyası (MediaItems.Id).</summary>
        public Guid MediaItemId { get; set; }

        /// <summary>Eki yükleyen taraf.</summary>
        public ReturnActorType UploadedByActorType { get; set; }

        /// <summary>Ek açıklaması ("kutunun ezik hali", "eksik kablo").</summary>
        public string? Caption { get; set; }

        /// <summary>Gösterim sırası.</summary>
        public int SortOrder { get; set; }
    }

    /// <summary>
    /// İADE DURUM TARİHÇESİ (append-only). ReturnRequests üzerindeki her durum geçişi buraya
    /// YENİ SATIR olarak yazılır; satırlar ASLA güncellenmez veya silinmez
    /// (Faz 1 OrderStatusHistory ile aynı felsefe).
    /// İhtilaf halinde tarafların iddialarını doğrulayan birincil kanıttır.
    /// </summary>
    public class ReturnStatusHistory : ReturnEntityBase
    {
        /// <summary>Bağlı iade talebi (ReturnRequests.Id).</summary>
        public Guid ReturnRequestId { get; set; }

        /// <summary>Önceki durum. İlk kayıt (talep açılışı) için null.</summary>
        public ReturnRequestStatus? FromStatus { get; set; }

        /// <summary>Yeni durum.</summary>
        public ReturnRequestStatus ToStatus { get; set; }

        /// <summary>Geçişi yapan taraf. Kişi izi: taban sınıftaki CreatedByUserId.</summary>
        public ReturnActorType ActorType { get; set; }

        /// <summary>Serbest not ("Ürün eksik parçayla geldi, %20 kesinti uygulandı").</summary>
        public string? Note { get; set; }

        /// <summary>Bu geçişi tetikleyen ihtilaf kararı izi (Disputes.Id), varsa.</summary>
        public Guid? DisputeId { get; set; }
    }
}
