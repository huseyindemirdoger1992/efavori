using Microsoft.EntityFrameworkCore;

namespace data._Promotions
{
    /// <summary>
    /// Kampanya ve Kupon Sistemi V1 — tüm FK, indeks, silme davranışı ve decimal precision tanımları.
    /// Mevcut _ProductModelConfiguration / _OrderModelConfiguration deseni ile birebir aynıdır.
    /// Kullanım: OnModelCreating içinde tek satır → _PromotionModelConfiguration.Apply(modelBuilder);
    /// </summary>
    public static class _PromotionModelConfiguration
    {
        /// <summary>
        /// Soft-delete filtreli tekil indeks sabiti.
        /// Owned tip data.Owned.IsDeleted içindeki property adı 'IsDeletedStatu' olduğundan
        /// EF'in ürettiği kolon adı 'IsDeleted_IsDeletedStatu'dur.
        /// (_AttributeModelConfiguration ve _ProductModelConfiguration ile aynı sabit.)
        /// </summary>
        private const string SoftDeleteFilter = "[IsDeleted_IsDeletedStatu] = 0";

        public static void Apply(ModelBuilder modelBuilder)
        {
            // =====================================================================
            // Coupons — kupon tanımları
            // =====================================================================
            modelBuilder.Entity<Coupons>(e =>
            {
                e.ToTable("Coupons", t =>
                {
                    t.HasCheckConstraint("CK_Coupons_ValidRange", "[ValidToUtc] > [ValidFromUtc]");
                    t.HasCheckConstraint("CK_Coupons_UsedCount", "[UsedCount] >= 0");
                });
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.Code).HasMaxLength(50).IsRequired();
                e.Property(x => x.Name).HasMaxLength(200).IsRequired();
                e.Property(x => x.Description).HasMaxLength(1000);

                e.Property(x => x.Value).HasPrecision(18, 2);
                e.Property(x => x.MaxDiscountAmount).HasPrecision(18, 2);
                e.Property(x => x.MinOrderAmount).HasPrecision(18, 2);
                e.Property(x => x.BudgetCap).HasPrecision(18, 2);
                e.Property(x => x.BudgetUsed).HasPrecision(18, 2);
                e.Property(x => x.PlatformSharePercent).HasPrecision(5, 2);

                e.HasOne(typeof(data._Store.Store)).WithMany().HasForeignKey(nameof(Coupons.OwnerStoreId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Users.Users)).WithMany().HasForeignKey(nameof(Coupons.TargetUserId)).OnDelete(DeleteBehavior.Restrict);

                // Kupon kodu GLOBAL tekildir (soft-delete filtreli): silinen kodun tekrar kullanımına izin verir.
                e.HasIndex(x => x.Code).IsUnique().HasFilter(SoftDeleteFilter);
                e.HasIndex(x => new { x.IsActive, x.ValidFromUtc, x.ValidToUtc });               // geçerli kupon taraması
                e.HasIndex(x => new { x.OwnerStoreId, x.IsActive });                             // satıcının kuponları
                e.HasIndex(x => x.TargetUserId);                                                 // kişiye özel kuponlar
            });

