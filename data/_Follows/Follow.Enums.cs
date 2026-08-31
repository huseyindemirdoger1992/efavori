using System;

namespace data._Follows
{
    // ════════════════════════════════════════════════════════════════════════
    //  efavori — Sosyal Graf Modülü (Follows & Friendships V1)
    //  Enum kataloğu — tinyint (byte) olarak saklanır.
    //  KURAL: Değerlerin ARASINA ekleme yapılmaz; yalnızca SONA eklenir.
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Arkadaşlık isteğinin/ilişkisinin yaşam döngüsü durumu.
    /// Eski <c>bool? Status</c> alanının yerini alır: bir boolean "beklemede",
    /// "reddedildi", "iptal edildi" ve "arkadaşlıktan çıkarıldı" durumlarını
    /// birbirinden ayıramaz.
    ///
    /// ENGELLEME BU ENUM'DA YOKTUR (§8): engelleme arkadaşlıktan bağımsız bir
    /// ilişkidir ve <see cref="UserBlocks"/> tablosunda tutulur.
    /// </summary>
    public enum FriendshipStatus : byte
    {
        /// <summary>İstek gönderildi, alıcının yanıtı bekleniyor.</summary>
        Pending = 1,

        /// <summary>Kabul edildi — iki kullanıcı arkadaştır.</summary>
        Accepted = 2,

        /// <summary>Alıcı isteği reddetti.</summary>
        Rejected = 3,

        /// <summary>Gönderen isteği yanıtlanmadan geri çekti.</summary>
        Cancelled = 4,

        /// <summary>Daha önce kabul edilmiş arkadaşlık taraflardan biri tarafından sonlandırıldı.</summary>
        Unfriended = 5,

        /// <summary>İstek yanıtlanmadan geçerlilik süresi doldu (arka plan servisi işaretler).</summary>
        Expired = 6
    }

    /// <summary>
    /// Takip ilişkisinin durumu. Gizli (Private) hesaplarda takip onay gerektirir;
    /// bu yüzden takip de bir durum makinesidir.
    /// </summary>
    public enum FollowStatus : byte
    {
        /// <summary>Takip aktif.</summary>
        Active = 1,

        /// <summary>Gizli hesap — takip isteği onay bekliyor.</summary>
        PendingApproval = 2,

        /// <summary>Takip isteği reddedildi.</summary>
        Rejected = 3,

        /// <summary>Takipten çıkıldı (geçmiş kaydı olarak durur).</summary>
        Unfollowed = 4,

        /// <summary>Hedef kullanıcı takipçiyi kaldırdı ("takipçiyi çıkar").</summary>
        RemovedByTarget = 5
    }

    /// <summary>
    /// Kullanıcı engelleme gerekçesi. Serbest metin yerine enum kullanılır ki
    /// moderasyon ekibi engel istatistiklerini anlamlı biçimde raporlayabilsin.
    /// </summary>
    public enum BlockReason : byte
    {
        /// <summary>Belirtilmedi.</summary>
        Unspecified = 1,

        /// <summary>Taciz / rahatsız etme.</summary>
        Harassment = 2,

        /// <summary>Spam / istenmeyen reklam.</summary>
        Spam = 3,

        /// <summary>Uygunsuz içerik.</summary>
        InappropriateContent = 4,

        /// <summary>Sahte hesap / taklit.</summary>
        Impersonation = 5,

        /// <summary>Gizlilik kaygısı — sadece görünmek istemiyor.</summary>
        Privacy = 6,

        /// <summary>Dolandırıcılık şüphesi.</summary>
        Fraud = 7,

        /// <summary>Diğer.</summary>
        Other = 100
    }
}
