using System;

namespace data._Users
{
    /// <summary>
    /// GİZLİLİK AYARLARI — "kimler ne yapabilir?" (§47).
    ///
    /// <see cref="Users"/> ile 1:1'dir. Satır YOKSA aşağıdaki C# varsayılanları geçerlidir;
    /// kayıt anında oluşturmak zorunlu değildir.
    ///
    /// UYGULANMA SIRASI (servis katmanı kuralı — bu sıra ASLA değişmemelidir):
    ///   1) <c>data._Follows.UserBlocks</c> — engel varsa her şey reddedilir.
    ///   2) <see cref="UserProfiles.ProfileVisibility"/> — profil erişilebilir mi?
    ///   3) Buradaki ilgili "WhoCan..." kuralı.
    ///   4) İçerik bazlı görünürlük (<c>data._Shares.Posts.Visibility</c>).
    /// </summary>
    public class UserPrivacySettings : UserEntityBase
    {
        /// <summary>Ayarların sahibi (Users.Id). 1:1 — soft-delete filtreli TEKİL indeks.</summary>
        public Guid UserId { get; set; }

        // ── İletişim ──────────────────────────────────────────────────────────
        /// <summary>Kimler doğrudan mesaj gönderebilir?</summary>
        public PrivacyAudience WhoCanMessage { get; set; } = PrivacyAudience.Everyone;

        /// <summary>Kimler arkadaşlık isteği gönderebilir?</summary>
        public PrivacyAudience WhoCanSendFriendRequest { get; set; } = PrivacyAudience.Everyone;

        /// <summary>Kimler takip edebilir? (NoOne = takip özelliği kapalı)</summary>
        public PrivacyAudience WhoCanFollow { get; set; } = PrivacyAudience.Everyone;

        /// <summary>Kimler sesli arayabilir?</summary>
        public PrivacyAudience WhoCanVoiceCall { get; set; } = PrivacyAudience.FriendsOnly;

        /// <summary>Kimler görüntülü arayabilir?</summary>
        public PrivacyAudience WhoCanVideoCall { get; set; } = PrivacyAudience.FriendsOnly;

        // ── İçerik etkileşimi ─────────────────────────────────────────────────
        /// <summary>Kimler gönderilere yorum yapabilir?</summary>
        public PrivacyAudience WhoCanComment { get; set; } = PrivacyAudience.Everyone;

        /// <summary>Kimler bu kullanıcıyı gönderi/fotoğraflarda etiketleyebilir?</summary>
        public PrivacyAudience WhoCanTag { get; set; } = PrivacyAudience.FriendsOnly;

        /// <summary>Kimler bu kullanıcıyı metin içinde (@) anabilir?</summary>
        public PrivacyAudience WhoCanMention { get; set; } = PrivacyAudience.Everyone;

        /// <summary>Kimler gönderileri yeniden paylaşabilir/paylaşabilir?</summary>
        public PrivacyAudience WhoCanShareMyPosts { get; set; } = PrivacyAudience.Everyone;

        /// <summary>Yeni gönderilerin varsayılan görünürlüğü (data._Shares.PostVisibility byte karşılığı).</summary>
        public byte DefaultPostVisibility { get; set; } = 1; // PostVisibility.Public

        /// <summary>Etiketlenen gönderiler profilde görünmeden önce onay istensin mi?</summary>
        public bool RequireTagApproval { get; set; }

        // ── Liste / bilgi görünürlüğü ─────────────────────────────────────────
        /// <summary>Kimler takipçi listesini görebilir?</summary>
        public PrivacyAudience WhoCanSeeFollowerList { get; set; } = PrivacyAudience.Everyone;

        /// <summary>Kimler takip edilenler listesini görebilir?</summary>
        public PrivacyAudience WhoCanSeeFollowingList { get; set; } = PrivacyAudience.Everyone;

        /// <summary>Kimler arkadaş listesini görebilir?</summary>
        public PrivacyAudience WhoCanSeeFriendList { get; set; } = PrivacyAudience.FriendsOnly;

        /// <summary>Kimler doğum tarihini görebilir?</summary>
        public PrivacyAudience WhoCanSeeBirthDate { get; set; } = PrivacyAudience.FriendsOnly;

        /// <summary>Kimler e-posta adresini görebilir?</summary>
        public PrivacyAudience WhoCanSeeEmail { get; set; } = PrivacyAudience.NoOne;

        /// <summary>Kimler telefon numarasını görebilir?</summary>
        public PrivacyAudience WhoCanSeePhone { get; set; } = PrivacyAudience.NoOne;

        /// <summary>Kimler konum bilgisini görebilir?</summary>
        public PrivacyAudience WhoCanSeeLocation { get; set; } = PrivacyAudience.FriendsOnly;

        /// <summary>Takipçi sayısı herkese gösterilsin mi?</summary>
        public bool ShowFollowerCount { get; set; } = true;

        /// <summary>Cinsiyet bilgisi profilde gösterilsin mi? (HASSAS VERİ — varsayılan kapalı)</summary>
        public bool ShowGender { get; set; }

        // ── Çevrimiçi durum ───────────────────────────────────────────────────
        /// <summary>"Son görülme" bilgisi gösterilsin mi?</summary>
        public bool ShowLastSeen { get; set; } = true;

        /// <summary>Çevrimiçi durumu gösterilsin mi?</summary>
        public bool ShowOnlineStatus { get; set; } = true;

        /// <summary>Mesajlarda okundu bilgisi (mavi tik) gönderilsin mi?</summary>
        public bool SendReadReceipts { get; set; } = true;

        /// <summary>"Yazıyor..." göstergesi gönderilsin mi?</summary>
        public bool SendTypingIndicator { get; set; } = true;

        // ── İçerik koruma ─────────────────────────────────────────────────────
        /// <summary>İçeriklerin indirilmesine izin veriliyor mu?</summary>
        public bool AllowContentDownload { get; set; }

        /// <summary>Profilin arama sonuçlarında (site içi) bulunmasına izin veriliyor mu?</summary>
        public bool DiscoverableBySearch { get; set; } = true;

        /// <summary>Profilin e-posta adresiyle bulunmasına izin veriliyor mu?</summary>
        public bool DiscoverableByEmail { get; set; }

        /// <summary>Profilin telefon numarasıyla bulunmasına izin veriliyor mu?</summary>
        public bool DiscoverableByPhone { get; set; }
    }
}