            // =====================================================================
            // CouponScopes — kupon kapsam satırları
            // =====================================================================
            modelBuilder.Entity<CouponScopes>(e =>
            {
                e.ToTable("CouponScopes");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.HasOne(typeof(Coupons)).WithMany().HasForeignKey(nameof(CouponScopes.CouponId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Store.Store)).WithMany().HasForeignKey(nameof(CouponScopes.StoreId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Categories.CategoriesProduct)).WithMany().HasForeignKey(nameof(CouponScopes.CategoryId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Products.Products)).WithMany().HasForeignKey(nameof(CouponScopes.ProductId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Products.ProductVariants)).WithMany().HasForeignKey(nameof(CouponScopes.VariantId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Products.Brands)).WithMany().HasForeignKey(nameof(CouponScopes.BrandId)).OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.CouponId, x.ScopeType });                                 // kupon kapsam çözümü
                e.HasIndex(x => new { x.ProductId, x.IsExcluded });                               // ürün → geçerli kuponlar
                e.HasIndex(x => new { x.CategoryId, x.IsExcluded });
            });

            // =====================================================================
            // CouponUsages — append-only kullanım kaydı
            // =====================================================================
            modelBuilder.Entity<CouponUsages>(e =>
            {
                e.ToTable("CouponUsages");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.GuestEmail).HasMaxLength(256);
                e.Property(x => x.DiscountAmount).HasPrecision(18, 2);
                e.Property(x => x.PlatformSharePercentSnapshot).HasPrecision(5, 2);
                e.Property(x => x.IdempotencyKey).HasMaxLength(100).IsRequired();

                e.HasOne(typeof(Coupons)).WithMany().HasForeignKey(nameof(CouponUsages.CouponId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Users.Users)).WithMany().HasForeignKey(nameof(CouponUsages.UserId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Orders.Orders)).WithMany().HasForeignKey(nameof(CouponUsages.OrderId)).OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => x.IdempotencyKey).IsUnique().HasFilter(SoftDeleteFilter);         // aynı siparişte çift sayım engeli
                e.HasIndex(x => new { x.CouponId, x.UserId, x.IsReverted });                      // kullanıcı başına limit kontrolü
                e.HasIndex(x => new { x.CouponId, x.GuestEmail, x.IsReverted });                  // misafir limit kontrolü
                e.HasIndex(x => new { x.CouponId, x.CreatedAtUtc });                              // kupon performans raporu + mutabakat
                e.HasIndex(x => x.OrderId);                                                       // siparişin kupon izi
            });

            // =====================================================================
            // Campaigns — zamanlı indirim kampanyaları
            // =====================================================================
            modelBuilder.Entity<Campaigns>(e =>
            {
                e.ToTable("Campaigns", t =>
                {
                    t.HasCheckConstraint("CK_Campaigns_DateRange", "[EndsAtUtc] > [StartsAtUtc]");
                });
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.Name).HasMaxLength(200).IsRequired();
                e.Property(x => x.Code).HasMaxLength(50).IsRequired();
                e.Property(x => x.Description).HasMaxLength(1000);
                e.Property(x => x.Slug).HasMaxLength(200);
                e.Property(x => x.LeasedBy).HasMaxLength(100);

                e.Property(x => x.DiscountValue).HasPrecision(18, 2);
                e.Property(x => x.MaxDiscountAmount).HasPrecision(18, 2);
                e.Property(x => x.BudgetCap).HasPrecision(18, 2);
                e.Property(x => x.BudgetUsed).HasPrecision(18, 2);

                e.HasOne(typeof(data._Store.Store)).WithMany().HasForeignKey(nameof(Campaigns.OwnerStoreId)).OnDelete(DeleteBehavior.Restrict);
                // BannerMediaItemId: iz alanı — media temizliği kampanyayı kilitlemesin.

                e.HasIndex(x => x.Code).IsUnique().HasFilter(SoftDeleteFilter);
                e.HasIndex(x => x.Slug).IsUnique().HasFilter($"{SoftDeleteFilter} AND [Slug] IS NOT NULL");
                // Motor kuyruğu: başlatılacak/bitirilecek kampanyaların lease ile taranması.
                e.HasIndex(x => new { x.Status, x.StartsAtUtc, x.LeasedUntilUtc });
                e.HasIndex(x => new { x.Status, x.EndsAtUtc, x.LeasedUntilUtc });
                e.HasIndex(x => new { x.OwnerStoreId, x.Status });                                // satıcının kampanyaları
            });

            // =====================================================================
            // CampaignScopes — kampanya kapsam satırları
            // =====================================================================
            modelBuilder.Entity<CampaignScopes>(e =>
            {
                e.ToTable("CampaignScopes");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.OverrideDiscountValue).HasPrecision(18, 2);

                e.HasOne(typeof(Campaigns)).WithMany().HasForeignKey(nameof(CampaignScopes.CampaignId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Store.Store)).WithMany().HasForeignKey(nameof(CampaignScopes.StoreId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Categories.CategoriesProduct)).WithMany().HasForeignKey(nameof(CampaignScopes.CategoryId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Products.Products)).WithMany().HasForeignKey(nameof(CampaignScopes.ProductId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Products.ProductVariants)).WithMany().HasForeignKey(nameof(CampaignScopes.VariantId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Products.Brands)).WithMany().HasForeignKey(nameof(CampaignScopes.BrandId)).OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.CampaignId, x.ScopeType });                               // motorun kapsam çözümü
                e.HasIndex(x => new { x.ProductId, x.IsExcluded });
                e.HasIndex(x => new { x.CategoryId, x.IsExcluded });
            });
        }
    }
}