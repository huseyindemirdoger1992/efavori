using System;

namespace data._Chat
{
    // ════════════════════════════════════════════════════════════════════════
    //  efavori — Mesajlaşma Modülü (Chat V1)
    //  Enum kataloğu — tinyint (byte) olarak saklanır.
    //  KURAL: Değerlerin ARASINA ekleme yapılmaz; yalnızca SONA eklenir.
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Sohbetin türü. Eski <c>ChatMessage</c> tablosu yalnızca (SenderId, ReceiverId)
    /// ikilisini destekliyordu; grup sohbeti, mağaza sohbeti ve sipariş/ürün bağlamlı
    /// sohbet modellenemiyordu. Konuşma (conversation) kavramı bu yüzden ayrıldı.
    /// </summary>
    public enum ConversationType : byte
    {
        /// <summary>İki kullanıcı arasında birebir sohbet.</summary>
        Direct = 1,

        /// <summary>Çok kişili grup sohbeti.</summary>
        Group = 2,

        /// <summary>Kullanıcı ile mağaza arasında sohbet (mağaza tarafında birden çok temsilci olabilir).</summary>
        UserToStore = 3,

        /// <summary>Belirli bir ürün hakkında başlatılmış sohbet.</summary>
        ProductInquiry = 4,

        /// <summary>Belirli bir sipariş hakkında sohbet (alıcı–satıcı).</summary>
        OrderSupport = 5,

        /// <summary>İhtilaf (dispute) görüşme kanalı — platform hakemi de katılabilir.</summary>
        DisputeThread = 6,

        /// <summary>Destek talebi kanalı (kullanıcı ↔ platform desteği).</summary>
        SupportTicket = 7,

        /// <summary>Sistem duyuru kanalı (tek yönlü).</summary>
        SystemChannel = 8
    }

    /// <summary>Sohbet katılımcısının rolü.</summary>
    public enum ChatParticipantRole : byte
    {
        /// <summary>Sıradan katılımcı.</summary>
        Member = 1,

        /// <summary>Grup yöneticisi (katılımcı ekleyip çıkarabilir).</summary>
        Admin = 2,

        /// <summary>Grubu oluşturan sahip.</summary>
        Owner = 3,

        /// <summary>Mağaza adına yanıt veren temsilci.</summary>
        StoreAgent = 4,

        /// <summary>Platform destek personeli.</summary>
        SupportAgent = 5,

        /// <summary>İhtilaf hakemi (yalnızca DisputeThread'de).</summary>
        Moderator = 6,

        /// <summary>Yalnızca okuyabilen gözlemci.</summary>
        Observer = 7
    }

    /// <summary>Mesajın içerik türü.</summary>
    public enum ChatMessageType : byte
    {
        /// <summary>Düz metin.</summary>
        Text = 1,

        /// <summary>Görsel.</summary>
        Image = 2,

        /// <summary>Video.</summary>
        Video = 3,

        /// <summary>Sesli mesaj.</summary>
        Audio = 4,

        /// <summary>Belge/dosya.</summary>
        Document = 5,

        /// <summary>Konum paylaşımı.</summary>
        Location = 6,

        /// <summary>Ürün kartı (ürün bağlantısı zengin önizlemeyle).</summary>
        ProductCard = 7,

        /// <summary>Sipariş kartı.</summary>
        OrderCard = 8,

        /// <summary>Gönderi kartı (paylaşılan sosyal içerik).</summary>
        PostCard = 9,

        /// <summary>Çıkartma/emoji.</summary>
        Sticker = 10,

        /// <summary>
        /// Sistem mesajı ("X gruba katıldı", "sipariş kargoya verildi").
        /// Göndereni yoktur; SenderUserId null olur.
        /// </summary>
        System = 11
    }

    /// <summary>
    /// Mesajın gönderim/teslim durumu. Okunma bilgisi kişi bazlı olduğu için burada
    /// DEĞİL, <c>ChatMessageReads</c> tablosundadır — grup sohbetinde "okundu" tek bir
    /// boolean ile ifade edilemez.
    /// </summary>
    public enum ChatMessageStatus : byte
    {
        /// <summary>İstemcide oluşturuldu, sunucuya gönderiliyor.</summary>
        Sending = 1,

        /// <summary>Sunucuya ulaştı.</summary>
        Sent = 2,

        /// <summary>En az bir alıcının cihazına teslim edildi.</summary>
        Delivered = 3,

        /// <summary>Tüm alıcılar tarafından okundu.</summary>
        Read = 4,

        /// <summary>Gönderilemedi.</summary>
        Failed = 5,

        /// <summary>Gönderen tarafından silindi ("Bu mesaj silindi" olarak görünür).</summary>
        Deleted = 6,

        /// <summary>Moderasyon tarafından kaldırıldı.</summary>
        RemovedByModerator = 7
    }
}
