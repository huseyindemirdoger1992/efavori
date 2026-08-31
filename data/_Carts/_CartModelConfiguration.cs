using System;
using Microsoft.EntityFrameworkCore;
using data._Products;
using data._Store;
using data._Users;

namespace data._Carts
{
    /// <summary>
    /// Sepet modülünün EF Core Fluent yapılandırması.
    ///
    /// _ApplicationConnectionDb.OnModelCreating içinde şu satırla uygulanır:
    ///     _CartModelConfiguration.Apply(modelBuilder);
    /// </summary>
    public static class _CartModelConfiguration
    {
        private const string SoftDeleteFilter = "[IsDeleted_IsDeletedStatu] = 0";

        /// <summary>Sepet yapılandırmasını uygular.</summary>
        public static void Apply(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CartsProduct>(b =>
            {
                b.HasKey(x => x.Id);
                b.OwnsOne(x => x.IsDeleted);
                b.OwnsOne(x => x.ProductSnapshot, s =>
                {
                    s.Property(sp => sp.SalePriceUsd).HasPrecision(18, 4);
                    s.Property(sp => sp.DiscountAmountUsd).HasPrecision(18, 4);
                    s.Property(sp => sp.ShippingPriceUsd).HasPrecision(18, 4);
                    s.Property(sp => sp.SalePriceTry).HasPrecision(18, 4);
                    s.Property(sp => sp.DiscountAmountTry).HasPrecision(18, 4);
                    s.Property(sp => sp.ShippingPriceTry).HasPrecision(18, 4);
                    s.Property(sp => sp.SalePriceEur).HasPrecision(18, 4);
                    s.Property(sp => sp.DiscountAmountEur).HasPrecision(18, 4);
                    s.Property(sp => sp.ShippingPriceEur).HasPrecision(18, 4);
                    s.Property(sp => sp.SalePriceAzn).HasPrecision(18, 4);
                    s.Property(sp => sp.DiscountAmountAzn).HasPrecision(18, 4);
                    s.Property(sp => sp.ShippingPriceAzn).HasPrecision(18, 4);
                    s.Property(sp => sp.VatRate).HasPrecision(5, 2);
                    s.Property(sp => sp.Weight).HasPrecision(18, 3);
                    s.Property(sp => sp.Desi).HasPrecision(18, 3);
                    s.Property(sp => sp.ProductName).HasMaxLength(512);
                    s.Property(sp => sp.ProductShortDescription).HasMaxLength(2000);
                    s.Property(sp => sp.BrandName).HasMaxLength(256);
                    s.Property(sp => sp.CategoryName).HasMaxLength(256);
                    s.Property(sp => sp.Sku).HasMaxLength(128);
                    s.Property(sp => sp.Barcode).HasMaxLength(64);
                    s.Property(sp => sp.CouponCode).HasMaxLength(64);
                    s.Property(sp => sp.DeliveryTimeText).HasMaxLength(128);
                    s.Property(sp => sp.CustomerNote).HasMaxLength(1000);
                });
                b.Property(x => x.RowVersion).IsRowVersion();
                b.Property(x => x.ProductSlug).HasMaxLength(256);

                // Aynı ürün/varyant sepete iki satır olarak eklenemez (adet artırılır).
                b.HasIndex(x => new { x.UserId, x.ProductId, x.ProductVariantId })
                    .IsUnique()
                    .HasDatabaseName("UX_CartsProduct_UserProductVariant")
                    .HasFilter(SoftDeleteFilter);

                // Sepet ekranı: mağaza bazlı gruplama.
                b.HasIndex(x => new { x.UserId, x.StoreId });
                // Terk edilmiş sepet hatırlatma işi.
                b.HasIndex(x => x.CreatedAtUtc);
                // "Bu ürün kaç sepette" — stok/talep analizi.
                b.HasIndex(x => x.ProductId);

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(CartsProduct.UserId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Store)).WithMany()
                    .HasForeignKey(nameof(CartsProduct.StoreId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(CartsProduct.ProductId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(ProductVariants)).WithMany()
                    .HasForeignKey(nameof(CartsProduct.ProductVariantId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
