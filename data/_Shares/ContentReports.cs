using System;
using data._Products;

namespace data._Shares
{
    /// <summary>
    /// BİRLEŞİK İÇERİK ŞİKÂYETİ (§22).
    ///
    /// Gönderi, yorum, kullanıcı, mağaza, ürün, sohbet mesajı, makale ve medya
    /// şikâyetlerini TEK moderasyon kuyruğunda toplar.
    ///
    /// POLİMORFİZM YERİNE TİPLİ FK'LER (§42):
    /// "Guid TargetId + string TargetType" tasarımı bilinçli olarak REDDEDİLMİŞTİR.
    /// Bunun yerine her hedef türü için AYRI ve NULLABLE gerçek FK kolonu vardır.
    /// <see cref="TargetType"/> hangisinin dolu olduğunu belirtir ve
    /// <c>CK_ContentReports_TargetMatchesType</c> CHECK kısıtı, tam olarak BİR hedef
    /// kolonunun dolu olmasını ve bunun TargetType ile uyuşmasını garanti eder.
    /// Böylece hem tek kuyruk hem tam referans bütünlüğü elde edilir; silinmiş bir
    /// gönderiye ait öksüz şikâyet oluşamaz.
    ///
    /// MEVCUT ÜRÜN ŞİKÂYET TABLOLARIYLA İLİŞKİSİ (§22, §71):
    /// <c>ProductReviewReports</c> ve <c>ProductQuestionReports</c> KORUNMUŞTUR —
    /// ürün yorumları için zaten doğru, FK'li ve tekil-indeksli çalışıyorlar ve
    /// mevcut uygulama kodu bunlara bağlı. Kavramlar ise STANDARTLAŞTIRILMIŞTIR:
    /// üçü de aynı <see cref="ReportReason"/> ve <see cref="ReportStatus"/> enum'larını
    /// kullanır, böylece moderasyon paneli tek bir sözlükle rapor üretebilir.
    /// Bu tablonun ProductReview/ProductQuestion hedefleri, sosyal akıştan gelen
    /// şikâyetlerin de aynı kuyruğa düşebilmesi için tanımlıdır.
    ///
    /// TEKİLLİK: Bir kullanıcı aynı içeriği bir kez şikâyet edebilir; tekillik
    /// (ReporterUserId, TargetType, hedef kolonu) üzerinden filtreli indekslerle kurulur.
    /// </summary>
    public class ContentReports : SocialEntityBase
    {
        /// <summary>Şikâyeti yapan kullanıcı (Users.Id).</summary>
        public Guid ReporterUserId { get; set; }

        /// <summary>Şikâyet edilen içeriğin türü — hangi hedef kolonunun dolu olduğunu belirtir.</summary>
        public ContentTargetType TargetType { get; set; }

        // ── Tipli hedef kolonları (tam olarak biri dolu olur) ─────────────────
        /// <summary>Hedef gönderi (Posts.Id). TargetType = Post iken dolu.</summary>
        public Guid? TargetPostId { get; set; }

        /// <summary>Hedef yorum (PostComments.Id). TargetType = PostComment iken dolu.</summary>
        public Guid? TargetCommentId { get; set; }

        /// <summary>Hedef kullanıcı (Users.Id). TargetType = User iken dolu.</summary>
        public Guid? TargetUserId { get; set; }

        /// <summary>Hedef mağaza (Store.Id). TargetType = Store iken dolu.</summary>
        public Guid? TargetStoreId { get; set; }

        /// <summary>Hedef ürün (Products.Id). TargetType = Product iken dolu.</summary>
        public Guid? TargetProductId { get; set; }

        /// <summary>Hedef ürün yorumu (ProductReviews.Id). TargetType = ProductReview iken dolu.</summary>
        public Guid? TargetProductReviewId { get; set; }

        /// <summary>Hedef ürün sorusu (ProductQuestions.Id). TargetType = ProductQuestion iken dolu.</summary>
        public Guid? TargetProductQuestionId { get; set; }

