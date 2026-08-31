using System;
using data._Attribute;
using data._Products;
using data.Owned;

namespace data._Shares
{
    /// <summary>
    /// SOSYAL GÖNDERİ (§11) — platformun sosyal ağ çekirdeği.
    ///
    /// ESKİ MODELDEN FARKI:
    /// Önceki tasarımda sahiplik <c>bool IsUser + Guid UserStoreId</c> ile
    /// modellenmişti. Bu yaklaşımda tek bir Guid kolonu duruma göre Users veya Store
    /// tablosunu işaret ettiği için FOREIGN KEY kurulamıyordu; var olmayan bir mağazaya
    /// ait gönderi oluşturulmasını veritabanı engelleyemiyordu ve JOIN'ler tip güvensizdi.
    ///
    /// YENİ MODEL: <see cref="AuthorUserId"/> ve <see cref="AuthorStoreId"/> ayrı,
    /// nullable ve GERÇEK FK'li kolonlardır. <see cref="AuthorType"/> hangisinin dolu
    /// olduğunu belirtir ve <c>CK_Posts_AuthorXor</c> CHECK kısıtı tutarlılığı
    /// veritabanı seviyesinde zorlar. Mağaza gönderilerinde işlemi yapan personel
    /// <c>CreatedByUserId</c> alanında izlenir; böylece hem kurumsal sahiplik hem
    /// bireysel sorumluluk kaydedilir.
    ///
    /// MEDYA (§13, §72): Gönderi üzerinde <c>CoverImage</c> gibi fiziksel medya alanı
    /// YOKTUR. Tüm görsel/video/ses/belge <see cref="PostMedia"/> üzerinden merkezî
    /// <c>data._Galleries.Media</c> deposuna bağlanır. Böylece tek gönderide 1 fotoğraf,
    /// 10 fotoğraf, video, ses ve belge birlikte kullanılabilir.
    ///
    /// SAYAÇLAR (§24): <see cref="Interaction"/> yalnızca ÖNBELLEKTİR; gerçeğin kaynağı
    /// PostReactions / PostComments / PostShares / SavedPosts tablolarıdır.
    ///
    /// GÖRÜNTÜLENME (§63): Ham görüntülenme kayıtları bu tabloda TUTULMAZ; ayrı ve
    /// salt-ekleme olan <see cref="ContentViewEvents"/> tablosuna yazılır, günlük
    /// olarak toplanır ve buradaki sayaca yalnızca özet yansıtılır.
    /// </summary>
    public class Posts : SocialEntityBase
    {
        // ── Sahiplik ──────────────────────────────────────────────────────────
        /// <summary>Gönderi sahibinin türü (kullanıcı veya mağaza).</summary>
        public PostAuthorType AuthorType { get; set; } = PostAuthorType.User;

        /// <summary>Gönderiyi paylaşan kullanıcı (Users.Id). AuthorType = User iken DOLU.</summary>
        public Guid? AuthorUserId { get; set; }

        /// <summary>Gönderiyi paylaşan mağaza (Store.Id). AuthorType = Store iken DOLU.</summary>
        public Guid? AuthorStoreId { get; set; }

        // ── İçerik ────────────────────────────────────────────────────────────
        /// <summary>Gönderinin biçim türü.</summary>
        public PostType PostType { get; set; } = PostType.Text;

        /// <summary>Başlık (opsiyonel — uzun gönderiler ve mağaza duyuruları için).</summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gönderi metni. Zengin metin değil DÜZ METİN olarak saklanır; mention ve
        /// hashtag konumları ayrı tablolarda indekslidir (PostMentions / PostHashtags).
        /// </summary>
        public string? Content { get; set; }

        /// <summary>İçeriğin dili (mevcut data._Attribute.Language enum'ı) — çeviri ve akış filtresi için.</summary>
        public Language? Language { get; set; }

        /// <summary>
        /// SEO uyumlu URL parçası. YALNIZCA <see cref="Visibility"/> = Public olan
        /// gönderilerde üretilir; gizli içeriğin adresi tahmin edilebilir olmamalıdır.
        /// </summary>
        public string? Slug { get; set; }

        /// <summary>SEO meta verileri (mevcut ortak owned tip).</summary>
        public Meta? Meta { get; set; }

        // ── Bağlamsal referanslar ─────────────────────────────────────────────
        /// <summary>Tanıtılan ürün (Products.Id). PostType = Product iken dolu.</summary>
        public Guid? RelatedProductId { get; set; }

        /// <summary>Tanıtılan kampanya (Campaigns.Id). PostType = Campaign iken dolu.</summary>
        public Guid? RelatedCampaignId { get; set; }

