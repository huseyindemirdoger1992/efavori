using data._Products;

namespace data._Returns
{
    /// <summary>
    /// İHTİLAF (PLATFORM HAKEMLİĞİ). AliExpress modeli: satıcı iadeyi reddettiğinde veya
    /// taraflar anlaşamadığında müşteri platformu hakem olarak çağırır.
    ///
    /// KANIT: Taraf mesajları DisputeMessages, dosya ekleri DisputeAttachments tablosundadır.
    /// Karar verildiğinde Faz 2 Refunds kaydı üretilir ve gerekiyorsa satıcı defterine
    /// (SellerLedgerEntries) ceza/kesinti girişi yazılır.
    ///
    /// SÜRE YÖNETİMİ: SellerResponseDeadlineUtc ve EvidenceDeadlineUtc dolduğunda
    /// BackgroundService ihtilafı otomatik ilerletir — sessiz kalan taraf lehine sonuç doğmaz.
    /// </summary>
    public class Disputes : ReturnEntityBase
    {
        /// <summary>İnsan-okur ihtilaf numarası ("IHT-2026-000045").</summary>
        public string DisputeNumber { get; set; } = string.Empty;

        /// <summary>İhtilafın kaynaklandığı iade talebi (ReturnRequests.Id).
        /// Null olabilir: iade talebi olmadan doğrudan açılan ihtilaflar (ürün hiç ulaşmadı vb.).</summary>
        public Guid? ReturnRequestId { get; set; }

        /// <summary>Bağlı alt sipariş (SubOrders.Id).</summary>
        public Guid SubOrderId { get; set; }

        /// <summary>Üst sipariş izi (Orders.Id). DENORMALIZE: kaynak SubOrders.OrderId.</summary>
        public Guid OrderId { get; set; }

        /// <summary>Satıcı mağaza (Store.Id). DENORMALIZE: satıcı ihtilaf oranı metriği (Faz 8) için.</summary>
        public Guid StoreId { get; set; }

        /// <summary>İhtilafı açan müşteri (Users.Id). Misafirde null.</summary>
        public Guid? UserId { get; set; }

        /// <summary>Misafir iletişim e-postası.</summary>
        public string? GuestEmail { get; set; }

        /// <summary>Durum makinesi.</summary>
        public DisputeStatus Status { get; set; } = DisputeStatus.Opened;

        /// <summary>Müşterinin açılış iddiası.</summary>
        public string OpeningClaim { get; set; } = string.Empty;

        /// <summary>Müşterinin talep ettiği tutar.</summary>
        public decimal ClaimedAmount { get; set; }

        /// <summary>Para birimi.</summary>
        public CurrencyCode Currency { get; set; }

        // ---------------- KARAR ----------------

        /// <summary>Platformun kararı.</summary>
        public DisputeDecisionType Decision { get; set; } = DisputeDecisionType.Pending;

        /// <summary>Karara bağlanan iade tutarı. Partial kararlarda kısmi tutar; SellerFavor'da 0.</summary>
        public decimal? DecidedRefundAmount { get; set; }

        /// <summary>İade kargo bedeli kime yüklendi.</summary>
        public ReturnShippingPayer? DecidedShippingPayer { get; set; }

        /// <summary>Kararın gerekçesi (taraflara gösterilir).</summary>
        public string? DecisionReason { get; set; }

        /// <summary>Kararı veren platform yöneticisi (Users.Id).</summary>
        public Guid? DecidedByUserId { get; set; }

        /// <summary>Kararın verildiği an (UTC).</summary>
        public DateTime? DecidedAtUtc { get; set; }

        /// <summary>Karar sonucu üretilen finansal iade izi (Faz 2 Refunds.Id).</summary>
        public Guid? RefundId { get; set; }

        /// <summary>Satıcıya kesilen ceza izi (Faz 2 SellerLedgerEntries.Id, EntryType=Penalty).
        /// Kötü niyetli satıcı davranışı tespit edilirse doldurulur.</summary>
        public Guid? PenaltyLedgerEntryId { get; set; }

        // ---------------- SÜRELER ----------------

        /// <summary>Satıcının yanıt vermesi gereken son an (UTC). Geçerse UnderReview'a otomatik geçilir
        /// ve sessizlik satıcı aleyhine değerlendirilir (karar hakemindir).</summary>
        public DateTime? SellerResponseDeadlineUtc { get; set; }

