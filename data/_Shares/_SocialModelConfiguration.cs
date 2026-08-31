using System;
using Microsoft.EntityFrameworkCore;
using data._Categories;
using data._Follows;
using data._Galleries;
using data._Products;
using data._Store;
using data._Users;
using data.Owned;

namespace data._Shares
{
    /// <summary>
    /// Sosyal içerik modülünün (gönderi, medya, tepki, yorum, paylaşım, repost,
    /// kaydetme, anma, hashtag, şikâyet, moderasyon, görüntülenme) EF Core Fluent
    /// yapılandırması.
    ///
    /// _ApplicationConnectionDb.OnModelCreating içinde şu satırla uygulanır:
    ///     _SocialModelConfiguration.Apply(modelBuilder);
    ///
    /// SİLME DAVRANIŞI POLİTİKASI (§43):
    ///  • Gönderi → alt etkileşimleri (medya bağı, tepki, yorum, hashtag bağı, anma,
    ///    kaydetme, paylaşım, hedef kitle) CASCADE'dir: gönderi yoksa bu kayıtların
    ///    hiçbir anlamı kalmaz.
    ///  • Gönderi → Media CASCADE DEĞİLDİR: medya merkezî varlıktır ve başka yerlerde
    ///    kullanılıyor olabilir.
    ///  • Kullanıcı ve mağaza referansları NoAction'dır: bir kullanıcı silindiğinde
    ///    sosyal geçmiş sessizce yok edilmemeli, anonimleştirme akışıyla ele alınmalıdır.
    ///  • Şikâyet ve moderasyon kayıtları HİÇBİR ZAMAN cascade ile silinmez —
    ///    bunlar denetim izidir.
    /// </summary>
    public static class _SocialModelConfiguration
    {
        private const string SoftDeleteFilter = "[IsDeleted_IsDeletedStatu] = 0";

        /// <summary>Tüm sosyal içerik yapılandırmasını uygular.</summary>
        public static void Apply(ModelBuilder modelBuilder)
        {
            ApplyBaseConventions(modelBuilder);

            ApplyPosts(modelBuilder);
            ApplyPostMedia(modelBuilder);
            ApplyPostAudience(modelBuilder);
            ApplyReactions(modelBuilder);
            ApplyComments(modelBuilder);
            ApplySharesAndReposts(modelBuilder);
            ApplySavedPosts(modelBuilder);
            ApplyMentionsAndTags(modelBuilder);
            ApplyHashtags(modelBuilder);
            ApplyArticles(modelBuilder);
            ApplyContentReports(modelBuilder);
            ApplyModerationActions(modelBuilder);
            ApplyViewEvents(modelBuilder);
            ApplyCrossModuleForeignKeys(modelBuilder);
        }

        // ── Ortak konvansiyonlar ──────────────────────────────────────────────
        private static void ApplyBaseConventions(ModelBuilder modelBuilder)
        {
            ConfigureBase<Posts>(modelBuilder);
            ConfigureBase<PostMedia>(modelBuilder);
            ConfigureBase<PostAudienceUsers>(modelBuilder);
            ConfigureBase<PostAudienceRules>(modelBuilder);
            ConfigureBase<PostReactions>(modelBuilder);
            ConfigureBase<PostComments>(modelBuilder);
            ConfigureBase<CommentReactions>(modelBuilder);
            ConfigureBase<PostShares>(modelBuilder);
            ConfigureBase<PostReposts>(modelBuilder);
            ConfigureBase<SavedPosts>(modelBuilder);
            ConfigureBase<PostMentions>(modelBuilder);
            ConfigureBase<CommentMentions>(modelBuilder);
            ConfigureBase<PostProductTags>(modelBuilder);
            ConfigureBase<Hashtags>(modelBuilder);
            ConfigureBase<PostHashtags>(modelBuilder);
            ConfigureBase<Articles>(modelBuilder);
            ConfigureBase<ContentReports>(modelBuilder);
        }

        private static void ConfigureBase<TEntity>(ModelBuilder modelBuilder)
            where TEntity : SocialEntityBase
        {
            modelBuilder.Entity<TEntity>(b =>
            {
                b.HasKey(x => x.Id);
                b.OwnsOne(x => x.IsDeleted);
                b.Property(x => x.RowVersion).IsRowVersion();
            });
        }

