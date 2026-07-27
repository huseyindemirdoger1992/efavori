using data._Products;

namespace data._Orders
{
    /// <summary>
    /// CHECKOUT OTURUMU — sepet → sipariş dönüşümünün idempotency ve kilit kaydı.
    /// AKIŞ:
    /// 1) Müşteri "Ödemeye Geç" dediğinde Active oturum açılır; sepet içeriğinin doğrulama
    ///    hash'i (CartHash) ve beklenen toplam (GrandTotal) yazılır, ExpiresAtUtc kurulur.
    /// 2) İstemci sipariş isteğinde bu oturumun IdempotencyKey'ini gönderir. Çift tıklama /
    ///    ağ tekrarı durumunda aynı anahtar geldiğinden İKİNCİ sipariş OLUŞMAZ:
    ///    Completed oturumda mevcut OrderId döndürülür (README'deki örnek metoda bakınız).
    /// 3) Sipariş oluşurken sepet içeriği yeniden hash'lenir; CartHash tutmuyorsa
    ///    (fiyat/stok değişti) oturum reddedilir ve müşteriye güncel sepet gösterilir.
    /// 4) Süresi dolan Active oturumları BackgroundService Expired'a çeker ve sepet kilidini çözer.
    /// </summary>
    public class CheckoutSessions : OrderEntityBase
    {
        /// <summary>Oturum sahibi kullanıcı (Users.Id). Misafirde null.</summary>
        public Guid? UserId { get; set; }

        /// <summary>Misafir checkout e-postası (UserId null iken).</summary>
        public string? GuestEmail { get; set; }

        /// <summary>Global tekil idempotency anahtarı (soft-delete filtreli tekil indeks).
        /// İstemciye oturum açılışında verilir; sipariş oluşturma isteği bu anahtarla gelir.</summary>
        public string IdempotencyKey { get; set; } = string.Empty;

        /// <summary>Oturum durumu.</summary>
        public CheckoutSessionStatus Status { get; set; } = CheckoutSessionStatus.Active;

        /// <summary>Oturumun geçerlilik sonu (UTC). Örn. açılış + 30 dk. Süresi dolanı BackgroundService düşürür.</summary>
        public DateTime ExpiresAtUtc { get; set; }

        /// <summary>Sepet doğrulama hash'i: sıralı (VariantId, Quantity, UnitPrice, Currency) dizisinin SHA-256'sı.
        /// Sipariş anında yeniden hesaplanır; eşleşmezse sepet değişmiş demektir, sipariş reddedilir.</summary>
        public string CartHash { get; set; } = string.Empty;

        /// <summary>Oturum açılışındaki sepetin tam JSON snapshot'ı (uyuşmazlık analizinde kanıt; opsiyonel ama önerilir).</summary>
        public string? CartSnapshotJson { get; set; }

        /// <summary>Beklenen genel toplam (oturum açılışında hesaplanan). Sipariş anındaki toplamla karşılaştırılır.</summary>
        public decimal GrandTotal { get; set; }

        /// <summary>Para birimi.</summary>
        public CurrencyCode Currency { get; set; }

        /// <summary>Başarıyla oluşan sipariş (Orders.Id). Status=Completed olduğunda dolar;
        /// aynı IdempotencyKey ile gelen tekrar isteklere bu Id döndürülür.</summary>
        public Guid? OrderId { get; set; }

        /// <summary>Oturumun tamamlandığı an (UTC).</summary>
        public DateTime? CompletedAtUtc { get; set; }
    }

    /// <summary>
    /// SİPARİŞ NUMARASI SIRA TABLOSU. "EF-{Yıl}-{ArtanNo:000000}" biçimindeki insan-okur
    /// OrderNumber üretiminin kaynağıdır. Yıl başına TEK satır tutulur (Year tekil indeks).
    /// RACE CONDITION ÇÖZÜMÜ: numara, tek atomik UPDATE ile alınır:
    ///   UPDATE OrderNumberSequences SET LastValue = LastValue + 1
    ///   OUTPUT inserted.LastValue WHERE Year = @year;
    /// SQL Server satır kilidi sayesinde iki eşzamanlı istek asla aynı değeri alamaz.
    /// Yıl satırı yoksa insert yarışı tekil indeks ihlaliyle yakalanıp yeniden denenir.
    /// (Ayrıntılı örnek metod README'dedir.)
    /// </summary>
    public class OrderNumberSequences : OrderEntityBase
    {
        /// <summary>Sıra yılı (örn. 2026). Tekil.</summary>
        public int Year { get; set; }

        /// <summary>Son verilen sıra değeri. Atomik UPDATE ile artırılır; uygulama katmanında ASLA okunup-yazılmaz.</summary>
        public long LastValue { get; set; }
    }
}