        /// <summary>Hedef sohbet mesajı (ChatMessages.Id). TargetType = ChatMessage iken dolu.</summary>
        public Guid? TargetChatMessageId { get; set; }

        /// <summary>Hedef makale (Articles.Id). TargetType = Article iken dolu.</summary>
        public Guid? TargetArticleId { get; set; }

        /// <summary>Hedef medya (Media.Id). TargetType = Media iken dolu.</summary>
        public Guid? TargetMediaId { get; set; }

        /// <summary>
        /// HESAPLANMIŞ KOLON (persisted). Yukarıdaki hedef kolonlarından dolu olanın
        /// değeri. Uygulama tarafından YAZILMAZ; SQL Server üretir.
        ///
        /// Amacı: "bir kullanıcı aynı içeriği bir kez şikâyet edebilir" kuralını
        /// hedef türü başına ayrı ayrı indeks açmadan TEK filtreli tekil indeksle
        /// uygulayabilmektir.
        /// </summary>
        public Guid? TargetEntityId { get; private set; }

        // ── Şikâyet içeriği ───────────────────────────────────────────────────
        /// <summary>Şikâyet gerekçesi (platform genelinde ortak enum).</summary>
        public ReportReason Reason { get; set; }

        /// <summary>Serbest açıklama (Reason = Other iken zorunlu önerilir).</summary>
        public string? Description { get; set; }

        /// <summary>
        /// Şikâyete eklenen kanıt medyaları (Media.Id listesi, JSON dizi).
        /// JSON KULLANIMI BURADA MEŞRUDUR (§46): sorgulanmayan, en fazla birkaç
        /// öğelik ek listesidir; ilişkisel sorgu ihtiyacı doğarsa
        /// MediaItems (ItemType = ContentReport) üzerinden normalize edilir.
        /// </summary>
        public string? EvidenceMediaIdsJson { get; set; }

        /// <summary>Şikâyetin geldiği yüzey ("PostDetail", "Feed", "Profile").</summary>
        public string? SourceSurface { get; set; }

        // ── İşlenme durumu ────────────────────────────────────────────────────
        /// <summary>Şikâyetin işlenme durumu (platform genelinde ortak enum).</summary>
        public ReportStatus Status { get; set; } = ReportStatus.Open;

        /// <summary>
        /// Öncelik (0 = normal, yükseldikçe acil). ChildSafety ve SelfHarm gerekçeleri
        /// servis katmanında otomatik olarak en yüksek önceliğe çekilir.
        /// </summary>
        public byte Priority { get; set; }

        /// <summary>Şikâyeti inceleyen moderatör (Users.Id).</summary>
        public Guid? ModeratedByUserId { get; set; }

        /// <summary>İncelemenin tamamlandığı an (UTC).</summary>
        public DateTime? ModeratedAtUtc { get; set; }

        /// <summary>Moderatörün karar notu (yalnızca yönetim panelinde görünür).</summary>
        public string? ResolutionNote { get; set; }

        /// <summary>Bu şikâyet başka bir şikâyetle birleştirildiyse ana kayıt (ContentReports.Id).</summary>
        public Guid? DuplicateOfReportId { get; set; }

        // ── Moderatör kuyruğu lease deseni (mevcut proje deseni) ──────────────
        /// <summary>Kaydı inceleme için kilitleyen işleyici/moderatör kimliği.</summary>
        public string? LeasedBy { get; set; }

        /// <summary>Kilidin bitiş anı (UTC). Geçmişse kayıt yeniden kuyruğa düşer.</summary>
        public DateTime? LeasedUntilUtc { get; set; }
    }

