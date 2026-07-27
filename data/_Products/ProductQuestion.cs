using System;
using data._Attribute; // Language enum'ı

namespace data._Products
{
    /// <summary>
    /// ÜRÜN SORU-CEVAP (Q&amp;A) — Amazon "Customer questions & answers" karşılığı.
    ///
    /// Tek tablo, iki rol: bir kayıt ya SORU'dur (<see cref="ParentQuestionId"/> = null)
    /// ya da o soruya CEVAP'tır (ParentQuestionId dolu). Bir soruya birden çok cevap
    /// gelebilir; cevaplar da kendi aralarında SONSUZ DERİNLİKTE
    /// (ParentQuestionId bir cevabı işaret ederek) tartışma dalı oluşturabilir.
    ///
    /// Cevabı satıcı yazarsa <see cref="IsSellerAnswer"/> = true olur ve vitrinde
    /// "Satıcı" rozetiyle öne çıkar. Fayda oyları <see cref="ProductQuestionVotes"/>'ta,
    /// toplamları ilgili kaydın denormalize <see cref="HelpfulVoteCount"/> alanında tutulur.
    /// </summary>
    public class ProductQuestions : ProductEntityBase
    {
        /// <summary>Sorunun/cevabın ait olduğu ürün (Products.Id).</summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Bu kayıt bir CEVAP ise, cevapladığı soru/cevap (ProductQuestions.Id).
        /// null = kök SORU. Dolu = CEVAP (soruya veya başka bir cevaba).
        /// </summary>
        public Guid? ParentQuestionId { get; set; }

        /// <summary>Soruyu/cevabı yazan kullanıcı (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>Soru/cevap metni.</summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>İçeriğin dili (çok dilli mağazada gösterim/çeviri için).</summary>
        public Language Language { get; set; } = Language.Tr;

        // ── Rol / durum ──────────────────────────────────────────────────────
        /// <summary>Bu kayıt satıcı tarafından yazılmış resmî bir cevap mı?</summary>
        public bool IsSellerAnswer { get; set; }

        /// <summary>
        /// Soru "en iyi/kabul edilen cevap" olarak işaretlenmiş bir cevaba sahip mi?
        /// Yalnızca kök soruda anlamlıdır; işaretli cevabın Id'sini tutar (opsiyonel).
        /// </summary>
        public Guid? AcceptedAnswerId { get; set; }

        /// <summary>Moderasyon durumu (yalnızca Approved vitrinde görünür).</summary>
        public ModerationStatus Status { get; set; } = ModerationStatus.Pending;

        /// <summary>Reddetme/gizleme gerekçesi (Status != Approved iken).</summary>
        public string? ModerationNote { get; set; }

        /// <summary>Moderasyonu yapan admin (Users.Id).</summary>
        public Guid? ModeratedByUserId { get; set; }

        /// <summary>Moderasyon anı (UTC).</summary>
        public DateTime? ModeratedAtUtc { get; set; }

        // ── Denormalize sayaçlar ─────────────────────────────────────────────
        /// <summary>"Faydalı" oy sayısı (kaynak: ProductQuestionVotes) — denormalize.</summary>
        public int HelpfulVoteCount { get; set; }

        /// <summary>"Faydalı değil" oy sayısı — denormalize.</summary>
        public int NotHelpfulVoteCount { get; set; }

        /// <summary>Bu soruya gelen doğrudan cevap sayısı — denormalize (liste için).</summary>
        public int AnswerCount { get; set; }
    }

    /// <summary>
    /// Bir soruya/cevaba verilen FAYDA OYU ("Bu cevap faydalı mıydı?").
    /// (ProductQuestionId, UserId) benzersizdir — bir kullanıcı bir içeriğe tek oy verir.
    /// Toplamlar <see cref="ProductQuestions.HelpfulVoteCount"/> alanına denormalize edilir.
    /// </summary>
    public class ProductQuestionVotes : ProductEntityBase
    {
        /// <summary>Oy verilen soru/cevap (ProductQuestions.Id).</summary>
        public Guid ProductQuestionId { get; set; }

        /// <summary>Oyu veren kullanıcı (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>Oy türü (Helpful / NotHelpful).</summary>
        public VoteType Vote { get; set; }
    }

    /// <summary>
    /// Bir soru/cevap hakkında yapılan ŞİKÂYET (report/flag).
    /// (ProductQuestionId, ReportedByUserId) benzersizdir. Moderasyon kuyruğunu besler.
    /// </summary>
    public class ProductQuestionReports : ProductEntityBase
    {
        /// <summary>Şikâyet edilen soru/cevap (ProductQuestions.Id).</summary>
        public Guid ProductQuestionId { get; set; }

        /// <summary>Şikâyet eden kullanıcı (Users.Id).</summary>
        public Guid ReportedByUserId { get; set; }

        /// <summary>Şikâyet gerekçesi.</summary>
        public ReportReason Reason { get; set; }

        /// <summary>Serbest açıklama (Reason = Other iken önerilir).</summary>
        public string? Description { get; set; }

        /// <summary>Şikâyetin işlenme durumu.</summary>
        public ReportStatus Status { get; set; } = ReportStatus.Open;

        /// <summary>Şikâyeti çözen admin (Users.Id).</summary>
        public Guid? ResolvedByUserId { get; set; }

        /// <summary>Çözüm anı (UTC).</summary>
        public DateTime? ResolvedAtUtc { get; set; }
    }
}
