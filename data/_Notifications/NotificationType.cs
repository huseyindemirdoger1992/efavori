namespace data._Notifications
{
    // ============================================================================================
    // BİLDİRİM SİSTEMİ V1 — ENUM SÖZLÜĞÜ
    // KURAL: Enum değerleri byte olarak saklanır. Mevcut değerlerin ARASINA yeni değer EKLENMEZ,
    // yeni değerler her zaman SONA eklenir. Değer silinmez, yeniden adlandırılmaz.
    // ============================================================================================

    /// <summary>
    /// Bildirim tipi. Her tip bir NotificationTemplates satırına karşılık gelir
    /// (şablon metinleri 10 dilde NotificationTemplateTranslations'ta tutulur).
    /// </summary>
    public enum NotificationType : byte
    {
        /// <summary>Sipariş durumu değişti (Faz 1 OrderStatusHistory kaynaklı).</summary>
        OrderStatusChanged = 0,

        /// <summary>Takip ettiği ürünün fiyatı düştü (Faz 3 PriceAlerts kaynaklı).</summary>
        PriceDropAlert = 1,

        /// <summary>Stok uyarısı — satıcıya: ReorderPoint altına düşüldü (Faz 4 kaynaklı).</summary>
        StockAlert = 2,

        /// <summary>İçerik moderasyon sonucu (yorum/ürün onay-ret).</summary>
        ModerationResult = 3,

        /// <summary>Hakediş ödemesi tamamlandı (Faz 2 SellerPayouts kaynaklı).</summary>
        PayoutCompleted = 4,

        /// <summary>Kargo yola çıktı (Faz 5 Shipments kaynaklı).</summary>
        ShipmentShipped = 5,

        /// <summary>Kargo teslim edildi.</summary>
        ShipmentDelivered = 6,

        /// <summary>Teslim denemesi başarısız oldu.</summary>
        ShipmentDeliveryFailed = 7,

        /// <summary>İade talebi durumu değişti (Faz 7 kaynaklı).</summary>
        ReturnStatusChanged = 8,

        /// <summary>İhtilaf açıldı veya karara bağlandı (Faz 7 Disputes kaynaklı).</summary>
        DisputeUpdated = 9,

        /// <summary>Ödeme başarısız oldu; yeniden deneme gerekiyor.</summary>
        PaymentFailed = 10,

        /// <summary>İade tutarı hesaba geçti (Faz 2 Refunds kaynaklı).</summary>
        RefundCompleted = 11,

        /// <summary>Favorideki ürün yeniden stoğa girdi.</summary>
        BackInStock = 12,

        /// <summary>Ürüne yeni soru/yorum geldi (satıcıya).</summary>
        NewProductQuestion = 13,

        /// <summary>Yeni sipariş alındı (satıcıya).</summary>
        NewOrderReceived = 14,

        /// <summary>Kampanya/kupon duyurusu (pazarlama).</summary>
        PromotionAnnouncement = 15,

        /// <summary>Hesap güvenliği bildirimi (şifre değişimi, yeni cihaz girişi).</summary>
        AccountSecurity = 16,

        /// <summary>Mağaza hesabıyla ilgili yönetimsel bildirim (askıya alma, belge talebi).</summary>
        StoreAccountNotice = 17,

        /// <summary>Sistem duyurusu (bakım, kesinti).</summary>
        SystemAnnouncement = 18,

        // ════════════════════════════════════════════════════════════════════
        //  SOSYAL AĞ OLAYLARI (§34) — mevcut değerlerin ARDINA eklenmiştir.
        //  Bu tiplerde ActorUserId / ActorStoreId alanları doldurulur
        //  ("Ahmet gönderini beğendi" — aktör bildirimin ayrılmaz parçasıdır).
        // ════════════════════════════════════════════════════════════════════

        /// <summary>Yeni arkadaşlık isteği alındı.</summary>
        FriendRequestReceived = 19,

        /// <summary>Gönderilen arkadaşlık isteği kabul edildi.</summary>
        FriendRequestAccepted = 20,

        /// <summary>Yeni takipçi kazanıldı.</summary>
        NewFollower = 21,

        /// <summary>Gizli hesapta takip isteği onay bekliyor.</summary>
        FollowRequestReceived = 22,

        /// <summary>Takip isteği onaylandı.</summary>
        FollowRequestApproved = 23,

        /// <summary>Mağaza yeni bir takipçi kazandı (satıcıya).</summary>
        StoreFollowed = 24,

        /// <summary>Gönderiye tepki verildi (beğeni dâhil).</summary>
        PostReactionReceived = 25,

        /// <summary>Gönderiye yorum yapıldı.</summary>
        PostCommentReceived = 26,

        /// <summary>Yoruma yanıt verildi.</summary>
        CommentReplyReceived = 27,

        /// <summary>Yoruma tepki verildi.</summary>
        CommentReactionReceived = 28,

        /// <summary>Gönderi veya yorumda anıldın (@mention).</summary>
        MentionReceived = 29,

        /// <summary>Gönderi yeniden paylaşıldı (repost).</summary>
        PostReposted = 30,

        /// <summary>Gönderi paylaşıldı (share).</summary>
        PostShared = 31,

        /// <summary>Takip edilen kullanıcı veya mağaza yeni gönderi paylaştı.</summary>
        FollowedActorPosted = 32,

        /// <summary>Gönderi veya yorumda ürün etiketlendi.</summary>
        ProductTaggedInPost = 33,

        /// <summary>Yeni sohbet mesajı alındı.</summary>
        MessageReceived = 34,

        /// <summary>Ürün favorilendi (satıcıya).</summary>
        ProductFavorited = 35,

        /// <summary>Ürün sorusuna cevap verildi (soruyu sorana).</summary>
        ProductQuestionAnswered = 36,

        /// <summary>Ürün yorumuna satıcı yanıtı verildi (yorumu yazana).</summary>
        ProductReviewResponded = 37,

        /// <summary>Ürüne yeni yorum yapıldı (satıcıya).</summary>
        NewProductReview = 38,

        /// <summary>İçerik şikâyeti sonuçlandı (şikâyet edene).</summary>
        ContentReportResolved = 39,

        /// <summary>İçerik moderasyon tarafından kaldırıldı (yazarına).</summary>
        ContentRemovedByModeration = 40,

        /// <summary>İstek listesindeki ürün için hediye alındı.</summary>
        WishlistItemPurchased = 41,

        /// <summary>Mağaza doğrulama belgesinin süresi dolmak üzere (satıcıya).</summary>
        StoreDocumentExpiring = 42
    }

    /// <summary>
    /// Bildirim kanalları — BAYRAK (flags) enum'ı.
    /// Bir bildirim aynı anda birden çok kanaldan gönderilebilir: InApp | Email.
    ///
    /// DİKKAT — bu enum bayraklı olduğundan yeni değerler İKİNİN KUVVETİ olarak SONA eklenir
    /// (32, 64, 128). Byte sınırı 128'dir; daha fazlası gerekirse alan tipi büyütülmeli
    /// ve bu yorum güncellenmelidir.
    /// </summary>
    [Flags]
    public enum NotificationChannel : byte
    {
        /// <summary>Kanal yok.</summary>
        None = 0,

        /// <summary>Uygulama içi bildirim (zil ikonu).</summary>
        InApp = 1,

        /// <summary>E-posta.</summary>
        Email = 2,

        /// <summary>Mobil push bildirimi.</summary>
        Push = 4,

        /// <summary>SMS.</summary>
        Sms = 8,

        /// <summary>Webhook (kurumsal satıcı entegrasyonları için).</summary>
        Webhook = 16
    }

    /// <summary>
    /// Kanal bazlı gönderim durumu (NotificationDeliveries.Status).
    /// Kuyruk tablosunun durum makinesidir.
    /// </summary>
    public enum NotificationDeliveryStatus : byte
    {
        /// <summary>Kuyrukta, gönderim bekliyor.</summary>
        Pending = 0,

        /// <summary>İşleyici lease aldı, gönderiliyor.</summary>
        Processing = 1,

        /// <summary>Sağlayıcıya başarıyla teslim edildi.</summary>
        Sent = 2,

        /// <summary>Alıcı tarafından teslim alındığı doğrulandı (e-posta açılma/push delivery receipt).</summary>
        Delivered = 3,

        /// <summary>Gönderim başarısız; NextRetryAtUtc'ye göre yeniden denenecek.</summary>
        Failed = 4,

        /// <summary>MaxAttempts aşıldı; kalıcı başarısız, manuel inceleme gerekir.</summary>
        PermanentlyFailed = 5,

        /// <summary>Kullanıcı tercihi veya abonelikten çıkma nedeniyle gönderilmedi.</summary>
        Suppressed = 6,

        /// <summary>Alıcı adresi geçersiz (hard bounce); bu kanal kullanıcı için devre dışı bırakılmalı.</summary>
        Bounced = 7
    }

    /// <summary>Bildirim önem düzeyi. Uygulama içi gösterim ve gruplama için.</summary>
    public enum NotificationPriority : byte
    {
        /// <summary>Düşük — pazarlama, duyuru.</summary>
        Low = 0,

        /// <summary>Normal — sipariş/kargo akışı.</summary>
        Normal = 1,

        /// <summary>Yüksek — ödeme hatası, ihtilaf, güvenlik.</summary>
        High = 2,

        /// <summary>Kritik — hesap askıya alma, dolandırıcılık uyarısı.</summary>
        Critical = 3
    }
}
