using System;

namespace data._Shares
{
    // ════════════════════════════════════════════════════════════════════════
    //  efavori — Sosyal İçerik Modülü (Social Content V1)
    //  Enum kataloğu — tinyint (byte) olarak saklanır.
    //  KURAL: Değerlerin ARASINA ekleme yapılmaz; yalnızca SONA eklenir.
    //
    //  MODERASYON ENUM'LARI BURADA YENİDEN TANIMLANMAZ. Platform genelinde ortak
    //  olan ModerationStatus / ReportReason / ReportStatus enum'ları
    //  data._Products namespace'inde tanımlıdır ve buradan REFERANS ALINIR (§22).
    //  Aynı kavram için ikinci bir enum üretmek, kavram tekrarına yol açardı.
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Gönderinin sahibi bir KULLANICI mı yoksa bir MAĞAZA mı?
    ///
    /// Eski modeldeki <c>bool IsUser + Guid UserStoreId</c> yaklaşımının yerini alır.
    /// O yaklaşım tip güvenliği ve referans bütünlüğü açısından yetersizdi: tek bir
    /// Guid kolonu iki farklı tabloyu işaret ettiği için FOREIGN KEY kurulamıyor,
    /// yanlış tablodan gelen bir kimlik veritabanı tarafından reddedilemiyordu.
    ///
    /// Yeni model: <c>AuthorUserId</c> ve <c>AuthorStoreId</c> ayrı, nullable ve
    /// GERÇEK FK'li kolonlardır; bu enum hangisinin dolu olduğunu belirtir ve bir
    /// CHECK kısıtı tutarlılığı veritabanı seviyesinde garanti eder.
    /// </summary>
    public enum PostAuthorType : byte
    {
        /// <summary>Gönderiyi bir kullanıcı kendi profilinden paylaştı.</summary>
        User = 1,

        /// <summary>Gönderiyi bir mağaza kendi sayfasından paylaştı.</summary>
        Store = 2
    }

    /// <summary>Gönderinin biçim türü — vitrin şablonunu ve doğrulama kurallarını belirler.</summary>
    public enum PostType : byte
    {
        /// <summary>Yalnızca metin.</summary>
        Text = 1,

        /// <summary>Tek fotoğraf.</summary>
        Photo = 2,

        /// <summary>Çoklu fotoğraf (albüm/karusel).</summary>
        Album = 3,

        /// <summary>Video.</summary>
        Video = 4,

        /// <summary>Kısa dikey video (reel).</summary>
        Reel = 5,

        /// <summary>Ses kaydı.</summary>
        Audio = 6,

        /// <summary>Dış bağlantı önizlemesi.</summary>
        Link = 7,

        /// <summary>Ürün tanıtımı — bağlı ürün(ler) kart olarak gösterilir.</summary>
        Product = 8,

        /// <summary>Kampanya/indirim duyurusu (mağaza gönderisi).</summary>
        Campaign = 9,

        /// <summary>Anket.</summary>
        Poll = 10,

        /// <summary>24 saatlik hikâye.</summary>
        Story = 11,

        /// <summary>Yeniden paylaşım (repost) — orijinal gönderiye referans taşır.</summary>
        Repost = 12,

        /// <summary>Belge paylaşımı.</summary>
        Document = 13,

        /// <summary>Sistem/duyuru gönderisi.</summary>
        Announcement = 14
    }

    /// <summary>
    /// Gönderinin YAYIN yaşam döngüsü. Moderasyon durumundan (ModerationStatus)
    /// BAĞIMSIZDIR: bir gönderi Published olsa bile moderasyon Rejected ise görünmez.
    /// </summary>
    public enum PostStatus : byte
    {
        /// <summary>Taslak — yalnızca yazarı görür.</summary>
        Draft = 1,

        /// <summary>Zamanlanmış — PublishAtUtc geldiğinde yayına alınacak.</summary>
        Scheduled = 2,

        /// <summary>Yayında.</summary>
        Published = 3,

        /// <summary>Yazar tarafından arşivlendi (profilde görünmez, bağlantısı çalışır).</summary>
        Archived = 4,

        /// <summary>Yazar tarafından kaldırıldı.</summary>
        RemovedByAuthor = 5,

        /// <summary>Moderasyon tarafından kaldırıldı.</summary>
        RemovedByModerator = 6,

        /// <summary>Süresi doldu (hikâye vb.).</summary>
        Expired = 7
    }

