using System;

namespace data._Products
{
    // ════════════════════════════════════════════════════════════════════════
    //  efavori — Ürün Sosyal Etkileşim Modülü (Product Social V1)
    //  Enum kataloğu — tinyint (byte) olarak saklanır.
    //  KURAL: Değerlerin ARASINA ekleme yapılmaz; yalnızca SONA eklenir.
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Ürünün hangi kanaldan paylaşıldığı (§25) — pazarlama dönüşümünü ölçmek için.
    /// </summary>
    public enum ProductShareChannel : byte
    {
        /// <summary>Platform içi paylaşım (sohbet, gönderi).</summary>
        Internal = 1,

        /// <summary>Bağlantı kopyalandı.</summary>
        CopyLink = 2,

        /// <summary>WhatsApp.</summary>
        WhatsApp = 3,

        /// <summary>Facebook.</summary>
        Facebook = 4,

        /// <summary>Instagram.</summary>
        Instagram = 5,

        /// <summary>X (Twitter).</summary>
        X = 6,

        /// <summary>Telegram.</summary>
        Telegram = 7,

        /// <summary>E-posta.</summary>
        Email = 8,

        /// <summary>SMS.</summary>
        Sms = 9,

        /// <summary>Pinterest.</summary>
        Pinterest = 10,

        /// <summary>QR kod.</summary>
        QrCode = 11,

        /// <summary>Diğer.</summary>
        Other = 100
    }

    /// <summary>
    /// İstek listesinin (wishlist) görünürlüğü (§29).
    /// </summary>
    public enum WishlistVisibility : byte
    {
        /// <summary>Yalnızca sahibi görebilir.</summary>
        Private = 1,

        /// <summary>Herkese açık — profilde listelenir.</summary>
        Public = 2,

        /// <summary>Yalnızca arkadaşlar görebilir.</summary>
        FriendsOnly = 3,

        /// <summary>Bağlantısı olan herkes görebilir (listelenmez) — hediye listesi paylaşımı.</summary>
        LinkOnly = 4
    }

    /// <summary>
    /// İstek listesinin amacı — hediye listesi senaryolarını ayırt etmek için.
    /// </summary>
    public enum WishlistPurpose : byte
    {
        /// <summary>Genel istek listesi.</summary>
        General = 1,

        /// <summary>Doğum günü.</summary>
        Birthday = 2,

        /// <summary>Düğün / çeyiz listesi.</summary>
        Wedding = 3,

        /// <summary>Bebek listesi.</summary>
        BabyShower = 4,

        /// <summary>Yılbaşı / bayram.</summary>
        Holiday = 5,

        /// <summary>Daha sonra almak üzere kaydedilenler.</summary>
        SaveForLater = 6
    }
}
