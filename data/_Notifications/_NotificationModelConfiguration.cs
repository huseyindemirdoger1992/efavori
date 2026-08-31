using Microsoft.EntityFrameworkCore;

namespace data._Notifications
{
    /// <summary>
    /// Bildirim Sistemi V1 — tüm FK, indeks ve silme davranışı tanımları.
    /// Mevcut _ProductModelConfiguration / _OrderModelConfiguration deseni ile birebir aynıdır.
    /// Kullanım: OnModelCreating içinde tek satır → _NotificationModelConfiguration.Apply(modelBuilder);
    /// </summary>
    public static class _NotificationModelConfiguration
    {
        /// <summary>Soft-delete filtreli tekil indeks sabiti (mevcut _ProductModelConfiguration'daki sabitle aynı tutulmalıdır).</summary>
        private const string SoftDeleteFilter = "[IsDeleted_IsDeletedStatu] = 0";

        public static void Apply(ModelBuilder modelBuilder)
        {
            // =====================================================================
            // NotificationTemplates — tip başına şablon
            // =====================================================================
            modelBuilder.Entity<NotificationTemplates>(e =>
            {
                e.ToTable("NotificationTemplates");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.Code).HasMaxLength(100).IsRequired();

                e.HasIndex(x => x.NotificationType).IsUnique().HasFilter(SoftDeleteFilter);       // tip başına tek şablon
                e.HasIndex(x => x.Code).IsUnique().HasFilter(SoftDeleteFilter);
            });

            // =====================================================================
            // NotificationTemplateTranslations — 10 dilli çeviriler (proje çeviri deseni)
            // =====================================================================
            modelBuilder.Entity<NotificationTemplateTranslations>(e =>
            {
                e.ToTable("NotificationTemplateTranslations");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.Title).HasMaxLength(300).IsRequired();
                e.Property(x => x.Body).HasMaxLength(2000).IsRequired();
                e.Property(x => x.EmailSubject).HasMaxLength(300);
                e.Property(x => x.PushBody).HasMaxLength(300);
                e.Property(x => x.SmsBody).HasMaxLength(500);
                // EmailBodyHtml: nvarchar(max) — sınır konulmaz.

                e.HasOne(typeof(NotificationTemplates)).WithMany().HasForeignKey(nameof(NotificationTemplateTranslations.ParentId)).OnDelete(DeleteBehavior.Restrict);

                // PROJE ÇEVİRİ DESENİ: (ParentId, Language) bileşik TEKİL indeks.
                e.HasIndex(x => new { x.ParentId, x.Language }).IsUnique().HasFilter(SoftDeleteFilter);
                e.HasIndex(x => new { x.Language, x.IsManuallyEdited });                          // AI çeviri görevi taraması
            });

