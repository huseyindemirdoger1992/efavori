using System;

namespace data._Follows
{
    /// <summary>
    /// MAĞAZA TAKİBİ (§10) — kullanıcı ↔ mağaza aboneliği.
    ///
    /// Eski <c>StoreShip</c> entity'sinin yerini alır. <c>StoreShip</c>, iki adet
    /// belirsiz <c>Guid</c> alanı (RequestorId / ReceiverId) ve iki boolean ile
    /// modellenmişti; hangi tarafın kullanıcı hangi tarafın mağaza olduğu tip
    /// düzeyinde belli değildi ve FK kurulamıyordu. Yeni model açık ve FK'lidir.
    ///
    /// DESTEKLENEN SENARYOLAR:
    ///  • Mağazayı takip et / takibi bırak
    ///  • Mağaza takipçi sayısı (Store.FollowersCount önbelleği)
    ///  • Yeni ürün / yeni gönderi / kampanya bildirimleri (kanal bazlı)
    ///  • Mağaza gönderilerinin kullanıcı akışına düşmesi
    ///
    /// TEKİLLİK: (UserId, StoreId) çifti aktif kayıtlarda tekildir.
    /// </summary>
    public class StoreFollowers : FollowEntityBase
    {
        /// <summary>Takip eden kullanıcı (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>Takip edilen mağaza (Store.Id).</summary>
        public Guid StoreId { get; set; }

        /// <summary>
        /// Takip hâlen etkin mi? Takip bırakıldığında satır SİLİNMEZ, false yapılır —
        /// böylece "daha önce takip etmişti" bilgisi öneri algoritması için korunur.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Takibin bırakıldığı an (UTC).</summary>
        public DateTime? UnfollowedAtUtc { get; set; }

        /// <summary>Bu mağazadan herhangi bir bildirim alınsın mı? (ana anahtar)</summary>
        public bool NotificationEnabled { get; set; } = true;

        /// <summary>Mağaza yeni ürün eklediğinde bildirim gelsin mi?</summary>
        public bool NotifyOnNewProduct { get; set; } = true;

        /// <summary>Mağaza yeni gönderi paylaştığında bildirim gelsin mi?</summary>
        public bool NotifyOnNewPost { get; set; } = true;

        /// <summary>Mağaza kampanya/kupon yayımladığında bildirim gelsin mi?</summary>
        public bool NotifyOnCampaign { get; set; } = true;

        /// <summary>Takip edilen mağazanın ürünlerinde indirim olduğunda bildirim gelsin mi?</summary>
        public bool NotifyOnPriceDrop { get; set; }

        /// <summary>Mağaza gönderileri ana akışta gizlensin mi? (takibi bırakmadan sustur)</summary>
        public bool IsMutedInFeed { get; set; }

        /// <summary>Takip nereden başlatıldı? ("StorePage", "ProductDetail", "Search", "Order").</summary>
        public string? SourceContext { get; set; }
    }
}
