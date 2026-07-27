namespace data._Inventory
{
    /// <summary>
    /// VARYANT-DEPO STOK BAKİYESİ. Sistemdeki tek "canlı bakiye" tablosudur;
    /// (VariantId, WarehouseId) çifti TEKİLDİR.
    ///
    /// KAVRAMLAR:
    ///   OnHand    = depoda fiziksel olarak bulunan adet.
    ///   Reserved  = sipariş/checkout için taahhüt edilmiş, henüz sevk edilmemiş adet.
    ///   Available = OnHand − Reserved  (SATILABİLİR adet — kolon AÇILMAZ, sorguda hesaplanır).
    ///
    /// Available'ın kolon olarak tutulmaması bilinçlidir: iki kolonun tutarlılığını korumak
    /// yerine tek doğruluk kaynağı bırakılır; çifte satış önleme koşulu doğrudan
    /// "OnHand - Reserved >= @qty" biçiminde yazılır (README §6).
    ///
    /// MEVCUT YAPIYLA İLİŞKİ: ProductVariants.StockQuantity, bu tablodaki OnHand toplamının
    /// DENORMALIZE kopyası olarak KALIR (kırıcı değişiklik yapılmaz); BackgroundService senkron eder.
    /// Satış kararı ASLA ProductVariants.StockQuantity'ye bakılarak verilmez — o alan yalnızca
    /// listeleme/vitrin içindir.
    /// </summary>
    public class VariantWarehouseStock : InventoryEntityBase
    {
        /// <summary>Varyant (ProductVariants.Id). Basit üründe IsDefault=true tek varyant.</summary>
        public Guid VariantId { get; set; }

        /// <summary>Depo (WareHouse.Id).</summary>
        public Guid WarehouseId { get; set; }

        /// <summary>Ürün izi (Products.Id). DENORMALIZE: kaynak ProductVariants.ProductId;
        /// ürün bazlı toplam stok sorgularını join'siz yapmak için.</summary>
        public Guid ProductId { get; set; }

        /// <summary>Mağaza izi (Store.Id). DENORMALIZE: kaynak WareHouse.StoreId;
        /// satıcı bazlı stok raporlarını ve yetki filtresini join'siz yapmak için.</summary>
        public Guid StoreId { get; set; }

        /// <summary>Fiziksel mevcut adet. Negatif olamaz (servis katmanı ve CHECK kısıtı korur).</summary>
        public int OnHand { get; set; }

        /// <summary>Rezerve edilmiş adet. KAYNAK: Active durumdaki StockReservations toplamı;
        /// rezervasyon/serbest bırakma akışlarında koşullu UPDATE ile güncellenir.</summary>
        public int Reserved { get; set; }

        /// <summary>Yeniden sipariş noktası. OnHand bu değerin altına düşünce satıcıya
        /// düşük stok bildirimi üretilir (Faz 6 Notifications: StockAlert).</summary>
        public int ReorderPoint { get; set; }

        /// <summary>Depoya yolda olan adet (Shipped durumundaki transferlerin varış tarafı).
        /// KAYNAK: StockTransferItems; bilgi amaçlıdır, Available hesabına GİRMEZ.</summary>
        public int InTransitQuantity { get; set; }

        /// <summary>Bu depodaki rafyeri/lokasyon kodu ("A-12-3"). Toplama (picking) kolaylığı için.</summary>
        public string? BinLocation { get; set; }

        /// <summary>Son fiziksel sayım tarihi (UTC).</summary>
        public DateTime? LastCountedAtUtc { get; set; }

        /// <summary>Bu depo-varyant satırı satışa açık mı? false = stok var ama satılmaz
        /// (örn. vitrin ürünü, karantina). Available hesabında bu satır dışlanır.</summary>
        public bool IsSellable { get; set; } = true;
    }

    /// <summary>
    /// STOK HAREKET DEFTERİ (append-only). VariantWarehouseStock'taki her bakiye değişiminin
    /// gerekçesidir. Satırlar ASLA güncellenmez veya silinmez; hatalı hareket ters kayıtla
    /// (ManualAdjust) kapatılır — SellerLedgerEntries ile aynı felsefe.
    ///
    /// MUTABAKAT GARANTİSİ: Belirli bir (VariantId, WarehouseId) için
    ///   SUM(Quantity) WHERE hareket OnHand'i etkiliyor  ==  VariantWarehouseStock.OnHand
    /// olmalıdır. Gecelik doğrulama görevi bu eşitliği kontrol eder ve sapma varsa Logs'a yazar.
    ///
    /// BalanceAfter alanları, hareket ANINDAKİ bakiyenin fotoğrafıdır: geçmişe dönük
    /// "3 ay önce bu depoda kaç adet vardı" sorusu defteri baştan toplamadan yanıtlanır.
    /// </summary>
    public class StockMovements : InventoryEntityBase
    {
        /// <summary>Varyant (ProductVariants.Id).</summary>
        public Guid VariantId { get; set; }

        /// <summary>Depo (WareHouse.Id).</summary>
        public Guid WarehouseId { get; set; }

        /// <summary>Ürün izi (Products.Id). DENORMALIZE: kaynak ProductVariants.ProductId.</summary>
        public Guid ProductId { get; set; }

        /// <summary>Mağaza izi (Store.Id). DENORMALIZE: kaynak WareHouse.StoreId.</summary>
        public Guid StoreId { get; set; }

        /// <summary>Hareket tipi.</summary>
        public StockMovementType MovementType { get; set; }

        /// <summary>İşaretli miktar: giriş (+) / çıkış (−). Örn. sevkiyat −3, iade girişi +1.</summary>
        public int Quantity { get; set; }

        /// <summary>Hareket sonrası fiziksel bakiye (VariantWarehouseStock.OnHand fotoğrafı).</summary>
        public int BalanceAfterOnHand { get; set; }

        /// <summary>Hareket sonrası rezerve bakiye (VariantWarehouseStock.Reserved fotoğrafı).</summary>
        public int BalanceAfterReserved { get; set; }

        // ---------------- KAYNAK REFERANSLARI (yalnızca ilgili olan doldurulur) ----------------

        /// <summary>Kaynak: alt sipariş (SubOrders.Id). OrderReserve/Commit/Release hareketlerinde.</summary>
        public Guid? SubOrderId { get; set; }

        /// <summary>Kaynak: sipariş kalemi (OrderItems.Id).</summary>
        public Guid? OrderItemId { get; set; }

        /// <summary>Kaynak: rezervasyon kaydı (StockReservations.Id).</summary>
        public Guid? StockReservationId { get; set; }

        /// <summary>Kaynak: içe aktarım satırı (data._BulkImportProducts ImportRow.Id). ImportIn hareketlerinde.</summary>
        public Guid? ImportRowId { get; set; }

        /// <summary>Kaynak: iade talebi (Faz 7 ReturnRequests.Id). ReturnIn hareketlerinde.
        /// FK Faz 7'de tanımlanır; şimdilik iz alanıdır.</summary>
        public Guid? ReturnRequestId { get; set; }

        /// <summary>Kaynak: depo transferi (StockTransfers.Id). TransferOut/In/Return hareketlerinde.</summary>
        public Guid? StockTransferId { get; set; }

        /// <summary>İnsan-okur açıklama. ManualAdjust ve Damage hareketlerinde ZORUNLU tutulmalıdır (servis katmanı doğrular).</summary>
        public string? Description { get; set; }

        /// <summary>Global tekil idempotency anahtarı. Deterministik üretilir
        /// (örn. "commit:{orderItemId}", "reserve:{reservationId}", "transferout:{transferItemId}");
        /// aynı olayın çift hareket yazması DB seviyesinde engellenir — retry'lar güvenlidir.</summary>
        public string IdempotencyKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// STOK REZERVASYONU. Checkout başlangıcında (Faz 1 CheckoutSessions) veya sipariş
    /// oluşturulduğunda stok TAAHHÜT edilir; fiziksel düşüm sevkiyatta yapılır.
    ///
    /// YAŞAM DÖNGÜSÜ:
    ///   Active    → rezervasyon kuruldu, VariantWarehouseStock.Reserved arttı.
    ///   Committed → sipariş kargolandı; Reserved azaldı, OnHand azaldı (OrderCommit hareketi).
    ///   Released  → sipariş/checkout iptal; Reserved azaldı, OnHand değişmedi (OrderRelease).
    ///   Expired   → ExpiresAtUtc geçti; BackgroundService Released ile aynı etkiyi uygular.
    ///
    /// SÜRESİ DOLANLARI ÇÖZEN AKIŞ ve ProductVariants.ReservedQuantity SENKRONU README §5'tedir.
    /// </summary>
    public class StockReservations : InventoryEntityBase
    {
        /// <summary>Varyant (ProductVariants.Id).</summary>
        public Guid VariantId { get; set; }

        /// <summary>Rezervasyonun yapıldığı depo (WareHouse.Id). Depo seçimi servis katmanında
        /// (müşteriye en yakın / stoğu yeten depo) çözülür ve burada sabitlenir.</summary>
        public Guid WarehouseId { get; set; }

        /// <summary>Rezerve edilen adet (pozitif).</summary>
        public int Quantity { get; set; }

        /// <summary>Durum makinesi.</summary>
        public StockReservationStatus Status { get; set; } = StockReservationStatus.Active;

        /// <summary>Rezervasyonun geçerlilik sonu (UTC). Checkout rezervasyonlarında kısa (örn. +30 dk,
        /// CheckoutSessions.ExpiresAtUtc ile hizalı); sipariş rezervasyonlarında uzun (örn. +7 gün,
        /// kargolanma süresi) tutulur.</summary>
        public DateTime ExpiresAtUtc { get; set; }

        /// <summary>Kaynak: checkout oturumu (CheckoutSessions.Id). Sepet aşaması rezervasyonu.</summary>
        public Guid? CheckoutSessionId { get; set; }

        /// <summary>Kaynak: alt sipariş (SubOrders.Id). Sipariş oluştuktan sonraki rezervasyon.</summary>
        public Guid? SubOrderId { get; set; }

        /// <summary>Kaynak: sipariş kalemi (OrderItems.Id).</summary>
        public Guid? OrderItemId { get; set; }

        /// <summary>Rezervasyonun çözüldüğü an (UTC) — Committed/Released/Expired geçişi.</summary>
        public DateTime? ResolvedAtUtc { get; set; }

        /// <summary>Global tekil idempotency anahtarı. Deterministik: "resv:{checkoutSessionId}:{variantId}"
        /// veya "resv:{orderItemId}"; aynı kalem için çift rezervasyon DB seviyesinde engellenir.</summary>
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}