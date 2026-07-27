using data._Products;

namespace data._Orders
{
    /// <summary>
    /// SİPARİŞ DURUM TARİHÇESİ (append-only). Orders ve SubOrders üzerindeki her durum geçişi
    /// buraya YENİ SATIR olarak yazılır; mevcut satırlar ASLA güncellenmez veya silinmez
    /// (AiGenerationHistory ile aynı append-only felsefe).
    /// "Kim" bilgisi: ActorType + taban sınıftaki CreatedByUserId. "Ne zaman": CreatedAtUtc.
    /// </summary>
    public class OrderStatusHistory : OrderEntityBase
    {
        /// <summary>Üst sipariş (Orders.Id). Geçiş üst sipariş seviyesindeyse dolu; SubOrder geçişlerinde de iz olarak doldurulur.</summary>
        public Guid OrderId { get; set; }

        /// <summary>Alt sipariş (SubOrders.Id). Üst sipariş seviyesindeki toplu geçişlerde null olabilir.</summary>
        public Guid? SubOrderId { get; set; }

        /// <summary>Önceki durum. İlk kayıt (sipariş oluşturma) için null.</summary>
        public OrderStatus? FromStatus { get; set; }

        /// <summary>Yeni durum.</summary>
        public OrderStatus ToStatus { get; set; }

        /// <summary>Geçişi yapan aktör tipi (System/Customer/Seller/Admin). Kişi izi: CreatedByUserId.</summary>
        public OrderActorType ActorType { get; set; }

        /// <summary>Geçişe iptal eşlik ediyorsa gerekçe.</summary>
        public OrderCancellationReason? CancellationReason { get; set; }

        /// <summary>Serbest not (örn. "Kargo firması teslim edemedi, 2. deneme").</summary>
        public string? Note { get; set; }
    }

    /// <summary>
    /// SİPARİŞ FATURASI. SubOrder bazlıdır (her mağaza kendi faturasını keser).
    /// PDF çıktısı MediaItems'a, e-fatura kanalı StoreIntegration'a bağlanır.
    /// Fatura numarası mağaza içinde (StoreId, InvoiceSeries, InvoiceNumber) tekildir.
    /// </summary>
    public class OrderInvoices : OrderEntityBase
    {
        /// <summary>Bağlı alt sipariş (SubOrders.Id).</summary>
        public Guid SubOrderId { get; set; }

        /// <summary>Faturayı kesen mağaza (Store.Id). DENORMALIZE: kaynak SubOrders.StoreId; seri/no tekilliği bu alanla kurulur.</summary>
        public Guid StoreId { get; set; }

        /// <summary>Fatura serisi (örn. "EFA").</summary>
        public string InvoiceSeries { get; set; } = string.Empty;

        /// <summary>Seri içindeki fatura numarası (mağaza + seri kapsamında artan).</summary>
        public long InvoiceNumber { get; set; }

        /// <summary>Görüntülenecek tam numara ("EFA2026000000123"). Servis katmanı üretir.</summary>
        public string InvoiceFullNumber { get; set; } = string.Empty;

        /// <summary>Fatura düzenlenme tarihi (UTC).</summary>
        public DateTime IssuedAtUtc { get; set; }

        /// <summary>Fatura toplam tutarı (SubOrders.GrandTotal snapshot'ı; sonradan alt sipariş değişse bile fatura sabittir).</summary>
        public decimal TotalAmount { get; set; }

        /// <summary>Para birimi.</summary>
        public CurrencyCode Currency { get; set; }

        /// <summary>Fatura PDF dosyası (MediaItems.Id).</summary>
        public Guid? PdfMediaItemId { get; set; }

        /// <summary>E-fatura entegrasyon durumu.</summary>
        public EInvoiceStatus EInvoiceStatus { get; set; } = EInvoiceStatus.NotCreated;

        /// <summary>Kullanılan e-fatura entegrasyonu (StoreIntegration.Id).</summary>
        public Guid? StoreIntegrationId { get; set; }

        /// <summary>Entegratör/GİB tarafındaki belge UUID'i.</summary>
        public string? EInvoiceUuid { get; set; }

        /// <summary>Entegratörden dönen ham yanıt (JSON). Hata ayıklama ve denetim izi için.</summary>
        public string? ProviderResponseJson { get; set; }
    }
}
