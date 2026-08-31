using System;
using data.Owned;

namespace data._Users
{
    /// <summary>
    /// SOSYAL KULLANICI PROFİLİ — Facebook/Instagram karşılığı.
    ///
    /// <see cref="Users"/> ile 1:1 ilişkilidir (UserId üzerinde TEKİL indeks). Ayrılma
    /// gerekçesi: profil alanları her istekte değil, yalnızca profil/feed gösteriminde
    /// okunur; buna karşılık sayaçlar çok sık YAZILIR. Kimlik tablosunu bu yazma
    /// yükünden ve satır genişliğinden korumak için ayrı tutulmuştur.
    ///
    /// MEDYA KURALI (§72): Burada hiçbir fiziksel medya yolu/URL'i TUTULMAZ.
    /// Avatar ve kapak yalnızca <c>data._Galleries.Media</c> kayıtlarına FK'dir.
    /// </summary>
    public class UserProfiles : UserEntityBase
    {
        /// <summary>Profilin sahibi (Users.Id). 1:1 — soft-delete filtreli TEKİL indeks.</summary>
        public Guid UserId { get; set; }

        // ── Sosyal kimlik ─────────────────────────────────────────────────────
        /// <summary>
        /// Kullanıcı adı (handle) — "@ahmet". Kullanıcının gördüğü biçimdir
        /// (büyük/küçük harf korunur). Tekillik <see cref="NormalizedUsername"/> üzerindedir.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Küçük harfe normalize edilmiş kullanıcı adı. GLOBAL TEKİL (soft-delete filtreli).
        /// URL çözümlemesi ve mention (@) eşlemesi bu alan üzerinden yapılır.
        /// </summary>
        public string NormalizedUsername { get; set; } = string.Empty;

        /// <summary>Kullanıcı adının en son değiştirildiği an (UTC). Sık değişimi sınırlamak için.</summary>
        public DateTime? UsernameChangedAtUtc { get; set; }

        /// <summary>Görünen ad ("Ahmet Yılmaz", "Ahmet | Fotoğrafçı"). Tekil değildir.</summary>
        public string? DisplayName { get; set; }

        /// <summary>Ad. KİŞİSEL VERİ — görünürlüğü gizlilik ayarlarına tabidir.</summary>
        public string? FirstName { get; set; }

        /// <summary>Soyad. KİŞİSEL VERİ.</summary>
        public string? LastName { get; set; }

        /// <summary>Kısa biyografi / hakkında metni.</summary>
        public string? Bio { get; set; }

        /// <summary>Kişisel web sitesi adresi.</summary>
        public string? Website { get; set; }

        /// <summary>Serbest metin konum ("İstanbul, Türkiye"). Yapısal adres için bkz. <see cref="UserAddress"/>.</summary>
        public string? LocationText { get; set; }

        /// <summary>Ülke referansı (data._Locations.Country.Id) — dil/para birimi önerisi ve vitrin için.</summary>
        public int? CountryId { get; set; }

        /// <summary>Şehir referansı (data._Locations.Cities.Id).</summary>
        public int? CityId { get; set; }

        /// <summary>Doğum tarihi. KİŞİSEL VERİ — yaş sınırı denetimi ve doğum günü özelliği için.</summary>
        public DateOnly? BirthDate { get; set; }

        /// <summary>
        /// Cinsiyet. HASSAS VERİ — nullable'dır, zorunlu değildir ve varsayılan olarak gizlidir.
        /// Ayrıntılı uyarı için bkz. <see cref="Gender"/>.
        /// </summary>
        public Gender? Gender { get; set; }

        // ── Medya (yalnızca FK — fiziksel yol/URL yok) ────────────────────────
        /// <summary>Profil fotoğrafı (data._Galleries.Media.Id).</summary>
        public Guid? AvatarMediaId { get; set; }

        /// <summary>Kapak fotoğrafı (data._Galleries.Media.Id).</summary>
        public Guid? CoverMediaId { get; set; }

        // ── Görünürlük / doğrulama ────────────────────────────────────────────
        /// <summary>Profilin genel görünürlüğü.</summary>
        public ProfileVisibility ProfileVisibility { get; set; } = ProfileVisibility.Public;

        /// <summary>Doğrulama (mavi tik) durumu.</summary>
        public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.None;

        /// <summary>Rozetin verildiği an (UTC).</summary>
        public DateTime? VerifiedAtUtc { get; set; }

        /// <summary>Profilin arama motorlarınca indekslenmesine izin veriliyor mu?</summary>
        public bool AllowSearchEngineIndexing { get; set; }

        // ── Sosyal bağlantılar (mevcut owned tip yeniden kullanılır) ──────────
        /// <summary>
        /// Sosyal medya hesapları ve iletişim bağlantıları (mevcut ortak owned tip).
        /// Birincil e-posta/telefon burada DEĞİL, <see cref="Users"/> üzerindedir.
        /// </summary>
        public ContactInformation ContactInformation { get; set; } = new();

        // ── SAYAÇ ÖNBELLEĞİ (§24 — gerçeğin kaynağı değildir) ─────────────────
        //  Kaynak tablolar:
        //    FollowersCount → data._Follows.UserFollows (FollowingUserId = bu kullanıcı)
        //    FollowingCount → data._Follows.UserFollows (FollowerUserId  = bu kullanıcı)
        //    FriendsCount   → data._Follows.Friendships (Status = Accepted)
        //    PostCount      → data._Shares.Posts (AuthorUserId = bu kullanıcı, Published)
        /// <summary>Takipçi sayısı (önbellek).</summary>
        public int FollowersCount { get; set; }

        /// <summary>Takip edilen kullanıcı sayısı (önbellek).</summary>
        public int FollowingCount { get; set; }

        /// <summary>Takip edilen mağaza sayısı (önbellek).</summary>
        public int FollowedStoreCount { get; set; }

        /// <summary>Kabul edilmiş arkadaş sayısı (önbellek).</summary>
        public int FriendsCount { get; set; }

        /// <summary>Yayımlanmış gönderi sayısı (önbellek).</summary>
        public int PostCount { get; set; }

        /// <summary>Profil görüntülenme sayısı (önbellek — kaynak: ContentViewDailyAggregates).</summary>
        public int ProfileViewCount { get; set; }
    }
}
