using System;
using data._Products;

namespace data._Shares
{
    /// <summary>
    /// GÖNDERİ TEPKİSİ (§15) — Facebook tarzı çoklu tepki.
    ///
    /// TEKİLLİK: (PostId, UserId) çifti tekildir. Bir kullanıcının bir gönderide
    /// aynı anda YALNIZCA BİR aktif tepkisi olabilir. Tepki değiştirme, yeni satır
    /// eklemek yerine mevcut satırın <see cref="ReactionType"/> alanını günceller —
    /// böylece "kaç kişi tepki verdi" sayımı her zaman doğru kalır.
    ///
    /// Tepki geri çekildiğinde satır soft-delete edilir; tekil indeks soft-delete
    /// filtreli olduğu için kullanıcı daha sonra yeniden tepki verebilir.
    /// </summary>
    public class PostReactions : SocialEntityBase
    {
        /// <summary>Tepki verilen gönderi (Posts.Id).</summary>
        public Guid PostId { get; set; }

        /// <summary>Tepkiyi veren kullanıcı (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>Tepki türü.</summary>
        public ReactionType ReactionType { get; set; } = ReactionType.Like;

        /// <summary>Tepkinin en son değiştirildiği an (UTC). İlk kayıtta null.</summary>
        public DateTime? ChangedAtUtc { get; set; }
    }

    /// <summary>
    /// GÖNDERİ YORUMU (§16) — sınırsız derinlikte ağaç.
    ///
    /// <see cref="ParentCommentId"/> null ise kök yorum, dolu ise yanıttır. Veri modeli
    /// teorik olarak sınırsız derinliği destekler; ancak uygulama katmanı
    /// <see cref="Depth"/> alanına bakarak azami derinliği (öneri: 5) uygular ve daha
    /// derin yanıtları en alt seviyeye düzleştirir. Derinliğin sütunda tutulması,
    /// her okumada özyinelemeli sorgu çalıştırmayı önler.
    ///
    /// <see cref="RootCommentId"/> bir konuşma dalının tamamını TEK indeks aramasıyla
    /// getirmeyi sağlar (özyinelemeli CTE gerekmez).
    ///
    /// SİLME DAVRANIŞI: Yanıtı olan bir yorum silindiğinde satır fiziksel olarak
    /// KALDIRILMAZ; Status = DeletedByAuthor yapılır ve içerik boşaltılır. Böylece
    /// alt yanıtlar öksüz kalmaz ve konuşma bağlamı korunur.
    /// </summary>
    public class PostComments : SocialEntityBase
    {
        /// <summary>Yorumun ait olduğu gönderi (Posts.Id).</summary>
        public Guid PostId { get; set; }

        /// <summary>Yorumu yazan kullanıcı (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Mağaza adına yazıldıysa mağaza (Store.Id). Vitrinde "Satıcı" rozetiyle
        /// gösterilir; yazan personel yine <see cref="UserId"/> alanındadır.
        /// </summary>
        public Guid? AuthorStoreId { get; set; }

        /// <summary>Üst yorum (PostComments.Id). null = kök yorum.</summary>
        public Guid? ParentCommentId { get; set; }

        /// <summary>
        /// Bu yorumun bağlı olduğu KÖK yorum (PostComments.Id). Kök yorumlarda
        /// kendi Id'sine eşittir. Bir konuşma dalını tek sorguda getirmek içindir.
        /// </summary>
        public Guid RootCommentId { get; set; }

        /// <summary>Ağaçtaki derinlik (kök = 0). Azami derinlik denetimi için denormalize.</summary>
        public byte Depth { get; set; }

        /// <summary>Yorum metni (düz metin).</summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>Görünürlük durumu.</summary>
        public CommentStatus Status { get; set; } = CommentStatus.Published;

        /// <summary>Moderasyon durumu (platform genelinde ortak enum).</summary>
        public ModerationStatus ModerationStatus { get; set; } = ModerationStatus.Approved;

        /// <summary>Moderasyon gerekçesi.</summary>
        public string? ModerationReason { get; set; }

        /// <summary>Moderasyon kararını veren kullanıcı (Users.Id).</summary>
        public Guid? ModeratedByUserId { get; set; }

        /// <summary>Moderasyon anı (UTC).</summary>
        public DateTime? ModeratedAtUtc { get; set; }

        /// <summary>İçeriğin düzenlendiği an (UTC).</summary>
        public DateTime? EditedAtUtc { get; set; }

        /// <summary>Gönderi sahibi tarafından sabitlendi mi? (en üstte gösterilir)</summary>
        public bool IsPinned { get; set; }

        /// <summary>Gönderi sahibi bu yorumu beğendi mi? ("Yazar beğendi" rozeti)</summary>
        public bool IsLikedByAuthor { get; set; }

        // ── Sayaç önbelleği (kaynak: CommentReactions / alt yorumlar) ─────────
        /// <summary>Yalnızca Like türündeki tepki sayısı (önbellek).</summary>
        public int LikeCount { get; set; }

        /// <summary>Tüm tepki türlerinin toplamı (önbellek).</summary>
        public int ReactionCount { get; set; }

