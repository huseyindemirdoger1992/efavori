using System;
using data._Galleries;

namespace data._Chat
{
    /// <summary>
    /// SOHBET MESAJI (§33).
    ///
    /// Mesaj artık gönderen/alıcı ikilisine değil, bir KONUŞMAYA aittir. Alıcılar
    /// <see cref="ChatParticipants"/> üzerinden çözülür; böylece birebir, grup,
    /// kullanıcı–mağaza, ürün, sipariş ve ihtilaf sohbetleri tek modelle desteklenir.
    ///
    /// MEDYA (§72): Mesajın fiziksel dosya alanı YOKTUR; ekler
    /// <see cref="ChatMessageMedia"/> üzerinden merkezî medya deposuna bağlanır.
    ///
    /// OKUNMA (§33): Okundu bilgisi burada DEĞİL, kişi bazlı <see cref="ChatMessageReads"/>
    /// tablosundadır — grup sohbetinde "okundu" tek boolean ile ifade edilemez.
    ///
    /// SİLME: Mesaj fiziksel olarak silinmez. Gönderen kendi tarafından silerse
    /// katılımcının <c>ClearedBeforeUtc</c> alanı; herkesten silerse
    /// <see cref="Status"/> = Deleted kullanılır ve içerik boşaltılır.
    /// </summary>
    public class ChatMessages : ChatEntityBase
    {
        /// <summary>Bağlı sohbet (ChatConversations.Id).</summary>
        public Guid ConversationId { get; set; }

        /// <summary>Gönderen kullanıcı (Users.Id). Sistem mesajlarında null.</summary>
        public Guid? SenderUserId { get; set; }

        /// <summary>Gönderen mağaza adına konuşuyorsa mağaza (Store.Id).</summary>
        public Guid? SenderStoreId { get; set; }

        /// <summary>Mesajın içerik türü.</summary>
        public ChatMessageType MessageType { get; set; } = ChatMessageType.Text;

        /// <summary>Mesaj metni. Medya mesajlarında açıklama olarak kullanılabilir.</summary>
        public string? Content { get; set; }

        /// <summary>Gönderim/teslim durumu.</summary>
        public ChatMessageStatus Status { get; set; } = ChatMessageStatus.Sent;

        /// <summary>Yanıtlanan mesaj (ChatMessages.Id) — alıntılı yanıt.</summary>
        public Guid? ReplyToMessageId { get; set; }

        /// <summary>İletilen (forward) orijinal mesaj (ChatMessages.Id).</summary>
        public Guid? ForwardedFromMessageId { get; set; }

        // ── Zengin içerik referansları ────────────────────────────────────────
        /// <summary>Paylaşılan ürün (Products.Id). MessageType = ProductCard iken dolu.</summary>
        public Guid? SharedProductId { get; set; }

        /// <summary>Paylaşılan sipariş (Orders.Id). MessageType = OrderCard iken dolu.</summary>
        public Guid? SharedOrderId { get; set; }

        /// <summary>Paylaşılan gönderi (Posts.Id). MessageType = PostCard iken dolu.</summary>
        public Guid? SharedPostId { get; set; }

        /// <summary>Konum enlemi. MessageType = Location iken dolu.</summary>
        public decimal? Latitude { get; set; }

        /// <summary>Konum boylamı.</summary>
        public decimal? Longitude { get; set; }

        // ── Zaman damgaları ───────────────────────────────────────────────────
        /// <summary>Mesajın düzenlendiği an (UTC). "Düzenlendi" rozetini tetikler.</summary>
        public DateTime? EditedAtUtc { get; set; }

        /// <summary>Mesajın herkesten silindiği an (UTC).</summary>
        public DateTime? DeletedForEveryoneAtUtc { get; set; }

        /// <summary>Kendi kendini imha süresi (UTC) — kaybolan mesaj özelliği için.</summary>
        public DateTime? ExpiresAtUtc { get; set; }

        // ── Sayaç ve moderasyon ───────────────────────────────────────────────
        /// <summary>Okuyan katılımcı sayısı (önbellek — kaynak: ChatMessageReads).</summary>
        public int ReadCount { get; set; }

        /// <summary>Ek (medya) sayısı (önbellek — kaynak: ChatMessageMedia).</summary>
        public int MediaCount { get; set; }

        /// <summary>Moderasyon tarafından işaretlendi mi? (şikâyet sonrası)</summary>
        public bool IsFlagged { get; set; }

        /// <summary>
        /// İstemci tarafında üretilen benzersiz anahtar. Ağ kopmasında yapılan
        /// yeniden gönderimlerin aynı mesajı iki kez oluşturmasını DB seviyesinde
        /// engeller (idempotency).
        /// </summary>
        public string? ClientMessageId { get; set; }
    }

    /// <summary>
    /// MESAJ OKUNMA KAYDI (§33) — kişi bazlı okundu bilgisi.
    ///
    /// Grup sohbetinde "kimler okudu" listesi ve birebir sohbette mavi tik bu
    /// tablodan üretilir. Kullanıcının <c>UserPrivacySettings.SendReadReceipts</c>
    /// ayarı kapalıysa satır YAZILMAZ.
    ///
    /// TEKİLLİK: (MessageId, UserId).
    /// </summary>
    public class ChatMessageReads
    {
        /// <summary>Birincil anahtar.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Okunan mesaj (ChatMessages.Id).</summary>
        public Guid MessageId { get; set; }

        /// <summary>
        /// Sohbet izi (ChatConversations.Id). DENORMALIZE: "bu sohbette okunmamış
        /// mesajlarım" sorgusunu tek tabloda çözmek için; kaynak ChatMessages.ConversationId.
        /// </summary>
        public Guid ConversationId { get; set; }

        /// <summary>Okuyan kullanıcı (Users.Id).</summary>
        public Guid UserId { get; set; }

        /// <summary>Okunma anı (UTC).</summary>
        public DateTime ReadAtUtc { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// MESAJ EKİ (§33, §72).
    ///
    /// Mesaja bağlanan medyanın ilişki kaydıdır; asset merkezî
    /// <c>data._Galleries.Media</c> deposundadır. Bir mesajda birden çok ek olabilir.
    ///
    /// GÜVENLİK (§61): Sohbet ekleri ASLA <c>MediaVisibility.Public</c> olmamalıdır;
    /// yalnızca sohbetin katılımcıları erişebilmelidir (Restricted).
    ///
    /// TEKİLLİK: (MessageId, MediaId).
    /// </summary>
    public class ChatMessageMedia : ChatEntityBase
    {
        /// <summary>Bağlı mesaj (ChatMessages.Id).</summary>
        public Guid MessageId { get; set; }

        /// <summary>Merkezî medya asset'i (Media.Id).</summary>
        public Guid MediaId { get; set; }

        /// <summary>Medyanın mesaj içindeki rolü (Attachment / Image / Video / Document).</summary>
        public MediaRole MediaRole { get; set; } = MediaRole.Attachment;

        /// <summary>Birden çok ekte gösterim sırası (0 tabanlı).</summary>
        public int DisplayOrder { get; set; }

        /// <summary>Erişilebilirlik alt metni.</summary>
        public string? AltText { get; set; }
    }
}
