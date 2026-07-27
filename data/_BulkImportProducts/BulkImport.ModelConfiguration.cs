using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using data.Owned;        // ortak soft-delete owned tipi (data.Owned.IsDeleted)
using data._Attribute;   // IntegrationPlatform, AttributeDefinition
using data._Categories;  // CategoriesProduct
using data._Galleries;   // Media
using data._Products;    // Products, ProductVariants, Brands
using data._Store;       // Store
using data._Users;       // Users

namespace data._BulkImportProducts
{
    /// <summary>
    /// Toplu Ürün İçe Aktarım Sistemi için EF Core Fluent yapılandırması.
    /// _ProductModelConfiguration desenini birebir izler.
    ///
    /// _ApplicationConnectionDb'ye eklenecekler:
    ///
    ///     // === _BulkImportProducts (Toplu Ürün İçe Aktarım) ===
    ///     public DbSet&lt;ImportProfile&gt; ImportProfiles => Set&lt;ImportProfile&gt;();
    ///     public DbSet&lt;ImportCredential&gt; ImportCredentials => Set&lt;ImportCredential&gt;();
    ///     public DbSet&lt;ImportFieldMapping&gt; ImportFieldMappings => Set&lt;ImportFieldMapping&gt;();
    ///     public DbSet&lt;ImportCategoryMapping&gt; ImportCategoryMappings => Set&lt;ImportCategoryMapping&gt;();
    ///     public DbSet&lt;ImportJob&gt; ImportJobs => Set&lt;ImportJob&gt;();
    ///     public DbSet&lt;ImportRow&gt; ImportRows => Set&lt;ImportRow&gt;();
    ///     public DbSet&lt;ImportRowLog&gt; ImportRowLogs => Set&lt;ImportRowLog&gt;();
    ///
    ///     protected override void OnModelCreating(ModelBuilder modelBuilder)
    ///     {
    ///         base.OnModelCreating(modelBuilder);
    ///         _AttributeModelConfiguration.Apply(modelBuilder);
    ///         _ProductModelConfiguration.Apply(modelBuilder);
    ///         _ProductReviewModelConfiguration.Apply(modelBuilder);
    ///         _BulkImportModelConfiguration.Apply(modelBuilder);   // ← bu satır
    ///     }
    ///
    /// Konvansiyon: navigation property YOK; ilişkiler yalnızca FK üzerinden kurulur.
    /// Çoklu-cascade-path riski taşıyan FK'ler NoAction'dır.
    /// </summary>
    public static class _BulkImportModelConfiguration
    {
        private const string SoftDeleteFilter = "[IsDeleted_IsDeletedStatu] = 0";

        /// <summary>Tüm içe aktarım sistemi yapılandırmasını uygular.</summary>
        public static void Apply(ModelBuilder modelBuilder)
        {
            ApplyProfiles(modelBuilder);
            ApplyCredentials(modelBuilder);
            ApplyMappings(modelBuilder);
            ApplyJobs(modelBuilder);
            ApplyRows(modelBuilder);

            ApplyBaseConventions(modelBuilder);
        }

        // ── Ortak konvansiyonlar: owned soft-delete + rowversion ──────────────
        private static void ApplyBaseConventions(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes().ToList())
            {
                var clr = entityType.ClrType;
                if (!typeof(BulkImportEntityBase).IsAssignableFrom(clr))
                    continue;

                var b = modelBuilder.Entity(clr);
                b.OwnsOne(typeof(IsDeleted), nameof(BulkImportEntityBase.IsDeleted));
                b.Property(nameof(BulkImportEntityBase.RowVersion)).IsRowVersion();
            }
        }

