using data._Products;

namespace data._Payments
{
    /// <summary>
    /// KOMİSYON ORANLARI. Kapsam çözüm ÖNCELİK SIRASI (en özelden genele):
    ///   1) StoreId + CategoryId dolu  → mağaza+kategori özel oranı
    ///   2) StoreId dolu               → mağaza oranı
    ///   3) CategoryId dolu            → kategori oranı
    ///   4) ikisi de null              → global varsayılan
    /// Komisyon = komisyona esas tutar × RatePercent/100 + FixedAmount.
    /// TARİHÇE KORUNUR: satır ASLA güncellenmez; oran değişikliği = eski satıra ValidToUtc yazılır
    /// + YENİ satır açılır. Sipariş anında çözülen oran OrderItems.CommissionRate/Amount alanlarına
    /// snapshot'lanır; bu tablo yalnızca çözüm kaynağıdır.
    /// </summary>
    public class CommissionRates : PaymentEntityBase
    {
        /// <summary>Kapsam: mağaza (Store.Id). Null = tüm mağazalar.</summary>
        public Guid? StoreId { get; set; }

        /// <summary>Kapsam: kategori (CategoriesProduct.Id). Null = tüm kategoriler.
        /// Çözümde ürünün bağlı olduğu kategori + üst kategori zinciri sırayla denenir (en derin eşleşme kazanır).</summary>
        public Guid? CategoryId { get; set; }

        /// <summary>Yüzde bileşeni (örn. 12.50 = %12,5).</summary>
        public decimal RatePercent { get; set; }

        /// <summary>Sabit tutar bileşeni (işlem başına; 0 olabilir).</summary>
        public decimal FixedAmount { get; set; }

        /// <summary>Sabit tutarın para birimi. Yüzde bileşeni para biriminden bağımsızdır.</summary>
        public CurrencyCode Currency { get; set; }

        /// <summary>Geçerlilik başlangıcı (UTC).</summary>
        public DateTime ValidFromUtc { get; set; }

        /// <summary>Geçerlilik sonu (UTC). Null = halen geçerli. Yeni oran girildiğinde eski satıra yazılır.</summary>
        public DateTime? ValidToUtc { get; set; }

        /// <summary>Admin açıklaması ("2026 Q3 elektronik kampanya oranı").</summary>
        public string? Note { get; set; }
    }

    /// <summary>
    /// SATICI DEFTERİ (append-only). Satıcının platformdaki tüm parasal hareketleri satır satır
    /// buraya yazılır; BAKİYE SAKLANMAZ, daima defterden türetilir:
    ///   Toplam bakiye      = SUM(Amount) WHERE StoreId=@s
    ///   Kullanılabilir     = SUM(Amount) WHERE ReleaseAfterUtc IS NULL OR ReleaseAfterUtc &lt;= GETUTCDATE()
    ///   Ödenebilir         = Kullanılabilir AND PayoutId IS NULL (henüz bir ödemeye bağlanmamış satırlar)
    /// APPEND-ONLY İSTİSNASI: PayoutId alanı, satır bir hakediş ödemesine bağlanırken TEK SEFER yazılır;
    /// bunun dışında hiçbir alan güncellenmez. Hatalı giriş DÜZELTİLMEZ, ters kayıtla (ManualAdjustment) kapatılır.
    /// ESCROW: Tahsilat satırları ReleaseAfterUtc dolana kadar bloke sayılır (README'de akış).
    /// </summary>
    public class SellerLedgerEntries : PaymentEntityBase
    {
        /// <summary>Satıcı mağaza (Store.Id).</summary>
        public Guid StoreId { get; set; }

        /// <summary>Giriş tipi (yön açıklamaları enum XML doc'unda).</summary>
        public LedgerEntryType EntryType { get; set; }

        /// <summary>İşaretli tutar: alacak (+) / borç (−). Örn. tahsilat +1250.00, komisyon −150.00.</summary>
        public decimal Amount { get; set; }

        /// <summary>Para birimi. Bakiye türetimi para birimi bazında ayrı ayrı yapılır.</summary>
        public CurrencyCode Currency { get; set; }

