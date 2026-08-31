using System;
using Microsoft.EntityFrameworkCore;
using data._Chat;
using data._Galleries;
using data._Shares;
using data._Store;
using data._Users;
using data.Owned;

namespace data._Products
{
    /// <summary>
    /// Ürün sosyal etkileşimleri (favori, tepki, paylaşım, istek listesi) ve
    /// yorum/soru medyası için EF Core Fluent yapılandırması.
    ///
    /// _ApplicationConnectionDb.OnModelCreating içinde şu satırla uygulanır:
    ///     _ProductSocialModelConfiguration.Apply(modelBuilder);
    /// ve _ProductModelConfiguration.Apply'dan SONRA çağrılmalıdır.
    /// </summary>
    public static class _ProductSocialModelConfiguration
    {
        private const string SoftDeleteFilter = "[IsDeleted_IsDeletedStatu] = 0";

        /// <summary>Tüm ürün sosyal etkileşim yapılandırmasını uygular.</summary>
        public static void Apply(ModelBuilder modelBuilder)
        {
            ApplyBaseConventions(modelBuilder);

            ApplyFavorites(modelBuilder);
            ApplyReactions(modelBuilder);
            ApplyShares(modelBuilder);
            ApplyWishlists(modelBuilder);
            ApplyReviewMedia(modelBuilder);
            ApplyQuestionMedia(modelBuilder);
        }

        private static void ApplyBaseConventions(ModelBuilder modelBuilder)
        {
            ConfigureBase<ProductFavorites>(modelBuilder);
            ConfigureBase<ProductReactions>(modelBuilder);
            ConfigureBase<ProductShares>(modelBuilder);
            ConfigureBase<Wishlists>(modelBuilder);
            ConfigureBase<WishlistItems>(modelBuilder);
            ConfigureBase<ProductReviewMedia>(modelBuilder);
            ConfigureBase<ProductQuestionMedia>(modelBuilder);
        }

        private static void ConfigureBase<TEntity>(ModelBuilder modelBuilder)
            where TEntity : ProductEntityBase
        {
            modelBuilder.Entity<TEntity>(b =>
            {
                b.HasKey(x => x.Id);
                b.OwnsOne(x => x.IsDeleted);
                b.Property(x => x.RowVersion).IsRowVersion();
            });
        }

