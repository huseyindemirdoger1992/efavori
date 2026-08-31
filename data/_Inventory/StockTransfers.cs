namespace data._Inventory
{
    /// <summary>
    /// DEPOLAR ARASI TRANSFER — BAŞLIK. Talep → onay → sevk → kabul akışını yürütür.
    ///
    /// STOK ETKİSİ:
    ///   Shipped  anında: çıkış deposunda TransferOut hareketi (OnHand −), varış deposunda
    ///                    InTransitQuantity artar (OnHand'e GİRMEZ — mal henüz varmadı).
    ///   Received anında: varış deposunda TransferIn hareketi (OnHand +), InTransitQuantity azalır.
    /// Yoldaki mal hiçbir deponun satılabilir stoğunda görünmez; bu bilinçlidir, aksi halde
    /// varmamış mal satılır.
    ///
    /// FARKLAR: Sevk edilen ile teslim alınan adet tutmazsa transfer PartiallyReceived olur;
    /// fark, varış deposunda ManualAdjust veya Damage hareketiyle açıkça kapatılır — sessizce
    /// eşitlenmez, çünkü kayıp/hasar denetim konusudur.
    ///
    /// YETKİ: Transfer yalnızca AYNI mağazanın depoları arasında yapılır (StoreId tekildir);
    /// mağazalar arası stok aktarımı bir satış/satın alma işlemidir, transfer değildir.
    /// </summary>
    public class StockTransfers : InventoryEntityBase
    {
        /// <summary>İnsan-okur transfer numarası ("TRF-2026-000045"). Faz 1'deki
        /// OrderNumberSequences ile aynı atomik UPDATE stratejisiyle üretilir.</summary>
        public string TransferNumber { get; set; } = string.Empty;

        /// <summary>Transferi yapan mağaza (Store.Id). Kaynak ve varış deposu bu mağazaya ait olmalıdır.</summary>
        public Guid StoreId { get; set; }

        /// <summary>Çıkış deposu (WareHouse.Id).</summary>
        public Guid SourceWarehouseId { get; set; }

        /// <summary>Varış deposu (WareHouse.Id). SourceWarehouseId'den farklı olmalıdır (servis katmanı doğrular).</summary>
        public Guid TargetWarehouseId { get; set; }

        /// <summary>Durum makinesi.</summary>
        public StockTransferStatus Status { get; set; } = StockTransferStatus.Requested;

        /// <summary>Onaylandığı an (UTC).</summary>
        public DateTime? ApprovedAtUtc { get; set; }

        /// <summary>Onaylayan kullanıcı (Users.Id).</summary>
        public Guid? ApprovedByUserId { get; set; }

        /// <summary>Sevk edildiği an (UTC). TransferOut hareketleri bu anda yazılır.</summary>
        public DateTime? ShippedAtUtc { get; set; }

        /// <summary>Teslim alındığı an (UTC). TransferIn hareketleri bu anda yazılır.</summary>
        public DateTime? ReceivedAtUtc { get; set; }

        /// <summary>Sevkiyat takip numarası (kendi araçlarıyla taşınıyorsa boş bırakılır).
        /// Kargo firmasıyla taşınıyorsa Faz 5 Shipments ile ilişkilendirilebilir.</summary>
        public string? TrackingNumber { get; set; }

        /// <summary>Reddedilme/iptal gerekçesi.</summary>
        public string? RejectionReason { get; set; }

        /// <summary>Transfer notu.</summary>
        public string? Note { get; set; }

        /// <summary>Toplam kalem çeşidi. DENORMALIZE sayaç; KAYNAK: StockTransferItems satır sayısı.</summary>
        public int ItemCount { get; set; }

        /// <summary>Toplam sevk edilen adet. DENORMALIZE sayaç; KAYNAK: StockTransferItems.ShippedQuantity toplamı.</summary>
        public int TotalShippedQuantity { get; set; }

        /// <summary>Toplam teslim alınan adet. DENORMALIZE sayaç; KAYNAK: StockTransferItems.ReceivedQuantity toplamı.</summary>
        public int TotalReceivedQuantity { get; set; }
    }

    /// <summary>
    /// DEPOLAR ARASI TRANSFER — KALEM. Hangi varyanttan kaç adet talep edildiği, sevk edildiği
    /// ve teslim alındığı ayrı ayrı izlenir; üç sayının farkı kayıp/hasar denetiminin girdisidir.
    /// </summary>
    public class StockTransferItems : InventoryEntityBase
    {
        /// <summary>Bağlı transfer (StockTransfers.Id).</summary>
        public Guid StockTransferId { get; set; }

        /// <summary>Varyant (ProductVariants.Id).</summary>
        public Guid VariantId { get; set; }

        /// <summary>Ürün izi (Products.Id). DENORMALIZE: kaynak ProductVariants.ProductId.</summary>
        public Guid ProductId { get; set; }

        /// <summary>Talep edilen adet.</summary>
        public int RequestedQuantity { get; set; }

        /// <summary>Fiilen sevk edilen adet (çıkış deposunda stok yetmemiş olabilir).</summary>
        public int ShippedQuantity { get; set; }

        /// <summary>Fiilen teslim alınan adet. ShippedQuantity'den azsa fark açıkça kapatılır (bkz. StockTransfers XML doc).</summary>
        public int ReceivedQuantity { get; set; }

        /// <summary>Hasarlı gelen adet (ReceivedQuantity'nin alt kümesi; Damage hareketiyle düşülür).</summary>
        public int DamagedQuantity { get; set; }

        /// <summary>Snapshot: transfer anındaki SKU (ürün sonradan değişse bile döküm okunabilir kalır).</summary>
        public string? SkuSnapshot { get; set; }

        /// <summary>Kalem notu (örn. "2 adet kutusu ezik").</summary>
        public string? Note { get; set; }
    }
}