    /// <summary>
    /// MODERASYON EYLEM KAYDI (§23) — moderasyonun DEĞİŞMEZ denetim izi.
    ///
    /// Her karar (onay, ret, gizleme, kaldırma, uyarı, askıya alma) buraya bir satır
    /// yazar. Amaç:
    ///  • İtiraz süreçlerinde "bu karar ne zaman, kim tarafından, hangi gerekçeyle
    ///    verildi?" sorusunun kesin yanıtlanabilmesi,
    ///  • Yapay zekâ ve insan kararlarının ayrı ayrı ölçülebilmesi,
    ///  • Yasal taleplerde şeffaflık raporu üretilebilmesi.
    ///
    /// SOFT DELETE UYGULANMAZ (§40): denetim izi silinmez.
    /// Hedefleme <see cref="ContentReports"/> ile aynı tipli-FK desenini kullanır.
    /// </summary>
    public class ContentModerationActions
    {
        /// <summary>Birincil anahtar.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Eylemin uygulandığı içerik türü.</summary>
        public ContentTargetType TargetType { get; set; }

        /// <summary>Hedef gönderi (Posts.Id).</summary>
        public Guid? TargetPostId { get; set; }

        /// <summary>Hedef yorum (PostComments.Id).</summary>
        public Guid? TargetCommentId { get; set; }

        /// <summary>Hedef kullanıcı (Users.Id).</summary>
        public Guid? TargetUserId { get; set; }

        /// <summary>Hedef mağaza (Store.Id).</summary>
        public Guid? TargetStoreId { get; set; }

        /// <summary>Hedef ürün yorumu (ProductReviews.Id).</summary>
        public Guid? TargetProductReviewId { get; set; }

        /// <summary>Hedef ürün sorusu (ProductQuestions.Id).</summary>
        public Guid? TargetProductQuestionId { get; set; }

        /// <summary>Hedef medya (Media.Id).</summary>
        public Guid? TargetMediaId { get; set; }

        /// <summary>Bu eylemi tetikleyen şikâyet (ContentReports.Id). Proaktif taramada null.</summary>
        public Guid? ContentReportId { get; set; }

        /// <summary>Uygulanan eylem.</summary>
        public ModerationActionType ActionType { get; set; }

        /// <summary>Kararı kimin verdiği (insan / kural motoru / yapay zekâ).</summary>
        public ModerationActorType ActorType { get; set; } = ModerationActorType.Moderator;

        /// <summary>Kararı veren kullanıcı (Users.Id). Otomatik kararlarda null.</summary>
        public Guid? ActorUserId { get; set; }

        /// <summary>Eylemin öncesindeki moderasyon durumu.</summary>
        public ModerationStatus? PreviousStatus { get; set; }

        /// <summary>Eylemin sonrasındaki moderasyon durumu.</summary>
        public ModerationStatus? NewStatus { get; set; }

        /// <summary>İhlal edilen politika kuralı kodu ("policy.harassment.v2").</summary>
        public string? PolicyCode { get; set; }

        /// <summary>Karar gerekçesi (kullanıcıya gösterilebilir metin).</summary>
        public string? Reason { get; set; }

        /// <summary>Yalnızca ekibe görünür iç not.</summary>
        public string? InternalNote { get; set; }

        /// <summary>Yapay zekâ risk skoru (0.0000–1.0000). ActorType = AiClassifier iken dolu.</summary>
        public decimal? AiScore { get; set; }

        /// <summary>Yapay zekâ modeli/sürümü ("moderation-v3"). Karar kalitesi izlemek için.</summary>
        public string? AiModelVersion { get; set; }

        /// <summary>Kısıtlamanın kendiliğinden biteceği an (UTC) — geçici askıya almalarda.</summary>
        public DateTime? EffectiveUntilUtc { get; set; }

        /// <summary>Yazar bu karara itiraz etti mi?</summary>
        public bool IsAppealed { get; set; }

        /// <summary>İtirazın yapıldığı an (UTC).</summary>
        public DateTime? AppealedAtUtc { get; set; }

        /// <summary>İtiraz sonucunda karar geri alındı mı?</summary>
        public bool IsReversed { get; set; }

        /// <summary>Eylemin gerçekleştiği an (UTC).</summary>
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