        /// <summary>
        /// Yeniden paylaşımda orijinal gönderi (Posts.Id). PostType = Repost iken dolu.
        /// Ayrıntılı repost kaydı ayrıca <see cref="PostReposts"/> tablosundadır.
        /// </summary>
        public Guid? OriginalPostId { get; set; }

        /// <summary>Dış bağlantı adresi. PostType = Link iken dolu.</summary>
        public string? LinkUrl { get; set; }

        /// <summary>Dış bağlantının önizleme başlığı (link unfurl snapshot).</summary>
        public string? LinkTitle { get; set; }

        /// <summary>Dış bağlantının önizleme açıklaması.</summary>
        public string? LinkDescription { get; set; }

        /// <summary>Serbest metin konum etiketi ("Kadıköy, İstanbul").</summary>
        public string? LocationText { get; set; }

        // ── Görünürlük ve yayın ───────────────────────────────────────────────
        /// <summary>Hedef kitle.</summary>
        public PostVisibility Visibility { get; set; } = PostVisibility.Public;

        /// <summary>Yayın yaşam döngüsü durumu.</summary>
        public PostStatus Status { get; set; } = PostStatus.Published;

        /// <summary>
        /// Yayın anı (UTC). Gelecek bir tarih verilir ve Status = Scheduled yapılırsa
        /// arka plan servisi o anda gönderiyi yayına alır.
        /// </summary>
        public DateTime PublishAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>Gönderinin otomatik olarak süreceği an (UTC) — hikâyeler için.</summary>
        public DateTime? ExpiresAtUtc { get; set; }

        /// <summary>İçeriğin en son düzenlendiği an (UTC). "Düzenlendi" rozetini tetikler.</summary>
        public DateTime? EditedAtUtc { get; set; }

        /// <summary>Kaç kez düzenlendi? Şeffaflık ve kötüye kullanım tespiti için.</summary>
        public int EditCount { get; set; }

        // ── Etkileşim izinleri ────────────────────────────────────────────────
        /// <summary>Yorum yapılabilir mi?</summary>
        public bool AllowComments { get; set; } = true;

        /// <summary>Paylaşılabilir/yeniden paylaşılabilir mi?</summary>
        public bool AllowSharing { get; set; } = true;

        /// <summary>Tepki verilebilir mi?</summary>
        public bool AllowReactions { get; set; } = true;

        /// <summary>Profil/mağaza sayfasının en üstüne sabitlendi mi?</summary>
        public bool IsPinned { get; set; }

        /// <summary>Editör/algoritma tarafından öne çıkarıldı mı?</summary>
        public bool IsFeatured { get; set; }

        /// <summary>Sponsorlu (ücretli) içerik mi? Yasal olarak etiketlenmesi gerekir.</summary>
        public bool IsSponsored { get; set; }

        /// <summary>Hassas içerik uyarısı gösterilsin mi? (kullanıcı beyanı)</summary>
        public bool IsSensitiveContent { get; set; }

        // ── Moderasyon (§23) ──────────────────────────────────────────────────
        /// <summary>Moderasyon durumu (platform genelinde ortak enum — data._Products).</summary>
        public ModerationStatus ModerationStatus { get; set; } = ModerationStatus.Approved;

        /// <summary>Moderasyon gerekçesi/notu.</summary>
        public string? ModerationReason { get; set; }

        /// <summary>Moderasyon kararını veren kullanıcı (Users.Id).</summary>
        public Guid? ModeratedByUserId { get; set; }

        /// <summary>Moderasyon kararının anı (UTC).</summary>
        public DateTime? ModeratedAtUtc { get; set; }

        /// <summary>Yapay zekâ risk skoru (0.0000–1.0000). Eşiği aşanlar insan kuyruğuna düşer.</summary>
        public decimal? AiModerationScore { get; set; }

        /// <summary>Açık şikâyet sayısı (önbellek — kaynak: ContentReports).</summary>
        public int ReportCount { get; set; }

        // ── Sayaç önbelleği ───────────────────────────────────────────────────
        /// <summary>Etkileşim sayaçları (ÖNBELLEK — gerçeğin kaynağı etkileşim tablolarıdır).</summary>
        public InteractionCounts Interaction { get; set; } = new();

        /// <summary>
        /// Sıralama puanı (önbellek). Etkileşim ve tazelikten türetilir; akış
        /// sıralamasını her istekte hesaplamak yerine arka planda güncellenir.
        /// </summary>
        public decimal RankScore { get; set; }
    }
}