        /// <summary>Kanıt sunma süresinin sonu (UTC). Sonrasında yeni mesaj/ek kabul edilmez.</summary>
        public DateTime? EvidenceDeadlineUtc { get; set; }

        /// <summary>İhtilafın kapandığı an (UTC).</summary>
        public DateTime? ClosedAtUtc { get; set; }

        // ---------------- DENORMALİZE SAYAÇLAR ----------------

        /// <summary>Toplam mesaj adedi. KAYNAK: DisputeMessages satır sayısı.</summary>
        public int MessageCount { get; set; }

        /// <summary>Toplam kanıt eki adedi. KAYNAK: DisputeAttachments satır sayısı.</summary>
        public int AttachmentCount { get; set; }

        /// <summary>Son mesajın zamanı (UTC). Yönetim panelinde "yanıt bekleyen" sıralaması için.</summary>
        public DateTime? LastMessageAtUtc { get; set; }

        /// <summary>Son mesajı gönderen taraf. "Top kimde" göstergesi.</summary>
        public DisputeMessageSenderType? LastMessageSenderType { get; set; }

        /// <summary>Global tekil idempotency anahtarı. Deterministik: "dsp:{subOrderId}:{returnRequestId}";
        /// aynı iade için ikinci ihtilaf açılmasını engeller.</summary>
        public string IdempotencyKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// İHTİLAF MESAJI (append-only). Taraflar arasındaki yazışma ve platform bildirimleri.
    /// Satırlar ASLA güncellenmez veya silinmez — mesajın sonradan değiştirilemez olması
    /// hakemliğin güvenilirliğinin şartıdır.
    /// </summary>
    public class DisputeMessages : ReturnEntityBase
    {
        /// <summary>Bağlı ihtilaf (Disputes.Id).</summary>
        public Guid DisputeId { get; set; }

        /// <summary>Mesajı gönderen taraf.</summary>
        public DisputeMessageSenderType SenderType { get; set; }

        /// <summary>Gönderen kullanıcı (Users.Id). Sistem mesajlarında null.</summary>
        public Guid? SenderUserId { get; set; }

        /// <summary>Mesaj metni.</summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>Yalnızca platform ekibine görünür iç not mu? true ise taraflara gösterilmez.</summary>
        public bool IsInternalNote { get; set; }

        /// <summary>Karşı tarafça okundu mu? (basit okundu bilgisi; kanal bazlı değil)</summary>
        public bool IsReadByCounterparty { get; set; }

        /// <summary>Okunma anı (UTC).</summary>
        public DateTime? ReadAtUtc { get; set; }

        /// <summary>Ek adedi. KAYNAK: DisputeAttachments satır sayısı.</summary>
        public int AttachmentCount { get; set; }
    }

    /// <summary>
    /// İHTİLAF KANIT EKİ. Mesaja veya doğrudan ihtilafa bağlı dosya (fotoğraf, video, fatura PDF'i,
    /// kargo tutanağı). Kanıt niteliği taşıdığından yükleyen taraf ve zaman damgası korunur.
    /// </summary>
    public class DisputeAttachments : ReturnEntityBase
    {
        /// <summary>Bağlı ihtilaf (Disputes.Id).</summary>
        public Guid DisputeId { get; set; }

        /// <summary>Bağlı mesaj (DisputeMessages.Id). Null = ihtilafa doğrudan eklenmiş kanıt.</summary>
        public Guid? DisputeMessageId { get; set; }

        /// <summary>Dosya (MediaItems.Id).</summary>
        public Guid MediaItemId { get; set; }

        /// <summary>Eki yükleyen taraf.</summary>
        public DisputeMessageSenderType UploadedBySenderType { get; set; }

        /// <summary>Kanıt açıklaması.</summary>
        public string? Caption { get; set; }

        /// <summary>Gösterim sırası.</summary>
        public int SortOrder { get; set; }
    }

