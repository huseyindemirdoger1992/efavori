using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using data.Owned;      // ortak soft-delete owned tipi (data.Owned.IsDeleted)
using data._Users;     // Users
using data._Categories; // (dolaylı referans yok; tutarlılık için)

namespace data._Products
{
    /// <summary>
    /// Ürün Yorum/Puanlama &amp; Soru-Cevap sistemi için EF Core Fluent yapılandırması.
    /// _ProductModelConfiguration desenini birebir izler.
    ///
    /// _ApplicationConnectionDb'ye eklenecekler:
    ///
    ///     // === _Products (Yorum / Puanlama) ===
    ///     public DbSet&lt;ProductReviews&gt; ProductReviews => Set&lt;ProductReviews&gt;();
    ///     public DbSet&lt;ProductReviewVotes&gt; ProductReviewVotes => Set&lt;ProductReviewVotes&gt;();
    ///     public DbSet&lt;ProductReviewReports&gt; ProductReviewReports => Set&lt;ProductReviewReports&gt;();
    ///     public DbSet&lt;ProductRatingSummary&gt; ProductRatingSummary => Set&lt;ProductRatingSummary&gt;();
    ///
    ///     // === _Products (Soru-Cevap) ===
    ///     public DbSet&lt;ProductQuestions&gt; ProductQuestions => Set&lt;ProductQuestions&gt;();
    ///     public DbSet&lt;ProductQuestionVotes&gt; ProductQuestionVotes => Set&lt;ProductQuestionVotes&gt;();
    ///     public DbSet&lt;ProductQuestionReports&gt; ProductQuestionReports => Set&lt;ProductQuestionReports&gt;();
    ///
    ///     protected override void OnModelCreating(ModelBuilder modelBuilder)
    ///     {
    ///         base.OnModelCreating(modelBuilder);
    ///         _AttributeModelConfiguration.Apply(modelBuilder);
    ///         _ProductModelConfiguration.Apply(modelBuilder);
    ///         _ProductReviewModelConfiguration.Apply(modelBuilder);   // ← bu satır
    ///     }
    ///
    /// Konvansiyon: navigation property YOK; ilişkiler yalnızca FK üzerinden kurulur.
    /// Ağaç (self-reference) ve çoklu-cascade-path riski olan tüm FK'ler NoAction'dır.
    /// </summary>
    public static class _ProductReviewModelConfiguration
    {
        private const string SoftDeleteFilter = "[IsDeleted_IsDeletedStatu] = 0";

        /// <summary>Tüm yorum/puanlama/Q&amp;A yapılandırmasını uygular.</summary>
        public static void Apply(ModelBuilder modelBuilder)
        {
            ApplyReviews(modelBuilder);
            ApplyRatingSummary(modelBuilder);
            ApplyQuestions(modelBuilder);

            ApplyBaseConventions(modelBuilder);
        }

        // ── Ortak konvansiyonlar: owned soft-delete + rowversion ──────────────
        private static void ApplyBaseConventions(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes().ToList())
            {
                var clr = entityType.ClrType;
                if (!typeof(ProductEntityBase).IsAssignableFrom(clr))
                    continue;

                // Yalnızca bu dosyada tanımlanan entity'lere dokun (diğerleri kendi
                // konfigürasyonlarında zaten işlenmiş olabilir; OwnsOne idempotent değildir).
                if (clr != typeof(ProductReviews) &&
                    clr != typeof(ProductReviewVotes) &&
                    clr != typeof(ProductReviewReports) &&
                    clr != typeof(ProductRatingSummary) &&
                    clr != typeof(ProductQuestions) &&
                    clr != typeof(ProductQuestionVotes) &&
                    clr != typeof(ProductQuestionReports))
                    continue;

                var b = modelBuilder.Entity(clr);
                b.OwnsOne(typeof(IsDeleted), nameof(ProductEntityBase.IsDeleted));
                b.Property(nameof(ProductEntityBase.RowVersion)).IsRowVersion();
            }
        }