            // =====================================================================
            // Notifications — in-app bildirim kaydı
            // =====================================================================
            modelBuilder.Entity<Notifications>(e =>
            {
                e.ToTable("Notifications");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.Title).HasMaxLength(300).IsRequired();
                e.Property(x => x.Body).HasMaxLength(2000).IsRequired();
                e.Property(x => x.TargetUrl).HasMaxLength(1000);
                e.Property(x => x.IdempotencyKey).HasMaxLength(150).IsRequired();

                e.HasOne(typeof(data._Users.Users)).WithMany().HasForeignKey(nameof(Notifications.UserId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(NotificationTemplates)).WithMany().HasForeignKey(nameof(Notifications.NotificationTemplateId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Orders.Orders)).WithMany().HasForeignKey(nameof(Notifications.OrderId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Orders.SubOrders)).WithMany().HasForeignKey(nameof(Notifications.SubOrderId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Products.Products)).WithMany().HasForeignKey(nameof(Notifications.ProductId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Store.Store)).WithMany().HasForeignKey(nameof(Notifications.StoreId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Shipping.Shipments)).WithMany().HasForeignKey(nameof(Notifications.ShipmentId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Products.PriceAlerts)).WithMany().HasForeignKey(nameof(Notifications.PriceAlertId)).OnDelete(DeleteBehavior.Restrict);
                // ImageMediaItemId / RelatedEntityId: iz alanlarıdır (FK yok).

                // ── SOSYAL AĞ ALANLARI (§34) ──────────────────────────────────
                e.Property(x => x.GroupKey).HasMaxLength(200);

                // AKTÖR: "Ahmet gönderini beğendi" cümlesindeki kişi/mağaza.
                e.HasOne(typeof(data._Users.Users)).WithMany()
                    .HasForeignKey(nameof(Notifications.ActorUserId)).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(typeof(data._Store.Store)).WithMany()
                    .HasForeignKey(nameof(Notifications.ActorStoreId)).OnDelete(DeleteBehavior.NoAction);

                // Sosyal kaynak referansları — hepsi NoAction: bildirim geçmişi,
                // kaynak içerik silinse bile öksüz kalmamalı ve sessizce yok olmamalıdır.
                e.HasOne(typeof(data._Shares.Posts)).WithMany()
                    .HasForeignKey(nameof(Notifications.PostId)).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(typeof(data._Shares.PostComments)).WithMany()
                    .HasForeignKey(nameof(Notifications.PostCommentId)).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(typeof(data._Chat.ChatConversations)).WithMany()
                    .HasForeignKey(nameof(Notifications.ConversationId)).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(typeof(data._Chat.ChatMessages)).WithMany()
                    .HasForeignKey(nameof(Notifications.ChatMessageId)).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(typeof(data._Follows.Friendships)).WithMany()
                    .HasForeignKey(nameof(Notifications.FriendshipId)).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(typeof(data._Products.ProductReviews)).WithMany()
                    .HasForeignKey(nameof(Notifications.ProductReviewId)).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(typeof(data._Products.ProductQuestions)).WithMany()
                    .HasForeignKey(nameof(Notifications.ProductQuestionId)).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(typeof(data._Shares.ContentReports)).WithMany()
                    .HasForeignKey(nameof(Notifications.ContentReportId)).OnDelete(DeleteBehavior.NoAction);

                e.HasIndex(x => x.IdempotencyKey).IsUnique().HasFilter(SoftDeleteFilter);         // çift bildirim engeli
                // Zil ikonundaki okunmamış sayacı ve liste — modülün en sık çalışan sorgusu.
                e.HasIndex(x => new { x.UserId, x.IsRead, x.IsArchived, x.CreatedAtUtc });
                e.HasIndex(x => new { x.UserId, x.CreatedAtUtc });                                // bildirim listesi (keyset pagination)
                e.HasIndex(x => new { x.NotificationType, x.CreatedAtUtc });                      // tip bazlı raporlama
                e.HasIndex(x => x.OrderId);
                e.HasIndex(x => x.StoreId);

                // Sosyal bildirim gruplaması ("Ahmet ve 12 kişi beğendi").
                e.HasIndex(x => new { x.UserId, x.GroupKey, x.IsRead });
                // Sosyal kaynak üzerinden geri izleme.
                e.HasIndex(x => x.PostId);
                e.HasIndex(x => x.ActorUserId);
            });

            // =====================================================================
            // NotificationDeliveries — kanal bazlı gönderim kuyruğu (lease)
            // =====================================================================
            modelBuilder.Entity<NotificationDeliveries>(e =>
            {
                e.ToTable("NotificationDeliveries");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.Destination).HasMaxLength(500);
                e.Property(x => x.ProviderMessageId).HasMaxLength(200);
                e.Property(x => x.FailureMessage).HasMaxLength(1000);
                e.Property(x => x.LeasedBy).HasMaxLength(100);
                e.Property(x => x.IdempotencyKey).HasMaxLength(200).IsRequired();

                e.HasOne(typeof(Notifications)).WithMany().HasForeignKey(nameof(NotificationDeliveries.NotificationId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Users.Users)).WithMany().HasForeignKey(nameof(NotificationDeliveries.UserId)).OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => x.IdempotencyKey).IsUnique().HasFilter(SoftDeleteFilter);         // aynı kanaldan çift gönderim engeli
                // KUYRUK TARAMASININ ANA İNDEKSİ: işlenecek satırları tek seek'le bulur.
                e.HasIndex(x => new { x.Status, x.Channel, x.ScheduledForUtc, x.NextRetryAtUtc, x.LeasedUntilUtc });
                e.HasIndex(x => x.NotificationId);                                                // bildirimin kanal durumları
                e.HasIndex(x => new { x.ProviderMessageId });                                     // bounce/webhook eşlemesi
                e.HasIndex(x => new { x.UserId, x.Channel, x.Status });                           // "bu kullanıcıya e-posta gidiyor mu" denetimi
            });

            // =====================================================================
            // UserNotificationPreferences — kullanıcı tercihleri
            // =====================================================================
            modelBuilder.Entity<UserNotificationPreferences>(e =>
            {
                e.ToTable("UserNotificationPreferences");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.TimeZoneId).HasMaxLength(60);

                e.HasOne(typeof(data._Users.Users)).WithMany().HasForeignKey(nameof(UserNotificationPreferences.UserId)).OnDelete(DeleteBehavior.Restrict);

                // Kullanıcı + tip başına tek tercih satırı.
                e.HasIndex(x => new { x.UserId, x.NotificationType }).IsUnique().HasFilter(SoftDeleteFilter);
            });
        }
    }
}
