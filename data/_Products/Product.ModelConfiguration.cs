using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using data.Owned;
using data._Attribute;
using data._Categories;
using data._Store;
using data._Users;

namespace data._Products
{
    /// <summary>
    /// Manuel Ürün Yönetim Sistemi için EF Core Fluent yapılandırması.
    /// </summary>
    public static class _ProductModelConfiguration
    {
        /// <summary>
        /// Soft-delete edilmiş kayıtların benzersiz indekslerde tekrar kullanılabilmesini sağlayan filtredir.
        /// </summary>
        private const string SoftDeleteFilter = "[IsDeleted_IsDeletedStatu] = 0";

        /// <summary>
        /// Tüm ürün sistemi yapılandırmasını uygular.
        /// </summary>
        public static void Apply(ModelBuilder modelBuilder)
        {
            ApplyProducts(modelBuilder);
            ApplyTranslations(modelBuilder);
            ApplyCategoryLinks(modelBuilder);
            ApplyVariants(modelBuilder);
            ApplyAttributeValues(modelBuilder);
            ApplyPrices(modelBuilder);

            // MEDYA: Media ve MediaItems yapılandırması bu dosyadan ÇIKARILMIŞTIR.
            // Merkezî medya sistemi artık kendi modülünde yapılandırılır:
            //     data._Galleries._MediaModelConfiguration.Apply(modelBuilder);
            // Bu çağrı _ApplicationConnectionDb.OnModelCreating içinde ve bu
            // yapılandırmadan ÖNCE yapılır. Ürün sistemi medyaya yalnızca
            // MediaItems (ItemType = Product / ProductVariant) üzerinden bağlanır.

            ApplyBaseConventions(modelBuilder);
        }

        /// <summary>
        /// Ürün sistemindeki ortak soft-delete ve optimistic concurrency alanlarını yapılandırır.
        /// </summary>
        private static void ApplyBaseConventions(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes().ToList())
            {
                var clr = entityType.ClrType;

                if (!typeof(ProductEntityBase).IsAssignableFrom(clr))
                    continue;

                var b = modelBuilder.Entity(clr);

                b.OwnsOne(
                    typeof(IsDeleted),
                    nameof(ProductEntityBase.IsDeleted));

                b.Property(
                    nameof(ProductEntityBase.RowVersion))
                    .IsRowVersion();
            }
        }

