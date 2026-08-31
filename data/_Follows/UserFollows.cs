using System;

namespace data._Follows
{
    /// <summary>
    /// KULLANICI TAKİBİ (§9) — Instagram/X tarzı TEK YÖNLÜ ilişki.
    ///
    /// Arkadaşlıktan farkı: arkadaşlık simetrik ve karşılıklı onaya bağlıdır;
    /// takip tek yönlüdür ve (hesap açıksa) onay gerektirmez. İki mekanizma
    /// birbirinin yerine geçmez, ayrı tablolarda tutulur.
    ///
    /// GİZLİ HESAP AKIŞI: Hedef kullanıcının profili Private/FollowersOnly ise kayıt
    /// <see cref="FollowStatus.PendingApproval"/> ile açılır ve onaylanana kadar
    /// takipçi sayılmaz.
    ///
    /// TEKİLLİK: (FollowerUserId, FollowingUserId) çifti aktif kayıtlarda tekildir.
    /// KENDİNİ TAKİP: veritabanı CHECK kısıtı ile engellenir.
    /// </summary>
    public class UserFollows : FollowEntityBase
    {
        /// <summary>Takip eden kullanıcı (Users.Id).</summary>
        public Guid FollowerUserId { get; set; }

        /// <summary>Takip edilen kullanıcı (Users.Id).</summary>
        public Guid FollowingUserId { get; set; }

        /// <summary>Takip durumu (gizli hesaplarda onay akışı için).</summary>
        public FollowStatus Status { get; set; } = FollowStatus.Active;

        /// <summary>
        /// Takip hâlen etkin mi? <see cref="Status"/> ile tutarlı tutulur
        /// (Active iken true). Sayaç ve indeks sorgularını sadeleştirmek için ayrı kolon.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Takibin onaylandığı an (UTC) — gizli hesaplarda.</summary>
        public DateTime? ApprovedAtUtc { get; set; }

        /// <summary>Takipten çıkılan an (UTC).</summary>
        public DateTime? UnfollowedAtUtc { get; set; }

        /// <summary>Bu kullanıcının gönderileri için bildirim alınsın mı? ("zil" ikonu)</summary>
        public bool NotificationEnabled { get; set; } = true;

        /// <summary>
        /// Takip ediliyor ama gönderileri ana akışta gizleniyor mu?
        /// (Facebook "Takibi bırakmadan gizle" davranışı.)
        /// </summary>
        public bool IsMutedInFeed { get; set; }

        /// <summary>
        /// Takip nereden başlatıldı? ("Profile", "Suggestion", "Search", "PostAuthor",
        /// "QrCode"). Öneri algoritmasının kalitesini ölçmek için tutulur.
        /// </summary>
        public string? SourceContext { get; set; }
    }
}
