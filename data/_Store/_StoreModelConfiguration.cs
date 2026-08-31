using System;
using Microsoft.EntityFrameworkCore;
using data._Categories;
using data._Galleries;
using data._Products;
using data._Promotions;
using data._Shares;
using data._Users;
using data.Owned;

namespace data._Store
{
    /// <summary>
    /// Mağaza modülünün EF Core Fluent yapılandırması.
    ///
    /// _ApplicationConnectionDb.OnModelCreating içinde şu satırla uygulanır:
    ///     _StoreModelConfiguration.Apply(modelBuilder);
    ///
    /// SİLME DAVRANIŞI (§43): Mağaza → ürün, sipariş, hakediş gibi TİCARİ kayıtlar
    /// ASLA cascade DEĞİLDİR. Mağaza kapansa bile geçmiş siparişler, faturalar ve
    /// hakediş kayıtları korunmalıdır. Yalnızca mağazanın kendi yardımcı kayıtları
    /// (belge, öne çıkarılan öğe, engelleme notu, entegrasyon) cascade'dir.
    /// </summary>
    public static class _StoreModelConfiguration
    {
        private const string SoftDeleteFilter = "[IsDeleted_IsDeletedStatu] = 0";

        /// <summary>Tüm mağaza yapılandırmasını uygular.</summary>
        public static void Apply(ModelBuilder modelBuilder)
        {
            ApplyBaseConventions(modelBuilder);

            ApplyStore(modelBuilder);
            ApplyStoreDocuments(modelBuilder);
            ApplyStoreFeaturedItems(modelBuilder);
            ApplyStoreBlockingInfos(modelBuilder);
            ApplyStoreIntegration(modelBuilder);
            ApplyWareHouse(modelBuilder);
        }

        private static void ApplyBaseConventions(ModelBuilder modelBuilder)
        {
            ConfigureBase<Store>(modelBuilder);
            ConfigureBase<StoreDocuments>(modelBuilder);
            ConfigureBase<StoreFeaturedItems>(modelBuilder);
            ConfigureBase<StoreBlockingInfos>(modelBuilder);
            ConfigureBase<StoreIntegration>(modelBuilder);
        }

        private static void ConfigureBase<TEntity>(ModelBuilder modelBuilder)
            where TEntity : StoreEntityBase
        {
            modelBuilder.Entity<TEntity>(b =>
            {
                b.HasKey(x => x.Id);
                b.OwnsOne(x => x.IsDeleted);
                b.Property(x => x.RowVersion).IsRowVersion();
            });
        }