        // ── ImportProfile ─────────────────────────────────────────────────────
        private static void ApplyProfiles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ImportProfile>(b =>
            {
                b.Property(x => x.Name).HasMaxLength(256).IsRequired();
                b.Property(x => x.Description).HasMaxLength(2048);
                b.Property(x => x.SourceEndpointUrl).HasMaxLength(2048);
                b.Property(x => x.SourceLanguageCode).HasMaxLength(8);
                b.Property(x => x.SourceCurrencyCode).HasMaxLength(8);
                b.Property(x => x.CsvDelimiter).HasMaxLength(8);
                b.Property(x => x.Encoding).HasMaxLength(32);
                b.Property(x => x.RecordsRootPath).HasMaxLength(512);
                b.Property(x => x.PriceMultiplier).HasPrecision(18, 6);

                // Kullanıcının profillerini hızlı listeleme
                b.HasIndex(x => new { x.UserId, x.IsActive });
                b.HasIndex(x => x.IntegrationPlatformId);

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ImportProfile.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Store)).WithMany()
                    .HasForeignKey(nameof(ImportProfile.StoreId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(IntegrationPlatform)).WithMany()
                    .HasForeignKey(nameof(ImportProfile.IntegrationPlatformId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(ImportCredential)).WithMany()
                    .HasForeignKey(nameof(ImportProfile.ImportCredentialId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── ImportCredential (şifreli kimlik) ────────────────────────────────
        private static void ApplyCredentials(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ImportCredential>(b =>
            {
                b.Property(x => x.Alias).HasMaxLength(256).IsRequired();
                b.Property(x => x.EncryptedApiKey).HasMaxLength(4000);
                b.Property(x => x.EncryptedApiSecret).HasMaxLength(4000);
                b.Property(x => x.EncryptedUserName).HasMaxLength(1024);
                b.Property(x => x.EncryptedPassword).HasMaxLength(1024);
                b.Property(x => x.EncryptedAccessToken).HasMaxLength(4000);
                b.Property(x => x.EncryptedRefreshToken).HasMaxLength(4000);
                b.Property(x => x.MarketplaceRegion).HasMaxLength(64);
                b.Property(x => x.ExternalSellerId).HasMaxLength(256);
                b.Property(x => x.LastValidationError).HasMaxLength(2048);
                // ExtraConfigJson → nvarchar(max)

                b.HasIndex(x => new { x.UserId, x.IntegrationPlatformId });

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ImportCredential.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(IntegrationPlatform)).WithMany()
                    .HasForeignKey(nameof(ImportCredential.IntegrationPlatformId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── ImportFieldMapping + ImportCategoryMapping ───────────────────────
        private static void ApplyMappings(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ImportFieldMapping>(b =>
            {
                b.Property(x => x.SourceField).HasMaxLength(512).IsRequired();
                b.Property(x => x.TargetLanguageCode).HasMaxLength(8);
                b.Property(x => x.TargetCurrencyCode).HasMaxLength(8);
                b.Property(x => x.DefaultValue).HasMaxLength(2048);
                // TransformConfigJson → nvarchar(max)

                b.HasIndex(x => x.ImportProfileId);

                b.HasOne(typeof(ImportProfile)).WithMany()
                    .HasForeignKey(nameof(ImportFieldMapping.ImportProfileId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(AttributeDefinition)).WithMany()
                    .HasForeignKey(nameof(ImportFieldMapping.TargetAttributeDefinitionId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ImportCategoryMapping>(b =>
            {
                b.Property(x => x.SourceCategoryCode).HasMaxLength(256);
                b.Property(x => x.SourceCategoryPath).HasMaxLength(2048).IsRequired();
                b.Property(x => x.NormalizedSourcePath).HasMaxLength(2048).IsRequired();
                b.Property(x => x.Confidence).HasPrecision(5, 4);

                // Kaynak yol → hedef kategori hızlı arama (kullanıcı + platform kapsamında)
                b.HasIndex(x => new { x.UserId, x.IntegrationPlatformId, x.NormalizedSourcePath });
                b.HasIndex(x => x.ImportProfileId);

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ImportCategoryMapping.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(IntegrationPlatform)).WithMany()
                    .HasForeignKey(nameof(ImportCategoryMapping.IntegrationPlatformId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(ImportProfile)).WithMany()
                    .HasForeignKey(nameof(ImportCategoryMapping.ImportProfileId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(CategoriesProduct)).WithMany()
                    .HasForeignKey(nameof(ImportCategoryMapping.TargetCategoryId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Brands)).WithMany()
                    .HasForeignKey(nameof(ImportCategoryMapping.DefaultBrandId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── ImportJob ─────────────────────────────────────────────────────────
        private static void ApplyJobs(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ImportJob>(b =>
            {
                b.Property(x => x.OriginalFileName).HasMaxLength(512);
                b.Property(x => x.SourceUrl).HasMaxLength(2048);
                b.Property(x => x.IdempotencyKey).HasMaxLength(256);
                b.Property(x => x.LeasedBy).HasMaxLength(256);
                b.Property(x => x.ResultSummary).HasMaxLength(2048);
                b.Property(x => x.ErrorMessage).HasMaxLength(4000);
                // ErrorDetailsJson → nvarchar(max)

                // Kullanıcının işlerini tarihe göre listeleme + kuyruk taraması
                b.HasIndex(x => new { x.UserId, x.Status });
                b.HasIndex(x => new { x.Status, x.NextRetryAtUtc });
                b.HasIndex(x => x.ImportProfileId);
                // Aynı çalıştırma ikinci kez kuyruklanmasın (global tekil, soft-delete hariç)
                b.HasIndex(x => x.IdempotencyKey).IsUnique().HasFilter("[IdempotencyKey] IS NOT NULL AND " + SoftDeleteFilter);

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ImportJob.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(ImportProfile)).WithMany()
                    .HasForeignKey(nameof(ImportJob.ImportProfileId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Store)).WithMany()
                    .HasForeignKey(nameof(ImportJob.StoreId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(IntegrationPlatform)).WithMany()
                    .HasForeignKey(nameof(ImportJob.IntegrationPlatformId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Media)).WithMany()
                    .HasForeignKey(nameof(ImportJob.SourceMediaId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── ImportRow + ImportRowLog ─────────────────────────────────────────
        private static void ApplyRows(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ImportRow>(b =>
            {
                b.Property(x => x.ExternalProductId).HasMaxLength(256);
                b.Property(x => x.SourceSku).HasMaxLength(256);
                b.Property(x => x.SourceGtin).HasMaxLength(64);
                b.Property(x => x.SourceName).HasMaxLength(1024);
                b.Property(x => x.SourceKeyHash).HasMaxLength(128);
                b.Property(x => x.ErrorMessage).HasMaxLength(4000);
                // RawRowJson / MappedDataJson / WarningsJson → nvarchar(max)

                // İş içindeki satırları durum/sıraya göre işleme
                b.HasIndex(x => new { x.ImportJobId, x.Status });
                b.HasIndex(x => new { x.ImportJobId, x.SourceRowIndex });
                // Satır tekilleştirme: aynı işte aynı ürün iki kez işlenmesin
                b.HasIndex(x => new { x.ImportJobId, x.SourceKeyHash })
                    .IsUnique().HasFilter("[SourceKeyHash] IS NOT NULL AND " + SoftDeleteFilter);
                b.HasIndex(x => x.CreatedProductId);

                b.HasOne(typeof(ImportJob)).WithMany()
                    .HasForeignKey(nameof(ImportRow.ImportJobId))
                    .OnDelete(DeleteBehavior.Cascade);
                // Üretilen ürüne referanslar → NoAction (ürün silme, staging'i cascade silmez).
                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(ImportRow.CreatedProductId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(ProductVariants)).WithMany()
                    .HasForeignKey(nameof(ImportRow.CreatedVariantId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(ImportRow.MatchedExistingProductId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ImportRowLog>(b =>
            {
                b.Property(x => x.Step).HasMaxLength(128).IsRequired();
                b.Property(x => x.Level).HasMaxLength(32).IsRequired();
                b.Property(x => x.Message).HasMaxLength(2048).IsRequired();
                // DetailJson → nvarchar(max)

                b.HasIndex(x => x.ImportRowId);
                b.HasIndex(x => x.ImportJobId);

                b.HasOne(typeof(ImportRow)).WithMany()
                    .HasForeignKey(nameof(ImportRowLog.ImportRowId))
                    .OnDelete(DeleteBehavior.Cascade);
                // İş referansı denormalize → çoklu cascade yolunu önlemek için NoAction.
                b.HasOne(typeof(ImportJob)).WithMany()
                    .HasForeignKey(nameof(ImportRowLog.ImportJobId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