    /// <summary>
    /// Gönderinin HEDEF KİTLESİ (§12).
    ///
    /// Değerlendirme sırası (servis katmanı): önce engel (UserBlocks), sonra yazarın
    /// profil görünürlüğü (ProfileVisibility), sonra bu alan.
    ///
    /// GENİŞLETİLEBİLİRLİK: Ülke, yaş, mağaza takipçisi, yakın arkadaş gibi ileri
    /// hedeflemeler için <see cref="PostAudienceRules"/> tablosu tasarıma dâhildir;
    /// bu enum'a yeni değer eklemeden kural satırı ekleyerek genişletilebilir.
    /// </summary>
    public enum PostVisibility : byte
    {
        /// <summary>Herkese açık — giriş yapmamış ziyaretçiler dâhil.</summary>
        Public = 1,

        /// <summary>Yalnızca takipçiler.</summary>
        Followers = 2,

        /// <summary>Yalnızca arkadaşlar.</summary>
        Friends = 3,

        /// <summary>Yalnızca yazarın kendisi.</summary>
        Private = 4,

        /// <summary>Özel liste — PostAudienceUsers tablosundaki kullanıcılar.</summary>
        Custom = 5,

        /// <summary>Arkadaşlar hariç belirli kişiler (PostAudienceUsers, IsExcluded = true).</summary>
        FriendsExcept = 6,

        /// <summary>Yakın arkadaşlar (Friendships.IsCloseFriend* işaretli olanlar).</summary>
        CloseFriends = 7,

        /// <summary>Yalnızca mağaza takipçileri (mağaza gönderileri için).</summary>
        StoreFollowers = 8
    }

    /// <summary>
    /// <see cref="PostAudienceRules"/> içindeki bir kuralın türü — ileri hedefleme.
    /// </summary>
    public enum PostAudienceRuleType : byte
    {
        /// <summary>Belirli ülkelerde görünür (Value = Country.id).</summary>
        Country = 1,

        /// <summary>Asgari yaş sınırı (Value = yaş).</summary>
        MinimumAge = 2,

        /// <summary>Azami yaş sınırı (Value = yaş).</summary>
        MaximumAge = 3,

        /// <summary>Belirli bir dili tercih edenlere görünür (Value = Language byte değeri).</summary>
        Language = 4,

        /// <summary>Belirli bir mağazanın takipçilerine görünür (Value = Store.Id).</summary>
        StoreFollower = 5,

        /// <summary>Yalnızca bu ürünü satın almış kullanıcılara görünür (Value = Products.Id).</summary>
        ProductBuyer = 6
    }

    /// <summary>
    /// Tepki (reaction) türü — Facebook tarzı çoklu tepki.
    /// <c>Like</c> özel bir durumdur: <c>InteractionCounts.LikeCount</c> yalnızca
    /// bu türü sayar, <c>ReactionCount</c> ise tüm türlerin toplamıdır.
    /// </summary>
    public enum ReactionType : byte
    {
        /// <summary>Beğen.</summary>
        Like = 1,

        /// <summary>Sevdim.</summary>
        Love = 2,

        /// <summary>Güldüm.</summary>
        Haha = 3,

        /// <summary>Şaşırdım.</summary>
        Wow = 4,

        /// <summary>Üzüldüm.</summary>
        Sad = 5,

        /// <summary>Kızdım.</summary>
        Angry = 6,

        /// <summary>İlgi/destek (care).</summary>
        Care = 7,

        /// <summary>Kutlarım.</summary>
        Celebrate = 8,

        /// <summary>Destekliyorum.</summary>
        Support = 9,

        /// <summary>İlgimi çekti / kaydettim niyeti.</summary>
        Insightful = 10
    }

    /// <summary>
    /// Yorumun görünürlük durumu (moderasyondan bağımsız yaşam döngüsü).
    /// </summary>
    public enum CommentStatus : byte
    {
        /// <summary>Yayında.</summary>
        Published = 1,

        /// <summary>Gönderi sahibi tarafından gizlendi.</summary>
        HiddenByPostAuthor = 2,

        /// <summary>Yazarı tarafından silindi ("Bu yorum silindi" olarak görünür).</summary>
        DeletedByAuthor = 3,

        /// <summary>Moderasyon tarafından kaldırıldı.</summary>
        RemovedByModerator = 4
    }

    /// <summary>
    /// Bir paylaşımın (share) nereye yapıldığı (§18).
    /// </summary>
    public enum ShareTargetType : byte
    {
        /// <summary>Kullanıcının kendi profil akışına.</summary>
        OwnTimeline = 1,

        /// <summary>Doğrudan mesaj olarak bir kullanıcıya.</summary>
        DirectMessage = 2,

        /// <summary>Bir sohbet grubuna.</summary>
        GroupConversation = 3,

        /// <summary>Kullanıcının yönettiği bir mağaza sayfasına.</summary>
        StorePage = 4,

        /// <summary>Hikâye olarak.</summary>
        Story = 5,

        /// <summary>Bağlantı kopyalandı (dış paylaşım).</summary>
        CopyLink = 6,

        /// <summary>Harici platforma (WhatsApp, X, Facebook...).</summary>
        ExternalPlatform = 7
    }

