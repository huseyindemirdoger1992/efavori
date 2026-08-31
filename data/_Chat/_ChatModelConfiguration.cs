using System;
using Microsoft.EntityFrameworkCore;
using data._Galleries;
using data._Orders;
using data._Products;
using data._Returns;
using data._Shares;
using data._Store;
using data._Users;

namespace data._Chat
{
    /// <summary>
    /// Mesajlaşma modülünün EF Core Fluent yapılandırması.
    ///
    /// _ApplicationConnectionDb.OnModelCreating içinde şu satırla uygulanır:
    ///     _ChatModelConfiguration.Apply(modelBuilder);
    ///
    /// SİLME DAVRANIŞI (§43):
    ///  • Konuşma → katılımcı ve mesaj CASCADE'dir.
    ///  • Mesaj → okunma kaydı ve ek CASCADE'dir.
    ///  • Kullanıcı, mağaza, ürün, sipariş referansları NoAction'dır — sohbet
    ///    geçmişi ticari bir kayıttır ve sessizce yok edilmemelidir.
    ///  • Medya NoAction'dır (merkezî varlık).
    /// </summary>
    public static class _ChatModelConfiguration
    {
        private const string SoftDeleteFilter = "[IsDeleted_IsDeletedStatu] = 0";

        /// <summary>Tüm mesajlaşma yapılandırmasını uygular.</summary>
        public static void Apply(ModelBuilder modelBuilder)
        {
            ApplyBaseConventions(modelBuilder);

            ApplyConversations(modelBuilder);
            ApplyParticipants(modelBuilder);
            ApplyMessages(modelBuilder);
            ApplyMessageReads(modelBuilder);
            ApplyMessageMedia(modelBuilder);
        }

        private static void ApplyBaseConventions(ModelBuilder modelBuilder)
        {
            ConfigureBase<ChatConversations>(modelBuilder);
            ConfigureBase<ChatParticipants>(modelBuilder);
            ConfigureBase<ChatMessages>(modelBuilder);
            ConfigureBase<ChatMessageMedia>(modelBuilder);
        }

        private static void ConfigureBase<TEntity>(ModelBuilder modelBuilder)
            where TEntity : ChatEntityBase
        {
            modelBuilder.Entity<TEntity>(b =>
            {
                b.HasKey(x => x.Id);
                b.OwnsOne(x => x.IsDeleted);
                b.Property(x => x.RowVersion).IsRowVersion();
            });
        }