        /// <summary>Kaynak: alt sipariş (SubOrders.Id). Tahsilat/komisyon girişlerinde dolu.</summary>
        public Guid? SubOrderId { get; set; }

        /// <summary>Kaynak: sipariş kalemi (OrderItems.Id). Kalem bazlı kesintilerde dolu.</summary>
        public Guid? OrderItemId { get; set; }

        /// <summary>Kaynak: iade (Refunds.Id). Refund tipli girişlerde dolu.</summary>
        public Guid? RefundId { get; set; }

        /// <summary>Kaynak: ödeme işlemi (PaymentTransactions.Id). Chargeback girişlerinde dolu.</summary>
        public Guid? PaymentTransactionId { get; set; }

        /// <summary>İnsan-okur açıklama. ManualAdjustment girişlerinde ZORUNLU tutulmalıdır (servis katmanı doğrular).</summary>
        public string? Description { get; set; }

        /// <summary>ESCROW serbest bırakma zamanı (UTC): teslimat + iade süresi.
        /// Null = anında kullanılabilir (komisyon, ceza, düzeltme girişleri).
        /// Dolu ve gelecekte = bloke; BackgroundService ayrıca bir şey YAPMAZ, sorgu anında karşılaştırılır.</summary>
        public DateTime? ReleaseAfterUtc { get; set; }

        /// <summary>Bağlandığı hakediş ödemesi (SellerPayouts.Id). Payout oluşturulurken koşullu UPDATE ile
        /// tek sefer yazılır (append-only istisnası); eşzamanlılık stratejisi README'dedir.</summary>
        public Guid? PayoutId { get; set; }

        /// <summary>Global tekil idempotency anahtarı. Deterministik üretilir
        /// (örn. "collect:{subOrderId}", "commission:{subOrderId}", "refund:{refundId}");
        /// aynı olayın çift yazımı DB seviyesinde engellenir.</summary>
        public string IdempotencyKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// SATICI HAKEDİŞ ÖDEMELERİ. Dönem kapanışında, ödenebilir ledger satırları (bkz. SellerLedgerEntries)
    /// bir payout'a bağlanır ve banka transferi BackgroundService tarafından yürütülür.
    /// KUYRUK TABLOSUDUR: lease deseni (LeasedBy/LeasedUntilUtc/AttemptCount/MaxAttempts/NextRetryAtUtc)
    /// + global tekil IdempotencyKey taşır (mevcut AiGenerationJob / ImportJob deseni).
    /// IBAN sipariş anı SNAPSHOT'tır: mağaza sonradan IBAN değiştirse bile dekont izi bozulmaz.
    /// </summary>
    public class SellerPayouts : PaymentEntityBase
    {
        /// <summary>Satıcı mağaza (Store.Id).</summary>
        public Guid StoreId { get; set; }

        /// <summary>Hakediş dönemi başlangıcı (UTC, dâhil).</summary>
        public DateTime PeriodStartUtc { get; set; }

        /// <summary>Hakediş dönemi sonu (UTC, hariç).</summary>
        public DateTime PeriodEndUtc { get; set; }

        /// <summary>Ödeme tutarı = döneme bağlanan ledger satırlarının toplamı (daima ≥ 0; negatif bakiye devrolur).</summary>
        public decimal Amount { get; set; }

        /// <summary>Para birimi (para birimi başına ayrı payout satırı açılır).</summary>
        public CurrencyCode Currency { get; set; }

        /// <summary>IBAN snapshot'ı (ödeme anındaki mağaza IBAN'ı).</summary>
        public string IbanSnapshot { get; set; } = string.Empty;

        /// <summary>Hesap sahibi adı snapshot'ı.</summary>
        public string AccountHolderSnapshot { get; set; } = string.Empty;

        /// <summary>Durum makinesi: Pending → Processing → Paid/Failed.</summary>
        public PayoutStatus Status { get; set; } = PayoutStatus.Pending;

        /// <summary>Banka dekontu (MediaItems.Id). Paid durumunda doldurulur.</summary>
        public Guid? ReceiptMediaItemId { get; set; }

        /// <summary>Ödemenin gerçekleştiği an (UTC).</summary>
        public DateTime? PaidAtUtc { get; set; }