        /// <summary>
        /// Ana ürün entity'sinin alanlarını, indekslerini ve ilişkilerini yapılandırır.
        /// </summary>
        private static void ApplyProducts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Products>(b =>
            {
                b.Property(x => x.ModelCode)
                    .HasMaxLength(128);

                b.Property(x => x.Mpn)
                    .HasMaxLength(128);

                b.Property(x => x.Gtin)
                    .HasMaxLength(64);

                b.Property(x => x.CountryOfOriginCode)
                    .HasMaxLength(2);

                b.Property(x => x.HsCode)
                    .HasMaxLength(32);

                b.Property(x => x.RejectionReason)
                    .HasMaxLength(2048);

                b.Property(x => x.VatRate)
                    .HasPrecision(5, 2);

                b.Property(x => x.WeightValue)
                    .HasPrecision(18, 3);

                b.Property(x => x.LengthValue)
                    .HasPrecision(18, 3);

                b.Property(x => x.WidthValue)
                    .HasPrecision(18, 3);

                b.Property(x => x.HeightValue)
                    .HasPrecision(18, 3);

                b.Property(x => x.Desi)
                    .HasPrecision(18, 3);

                b.HasIndex(x => new
                {
                    x.Status,
                    x.IsPublished,
                    x.PublishStartDate,
                    x.PublishEndDate
                });

                b.HasIndex(x => x.StoreId);
                b.HasIndex(x => x.UserId);
                b.HasIndex(x => x.PrimaryCategoryId);
                b.HasIndex(x => x.BrandId);
                b.HasIndex(x => x.Gtin);
                b.HasIndex(x => x.Mpn);
                b.HasIndex(x => new { x.StoreId, x.ModelCode });
                b.HasIndex(x => x.ShipsFromWarehouseId);

                b.HasOne(typeof(Store))
                    .WithMany()
                    .HasForeignKey(nameof(Products.StoreId))
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(typeof(Users))
                    .WithMany()
                    .HasForeignKey(nameof(Products.UserId))
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(typeof(Brands))
                    .WithMany()
                    .HasForeignKey(nameof(Products.BrandId))
                    .OnDelete(DeleteBehavior.SetNull);

                b.HasOne(typeof(CategoriesProduct))
                    .WithMany()
                    .HasForeignKey(nameof(Products.PrimaryCategoryId))
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(typeof(ProductVariants))
                    .WithMany()
                    .HasForeignKey(nameof(Products.DefaultVariantId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        /// <summary>
        /// Ürünlerin çok dilli çeviri kayıtlarını yapılandırır.
        /// </summary>
        private static void ApplyTranslations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductTranslations>(b =>
            {
                b.Property(x => x.Name)
                    .HasMaxLength(512)
                    .IsRequired();

                b.Property(x => x.SubTitle)
                    .HasMaxLength(512);

                b.Property(x => x.ShortDescription)
                    .HasMaxLength(2048);

                b.Property(x => x.SearchKeywords)
                    .HasMaxLength(2048);

                b.Property(x => x.Slug)
                    .HasMaxLength(512)
                    .IsRequired();

                b.Property(x => x.MetaTitle)
                    .HasMaxLength(512);

                b.Property(x => x.MetaDescription)
                    .HasMaxLength(1024);

                b.Property(x => x.FocusKeywords)
                    .HasMaxLength(1024);

                b.Property(x => x.CanonicalUrl)
                    .HasMaxLength(1024);

                b.Property(x => x.OgTitle)
                    .HasMaxLength(512);

                b.Property(x => x.OgDescription)
                    .HasMaxLength(1024);

                b.Property(x => x.RobotsIndex)
                    .HasMaxLength(64);

                b.Property(x => x.AiModel)
                    .HasMaxLength(128);

                b.Property(x => x.ErrorMessage)
                    .HasMaxLength(2048);

                b.Property(x => x.AiConfidence)
                    .HasPrecision(5, 4);

                b.HasIndex(x => new
                {
                    x.ProductId,
                    x.Language
                })
                .IsUnique()
                .HasFilter(SoftDeleteFilter);

                b.HasIndex(x => new
                {
                    x.Language,
                    x.Slug
                })
                .IsUnique()
                .HasFilter(SoftDeleteFilter);

                b.HasIndex(x => x.Status);

                b.HasOne(typeof(Products))
                    .WithMany()
                    .HasForeignKey(nameof(ProductTranslations.ProductId))
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        /// <summary>
        /// Ürün ile kategori arasındaki çoklu ilişki kayıtlarını yapılandırır.
        /// </summary>
        private static void ApplyCategoryLinks(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductCategoryLinks>(b =>
            {
                b.HasIndex(x => new
                {
                    x.ProductId,
                    x.CategoryId
                })
                .IsUnique()
                .HasFilter(SoftDeleteFilter);

                b.HasIndex(x => x.CategoryId);

                b.HasOne(typeof(Products))
                    .WithMany()
                    .HasForeignKey(nameof(ProductCategoryLinks.ProductId))
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(typeof(CategoriesProduct))
                    .WithMany()
                    .HasForeignKey(nameof(ProductCategoryLinks.CategoryId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        /// <summary>
        /// Ürün varyantları ve varyant attribute değerlerini yapılandırır.
        /// </summary>
        private static void ApplyVariants(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductVariants>(b =>
            {
                b.Property(x => x.Sku)
                    .HasMaxLength(128)
                    .IsRequired();

                b.Property(x => x.Barcode)
                    .HasMaxLength(64);

                b.Property(x => x.Gtin)
                    .HasMaxLength(64);

                b.Property(x => x.Mpn)
                    .HasMaxLength(128);

                b.Property(x => x.WeightValue)
                    .HasPrecision(18, 3);

                b.Property(x => x.LengthValue)
                    .HasPrecision(18, 3);

                b.Property(x => x.WidthValue)
                    .HasPrecision(18, 3);

                b.Property(x => x.HeightValue)
                    .HasPrecision(18, 3);

                b.Property(x => x.Desi)
                    .HasPrecision(18, 3);

                b.HasIndex(x => x.Sku)
                    .IsUnique()
                    .HasFilter(SoftDeleteFilter);

                b.HasIndex(x => x.ProductId);
                b.HasIndex(x => x.Barcode);
                b.HasIndex(x => x.Gtin);

                b.HasOne(typeof(Products))
                    .WithMany()
                    .HasForeignKey(nameof(ProductVariants.ProductId))
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProductVariantAttributeValues>(b =>
            {
                b.HasIndex(x => new
                {
                    x.ProductVariantId,
                    x.AttributeDefinitionId
                })
                .IsUnique()
                .HasFilter(SoftDeleteFilter);

                b.HasIndex(x => new
                {
                    x.ProductId,
                    x.AttributeDefinitionId,
                    x.AttributeOptionId
                });

                b.HasOne(typeof(ProductVariants))
                    .WithMany()
                    .HasForeignKey(nameof(ProductVariantAttributeValues.ProductVariantId))
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(typeof(Products))
                    .WithMany()
                    .HasForeignKey(nameof(ProductVariantAttributeValues.ProductId))
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(typeof(AttributeDefinition))
                    .WithMany()
                    .HasForeignKey(nameof(ProductVariantAttributeValues.AttributeDefinitionId))
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(typeof(AttributeOption))
                    .WithMany()
                    .HasForeignKey(nameof(ProductVariantAttributeValues.AttributeOptionId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        /// <summary>
        /// Ürün teknik attribute değerlerini yapılandırır.
        /// </summary>
        private static void ApplyAttributeValues(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductAttributeValues>(b =>
            {
                b.Property(x => x.ValueText)
                    .HasMaxLength(4000);

                b.Property(x => x.ValueNumeric)
                    .HasPrecision(38, 10);

                b.Property(x => x.ValueNumericInBaseUnit)
                    .HasPrecision(38, 10);

                b.HasIndex(x => new
                {
                    x.ProductId,
                    x.AttributeDefinitionId
                });

                b.HasIndex(x => new
                {
                    x.AttributeDefinitionId,
                    x.AttributeOptionId
                });

                b.HasIndex(x => new
                {
                    x.AttributeDefinitionId,
                    x.ValueNumericInBaseUnit
                });

                b.HasOne(typeof(Products))
                    .WithMany()
                    .HasForeignKey(nameof(ProductAttributeValues.ProductId))
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(typeof(AttributeDefinition))
                    .WithMany()
                    .HasForeignKey(nameof(ProductAttributeValues.AttributeDefinitionId))
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(typeof(AttributeOption))
                    .WithMany()
                    .HasForeignKey(nameof(ProductAttributeValues.AttributeOptionId))
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(typeof(Unit))
                    .WithMany()
                    .HasForeignKey(nameof(ProductAttributeValues.UnitId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        /// <summary>
        /// Ürün fiyat kayıtlarını ve para birimi bazlı benzersizliği yapılandırır.
        /// </summary>
        private static void ApplyPrices(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductPrices>(b =>
            {
                b.Property(x => x.ListPrice)
                    .HasPrecision(18, 4);

                b.Property(x => x.SalePrice)
                    .HasPrecision(18, 4);

                b.Property(x => x.DiscountedPrice)
                    .HasPrecision(18, 4);

                b.Property(x => x.ConversionRateUsed)
                    .HasPrecision(18, 6);

                b.HasIndex(x => new
                {
                    x.ProductId,
                    x.ProductVariantId,
                    x.Currency
                })
                .IsUnique()
                .HasFilter(SoftDeleteFilter);

                b.HasIndex(x => x.ProductVariantId);

                b.HasOne(typeof(Products))
                    .WithMany()
                    .HasForeignKey(nameof(ProductPrices.ProductId))
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(typeof(ProductVariants))
                    .WithMany()
                    .HasForeignKey(nameof(ProductPrices.ProductVariantId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

    }
}