        // ── ProductReviews + oy/şikâyet ──────────────────────────────────────
        private static void ApplyReviews(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductReviews>(b =>
            {
                b.Property(x => x.Title).HasMaxLength(512);
                b.Property(x => x.Body).HasMaxLength(8000).IsRequired();
                b.Property(x => x.ModerationNote).HasMaxLength(2048);
                // MediaIdsJson → nvarchar(max) (varsayılan)

                // Ürün sayfası yorum listeleme + moderasyon kuyruğu ana indeksleri
                b.HasIndex(x => new { x.ProductId, x.Status, x.ParentReviewId });
                b.HasIndex(x => x.UserId);
                b.HasIndex(x => x.ParentReviewId);
                b.HasIndex(x => x.Status);
                // "Bir kullanıcı bir ürüne bir kök yorum" kuralı uygulama katmanında
                // doğrulanır (yanıtlar hariç tutulacağı için filtreli tekil indeks
                // yerine bilinçli olarak uygulama kontrolü tercih edildi).

                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(ProductReviews.ProductId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ProductReviews.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
                // Varyant referansı opsiyonel; çoklu cascade yolunu önlemek için NoAction.
                b.HasOne(typeof(ProductVariants)).WithMany()
                    .HasForeignKey(nameof(ProductReviews.ProductVariantId))
                    .OnDelete(DeleteBehavior.NoAction);
                // Ağaç kendine referans → döngü/çoklu-yol hatasını önlemek için NoAction.
                b.HasOne(typeof(ProductReviews)).WithMany()
                    .HasForeignKey(nameof(ProductReviews.ParentReviewId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ProductReviewVotes>(b =>
            {
                // Bir kullanıcı bir yoruma tek oy verir.
                b.HasIndex(x => new { x.ProductReviewId, x.UserId }).IsUnique().HasFilter(SoftDeleteFilter);

                b.HasOne(typeof(ProductReviews)).WithMany()
                    .HasForeignKey(nameof(ProductReviewVotes.ProductReviewId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ProductReviewVotes.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ProductReviewReports>(b =>
            {
                b.Property(x => x.Description).HasMaxLength(2048);

                // Bir kullanıcı bir yorumu bir kez şikâyet eder.
                b.HasIndex(x => new { x.ProductReviewId, x.ReportedByUserId }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => x.Status); // moderasyon kuyruğu

                b.HasOne(typeof(ProductReviews)).WithMany()
                    .HasForeignKey(nameof(ProductReviewReports.ProductReviewId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ProductReviewReports.ReportedByUserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── ProductRatingSummary (türetilmiş özet) ───────────────────────────
        private static void ApplyRatingSummary(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductRatingSummary>(b =>
            {
                b.Property(x => x.AverageRating).HasPrecision(3, 2);
                b.Property(x => x.AverageQualityRating).HasPrecision(3, 2);
                b.Property(x => x.AverageShippingRating).HasPrecision(3, 2);
                b.Property(x => x.AverageValueRating).HasPrecision(3, 2);

                // Ürün başına tek özet satırı.
                b.HasIndex(x => x.ProductId).IsUnique().HasFilter(SoftDeleteFilter);

                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(ProductRatingSummary.ProductId))
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        // ── ProductQuestions + oy/şikâyet ────────────────────────────────────
        private static void ApplyQuestions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductQuestions>(b =>
            {
                b.Property(x => x.Body).HasMaxLength(8000).IsRequired();
                b.Property(x => x.ModerationNote).HasMaxLength(2048);

                b.HasIndex(x => new { x.ProductId, x.Status, x.ParentQuestionId });
                b.HasIndex(x => x.UserId);
                b.HasIndex(x => x.ParentQuestionId);
                b.HasIndex(x => x.Status);

                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(ProductQuestions.ProductId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ProductQuestions.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
                // Ağaç kendine referans → NoAction.
                b.HasOne(typeof(ProductQuestions)).WithMany()
                    .HasForeignKey(nameof(ProductQuestions.ParentQuestionId))
                    .OnDelete(DeleteBehavior.NoAction);
                // AcceptedAnswerId da aynı tabloya işaret eder → NoAction.
                b.HasOne(typeof(ProductQuestions)).WithMany()
                    .HasForeignKey(nameof(ProductQuestions.AcceptedAnswerId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ProductQuestionVotes>(b =>
            {
                b.HasIndex(x => new { x.ProductQuestionId, x.UserId }).IsUnique().HasFilter(SoftDeleteFilter);

                b.HasOne(typeof(ProductQuestions)).WithMany()
                    .HasForeignKey(nameof(ProductQuestionVotes.ProductQuestionId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ProductQuestionVotes.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ProductQuestionReports>(b =>
            {
                b.Property(x => x.Description).HasMaxLength(2048);

                b.HasIndex(x => new { x.ProductQuestionId, x.ReportedByUserId }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => x.Status);

                b.HasOne(typeof(ProductQuestions)).WithMany()
                    .HasForeignKey(nameof(ProductQuestionReports.ProductQuestionId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(ProductQuestionReports.ReportedByUserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
