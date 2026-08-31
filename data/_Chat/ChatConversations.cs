using System;

namespace data._Chat
{
    /// <summary>
    /// SOHBET (KONUŞMA) BAŞLIĞI (§33).
    ///
    /// Eski <c>ChatMessage</c> tablosu (SenderId + ReceiverId + Content) profesyonel
    /// bir mesajlaşma sistemi için yetersizdi:
    ///  • Grup sohbeti modellenemiyordu (iki kolon sabit).
    ///  • Kullanıcı–mağaza sohbetinde mağaza tarafında birden çok temsilci olamıyordu.
    ///  • "Sohbet listesi" ekranı için her seferinde tüm mesajları gruplamak gerekiyordu.
    ///  • Okundu bilgisi tek boolean'dı; grupta kimin okuduğu bilinemiyordu.
    ///  • Medya eki desteklenmiyordu.
    ///
    /// Yeni model konuşma / katılımcı / mesaj / okunma / ek olarak beşe ayrılmıştır.
    /// Sohbet listesi ekranı yalnızca bu tabloyu ve katılımcı satırını okur;
    /// mesaj tablosuna hiç dokunmaz.
    /// </summary>
    public class ChatConversations : ChatEntityBase
    {
        /// <summary>Sohbetin türü.</summary>
        public ConversationType ConversationType { get; set; } = ConversationType.Direct;

        /// <summary>Grup/kanal başlığı. Birebir sohbetlerde null (karşı tarafın adı gösterilir).</summary>
        public string? Title { get; set; }

        /// <summary>Grup görseli (data._Galleries.Media.Id).</summary>
        public Guid? AvatarMediaId { get; set; }

        // ── Bağlam referansları ───────────────────────────────────────────────
        /// <summary>İlgili mağaza (Store.Id). ConversationType = UserToStore / ProductInquiry iken dolu.</summary>
        public Guid? StoreId { get; set; }

        /// <summary>İlgili ürün (Products.Id). ConversationType = ProductInquiry iken dolu.</summary>
        public Guid? ProductId { get; set; }

        /// <summary>İlgili sipariş (Orders.Id). ConversationType = OrderSupport iken dolu.</summary>
        public Guid? OrderId { get; set; }

        /// <summary>İlgili alt sipariş (SubOrders.Id) — satıcı bazlı destek için.</summary>
        public Guid? SubOrderId { get; set; }

        /// <summary>İlgili ihtilaf (Disputes.Id). ConversationType = DisputeThread iken dolu.</summary>
        public Guid? DisputeId { get; set; }

        /// <summary>İlgili destek talebi (SupportTickets.Id).</summary>
        public Guid? SupportTicketId { get; set; }

        // ── Birebir sohbet tekilliği ──────────────────────────────────────────
        /// <summary>
        /// HESAPLANMIŞ KOLON (persisted). Birebir sohbette iki kullanıcı kimliğinden
        /// KÜÇÜK olanı. <see cref="ChatConversations"/> için doldurulan
        /// <see cref="DirectUserAId"/> / <see cref="DirectUserBId"/> alanlarından üretilir.
        ///
        /// Amacı: aynı iki kişi arasında ikinci bir birebir sohbet açılmasını
        /// veritabanı seviyesinde engellemektir (Friendships ile aynı desen).
        /// </summary>
        public Guid? DirectPairLowId { get; private set; }

        /// <summary>HESAPLANMIŞ KOLON (persisted). İki kimlikten BÜYÜK olanı.</summary>
        public Guid? DirectPairHighId { get; private set; }

        /// <summary>Birebir sohbetin taraflarından biri (Users.Id). Diğer türlerde null.</summary>
        public Guid? DirectUserAId { get; set; }

        /// <summary>Birebir sohbetin diğer tarafı (Users.Id). Diğer türlerde null.</summary>
        public Guid? DirectUserBId { get; set; }

        // ── Durum / özet ──────────────────────────────────────────────────────
        /// <summary>Sohbet aktif mi? (arşivlenmiş/kapatılmış sohbetler false)</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Son mesajın anı (UTC) — sohbet listesi bu kolona göre sıralanır.
        /// DENORMALIZE: mesaj tablosuna gitmeden sıralama yapılabilsin diye.
        /// </summary>
        public DateTime? LastMessageAtUtc { get; set; }

        /// <summary>Son mesajın kimliği (ChatMessages.Id) — önizleme için.</summary>
        public Guid? LastMessageId { get; set; }

        /// <summary>Son mesajın kısa önizlemesi (ilk ~140 karakter). Liste ekranı için snapshot.</summary>
        public string? LastMessagePreview { get; set; }

        /// <summary>Son mesajı gönderen kullanıcı (Users.Id).</summary>
        public Guid? LastMessageSenderUserId { get; set; }

        /// <summary>Sohbetteki toplam mesaj sayısı (önbellek).</summary>
        public int MessageCount { get; set; }

        /// <summary>Aktif katılımcı sayısı (önbellek).</summary>
        public int ParticipantCount { get; set; }
    }
}