        /// <summary>Doğrudan alt yanıt sayısı (önbellek).</summary>
        public int ReplyCount { get; set; }

        /// <summary>Açık şikâyet sayısı (önbellek — kaynak: ContentReports).</summary>
        public int ReportCount { get; set; }
    }

    /// <summary>
    /// YORUM TEPKİSİ (§17).
    /// TEKİLLİK: (CommentId, UserId) — bir kullanıcı bir yoruma tek tepki verir.
    /// </summary>
    public class CommentReactions : SocialEntityBase
    {
        /// <summary>Tepki verilen yorum (PostComments.Id).</summary>
        public Guid CommentId { get; set; }

        /// <summary>Tepkiyi veren kullanıcı (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>Tepki türü.</summary>
        public ReactionType ReactionType { get; set; } = ReactionType.Like;

        /// <summary>Tepkinin en son değiştirildiği an (UTC).</summary>
        public DateTime? ChangedAtUtc { get; set; }
    }

    /// <summary>
    /// GÖNDERİ PAYLAŞIMI (§18) — "bu içeriği başka bir yere ilettim" olayı.
    ///
    /// <see cref="PostReposts"/> ile KARIŞTIRILMAMALIDIR:
    ///  • PostShares  = bir DAĞITIM OLAYIDIR (mesajla gönderdim, linki kopyaladım,
    ///                  WhatsApp'a attım). Yeni bir gönderi ÜRETMEZ.
    ///  • PostReposts = yeni bir Posts satırı ÜRETİR ve paylaşanın akışında görünür.
    ///
    /// İkisini ayırmak, "kaç kez iletildi" ile "kaç kez yeniden paylaşıldı"
    /// metriklerinin birbirine karışmasını önler.
    /// </summary>
    public class PostShares : SocialEntityBase
    {
        /// <summary>Paylaşılan gönderi (Posts.Id).</summary>
        public Guid PostId { get; set; }

        /// <summary>Paylaşan kullanıcı (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>Paylaşımın hedef türü.</summary>
        public ShareTargetType TargetType { get; set; } = ShareTargetType.OwnTimeline;

        /// <summary>
        /// Hedefin kimliği: DirectMessage'da alıcı Users.Id, GroupConversation'da
        /// ChatConversations.Id, StorePage'de Store.Id. CopyLink/ExternalPlatform'da null.
        /// </summary>
        public Guid? TargetId { get; set; }

        /// <summary>Harici platform adı ("WhatsApp", "X", "Facebook"). TargetType = ExternalPlatform iken dolu.</summary>
        public string? ExternalPlatform { get; set; }

        /// <summary>Paylaşıma eklenen kısa mesaj (opsiyonel).</summary>
        public string? Message { get; set; }
    }

    /// <summary>
    /// YENİDEN PAYLAŞIM / REPOST (§18).
    ///
    /// Repost işlemi İKİ kayıt üretir:
    ///  1) Yeni bir <c>Posts</c> satırı (PostType = Repost, OriginalPostId dolu),
    ///  2) Bu tabloda ikisini birbirine bağlayan bir satır.
    ///
    /// Bu ayrım sayesinde "bu gönderi kaç kez repost edildi" sorgusu, Posts tablosunu
    /// taramadan tek indeks aramasıyla yanıtlanır.
    ///
    /// TEKİLLİK: (OriginalPostId, UserId) — bir kullanıcı aynı gönderiyi bir kez
    /// repost edebilir (geri alıp tekrar yapabilir; soft-delete filtresi buna izin verir).
    /// </summary>
    public class PostReposts : SocialEntityBase
    {
        /// <summary>Yeniden paylaşılan ORİJİNAL gönderi (Posts.Id).</summary>
        public Guid OriginalPostId { get; set; }

        /// <summary>Repost işlemiyle oluşturulan YENİ gönderi (Posts.Id).</summary>
        public Guid RepostPostId { get; set; }

        /// <summary>Repost eden kullanıcı (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>Repost bir mağaza sayfasından yapıldıysa mağaza (Store.Id).</summary>
        public Guid? RepostStoreId { get; set; }

        /// <summary>Reposta eklenen yorum. Boşsa "yalın repost" sayılır.</summary>
        public string? QuoteText { get; set; }
    }

    /// <summary>
    /// KAYDEDİLEN GÖNDERİ / YER İMİ (§19).
    /// TEKİLLİK: (UserId, PostId) — aynı gönderi iki kez kaydedilemez.
    /// </summary>
    public class SavedPosts : SocialEntityBase
    {
        /// <summary>Kaydeden kullanıcı (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>Kaydedilen gönderi (Posts.Id).</summary>
        public Guid PostId { get; set; }

        /// <summary>
        /// Kullanıcının oluşturduğu koleksiyon adı ("Tarifler", "Sonra Oku").
        /// Null = varsayılan koleksiyon. Koleksiyonlar ayrı tablo gerektirecek kadar
        /// karmaşıklaşırsa buradan çıkarılıp normalize edilebilir.
        /// </summary>
        public string? CollectionName { get; set; }

        /// <summary>Kullanıcının kendine yazdığı özel not.</summary>
        public string? Note { get; set; }
    }
}