        /// <summary>Başarısızlık açıklaması (Status=Failed iken).</summary>
        public string? FailureMessage { get; set; }

        // ---------------- LEASE DESENİ (çok örnekli BackgroundService) ----------------

        /// <summary>Lease sahibi işleyici kimliği (makine+instance). Null = sahipsiz.</summary>
        public string? LeasedBy { get; set; }

        /// <summary>Lease bitiş zamanı (UTC). Geçmişse lease düşmüş sayılır, başka işleyici alabilir.</summary>
        public DateTime? LeasedUntilUtc { get; set; }

        /// <summary>Deneme sayısı.</summary>
        public int AttemptCount { get; set; }

        /// <summary>Azami deneme. Aşılırsa Failed'da kalır, manuel müdahale beklenir.</summary>
        public int MaxAttempts { get; set; } = 3;

        /// <summary>Bir sonraki deneme zamanı (UTC). Failed sonrası üstel geri çekilme ile kurulur.</summary>
        public DateTime? NextRetryAtUtc { get; set; }

        /// <summary>Global tekil idempotency anahtarı. Deterministik: "payout:{storeId}:{currency}:{periodEnd:yyyyMMdd}";
        /// aynı dönem için ikinci payout DB seviyesinde engellenir.</summary>
        public string IdempotencyKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// İADELER. OrderItem bazlı kısmi/tam iade FİNANSAL kaydıdır (fiziksel iade akışı Faz 7
    /// ReturnRequests'tedir; oradan buraya bağlanır). Para hareketini yürüten işlem
    /// PaymentTransactionId (Refund/PartialRefund tipli) ile bağlanır. Tamamlanan iade:
    /// - SellerLedgerEntries'e (−) girişi yazar,
    /// - OrderItems.RefundedQuantity'yi günceller,
    /// - SubOrder/Order durumunu (PartiallyRefunded/Refunded) servis katmanında türetir.
    /// </summary>
    public class Refunds : PaymentEntityBase
    {
        /// <summary>İade edilen kalem (OrderItems.Id).</summary>
        public Guid OrderItemId { get; set; }

        /// <summary>Alt sipariş izi (SubOrders.Id). DENORMALIZE: kaynak OrderItems.SubOrderId; satıcı iade raporları için.</summary>
        public Guid SubOrderId { get; set; }

        /// <summary>Üst sipariş izi (Orders.Id). DENORMALIZE: kaynak OrderItems.OrderId; müşteri iade listesi için.</summary>
        public Guid OrderId { get; set; }

        /// <summary>Mağaza izi (Store.Id). DENORMALIZE: kaynak OrderItems.StoreId.</summary>
        public Guid StoreId { get; set; }

        /// <summary>İade edilen adet (kısmi iade desteklenir; kalem adedini aşamaz — servis katmanı doğrular).</summary>
        public int Quantity { get; set; }

        /// <summary>İade tutarı (kalem birim fiyatı × adet − orantılı indirim payı; hesap servis katmanında).</summary>
        public decimal Amount { get; set; }

        /// <summary>Para birimi (kalemle aynı).</summary>
        public CurrencyCode Currency { get; set; }

        /// <summary>İade nedeni.</summary>
        public RefundReason Reason { get; set; }

        /// <summary>Serbest açıklama.</summary>
        public string? Note { get; set; }

        /// <summary>Durum makinesi.</summary>
        public RefundStatus Status { get; set; } = RefundStatus.Pending;

        /// <summary>Para hareketini yürüten ödeme işlemi (PaymentTransactions.Id, Refund/PartialRefund tipli).
        /// Sağlayıcı API iadesi başlatılınca doldurulur.</summary>
        public Guid? PaymentTransactionId { get; set; }

        /// <summary>Fiziksel iade talebi izi (Faz 7 ReturnRequests.Id). Şimdilik iz alanı; FK Faz 7'de tanımlanır.</summary>
        public Guid? ReturnRequestId { get; set; }

        /// <summary>İadenin tamamlandığı an (UTC).</summary>
        public DateTime? CompletedAtUtc { get; set; }
    }
}