        // ── ChatConversations ─────────────────────────────────────────────────
        private static void ApplyConversations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatConversations>(b =>
            {
                b.ToTable("ChatConversations", t =>
                {
                    // Kullanıcı kendisiyle birebir sohbet açamaz.
                    t.HasCheckConstraint(
                        "CK_ChatConversations_DirectNotSelf",
                        "[DirectUserAId] IS NULL OR [DirectUserBId] IS NULL OR [DirectUserAId] <> [DirectUserBId]");

                    // Birebir sohbette her iki taraf da dolu olmalıdır.
                    t.HasCheckConstraint(
                        "CK_ChatConversations_DirectPairComplete",
                        "[ConversationType] <> 1 OR ([DirectUserAId] IS NOT NULL AND [DirectUserBId] IS NOT NULL)");
                });

                b.Property(x => x.Title).HasMaxLength(256);
                b.Property(x => x.LastMessagePreview).HasMaxLength(256);
                b.Property(x => x.ConversationType).HasConversion<byte>();

                // Birebir sohbet tekilliği — sıralanmış çift deseni (Friendships ile aynı).
                b.Property(x => x.DirectPairLowId)
                    .HasComputedColumnSql(
                        "(CASE WHEN [DirectUserAId] IS NULL OR [DirectUserBId] IS NULL THEN NULL " +
                        "WHEN [DirectUserAId] < [DirectUserBId] THEN [DirectUserAId] ELSE [DirectUserBId] END)",
                        stored: true);

                b.Property(x => x.DirectPairHighId)
                    .HasComputedColumnSql(
                        "(CASE WHEN [DirectUserAId] IS NULL OR [DirectUserBId] IS NULL THEN NULL " +
                        "WHEN [DirectUserAId] < [DirectUserBId] THEN [DirectUserBId] ELSE [DirectUserAId] END)",
                        stored: true);

                // Aynı iki kişi arasında ikinci bir birebir sohbet açılamaz.
                b.HasIndex(x => new { x.DirectPairLowId, x.DirectPairHighId })
                    .IsUnique()
                    .HasDatabaseName("UX_ChatConversations_DirectPair")
                    .HasFilter("[ConversationType] = 1 AND [DirectPairLowId] IS NOT NULL AND [IsDeleted_IsDeletedStatu] = 0");

                // Sohbet listesi sıralaması.
                b.HasIndex(x => x.LastMessageAtUtc);
                // Mağaza gelen kutusu.
                b.HasIndex(x => new { x.StoreId, x.LastMessageAtUtc });
                // Bağlamdan sohbete geçiş ("bu sipariş hakkında mesajlaş").
                b.HasIndex(x => x.OrderId);
                b.HasIndex(x => x.SubOrderId);
                b.HasIndex(x => x.ProductId);
                b.HasIndex(x => x.DisputeId);
                b.HasIndex(x => x.SupportTicketId);
                b.HasIndex(x => new { x.ConversationType, x.IsActive });

                b.HasOne(typeof(Store)).WithMany()
                    .HasForeignKey(nameof(ChatConversations.StoreId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(ChatConversations.ProductId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Orders)).WithMany()
                    .HasForeignKey(nameof(ChatConversations.OrderId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(SubOrders)).WithMany()
                    .HasForeignKey(nameof(ChatConversations.SubOrderId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Disputes)).WithMany()
                    .HasForeignKey(nameof(ChatConversations.DisputeId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Media)).WithMany()
                    .HasForeignKey(nameof(ChatConversations.AvatarMediaId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ChatConversations.DirectUserAId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ChatConversations.DirectUserBId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ChatConversations.LastMessageSenderUserId))
                    .OnDelete(DeleteBehavior.NoAction);
                // LastMessageId için FK TANIMLANMAZ: Conversation → Message → Conversation
                // döngüsü oluşturur ve INSERT sırasında tavuk-yumurta sorununa yol açar.
                // Denormalize iz alanıdır; tutarlılığı servis katmanı sağlar.
            });
        }

        // ── ChatParticipants ──────────────────────────────────────────────────
        private static void ApplyParticipants(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatParticipants>(b =>
            {
                b.Property(x => x.Role).HasConversion<byte>();

                // Bir kullanıcı bir sohbete iki kez eklenemez.
                b.HasIndex(x => new { x.ConversationId, x.UserId })
                    .IsUnique()
                    .HasDatabaseName("UX_ChatParticipants_Pair")
                    .HasFilter(SoftDeleteFilter);

                // "Sohbetlerim" ekranı: kullanıcının aktif sohbetleri.
                b.HasIndex(x => new { x.UserId, x.IsArchived, x.LeftAtUtc });
                // Sabitlenmiş sohbetler.
                b.HasIndex(x => new { x.UserId, x.IsPinned });
                // Toplam okunmamış rozeti.
                b.HasIndex(x => new { x.UserId, x.UnreadCount });
                // Mağaza temsilcisi gelen kutusu.
                b.HasIndex(x => new { x.OnBehalfOfStoreId, x.LeftAtUtc });

                b.HasOne(typeof(ChatConversations)).WithMany()
                    .HasForeignKey(nameof(ChatParticipants.ConversationId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ChatParticipants.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ChatParticipants.AddedByUserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Store)).WithMany()
                    .HasForeignKey(nameof(ChatParticipants.OnBehalfOfStoreId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(ChatMessages)).WithMany()
                    .HasForeignKey(nameof(ChatParticipants.LastReadMessageId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── ChatMessages ──────────────────────────────────────────────────────
        private static void ApplyMessages(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatMessages>(b =>
            {
                b.ToTable("ChatMessages", t =>
                {
                    t.HasCheckConstraint(
                        "CK_ChatMessages_ReplyNotSelf",
                        "[ReplyToMessageId] IS NULL OR [ReplyToMessageId] <> [Id]");
                });

                b.Property(x => x.Content).HasMaxLength(8000);
                b.Property(x => x.ClientMessageId).HasMaxLength(64);

                b.Property(x => x.MessageType).HasConversion<byte>();
                b.Property(x => x.Status).HasConversion<byte>();

                b.Property(x => x.Latitude).HasPrecision(9, 6);
                b.Property(x => x.Longitude).HasPrecision(9, 6);

                // SOHBET EKRANININ ANA SORGUSU (§41): mesajları zaman sırasıyla getir.
                b.HasIndex(x => new { x.ConversationId, x.CreatedAtUtc });
                // "Gönderdiğim mesajlar" ve kullanıcı bazlı denetim.
                b.HasIndex(x => new { x.SenderUserId, x.CreatedAtUtc });
                // Yanıt zinciri.
                b.HasIndex(x => x.ReplyToMessageId);
                // Kaybolan mesajları temizleyen arka plan işi.
                b.HasIndex(x => x.ExpiresAtUtc);
                // Moderasyon kuyruğu.
                b.HasIndex(x => x.IsFlagged);

                // Aynı istemci mesajı iki kez kaydedilemez (idempotency).
                b.HasIndex(x => new { x.ConversationId, x.ClientMessageId })
                    .IsUnique()
                    .HasDatabaseName("UX_ChatMessages_ClientMessageId")
                    .HasFilter("[ClientMessageId] IS NOT NULL AND [IsDeleted_IsDeletedStatu] = 0");

                b.HasOne(typeof(ChatConversations)).WithMany()
                    .HasForeignKey(nameof(ChatMessages.ConversationId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ChatMessages.SenderUserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Store)).WithMany()
                    .HasForeignKey(nameof(ChatMessages.SenderStoreId))
                    .OnDelete(DeleteBehavior.NoAction);
                // Kendine referanslar — döngü riskine karşı NoAction.
                b.HasOne(typeof(ChatMessages)).WithMany()
                    .HasForeignKey(nameof(ChatMessages.ReplyToMessageId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(ChatMessages)).WithMany()
                    .HasForeignKey(nameof(ChatMessages.ForwardedFromMessageId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(ChatMessages.SharedProductId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Orders)).WithMany()
                    .HasForeignKey(nameof(ChatMessages.SharedOrderId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Posts)).WithMany()
                    .HasForeignKey(nameof(ChatMessages.SharedPostId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── ChatMessageReads (yüksek hacim — soft delete/rowversion yok) ──────
        private static void ApplyMessageReads(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatMessageReads>(b =>
            {
                b.HasKey(x => x.Id);

                // Bir kullanıcı bir mesajı bir kez okur.
                b.HasIndex(x => new { x.MessageId, x.UserId })
                    .IsUnique()
                    .HasDatabaseName("UX_ChatMessageReads_Pair");

                // "Kimler okudu" listesi.
                b.HasIndex(x => x.MessageId);
                // Okunmamış sayacı doğrulaması.
                b.HasIndex(x => new { x.ConversationId, x.UserId, x.ReadAtUtc });

                b.HasOne(typeof(ChatMessages)).WithMany()
                    .HasForeignKey(nameof(ChatMessageReads.MessageId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ChatMessageReads.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
                // ConversationId denormalize iz alanıdır; ikinci cascade yolu
                // oluşturmamak için FK TANIMLANMAZ.
            });
        }

        // ── ChatMessageMedia ──────────────────────────────────────────────────
        private static void ApplyMessageMedia(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatMessageMedia>(b =>
            {
                b.Property(x => x.AltText).HasMaxLength(1000);
                b.Property(x => x.MediaRole).HasConversion<byte>();

                b.HasIndex(x => new { x.MessageId, x.DisplayOrder });
                b.HasIndex(x => x.MediaId);

                b.HasIndex(x => new { x.MessageId, x.MediaId })
                    .IsUnique()
                    .HasDatabaseName("UX_ChatMessageMedia_Pair")
                    .HasFilter(SoftDeleteFilter);

                b.HasOne(typeof(ChatMessages)).WithMany()
                    .HasForeignKey(nameof(ChatMessageMedia.MessageId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Media)).WithMany()
                    .HasForeignKey(nameof(ChatMessageMedia.MediaId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
