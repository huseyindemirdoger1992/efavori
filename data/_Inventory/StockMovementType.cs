namespace data._Inventory
{
    // ============================================================================================
    // STOK DEFTERİ VE ÇOKLU DEPO SİSTEMİ V1 — ENUM SÖZLÜĞÜ
    // KURAL: Enum değerleri byte olarak saklanır. Mevcut değerlerin ARASINA yeni değer EKLENMEZ,
    // yeni değerler her zaman SONA eklenir. Değer silinmez, yeniden adlandırılmaz.
    // ============================================================================================

    /// <summary>
    /// Stok hareket tipi (StockMovements.MovementType).
    ///
    /// İŞARET KURALI: Hareketin yönü Quantity alanının işaretindedir (±), enum'da değil.
    /// Aşağıdaki açıklamalar TİPİK yönü belirtir ve hangi bakiyeyi (OnHand / Reserved)
    /// etkilediğini gösterir. Bu ayrım kritiktir: rezervasyon fiziksel stoğu azaltmaz,
    /// yalnızca taahhüt eder.
    /// </summary>
    public enum StockMovementType : byte
    {
        /// <summary>Sipariş rezervasyonu (Reserved +, OnHand değişmez). Kaynak: StockReservationId.</summary>
        OrderReserve = 0,

        /// <summary>Rezervasyonun fiili sevkiyata dönmesi (Reserved −, OnHand −). Kaynak: SubOrderId/OrderItemId.</summary>
        OrderCommit = 1,

        /// <summary>Rezervasyonun serbest bırakılması — iptal veya süre aşımı (Reserved −, OnHand değişmez).</summary>
        OrderRelease = 2,

        /// <summary>Müşteri iadesinin depoya girişi (OnHand +). Kaynak: ReturnRequestId (Faz 7).</summary>
        ReturnIn = 3,

        /// <summary>Toplu içe aktarımla stok girişi (OnHand +). Kaynak: ImportRowId.</summary>
        ImportIn = 4,

        /// <summary>Manuel düzeltme (OnHand ±). Sayım farkı, hatalı giriş düzeltmesi. Description ZORUNLU.</summary>
        ManualAdjust = 5,

        /// <summary>Depolar arası transfer — çıkış deposu (OnHand −). Kaynak: StockTransferId.</summary>
        TransferOut = 6,

        /// <summary>Depolar arası transfer — giriş deposu (OnHand +). Kaynak: StockTransferId.</summary>
        TransferIn = 7,

        /// <summary>Hasar/fire/kayıp (OnHand −).</summary>
        Damage = 8,

        /// <summary>Tedarikçiden mal kabulü (OnHand +).</summary>
        PurchaseIn = 9,

        /// <summary>Fiziksel sayım sonucu mutabakat (OnHand ±). Sayım sonrası bakiye eşitleme.</summary>
        StockCount = 10,

        /// <summary>Transferin yolda kaybı/reddi sonrası çıkış deposuna geri alma (OnHand +).</summary>
        TransferReturn = 11
    }

    /// <summary>
    /// Stok rezervasyonu durum makinesi (StockReservations.Status).
    /// Geçişler: Active → Committed / Released / Expired.
    /// </summary>
    public enum StockReservationStatus : byte
    {
        /// <summary>Aktif rezervasyon; ExpiresAtUtc dolana kadar stok taahhüt edilmiştir.</summary>
        Active = 0,

        /// <summary>Sipariş kargolandı/kesinleşti; rezervasyon fiili stok düşümüne dönüştü (OrderCommit).</summary>
        Committed = 1,

        /// <summary>Sipariş iptali veya checkout iptali ile serbest bırakıldı (OrderRelease).</summary>
        Released = 2,

        /// <summary>ExpiresAtUtc geçti; BackgroundService serbest bıraktı (OrderRelease).</summary>
        Expired = 3
    }

    /// <summary>
    /// Depolar arası transfer durum makinesi (StockTransfers.Status).
    /// Akış: Requested → Approved → Shipped → Received (veya Rejected / Cancelled).
    /// Stok hareketleri: Shipped anında TransferOut, Received anında TransferIn yazılır.
    /// Yoldaki mal iki deponun da OnHand'inde GÖRÜNMEZ (bilinçli; InTransitQuantity ile izlenir).
    /// </summary>
    public enum StockTransferStatus : byte
    {
        /// <summary>Transfer talebi oluşturuldu, onay bekliyor.</summary>
        Requested = 0,

        /// <summary>Talep onaylandı, sevkiyat bekleniyor.</summary>
        Approved = 1,

        /// <summary>Mal çıkış deposundan sevk edildi (TransferOut yazıldı); yolda.</summary>
        Shipped = 2,

        /// <summary>Varış deposunda teslim alındı (TransferIn yazıldı); transfer tamamlandı.</summary>
        Received = 3,

        /// <summary>Talep reddedildi (sevkiyat yapılmadı).</summary>
        Rejected = 4,

        /// <summary>Sevkiyat öncesi iptal edildi.</summary>
        Cancelled = 5,

        /// <summary>Kısmen teslim alındı (bazı kalemler eksik/hasarlı geldi); fark ManualAdjust veya Damage ile kapatılır.</summary>
        PartiallyReceived = 6
    }
}