    /// <summary>
    /// Şikâyet edilen / moderasyona konu olan / görüntülenen içeriğin türü.
    /// <see cref="ContentReports"/>, <see cref="ContentModerationActions"/> ve
    /// <see cref="ContentViewEvents"/> tablolarında ORTAK kullanılır.
    ///
    /// DİKKAT: Bu enum tek başına polimorfik bir FK oluşturmaz. İlgili tablolarda
    /// hedef başına AYRI ve NULLABLE gerçek FK kolonları vardır; bu enum yalnızca
    /// hangi kolonun dolu olduğunu belirten ayırıcıdır ve CHECK kısıtıyla
    /// tutarlılığı garanti edilir (§42).
    /// </summary>
    public enum ContentTargetType : byte
    {
        /// <summary>Sosyal gönderi (Posts.Id).</summary>
        Post = 1,

        /// <summary>Gönderi yorumu (PostComments.Id).</summary>
        PostComment = 2,

        /// <summary>Kullanıcı profili (Users.Id).</summary>
        User = 3,

        /// <summary>Mağaza (Store.Id).</summary>
        Store = 4,

        /// <summary>Ürün (Products.Id).</summary>
        Product = 5,

        /// <summary>Ürün yorumu (ProductReviews.Id).</summary>
        ProductReview = 6,

        /// <summary>Ürün sorusu/cevabı (ProductQuestions.Id).</summary>
        ProductQuestion = 7,

        /// <summary>Sohbet mesajı (ChatMessages.Id).</summary>
        ChatMessage = 8,

        /// <summary>Makale (Articles.Id).</summary>
        Article = 9,

        /// <summary>Medya asset'i (Media.Id).</summary>
        Media = 10
    }

    /// <summary>
    /// Moderasyon eyleminin türü (§23) — moderasyon denetim izinin kaydıdır.
    /// </summary>
    public enum ModerationActionType : byte
    {
        /// <summary>İçerik onaylandı.</summary>
        Approve = 1,

        /// <summary>İçerik reddedildi.</summary>
        Reject = 2,

        /// <summary>İçerik gizlendi (silinmedi).</summary>
        Hide = 3,

        /// <summary>İçerik kaldırıldı.</summary>
        Remove = 4,

        /// <summary>İçeriğe yaş kısıtı uygulandı.</summary>
        AgeRestrict = 5,

        /// <summary>Yazara uyarı gönderildi.</summary>
        WarnAuthor = 6,

        /// <summary>Yazarın içerik üretimi geçici olarak kısıtlandı.</summary>
        RestrictAuthor = 7,

        /// <summary>Yazarın hesabı askıya alındı.</summary>
        SuspendAuthor = 8,

        /// <summary>Yazarın hesabı kalıcı olarak yasaklandı.</summary>
        BanAuthor = 9,

        /// <summary>Önceki bir moderasyon kararı geri alındı (itiraz kabul edildi).</summary>
        ReverseDecision = 10,

        /// <summary>Kayıt yalnızca not amaçlıdır, içeriğe etki etmez.</summary>
        NoteOnly = 11
    }

    /// <summary>
    /// Moderasyon kararını kimin verdiği.
    /// </summary>
    public enum ModerationActorType : byte
    {
        /// <summary>İnsan moderatör.</summary>
        Moderator = 1,

        /// <summary>Otomatik kural motoru (kelime listesi, oran sınırı).</summary>
        AutomatedRule = 2,

        /// <summary>Yapay zekâ sınıflandırıcısı.</summary>
        AiClassifier = 3,

        /// <summary>Harici sağlayıcı (üçüncü parti içerik denetimi).</summary>
        ExternalProvider = 4,

        /// <summary>İçeriğin yazarı (kendi içeriğini kaldırdı).</summary>
        Author = 5
    }

    /// <summary>
    /// Bir görüntülenme olayının hangi yüzeyden geldiği (analitik kırılımı için).
    /// </summary>
    public enum ViewSourceSurface : byte
    {
        /// <summary>Ana akış.</summary>
        HomeFeed = 1,

        /// <summary>Profil sayfası.</summary>
        Profile = 2,

        /// <summary>Mağaza sayfası.</summary>
        StorePage = 3,

        /// <summary>İçeriğin kendi detay sayfası.</summary>
        DetailPage = 4,

        /// <summary>Keşfet/öneri.</summary>
        Explore = 5,

        /// <summary>Arama sonuçları.</summary>
        Search = 6,

        /// <summary>Hashtag sayfası.</summary>
        Hashtag = 7,

        /// <summary>Bildirimden gelen ziyaret.</summary>
        Notification = 8,

        /// <summary>Dış bağlantı (paylaşılan link).</summary>
        ExternalLink = 9
    }
}