    /// <summary>
    /// İADE POLİTİKASI. İade süresi (gün) ve koşullarının kapsam bazlı tanımıdır.
    ///
    /// ÇÖZÜM ÖNCELİK SIRASI (en özelden genele) — CommissionRates ile aynı desen:
    ///   1) StoreId + CategoryId dolu  → mağaza+kategori politikası
    ///   2) StoreId dolu               → mağaza politikası
    ///   3) CategoryId dolu            → kategori politikası
    ///   4) CountryId dolu             → ülke politikası (yasal asgari)
    ///   5) hepsi null                 → global varsayılan
    ///
    /// TARİHÇE KORUNUR: satır ASLA güncellenmez; değişiklik = eski satıra ValidToUtc yazılır
    /// + YENİ satır açılır. Sipariş anında çözülen süre ReturnRequests.AppliedReturnWindowDays
    /// alanına SNAPSHOT'lanır.
    ///
    /// NEDEN AYRI TABLO — CategoriesProduct'a kolon eklemek yerine (README §6):
    /// (a) politika mağazaya göre değişir, kategori kolonu bunu ifade edemez;
    /// (b) yasal asgari süre ülkeye göre farklıdır (TR 14 gün, AB 14 gün, başka pazarlar farklı);
    /// (c) politikanın tarihçesi olmalı — kategori kolonu geçmişi taşıyamaz;
    /// (d) iade süresi dışında koşullar da var (kutu açılmamış olmalı, kargo bedeli kime ait).
    /// </summary>
    public class ReturnPolicies : ReturnEntityBase
    {
        /// <summary>Kapsam seviyesi. Aşağıdaki ID alanlarının hangilerinin dolu olduğuyla tutarlı olmalıdır.</summary>
        public ReturnPolicyScopeType ScopeType { get; set; }

        /// <summary>Kapsam: mağaza (Store.Id). Null = tüm mağazalar.</summary>
        public Guid? StoreId { get; set; }

        /// <summary>Kapsam: kategori (CategoriesProduct.Id). Null = tüm kategoriler.
        /// Çözümde ürünün kategorisi + üst kategori zinciri sırayla denenir (en derin eşleşme kazanır).</summary>
        public Guid? CategoryId { get; set; }

        /// <summary>Kapsam: ülke (Country.Id). Yasal asgari süreler için. Null = tüm ülkeler.</summary>
        // DÜZELTME: data._Locations.Country tablosunun birincil anahtarı INT'tir
        // (int id). Bu alan önceden Guid? tipindeydi ve _ReturnModelConfiguration
        // içindeki HasForeignKey çağrısıyla TİP UYUŞMAZLIĞI oluşturuyordu; iade
        // modülü yapılandırması DbContext'e bağlandığı anda model kurulumu hata
        // verirdi. Tip int? olarak düzeltilmiştir.
        public int? CountryId { get; set; }

        /// <summary>İade hakkı süresi (gün). Teslim tarihinden itibaren sayılır.
        /// 0 = iade kabul edilmez (gıda, hijyen, kişiye özel üretim).</summary>
        public int ReturnWindowDays { get; set; }

        /// <summary>Bu kapsamda iadeye hiç izin var mı? false ise ReturnWindowDays yok sayılır.</summary>
        public bool IsReturnAllowed { get; set; } = true;

        /// <summary>Cayma hakkı (fikir değişikliği) kaynaklı iadelerde kargo bedelini kim öder.
        /// Satıcı kusuru kaynaklı iadelerde bu alan dikkate ALINMAZ; bedel daima satıcıya aittir.</summary>
        public ReturnShippingPayer DefaultShippingPayer { get; set; } = ReturnShippingPayer.Customer;

        /// <summary>Ürünün orijinal ambalajında olması şart mı?</summary>
        public bool RequiresOriginalPackaging { get; set; }

        /// <summary>Ürün açılmamış olmalı mı? (hijyen ürünleri, yazılım lisansları)</summary>
        public bool RequiresUnopened { get; set; }

        /// <summary>Satıcının iade talebine yanıt verme süresi (saat). Aşılırsa otomatik onaylanır.</summary>
        public int SellerResponseHours { get; set; } = 48;

        /// <summary>Müşterinin iade kargosunu vermesi için tanınan süre (gün). Aşılırsa talep düşer.</summary>
        public int ShipBackDays { get; set; } = 7;

        /// <summary>Politikanın müşteriye gösterilecek açıklaması. Çok dilli gösterim gerekirse
        /// ayrı çeviri tablosu eklenmelidir (Faz 7 kapsamına alınmadı — README §9).</summary>
        public string? PolicyText { get; set; }

        /// <summary>Geçerlilik başlangıcı (UTC).</summary>
        public DateTime ValidFromUtc { get; set; }

        /// <summary>Geçerlilik sonu (UTC). Null = halen geçerli.</summary>
        public DateTime? ValidToUtc { get; set; }

        /// <summary>Aynı özgüllükte birden çok politika eşleşirse küçük değer kazanır.</summary>
        public int Priority { get; set; }

        /// <summary>Admin açıklaması.</summary>
        public string? Note { get; set; }
    }
}