        // ── ProductFavorites ──────────────────────────────────────────────────
        private static void ApplyFavorites(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductFavorites>(b =>
            {
                b.Property(x => x.PriceAtFavoriteTime).HasPrecision(18, 4);
                b.Property(x => x.PriceCurrency).HasConversion<byte>();

                // Aynı ürün iki kez favorilenemez.
                b.HasIndex(x => new { x.UserId, x.ProductId })
                    .IsUnique()
                    .HasDatabaseName("UX_ProductFavorites_UserProduct")
                    .HasFilter(SoftDeleteFilter);

                // "Favorilerim" ekranı (§41).
                b.HasIndex(x => new { x.UserId, x.CreatedAtUtc });
                // "Bu ürünü kaç kişi favoriledi" ve fiyat düşünce bildirim hedef kitlesi.
                b.HasIndex(x => new { x.ProductId, x.CreatedAtUtc });
                b.HasIndex(x => new { x.ProductId, x.NotifyOnPriceDrop });
                b.HasIndex(x => new { x.ProductId, x.NotifyOnBackInStock });
                // Satıcı panelinde "en çok favorilenen ürünlerim".
                b.HasIndex(x => x.StoreId);

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ProductFavorites.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(ProductFavorites.ProductId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(ProductVariants)).WithMany()
                    .HasForeignKey(nameof(ProductFavorites.ProductVariantId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Store)).WithMany()
                    .HasForeignKey(nameof(ProductFavorites.StoreId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── ProductReactions ──────────────────────────────────────────────────
        private static void ApplyReactions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductReactions>(b =>
            {
                b.Property(x => x.ReactionType).HasConversion<byte>();

                b.HasIndex(x => new { x.UserId, x.ProductId })
                    .IsUnique()
                    .HasDatabaseName("UX_ProductReactions_UserProduct")
                    .HasFilter(SoftDeleteFilter);

                b.HasIndex(x => new { x.ProductId, x.ReactionType });
                b.HasIndex(x => new { x.UserId, x.CreatedAtUtc });

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ProductReactions.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(ProductReactions.ProductId))
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        // ── ProductShares ─────────────────────────────────────────────────────
        private static void ApplyShares(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductShares>(b =>
            {
                b.Property(x => x.Channel).HasConversion<byte>();

                // Kanal kırılımlı dönüşüm raporu.
                b.HasIndex(x => new { x.ProductId, x.Channel, x.CreatedAtUtc });
                b.HasIndex(x => new { x.UserId, x.CreatedAtUtc });

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ProductShares.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(ProductShares.ProductId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(ChatConversations)).WithMany()
                    .HasForeignKey(nameof(ProductShares.TargetConversationId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Posts)).WithMany()
                    .HasForeignKey(nameof(ProductShares.CreatedPostId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── Wishlists / WishlistItems ─────────────────────────────────────────
        private static void ApplyWishlists(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Wishlists>(b =>
            {
                b.Property(x => x.Name).HasMaxLength(128).IsRequired();
                b.Property(x => x.Description).HasMaxLength(2000);
                b.Property(x => x.ShareToken).HasMaxLength(64);

                b.Property(x => x.Purpose).HasConversion<byte>();
                b.Property(x => x.Visibility).HasConversion<byte>();

                // Kullanıcının varsayılan listesi tektir.
                b.HasIndex(x => x.UserId)
                    .IsUnique()
                    .HasDatabaseName("UX_Wishlists_DefaultPerUser")
                    .HasFilter("[IsDefault] = 1 AND [IsDeleted_IsDeletedStatu] = 0");

                // Paylaşım bağlantısı çözümlemesi.
                b.HasIndex(x => x.ShareToken)
                    .IsUnique()
                    .HasDatabaseName("UX_Wishlists_ShareToken")
                    .HasFilter("[ShareToken] IS NOT NULL AND [IsDeleted_IsDeletedStatu] = 0");

                b.HasIndex(x => new { x.UserId, x.CreatedAtUtc });
                b.HasIndex(x => new { x.Visibility, x.EventDate });

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(Wishlists.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Media)).WithMany()
                    .HasForeignKey(nameof(Wishlists.CoverMediaId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<WishlistItems>(b =>
            {
                b.Property(x => x.Note).HasMaxLength(1000);
                b.Property(x => x.PriceAtAddTime).HasPrecision(18, 4);
                b.Property(x => x.PriceCurrency).HasConversion<byte>();

                // Aynı ürün/varyant bir listeye iki kez eklenemez.
                b.HasIndex(x => new { x.WishlistId, x.ProductId, x.ProductVariantId })
                    .IsUnique()
                    .HasDatabaseName("UX_WishlistItems_ListProductVariant")
                    .HasFilter(SoftDeleteFilter);

                b.HasIndex(x => new { x.WishlistId, x.DisplayOrder });
                b.HasIndex(x => new { x.ProductId, x.CreatedAtUtc });
                b.HasIndex(x => x.IsPurchased);

                b.HasOne(typeof(Wishlists)).WithMany()
                    .HasForeignKey(nameof(WishlistItems.WishlistId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(WishlistItems.ProductId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(ProductVariants)).WithMany()
                    .HasForeignKey(nameof(WishlistItems.ProductVariantId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(WishlistItems.PurchasedByUserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── ProductReviewMedia ────────────────────────────────────────────────
        private static void ApplyReviewMedia(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductReviewMedia>(b =>
            {
                b.Property(x => x.AltText).HasMaxLength(1000);
                b.Property(x => x.MediaModerationStatus).HasConversion<byte>();

                b.HasIndex(x => new { x.ProductReviewId, x.DisplayOrder });
                b.HasIndex(x => x.MediaId);
                b.HasIndex(x => x.MediaModerationStatus);

                b.HasIndex(x => new { x.ProductReviewId, x.MediaId })
                    .IsUnique()
                    .HasDatabaseName("UX_ProductReviewMedia_Pair")
                    .HasFilter(SoftDeleteFilter);

                b.HasOne(typeof(ProductReviews)).WithMany()
                    .HasForeignKey(nameof(ProductReviewMedia.ProductReviewId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Media)).WithMany()
                    .HasForeignKey(nameof(ProductReviewMedia.MediaId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── ProductQuestionMedia ──────────────────────────────────────────────
        private static void ApplyQuestionMedia(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductQuestionMedia>(b =>
            {
                b.Property(x => x.AltText).HasMaxLength(1000);
                b.Property(x => x.MediaModerationStatus).HasConversion<byte>();

                b.HasIndex(x => new { x.ProductQuestionId, x.DisplayOrder });
                b.HasIndex(x => x.MediaId);

                b.HasIndex(x => new { x.ProductQuestionId, x.MediaId })
                    .IsUnique()
                    .HasDatabaseName("UX_ProductQuestionMedia_Pair")
                    .HasFilter(SoftDeleteFilter);

                b.HasOne(typeof(ProductQuestions)).WithMany()
                    .HasForeignKey(nameof(ProductQuestionMedia.ProductQuestionId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Media)).WithMany()
                    .HasForeignKey(nameof(ProductQuestionMedia.MediaId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
