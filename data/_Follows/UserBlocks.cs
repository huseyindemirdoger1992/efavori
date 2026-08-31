using System;

namespace data._Follows
{
    /// <summary>
    /// KULLANICI ENGELLEME (§8) — arkadaşlıktan tamamen BAĞIMSIZ ilişkidir.
    ///
    /// Eski modelde engelleme <c>FriendShip.Block</c> boolean'ı ile tutuluyordu; bu
    /// yanlıştı çünkü hiç arkadaşlık ilişkisi olmayan iki kullanıcı da birbirini
    /// engelleyebilir ve engel, arkadaşlık kaydı silinse bile sürmelidir.
    ///
    /// ENGEL YÖNLÜDÜR: A, B'yi engellerse yalnızca A→B satırı oluşur. Ancak etkisi
    /// çift yönlüdür — aşağıdaki kurallar HER İKİ TARAF için de uygulanır.
    ///
    /// SERVİS KATMANI KURALLARI (engel varsa, her iki yönde de):
    ///  1) Arkadaşlık isteği gönderilemez; mevcut Pending istek otomatik Cancelled olur.
    ///  2) Takip edilemez; mevcut takip kayıtları Unfollowed'a çekilir.
    ///  3) Gönderiler, yorumlar ve profil karşılıklı olarak GÖRÜNMEZ.
    ///  4) Beğeni/tepki/yorum/mention/etiketleme yapılamaz.
    ///  5) Mesaj gönderilemez; mevcut sohbet arşivlenir.
    ///  6) Arama sonuçlarında ve öneri akışında birbirlerine gösterilmez.
    ///
    /// Bu kurallar tek bir yerde (BlockGuard servisi) uygulanmalı ve tüm sosyal
    /// sorguların önüne konmalıdır.
    /// </summary>
    public class UserBlocks : FollowEntityBase
    {
        /// <summary>Engelleyen kullanıcı (Users.Id).</summary>
        public Guid BlockingUserId { get; set; }

        /// <summary>Engellenen kullanıcı (Users.Id).</summary>
        public Guid BlockedUserId { get; set; }

        /// <summary>Engelleme gerekçesi (istatistik ve moderasyon için).</summary>
        public BlockReason Reason { get; set; } = BlockReason.Unspecified;

        /// <summary>Kullanıcının yazdığı serbest açıklama (opsiyonel).</summary>
        public string? Description { get; set; }

        /// <summary>
        /// Engel hâlen etkin mi? Engel kaldırıldığında satır SİLİNMEZ, false yapılır —
        /// böylece "kaç kez engellendi" verisi moderasyon için korunur.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Engelin kaldırıldığı an (UTC).</summary>
        public DateTime? UnblockedAtUtc { get; set; }

        /// <summary>
        /// Engel bir şikâyet üzerinden mi oluşturuldu?
        /// (data._Shares.ContentReports.Id — moderasyon zinciri izlenebilsin diye).
        /// </summary>
        public Guid? SourceContentReportId { get; set; }
    }
}
