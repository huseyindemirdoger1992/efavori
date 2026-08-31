using System;

namespace data._Chat
{
    /// <summary>
    /// SOHBET KATILIMCISI (§33).
    ///
    /// Bir kullanıcının bir sohbetteki üyeliğini ve KİŞİYE ÖZEL durumunu tutar:
    /// susturma, arşivleme, sabitleme, okunmamış sayısı ve son okunan mesaj.
    /// Bu bilgiler sohbetin kendisinde tutulamaz — her katılımcı için farklıdır.
    ///
    /// Bir kullanıcı sohbetten ayrılıp tekrar eklenebilir; bu yüzden satır silinmez,
    /// <see cref="LeftAtUtc"/> doldurulur ve yeniden katılımda temizlenir.
    ///
    /// TEKİLLİK: (ConversationId, UserId) aktif kayıtlarda tekildir.
    /// </summary>
    public class ChatParticipants : ChatEntityBase
    {
        /// <summary>Bağlı sohbet (ChatConversations.Id).</summary>
        public Guid ConversationId { get; set; }

        /// <summary>Katılımcı kullanıcı (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Katılımcı bir mağaza adına konuşuyorsa mağaza (Store.Id).
        /// Mesajlarda "Mağaza" rozeti bu alandan türetilir.
        /// </summary>
        public Guid? OnBehalfOfStoreId { get; set; }

        /// <summary>Sohbetteki rolü.</summary>
        public ChatParticipantRole Role { get; set; } = ChatParticipantRole.Member;

        /// <summary>Sohbete katıldığı an (UTC).</summary>
        public DateTime JoinedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>Sohbetten ayrıldığı an (UTC). Null = hâlâ üye.</summary>
        public DateTime? LeftAtUtc { get; set; }

        /// <summary>Katılımcıyı ekleyen kullanıcı (Users.Id) — grup sohbetlerinde.</summary>
        public Guid? AddedByUserId { get; set; }

        // ── Kişiye özel durum ─────────────────────────────────────────────────
        /// <summary>Bildirimler susturuldu mu?</summary>
        public bool IsMuted { get; set; }

        /// <summary>Susturmanın kendiliğinden biteceği an (UTC). Null = süresiz.</summary>
        public DateTime? MutedUntilUtc { get; set; }

        /// <summary>Sohbet bu kullanıcı için arşivlendi mi?</summary>
        public bool IsArchived { get; set; }

        /// <summary>Sohbet listesinin en üstüne sabitlendi mi?</summary>
        public bool IsPinned { get; set; }

        /// <summary>Bu kullanıcının okuduğu son mesaj (ChatMessages.Id).</summary>
        public Guid? LastReadMessageId { get; set; }

        /// <summary>Son okuma anı (UTC).</summary>
        public DateTime? LastReadAtUtc { get; set; }

        /// <summary>
        /// Okunmamış mesaj sayısı (ÖNBELLEK). Gerçeğin kaynağı
        /// <c>ChatMessages</c> ile <c>LastReadMessageId</c> karşılaştırmasıdır;
        /// bu alan sohbet listesinde COUNT sorgusundan kaçınmak içindir.
        /// </summary>
        public int UnreadCount { get; set; }

        /// <summary>
        /// Bu kullanıcı sohbeti kendi tarafında sildiyse, bu andan ÖNCEKİ mesajlar
        /// ona gösterilmez. Diğer katılımcılar etkilenmez (WhatsApp "sohbeti temizle").
        /// </summary>
        public DateTime? ClearedBeforeUtc { get; set; }
    }
}