        // ── Store ─────────────────────────────────────────────────────────────
        private static void ApplyStore(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Store>(b =>
            {
                b.Property(x => x.Name).HasMaxLength(256).IsRequired();
                b.Property(x => x.Slug).HasMaxLength(128).IsRequired();
                b.Property(x => x.NormalizedSlug).HasMaxLength(128).IsRequired();
                b.Property(x => x.LegalName).HasMaxLength(512);
                b.Property(x => x.TaxNumber).HasMaxLength(32);
                b.Property(x => x.TaxOffice).HasMaxLength(128);
                b.Property(x => x.MersisNumber).HasMaxLength(32);
                b.Property(x => x.Bio).HasMaxLength(500);
                b.Property(x => x.Description).HasMaxLength(8000);
                b.Property(x => x.Website).HasMaxLength(512);
                b.Property(x => x.StatusReason).HasMaxLength(2000);
                b.Property(x => x.TemporaryClosureMessage).HasMaxLength(512);

                b.Property(x => x.Status).HasConversion<byte>();
                b.Property(x => x.VerificationStatus).HasConversion<byte>();
                b.Property(x => x.AverageRating).HasPrecision(3, 2);

                b.OwnsOne(x => x.ContactInformation);
                b.OwnsOne(x => x.AddressInfo);
                b.OwnsOne(x => x.WorkingHours);
                b.OwnsOne(x => x.Meta);

                // Mağaza adresi (handle) GLOBAL TEKİLDİR.
                b.HasIndex(x => x.NormalizedSlug)
                    .IsUnique()
                    .HasDatabaseName("UX_Store_NormalizedSlug")
                    .HasFilter(SoftDeleteFilter);

                // §58 KARARI: OwnerUserId üzerinde TEKİL indeks YOKTUR —
                // bir kullanıcı birden çok mağaza açabilir.
                b.HasIndex(x => new { x.OwnerUserId, x.Status });

                // Vitrin listeleme ve yönetim kuyruğu.
                b.HasIndex(x => new { x.Status, x.CreatedAtUtc });
                // Onay bekleyen başvurular.
                b.HasIndex(x => new { x.Status, x.AreDocumentsApproved });
                // "Doğrulanmış mağazalar" ve popülerlik sıralaması.
                b.HasIndex(x => new { x.VerificationStatus, x.FollowersCount });
                b.HasIndex(x => x.AverageRating);
                // Tatil modu biten mağazaları açan arka plan işi.
                b.HasIndex(x => x.TemporarilyClosedUntilUtc);
                b.HasIndex(x => x.Name);

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(Store.OwnerUserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(Store.StatusChangedByUserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Media)).WithMany()
                    .HasForeignKey(nameof(Store.AvatarMediaId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Media)).WithMany()
                    .HasForeignKey(nameof(Store.CoverMediaId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── StoreDocuments ────────────────────────────────────────────────────
        private static void ApplyStoreDocuments(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreDocuments>(b =>
            {
                b.Property(x => x.DocumentNumber).HasMaxLength(128);
                b.Property(x => x.Description).HasMaxLength(1000);
                b.Property(x => x.RejectionReason).HasMaxLength(2000);

                b.Property(x => x.DocumentType).HasConversion<byte>();
                b.Property(x => x.Status).HasConversion<byte>();

                // Bir mağazanın bir belge türünde tek GÜNCEL sürümü olur;
                // yenilemeler eski satırı IsCurrent = false yaparak tarihçeyi korur.
                b.HasIndex(x => new { x.StoreId, x.DocumentType })
                    .IsUnique()
                    .HasDatabaseName("UX_StoreDocuments_CurrentPerType")
                    .HasFilter("[IsCurrent] = 1 AND [IsDeleted_IsDeletedStatu] = 0");

                // Belge inceleme kuyruğu.
                b.HasIndex(x => new { x.Status, x.CreatedAtUtc });
                b.HasIndex(x => new { x.StoreId, x.Status });
                // Süresi dolmak üzere olan belgeler için uyarı işi.
                b.HasIndex(x => x.ExpiresOn);
                b.HasIndex(x => x.MediaId);

                b.HasOne(typeof(Store)).WithMany()
                    .HasForeignKey(nameof(StoreDocuments.StoreId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Media)).WithMany()
                    .HasForeignKey(nameof(StoreDocuments.MediaId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(StoreDocuments.ReviewedByUserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── StoreFeaturedItems ────────────────────────────────────────────────
        private static void ApplyStoreFeaturedItems(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreFeaturedItems>(b =>
            {
                b.ToTable("StoreFeaturedItems", t =>
                {
                    // Tam olarak BİR hedef kolonu dolu olmalı ve ItemType ile uyuşmalı.
                    t.HasCheckConstraint(
                        "CK_StoreFeaturedItems_TargetMatchesType",
                        "((CASE WHEN [ProductId]         IS NULL THEN 0 ELSE 1 END) + " +
                        " (CASE WHEN [PostId]            IS NULL THEN 0 ELSE 1 END) + " +
                        " (CASE WHEN [CampaignId]        IS NULL THEN 0 ELSE 1 END) + " +
                        " (CASE WHEN [ProductCategoryId] IS NULL THEN 0 ELSE 1 END) + " +
                        " (CASE WHEN [ArticleId]         IS NULL THEN 0 ELSE 1 END)) = 1 " +
                        "AND (" +
                        "  ([ItemType] = 1 AND [ProductId]         IS NOT NULL) OR " +
                        "  ([ItemType] = 2 AND [PostId]            IS NOT NULL) OR " +
                        "  ([ItemType] = 3 AND [CampaignId]        IS NOT NULL) OR " +
                        "  ([ItemType] = 4 AND [ProductCategoryId] IS NOT NULL) OR " +
                        "  ([ItemType] = 5 AND [ArticleId]         IS NOT NULL))");
                });

                b.Property(x => x.CustomTitle).HasMaxLength(256);
                b.Property(x => x.ItemType).HasConversion<byte>();

                // Mağaza vitrini sıralaması.
                b.HasIndex(x => new { x.StoreId, x.ItemType, x.DisplayOrder });
                // Süresi dolan öne çıkarmaları kaldıran arka plan işi.
                b.HasIndex(x => x.ExpiresAtUtc);

                b.HasOne(typeof(Store)).WithMany()
                    .HasForeignKey(nameof(StoreFeaturedItems.StoreId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(StoreFeaturedItems.ProductId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Posts)).WithMany()
                    .HasForeignKey(nameof(StoreFeaturedItems.PostId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Campaigns)).WithMany()
                    .HasForeignKey(nameof(StoreFeaturedItems.CampaignId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(CategoriesProduct)).WithMany()
                    .HasForeignKey(nameof(StoreFeaturedItems.ProductCategoryId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Articles)).WithMany()
                    .HasForeignKey(nameof(StoreFeaturedItems.ArticleId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── StoreBlockingInfos ────────────────────────────────────────────────
        private static void ApplyStoreBlockingInfos(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreBlockingInfos>(b =>
            {
                b.Property(x => x.BlockInfoDescription).HasMaxLength(4000).IsRequired();
                b.Property(x => x.RequiredAction).HasMaxLength(2000);

                b.HasIndex(x => new { x.StoreId, x.OccurredAtUtc });
                b.HasIndex(x => new { x.IsResolved, x.OccurredAtUtc });
                b.HasIndex(x => x.EffectiveUntilUtc);

                b.HasOne(typeof(Store)).WithMany()
                    .HasForeignKey(nameof(StoreBlockingInfos.StoreId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(StoreBlockingInfos.OwnerUserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(StoreBlockingInfos.AdminUserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(StoreBlockingInfos.ResolvedByUserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── StoreIntegration ──────────────────────────────────────────────────
        private static void ApplyStoreIntegration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreIntegration>(b =>
            {
                b.Property(x => x.Provider).HasMaxLength(128).IsRequired();
                b.Property(x => x.IntegrationType).HasMaxLength(64);
                b.Property(x => x.CompanyCode).HasMaxLength(128);
                b.Property(x => x.BaseUrl).HasMaxLength(512);
                b.Property(x => x.EncryptionKeyVersion).HasMaxLength(64);
                b.Property(x => x.LastErrorMessage).HasMaxLength(4000);

                // Bir mağazada bir sağlayıcının tek varsayılan yapılandırması olur.
                b.HasIndex(x => new { x.StoreId, x.Provider })
                    .IsUnique()
                    .HasDatabaseName("UX_StoreIntegration_DefaultPerProvider")
                    .HasFilter("[IsDefault] = 1 AND [IsDeleted_IsDeletedStatu] = 0");

                b.HasIndex(x => new { x.StoreId, x.IsActive });
                // Bağlantısı bozulan entegrasyonları izleyen panel.
                b.HasIndex(x => x.ConsecutiveFailureCount);
                b.HasIndex(x => x.TokenExpiresAtUtc);

                b.HasOne(typeof(Store)).WithMany()
                    .HasForeignKey(nameof(StoreIntegration.StoreId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(StoreIntegration.OwnerUserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── WareHouse (mevcut entity korunmuştur — yalnızca yapılandırma) ─────
        private static void ApplyWareHouse(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WareHouse>(b =>
            {
                b.HasKey(x => x.Id);
                b.OwnsOne(x => x.IsDeleted);

                b.Property(x => x.Name).HasMaxLength(256);
                b.Property(x => x.WarehouseCode).HasMaxLength(64);
                b.Property(x => x.Description).HasMaxLength(2000);

                // Mağaza içinde depo kodu tekildir (stok transferlerinde referans alınır).
                b.HasIndex(x => new { x.StoreId, x.WarehouseCode })
                    .IsUnique()
                    .HasDatabaseName("UX_WareHouse_StoreCode")
                    .HasFilter("[WarehouseCode] IS NOT NULL AND [IsDeleted_IsDeletedStatu] = 0");

                b.HasIndex(x => x.StoreId);
                b.HasIndex(x => x.UserId);

                b.HasOne(typeof(Store)).WithMany()
                    .HasForeignKey(nameof(WareHouse.StoreId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(WareHouse.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