        // ── Posts ─────────────────────────────────────────────────────────────
        private static void ApplyPosts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Posts>(b =>
            {
                b.ToTable("Posts", t =>
                {
                    // SAHİPLİK BÜTÜNLÜĞÜ: AuthorType ile dolu olan kolon uyuşmak
                    // ZORUNDADIR ve tam olarak biri dolu olmalıdır.
                    t.HasCheckConstraint(
                        "CK_Posts_AuthorXor",
                        "([AuthorType] = 1 AND [AuthorUserId] IS NOT NULL AND [AuthorStoreId] IS NULL) OR " +
                        "([AuthorType] = 2 AND [AuthorStoreId] IS NOT NULL AND [AuthorUserId] IS NULL)");

                    // Repost gönderisi kendi kendisinin orijinali olamaz.
                    t.HasCheckConstraint(
                        "CK_Posts_OriginalNotSelf",
                        "[OriginalPostId] IS NULL OR [OriginalPostId] <> [Id]");
                });

                b.Property(x => x.Title).HasMaxLength(512);
                b.Property(x => x.Content).HasMaxLength(20000);
                b.Property(x => x.Slug).HasMaxLength(256);
                b.Property(x => x.LinkUrl).HasMaxLength(2048);
                b.Property(x => x.LinkTitle).HasMaxLength(512);
                b.Property(x => x.LinkDescription).HasMaxLength(2048);
                b.Property(x => x.LocationText).HasMaxLength(256);
                b.Property(x => x.ModerationReason).HasMaxLength(1024);

                b.Property(x => x.AuthorType).HasConversion<byte>();
                b.Property(x => x.PostType).HasConversion<byte>();
                b.Property(x => x.Visibility).HasConversion<byte>();
                b.Property(x => x.Status).HasConversion<byte>();
                b.Property(x => x.ModerationStatus).HasConversion<byte>();
                b.Property(x => x.Language).HasConversion<byte>();

                b.Property(x => x.AiModerationScore).HasPrecision(5, 4);
                b.Property(x => x.RankScore).HasPrecision(18, 6);

                b.OwnsOne(x => x.Meta);
                b.OwnsOne(x => x.Interaction);

                // ── İNDEKSLER (§41, §64) ──────────────────────────────────────
                // Kullanıcı profili akışı (en yeniden eskiye).
                b.HasIndex(x => new { x.AuthorUserId, x.CreatedAtUtc });
                // Mağaza sayfası akışı.
                b.HasIndex(x => new { x.AuthorStoreId, x.CreatedAtUtc });
                // Ana akış (feed) filtresi: görünürlük + durum + yayın zamanı.
                b.HasIndex(x => new { x.Visibility, x.Status, x.PublishAtUtc });
                // Profil sayfası: sabitlenmiş gönderiler önce.
                b.HasIndex(x => new { x.AuthorUserId, x.Status, x.IsPinned, x.PublishAtUtc });
                // Genel zaman sıralaması.
                b.HasIndex(x => x.CreatedAtUtc);
                // Zamanlanmış yayın işini besleyen kuyruk.
                b.HasIndex(x => new { x.Status, x.PublishAtUtc });
                // Süresi dolan hikâyeleri kapatan iş.
                b.HasIndex(x => x.ExpiresAtUtc);
                // Moderasyon kuyruğu.
                b.HasIndex(x => new { x.ModerationStatus, x.CreatedAtUtc });
                // Keşfet / öne çıkanlar sıralaması.
                b.HasIndex(x => new { x.Visibility, x.Status, x.RankScore });
                // Ürün detay sayfasındaki "bu ürünle ilgili gönderiler".
                b.HasIndex(x => x.RelatedProductId);
                // "Bu gönderi kaç kez repost edildi" ve repost zinciri.
                b.HasIndex(x => x.OriginalPostId);

                // Herkese açık gönderilerin SEO adresi tekildir.
                b.HasIndex(x => x.Slug)
                    .IsUnique()
                    .HasDatabaseName("UX_Posts_Slug")
                    .HasFilter("[Slug] IS NOT NULL AND [IsDeleted_IsDeletedStatu] = 0");

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(Posts.AuthorUserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Store)).WithMany()
                    .HasForeignKey(nameof(Posts.AuthorStoreId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(Posts.RelatedProductId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(data._Promotions.Campaigns)).WithMany()
                    .HasForeignKey(nameof(Posts.RelatedCampaignId))
                    .OnDelete(DeleteBehavior.NoAction);
                // Kendine referans (repost zinciri) — döngü riskine karşı NoAction.
                b.HasOne(typeof(Posts)).WithMany()
                    .HasForeignKey(nameof(Posts.OriginalPostId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(Posts.ModeratedByUserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── PostMedia ─────────────────────────────────────────────────────────
        private static void ApplyPostMedia(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PostMedia>(b =>
            {
                b.Property(x => x.AltText).HasMaxLength(1000);
                b.Property(x => x.Caption).HasMaxLength(2000);
                b.Property(x => x.MediaRole).HasConversion<byte>();

                // Karusel sıralaması — gönderi detayının en sıcak sorgusu.
                b.HasIndex(x => new { x.PostId, x.DisplayOrder });
                // "Bu medya hangi gönderilerde kullanılıyor?"
                b.HasIndex(x => x.MediaId);

                // Aynı medya bir gönderiye aynı rolle iki kez eklenemez.
                b.HasIndex(x => new { x.PostId, x.MediaId, x.MediaRole })
                    .IsUnique()
                    .HasDatabaseName("UX_PostMedia_Usage")
                    .HasFilter(SoftDeleteFilter);

                b.HasOne(typeof(Posts)).WithMany()
                    .HasForeignKey(nameof(PostMedia.PostId))
                    .OnDelete(DeleteBehavior.Cascade);
                // Medya merkezîdir; gönderi silinse bile asset silinmez.
                b.HasOne(typeof(Media)).WithMany()
                    .HasForeignKey(nameof(PostMedia.MediaId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── Hedef kitle ───────────────────────────────────────────────────────
        private static void ApplyPostAudience(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PostAudienceUsers>(b =>
            {
                b.HasIndex(x => new { x.PostId, x.UserId })
                    .IsUnique()
                    .HasDatabaseName("UX_PostAudienceUsers_Pair")
                    .HasFilter(SoftDeleteFilter);

                // "Bu kullanıcı hangi özel listelerde?" — akış görünürlük denetimi.
                b.HasIndex(x => new { x.UserId, x.IsExcluded });

                b.HasOne(typeof(Posts)).WithMany()
                    .HasForeignKey(nameof(PostAudienceUsers.PostId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(PostAudienceUsers.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<PostAudienceRules>(b =>
            {
                b.Property(x => x.RuleType).HasConversion<byte>();

                b.HasIndex(x => new { x.PostId, x.RuleType });

                b.HasOne(typeof(Posts)).WithMany()
                    .HasForeignKey(nameof(PostAudienceRules.PostId))
                    .OnDelete(DeleteBehavior.Cascade);
                // TargetId polimorfiktir (RuleType'a göre Store veya Product) — FK yok.
            });
        }

        // ── Tepkiler ──────────────────────────────────────────────────────────
        private static void ApplyReactions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PostReactions>(b =>
            {
                b.Property(x => x.ReactionType).HasConversion<byte>();

                // BİR KULLANICI BİR GÖNDERİDE TEK AKTİF TEPKİ.
                b.HasIndex(x => new { x.PostId, x.UserId })
                    .IsUnique()
                    .HasDatabaseName("UX_PostReactions_UserPerPost")
                    .HasFilter(SoftDeleteFilter);

                // "Kimler tepki verdi" listesi ve tür kırılımı.
                b.HasIndex(x => new { x.PostId, x.ReactionType });
                // "Beğendiklerim" ekranı.
                b.HasIndex(x => new { x.UserId, x.CreatedAtUtc });

                b.HasOne(typeof(Posts)).WithMany()
                    .HasForeignKey(nameof(PostReactions.PostId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(PostReactions.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<CommentReactions>(b =>
            {
                b.Property(x => x.ReactionType).HasConversion<byte>();

                b.HasIndex(x => new { x.CommentId, x.UserId })
                    .IsUnique()
                    .HasDatabaseName("UX_CommentReactions_UserPerComment")
                    .HasFilter(SoftDeleteFilter);

                b.HasIndex(x => new { x.CommentId, x.ReactionType });
                b.HasIndex(x => new { x.UserId, x.CreatedAtUtc });

                b.HasOne(typeof(PostComments)).WithMany()
                    .HasForeignKey(nameof(CommentReactions.CommentId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(CommentReactions.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── Yorumlar ──────────────────────────────────────────────────────────
        private static void ApplyComments(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PostComments>(b =>
            {
                b.ToTable("PostComments", t =>
                {
                    // Yorum kendi kendisinin üstü olamaz.
                    t.HasCheckConstraint(
                        "CK_PostComments_ParentNotSelf",
                        "[ParentCommentId] IS NULL OR [ParentCommentId] <> [Id]");
                });

                b.Property(x => x.Content).HasMaxLength(8000).IsRequired();
                b.Property(x => x.ModerationReason).HasMaxLength(1024);

                b.Property(x => x.Status).HasConversion<byte>();
                b.Property(x => x.ModerationStatus).HasConversion<byte>();

                // Gönderi altındaki yorum listesi (en sıcak sorgu).
                b.HasIndex(x => new { x.PostId, x.CreatedAtUtc });
                // Sabitlenmiş yorum önce, sonra popülerlik.
                b.HasIndex(x => new { x.PostId, x.IsPinned, x.LikeCount });
                // Bir yanıt dalını tek aramayla getirme (özyinelemeli CTE gerekmez).
                b.HasIndex(x => new { x.RootCommentId, x.CreatedAtUtc });
                // Doğrudan alt yanıtlar.
                b.HasIndex(x => x.ParentCommentId);
                // "Yorumlarım" ekranı.
                b.HasIndex(x => new { x.UserId, x.CreatedAtUtc });
                // Moderasyon kuyruğu.
                b.HasIndex(x => new { x.ModerationStatus, x.CreatedAtUtc });

                b.HasOne(typeof(Posts)).WithMany()
                    .HasForeignKey(nameof(PostComments.PostId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(PostComments.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Store)).WithMany()
                    .HasForeignKey(nameof(PostComments.AuthorStoreId))
                    .OnDelete(DeleteBehavior.NoAction);
                // Ağaç kendine referans — çoklu yol/döngü hatasını önlemek için NoAction.
                b.HasOne(typeof(PostComments)).WithMany()
                    .HasForeignKey(nameof(PostComments.ParentCommentId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(PostComments)).WithMany()
                    .HasForeignKey(nameof(PostComments.RootCommentId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(PostComments.ModeratedByUserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── Paylaşım ve repost ────────────────────────────────────────────────
        private static void ApplySharesAndReposts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PostShares>(b =>
            {
                b.Property(x => x.TargetType).HasConversion<byte>();
                b.Property(x => x.ExternalPlatform).HasMaxLength(64);
                b.Property(x => x.Message).HasMaxLength(2000);

                // "Bu gönderi ne zaman, kaç kez paylaşıldı" (§41).
                b.HasIndex(x => new { x.PostId, x.CreatedAtUtc });
                // "Paylaştıklarım" ekranı.
                b.HasIndex(x => new { x.UserId, x.CreatedAtUtc });
                // Kanal kırılımlı analitik.
                b.HasIndex(x => new { x.PostId, x.TargetType });

                b.HasOne(typeof(Posts)).WithMany()
                    .HasForeignKey(nameof(PostShares.PostId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(PostShares.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
                // TargetId, TargetType'a göre Users/ChatConversations/Store işaret eder —
                // bilinçli polimorfik iz alanıdır, FK tanımlanmaz.
            });

            modelBuilder.Entity<PostReposts>(b =>
            {
                b.ToTable("PostReposts", t =>
                {
                    t.HasCheckConstraint(
                        "CK_PostReposts_NotSelf",
                        "[OriginalPostId] <> [RepostPostId]");
                });

                b.Property(x => x.QuoteText).HasMaxLength(2000);

                // Bir kullanıcı aynı gönderiyi bir kez repost edebilir.
                b.HasIndex(x => new { x.OriginalPostId, x.UserId })
                    .IsUnique()
                    .HasDatabaseName("UX_PostReposts_UserPerPost")
                    .HasFilter(SoftDeleteFilter);

                // Repost gönderisinden orijinaline geri izleme.
                b.HasIndex(x => x.RepostPostId);
                b.HasIndex(x => new { x.OriginalPostId, x.CreatedAtUtc });
                b.HasIndex(x => new { x.UserId, x.CreatedAtUtc });

                // İkisi de Posts'a bakar → SQL Server çoklu cascade yolunu reddeder.
                b.HasOne(typeof(Posts)).WithMany()
                    .HasForeignKey(nameof(PostReposts.OriginalPostId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Posts)).WithMany()
                    .HasForeignKey(nameof(PostReposts.RepostPostId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(PostReposts.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Store)).WithMany()
                    .HasForeignKey(nameof(PostReposts.RepostStoreId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── Kaydedilenler ─────────────────────────────────────────────────────
        private static void ApplySavedPosts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SavedPosts>(b =>
            {
                b.Property(x => x.CollectionName).HasMaxLength(128);
                b.Property(x => x.Note).HasMaxLength(1000);

                b.HasIndex(x => new { x.UserId, x.PostId })
                    .IsUnique()
                    .HasDatabaseName("UX_SavedPosts_UserPost")
                    .HasFilter(SoftDeleteFilter);

                // "Kaydettiklerim" ekranı (en yeniden eskiye).
                b.HasIndex(x => new { x.UserId, x.CreatedAtUtc });
                // Koleksiyon bazlı listeleme.
                b.HasIndex(x => new { x.UserId, x.CollectionName });
                // Gönderi bazlı sayaç doğrulaması.
                b.HasIndex(x => x.PostId);

                b.HasOne(typeof(Posts)).WithMany()
                    .HasForeignKey(nameof(SavedPosts.PostId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(SavedPosts.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── Anmalar ve ürün etiketleri ────────────────────────────────────────
        private static void ApplyMentionsAndTags(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PostMentions>(b =>
            {
                b.Property(x => x.MentionedUsernameSnapshot).HasMaxLength(32);

                b.HasIndex(x => new { x.PostId, x.MentionedUserId, x.StartIndex })
                    .IsUnique()
                    .HasDatabaseName("UX_PostMentions_Position")
                    .HasFilter(SoftDeleteFilter);

                // "Beni etiketleyen gönderiler" ekranı.
                b.HasIndex(x => new { x.MentionedUserId, x.CreatedAtUtc });
                b.HasIndex(x => x.PostId);

                b.HasOne(typeof(Posts)).WithMany()
                    .HasForeignKey(nameof(PostMentions.PostId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(PostMentions.MentionedUserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(PostMentions.MentionedByUserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<CommentMentions>(b =>
            {
                b.Property(x => x.MentionedUsernameSnapshot).HasMaxLength(32);

                b.HasIndex(x => new { x.CommentId, x.MentionedUserId, x.StartIndex })
                    .IsUnique()
                    .HasDatabaseName("UX_CommentMentions_Position")
                    .HasFilter(SoftDeleteFilter);

                b.HasIndex(x => new { x.MentionedUserId, x.CreatedAtUtc });

                b.HasOne(typeof(PostComments)).WithMany()
                    .HasForeignKey(nameof(CommentMentions.CommentId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(CommentMentions.MentionedUserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(CommentMentions.MentionedByUserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<PostProductTags>(b =>
            {
                b.Property(x => x.PositionX).HasPrecision(5, 4);
                b.Property(x => x.PositionY).HasPrecision(5, 4);

                b.HasIndex(x => new { x.PostId, x.ProductId })
                    .IsUnique()
                    .HasDatabaseName("UX_PostProductTags_Pair")
                    .HasFilter(SoftDeleteFilter);

                // "Bu ürünü etiketleyen gönderiler" — ürün detay sayfası sekmesi.
                b.HasIndex(x => new { x.ProductId, x.CreatedAtUtc });

                b.HasOne(typeof(Posts)).WithMany()
                    .HasForeignKey(nameof(PostProductTags.PostId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(PostProductTags.ProductId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(ProductVariants)).WithMany()
                    .HasForeignKey(nameof(PostProductTags.ProductVariantId))
                    .OnDelete(DeleteBehavior.NoAction);
                // Posts → PostMedia zaten cascade; ikinci cascade yolu oluşmasın diye NoAction.
                b.HasOne(typeof(PostMedia)).WithMany()
                    .HasForeignKey(nameof(PostProductTags.PostMediaId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── Hashtag ───────────────────────────────────────────────────────────
        private static void ApplyHashtags(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Hashtags>(b =>
            {
                b.Property(x => x.Name).HasMaxLength(140).IsRequired();
                b.Property(x => x.NormalizedName).HasMaxLength(140).IsRequired();
                b.Property(x => x.Slug).HasMaxLength(160);
                b.Property(x => x.Description).HasMaxLength(1000);
                b.Property(x => x.BlockReason).HasMaxLength(512);

                // Etiket sözlüğü GLOBAL TEKİLDİR — arama bu kolon üzerinden yapılır.
                b.HasIndex(x => x.NormalizedName)
                    .IsUnique()
                    .HasDatabaseName("UX_Hashtags_NormalizedName")
                    .HasFilter(SoftDeleteFilter);

                b.HasIndex(x => x.Slug);
                // Trend listesi.
                b.HasIndex(x => new { x.RecentUsageCount, x.LastUsedAtUtc });
                b.HasIndex(x => x.PostCount);
            });

            modelBuilder.Entity<PostHashtags>(b =>
            {
                b.HasIndex(x => new { x.PostId, x.HashtagId })
                    .IsUnique()
                    .HasDatabaseName("UX_PostHashtags_Pair")
                    .HasFilter(SoftDeleteFilter);

                // Hashtag sayfası: etikete ait gönderiler (en yeniden eskiye).
                b.HasIndex(x => new { x.HashtagId, x.CreatedAtUtc });

                b.HasOne(typeof(Posts)).WithMany()
                    .HasForeignKey(nameof(PostHashtags.PostId))
                    .OnDelete(DeleteBehavior.Cascade);
                // Etiket sözlüğü kalıcıdır; gönderi silinse bile etiket silinmez.
                b.HasOne(typeof(Hashtags)).WithMany()
                    .HasForeignKey(nameof(PostHashtags.HashtagId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── Articles ──────────────────────────────────────────────────────────
        private static void ApplyArticles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Articles>(b =>
            {
                b.ToTable("Articles", t =>
                {
                    t.HasCheckConstraint(
                        "CK_Articles_AuthorXor",
                        "([AuthorType] = 1 AND [AuthorUserId] IS NOT NULL AND [AuthorStoreId] IS NULL) OR " +
                        "([AuthorType] = 2 AND [AuthorStoreId] IS NOT NULL AND [AuthorUserId] IS NULL)");
                });

                b.Property(x => x.Title).HasMaxLength(512).IsRequired();
                b.Property(x => x.Slug).HasMaxLength(256).IsRequired();
                b.Property(x => x.Summary).HasMaxLength(2000);

                b.Property(x => x.AuthorType).HasConversion<byte>();
                b.Property(x => x.Status).HasConversion<byte>();
                b.Property(x => x.ModerationStatus).HasConversion<byte>();
                b.Property(x => x.Language).HasConversion<byte>();

                b.OwnsOne(x => x.Meta);
                b.OwnsOne(x => x.Interaction);

                // Aynı dilde aynı slug iki kez kullanılamaz (çok dilli SEO deseni).
                b.HasIndex(x => new { x.Slug, x.Language })
                    .IsUnique()
                    .HasDatabaseName("UX_Articles_SlugLanguage")
                    .HasFilter(SoftDeleteFilter);

                b.HasIndex(x => new { x.Status, x.PublishAtUtc });
                b.HasIndex(x => new { x.CategoriesArticleId, x.Status, x.PublishAtUtc });
                b.HasIndex(x => new { x.AuthorUserId, x.PublishAtUtc });
                b.HasIndex(x => new { x.AuthorStoreId, x.PublishAtUtc });
                b.HasIndex(x => x.IsFeatured);

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(Articles.AuthorUserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Store)).WithMany()
                    .HasForeignKey(nameof(Articles.AuthorStoreId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(CategoriesArticle)).WithMany()
                    .HasForeignKey(nameof(Articles.CategoriesArticleId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Media)).WithMany()
                    .HasForeignKey(nameof(Articles.CoverMediaId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── ContentReports ────────────────────────────────────────────────────
        private static void ApplyContentReports(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContentReports>(b =>
            {
                b.ToTable("ContentReports", t =>
                {
                    // TAM OLARAK BİR hedef kolonu dolu olmalı VE TargetType ile uyuşmalı.
                    t.HasCheckConstraint(
                        "CK_ContentReports_TargetMatchesType",
                        "((CASE WHEN [TargetPostId]            IS NULL THEN 0 ELSE 1 END) + " +
                        " (CASE WHEN [TargetCommentId]         IS NULL THEN 0 ELSE 1 END) + " +
                        " (CASE WHEN [TargetUserId]            IS NULL THEN 0 ELSE 1 END) + " +
                        " (CASE WHEN [TargetStoreId]           IS NULL THEN 0 ELSE 1 END) + " +
                        " (CASE WHEN [TargetProductId]         IS NULL THEN 0 ELSE 1 END) + " +
                        " (CASE WHEN [TargetProductReviewId]   IS NULL THEN 0 ELSE 1 END) + " +
                        " (CASE WHEN [TargetProductQuestionId] IS NULL THEN 0 ELSE 1 END) + " +
                        " (CASE WHEN [TargetChatMessageId]     IS NULL THEN 0 ELSE 1 END) + " +
                        " (CASE WHEN [TargetArticleId]         IS NULL THEN 0 ELSE 1 END) + " +
                        " (CASE WHEN [TargetMediaId]           IS NULL THEN 0 ELSE 1 END)) = 1 " +
                        "AND (" +
                        "  ([TargetType] = 1  AND [TargetPostId]            IS NOT NULL) OR " +
                        "  ([TargetType] = 2  AND [TargetCommentId]         IS NOT NULL) OR " +
                        "  ([TargetType] = 3  AND [TargetUserId]            IS NOT NULL) OR " +
                        "  ([TargetType] = 4  AND [TargetStoreId]           IS NOT NULL) OR " +
                        "  ([TargetType] = 5  AND [TargetProductId]         IS NOT NULL) OR " +
                        "  ([TargetType] = 6  AND [TargetProductReviewId]   IS NOT NULL) OR " +
                        "  ([TargetType] = 7  AND [TargetProductQuestionId] IS NOT NULL) OR " +
                        "  ([TargetType] = 8  AND [TargetChatMessageId]     IS NOT NULL) OR " +
                        "  ([TargetType] = 9  AND [TargetArticleId]         IS NOT NULL) OR " +
                        "  ([TargetType] = 10 AND [TargetMediaId]           IS NOT NULL))");

                    // Kullanıcı kendini şikâyet edemez.
                    t.HasCheckConstraint(
                        "CK_ContentReports_NotSelfUser",
                        "[TargetUserId] IS NULL OR [TargetUserId] <> [ReporterUserId]");
                });

                b.Property(x => x.TargetType).HasConversion<byte>();
                b.Property(x => x.Reason).HasConversion<byte>();
                b.Property(x => x.Status).HasConversion<byte>();

                b.Property(x => x.Description).HasMaxLength(4000);
                b.Property(x => x.SourceSurface).HasMaxLength(64);
                b.Property(x => x.ResolutionNote).HasMaxLength(4000);
                b.Property(x => x.LeasedBy).HasMaxLength(128);

                // Hedef kolonlarından dolu olanın değeri — tekillik indeksi için.
                b.Property(x => x.TargetEntityId)
                    .HasComputedColumnSql(
                        "COALESCE([TargetPostId], [TargetCommentId], [TargetUserId], [TargetStoreId], " +
                        "[TargetProductId], [TargetProductReviewId], [TargetProductQuestionId], " +
                        "[TargetChatMessageId], [TargetArticleId], [TargetMediaId])",
                        stored: true);

                // BİR KULLANICI AYNI İÇERİĞİ BİR KEZ ŞİKÂYET EDER (açık şikâyetler arasında).
                b.HasIndex(x => new { x.ReporterUserId, x.TargetType, x.TargetEntityId })
                    .IsUnique()
                    .HasDatabaseName("UX_ContentReports_ReporterTarget")
                    .HasFilter("[Status] IN (1, 4, 5) AND [IsDeleted_IsDeletedStatu] = 0");

                // Moderasyon kuyruğu: öncelik + eskilik sırası.
                b.HasIndex(x => new { x.Status, x.Priority, x.CreatedAtUtc });
                // Tür bazlı kuyruk ayrımı.
                b.HasIndex(x => new { x.TargetType, x.Status });
                // "Bu içeriğe kaç şikâyet var?" — sayaç doğrulaması.
                b.HasIndex(x => new { x.TargetType, x.TargetEntityId, x.Status });
                // Şikâyetçinin geçmişi (kötüye kullanım tespiti).
                b.HasIndex(x => new { x.ReporterUserId, x.CreatedAtUtc });
                // Lease süresi dolan kayıtları kuyruğa iade eden iş.
                b.HasIndex(x => x.LeasedUntilUtc);

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ContentReports.ReporterUserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ContentReports.ModeratedByUserId))
                    .OnDelete(DeleteBehavior.NoAction);

                // Hedef FK'leri: HİÇBİRİ cascade DEĞİLDİR — şikâyet bir denetim izidir.
                b.HasOne(typeof(Posts)).WithMany()
                    .HasForeignKey(nameof(ContentReports.TargetPostId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(PostComments)).WithMany()
                    .HasForeignKey(nameof(ContentReports.TargetCommentId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ContentReports.TargetUserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Store)).WithMany()
                    .HasForeignKey(nameof(ContentReports.TargetStoreId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(ContentReports.TargetProductId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(ProductReviews)).WithMany()
                    .HasForeignKey(nameof(ContentReports.TargetProductReviewId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(ProductQuestions)).WithMany()
                    .HasForeignKey(nameof(ContentReports.TargetProductQuestionId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(data._Chat.ChatMessages)).WithMany()
                    .HasForeignKey(nameof(ContentReports.TargetChatMessageId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Articles)).WithMany()
                    .HasForeignKey(nameof(ContentReports.TargetArticleId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Media)).WithMany()
                    .HasForeignKey(nameof(ContentReports.TargetMediaId))
                    .OnDelete(DeleteBehavior.NoAction);
                // Kendine referans (birleştirme) — NoAction.
                b.HasOne(typeof(ContentReports)).WithMany()
                    .HasForeignKey(nameof(ContentReports.DuplicateOfReportId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── ContentModerationActions (denetim izi — soft delete yok) ──────────
        private static void ApplyModerationActions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContentModerationActions>(b =>
            {
                b.HasKey(x => x.Id);

                b.Property(x => x.TargetType).HasConversion<byte>();
                b.Property(x => x.ActionType).HasConversion<byte>();
                b.Property(x => x.ActorType).HasConversion<byte>();
                b.Property(x => x.PreviousStatus).HasConversion<byte>();
                b.Property(x => x.NewStatus).HasConversion<byte>();

                b.Property(x => x.PolicyCode).HasMaxLength(128);
                b.Property(x => x.Reason).HasMaxLength(2000);
                b.Property(x => x.InternalNote).HasMaxLength(4000);
                b.Property(x => x.AiModelVersion).HasMaxLength(64);
                b.Property(x => x.AiScore).HasPrecision(5, 4);

                // "Bu içeriğin moderasyon geçmişi".
                b.HasIndex(x => new { x.TargetType, x.CreatedAtUtc });
                // "Bu moderatör hangi kararları verdi" — kalite denetimi.
                b.HasIndex(x => new { x.ActorUserId, x.CreatedAtUtc });
                // İtiraz kuyruğu.
                b.HasIndex(x => new { x.IsAppealed, x.IsReversed });
                // Şikâyetten karara izleme.
                b.HasIndex(x => x.ContentReportId);
                // Süreli kısıtlamaları kaldıran arka plan işi.
                b.HasIndex(x => x.EffectiveUntilUtc);
                b.HasIndex(x => new { x.TargetPostId, x.CreatedAtUtc });
                b.HasIndex(x => new { x.TargetUserId, x.CreatedAtUtc });

                b.HasOne(typeof(ContentReports)).WithMany()
                    .HasForeignKey(nameof(ContentModerationActions.ContentReportId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ContentModerationActions.ActorUserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Posts)).WithMany()
                    .HasForeignKey(nameof(ContentModerationActions.TargetPostId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(PostComments)).WithMany()
                    .HasForeignKey(nameof(ContentModerationActions.TargetCommentId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ContentModerationActions.TargetUserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Store)).WithMany()
                    .HasForeignKey(nameof(ContentModerationActions.TargetStoreId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(ProductReviews)).WithMany()
                    .HasForeignKey(nameof(ContentModerationActions.TargetProductReviewId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(ProductQuestions)).WithMany()
                    .HasForeignKey(nameof(ContentModerationActions.TargetProductQuestionId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Media)).WithMany()
                    .HasForeignKey(nameof(ContentModerationActions.TargetMediaId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── Görüntülenme olayları (yüksek hacim — FK ve soft delete yok) ──────
        private static void ApplyViewEvents(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContentViewEvents>(b =>
            {
                b.HasKey(x => x.Id);

                b.Property(x => x.TargetType).HasConversion<byte>();
                b.Property(x => x.SourceSurface).HasConversion<byte>();
                b.Property(x => x.SessionId).HasMaxLength(64);
                b.Property(x => x.Platform).HasMaxLength(32);
                b.Property(x => x.CountryCode).HasMaxLength(2);

                // YAZMA YOLU SICAK: yalnızca toplama işinin ve saklama süresi
                // temizliğinin ihtiyaç duyduğu iki indeks tanımlanır. Ek indeks
                // eklemek saniyede binlerce INSERT'i yavaşlatır.
                b.HasIndex(x => new { x.EventDate, x.TargetType });
                b.HasIndex(x => x.CreatedAtUtc);

                // FK TANIMLANMAZ — gerekçe entity XML dokümanındadır (analitik veri).
            });

            modelBuilder.Entity<ContentViewDailyAggregates>(b =>
            {
                b.HasKey(x => x.Id);

                b.Property(x => x.TargetType).HasConversion<byte>();

                b.HasIndex(x => new { x.TargetType, x.TargetId, x.EventDate })
                    .IsUnique()
                    .HasDatabaseName("UX_ContentViewDailyAggregates_TargetDate");

                // Grafik/rapor sorgusu: bir içeriğin tarih serisi.
                b.HasIndex(x => new { x.TargetId, x.EventDate });
                b.HasIndex(x => x.EventDate);
            });
        }

        // ── Modüller arası FK'ler ─────────────────────────────────────────────
        /// <summary>
        /// Başka modüllerde tanımlı entity'lerin bu modüldeki tablolara verdiği FK'ler.
        /// Döngüsel modül bağımlılığı oluşturmamak için burada tanımlanır.
        /// </summary>
        private static void ApplyCrossModuleForeignKeys(ModelBuilder modelBuilder)
        {
            // UserBlocks.SourceContentReportId → ContentReports.Id
            modelBuilder.Entity<UserBlocks>(b =>
            {
                b.HasIndex(x => x.SourceContentReportId);
                b.HasOne(typeof(ContentReports)).WithMany()
                    .HasForeignKey(nameof(UserBlocks.SourceContentReportId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
