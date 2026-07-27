using System;
using data._Attribute; // Language enum'ı

namespace data._Products
{
    /// <summary>
    /// ÜRÜN YORUMU + YILDIZ PUANI (Amazon/eBay/Trendyol tarzı değerlendirme).
    ///
    /// Bir kullanıcı bir ürün için TEK kök yorum bırakır (Rating dolu). O yoruma
    /// gelen yanıtlar (satıcı cevabı, başka kullanıcının tartışması) aynı tabloda
    /// <see cref="ParentReviewId"/> ile SONSUZ DERİNLİKTE ağaç olarak tutulur —
    /// yanıtlarda Rating = null olur (yalnızca kök yorumda puan bulunur).
    ///
    /// Puan ölçeği 1..5 (yıldız). Alt kırılım puanları (kalite, kargo, fiyat/performans)
    /// opsiyoneldir; dolduruldukça ürün detayında ayrı çubuklarda gösterilebilir.
    ///
    /// Not: Ortalama/istatistik burada TUTULMAZ — ana yorum tablosunu yazma
    /// yükünden korumak için özet ayrı <see cref="ProductRatingSummary"/> tablosunda,
    /// oy sayıları ise ilgili yorumun kendi <see cref="HelpfulVoteCount"/> /
    /// <see cref="NotHelpfulVoteCount"/> denormalize alanlarında tutulur (kaynak:
    /// <see cref="ProductReviewVotes"/>).
    /// </summary>
    public class ProductReviews : ProductEntityBase
    {
        /// <summary>Değerlendirilen ürün (Products.Id).</summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// İlgili varyant (ProductVariants.Id — opsiyonel). Kullanıcının hangi
        /// varyantı (ör. renk/beden) değerlendirdiğini belirtir; boş bırakılabilir.
        /// </summary>
        public Guid? ProductVariantId { get; set; }

        /// <summary>Yorumu yazan kullanıcı (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Kök yoruma yanıt ise üst yorum (ProductReviews.Id — ağaç yapısı).
        /// null = kök yorum (puan taşır). Dolu = yanıt (puan taşımaz).
        /// </summary>
        public Guid? ParentReviewId { get; set; }

        // ── Puanlama (yalnızca kök yorumda) ──────────────────────────────────
        /// <summary>Genel yıldız puanı (1..5). Yanıtlarda null.</summary>
        public byte? Rating { get; set; }

        /// <summary>Alt puan — ürün kalitesi (1..5, opsiyonel).</summary>
        public byte? QualityRating { get; set; }

        /// <summary>Alt puan — kargo/teslimat (1..5, opsiyonel).</summary>
        public byte? ShippingRating { get; set; }

        /// <summary>Alt puan — fiyat/performans (1..5, opsiyonel).</summary>
        public byte? ValueRating { get; set; }

        // ── İçerik ───────────────────────────────────────────────────────────
        /// <summary>Yorum başlığı (opsiyonel; yanıtlarda genelde boş).</summary>
        public string? Title { get; set; }

        /// <summary>Yorum/yanıt metni.</summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>Yorumun yazıldığı dil (çok dilli mağazada gösterim/çeviri için).</summary>
        public Language Language { get; set; } = Language.Tr;

        /// <summary>
        /// Yorum medyası (ProductReviewMedia yerine hızlı erişim için) — kullanıcının
        /// eklediği görsel/video Media.Id listesi, JSON dizi. Boşsa medyasız yorum.
        /// </summary>
        public string? MediaIdsJson { get; set; }

        // ── Güven / moderasyon ───────────────────────────────────────────────
        /// <summary>
        /// Doğrulanmış satın alma mı? (kullanıcı bu ürünü gerçekten satın aldı).
        /// Sipariş modülü bağlanınca sipariş kontrolüyle set edilir; şimdilik
        /// manuel/varsayılan false.
        /// </summary>
        public bool IsVerifiedPurchase { get; set; }

        /// <summary>Moderasyon durumu (yalnızca Approved vitrinde görünür).</summary>
        public ModerationStatus Status { get; set; } = ModerationStatus.Pending;

        /// <summary>Reddetme/gizleme gerekçesi (Status != Approved iken).</summary>
        public string? ModerationNote { get; set; }

        /// <summary>Moderasyonu yapan admin (Users.Id).</summary>
        public Guid? ModeratedByUserId { get; set; }

        /// <summary>Moderasyon anı (UTC).</summary>
        public DateTime? ModeratedAtUtc { get; set; }

        // ── Satıcı yanıtı işareti ────────────────────────────────────────────
        /// <summary>
        /// Bu kayıt, ürünün satıcısı tarafından yazılmış resmî bir yanıt mı?
        /// (Vitrinde "Satıcı yanıtı" rozetiyle vurgulanır.) Yalnızca yanıtlarda true olur.
        /// </summary>
        public bool IsSellerResponse { get; set; }

        // ── Denormalize oy sayaçları (kaynak: ProductReviewVotes) ────────────
        /// <summary>"Faydalı" oy sayısı — hızlı sıralama için denormalize.</summary>
        public int HelpfulVoteCount { get; set; }

        /// <summary>"Faydalı değil" oy sayısı — denormalize.</summary>
        public int NotHelpfulVoteCount { get; set; }

        /// <summary>Ağaçtaki doğrudan yanıt sayısı — denormalize (liste için).</summary>
        public int ReplyCount { get; set; }
    }

    /// <summary>
    /// Bir yoruma/yanıta verilen FAYDA OYU ("Bu yorum faydalı mıydı?").
    /// (ProductReviewId, UserId) benzersizdir — bir kullanıcı bir yoruma tek oy verir
    /// (oyunu değiştirebilir: VoteType güncellenir).
    /// Toplamlar <see cref="ProductReviews.HelpfulVoteCount"/> alanına denormalize edilir.
    /// </summary>
    public class ProductReviewVotes : ProductEntityBase
    {
        /// <summary>Oy verilen yorum (ProductReviews.Id).</summary>
        public Guid ProductReviewId { get; set; }

        /// <summary>Oyu veren kullanıcı (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>Oy türü (Helpful / NotHelpful).</summary>
        public VoteType Vote { get; set; }
    }

    /// <summary>
    /// Bir yorum/yanıt hakkında yapılan ŞİKÂYET (report/flag).
    /// (ProductReviewId, ReportedByUserId) benzersizdir — bir kullanıcı bir içeriği
    /// bir kez şikâyet eder. Moderasyon kuyruğunu besler.
    /// </summary>
    public class ProductReviewReports : ProductEntityBase
    {
        /// <summary>Şikâyet edilen yorum (ProductReviews.Id).</summary>
        public Guid ProductReviewId { get; set; }

        /// <summary>Şikâyet eden kullanıcı (Users.Id).</summary>
        public Guid ReportedByUserId { get; set; }

        /// <summary>Şikâyet gerekçesi.</summary>
        public ReportReason Reason { get; set; }

        /// <summary>Serbest açıklama (Reason = Other iken zorunlu önerilir).</summary>
        public string? Description { get; set; }

        /// <summary>Şikâyetin işlenme durumu.</summary>
        public ReportStatus Status { get; set; } = ReportStatus.Open;

        /// <summary>Şikâyeti çözen admin (Users.Id).</summary>
        public Guid? ResolvedByUserId { get; set; }

        /// <summary>Çözüm anı (UTC).</summary>
        public DateTime? ResolvedAtUtc { get; set; }
    }
}
