using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using data.Owned;
using data._Attribute;
using data._Categories;
using data._Galleries;
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

            // Ürünler artık ProductMedia kullanmaz; ortak medya sistemi kullanılır.
            ApplyMedia(modelBuilder);

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

        /// <summary>
        /// Ortak medya sisteminin Media ve MediaItems entity'lerini ürün sistemiyle uyumlu şekilde yapılandırır.
        /// </summary>
        private static void ApplyMedia(ModelBuilder modelBuilder)
        {
            // ================================================================
            // Media
            // Dosyanın fiziksel veya harici medya kaydıdır.
            // ================================================================
            modelBuilder.Entity<Media>(b =>
            {
                b.HasKey(x => x.Id);

                b.Property(x => x.FileName)
                    .HasMaxLength(512);

                b.Property(x => x.FileStoredName)
                    .HasMaxLength(512);

                b.Property(x => x.MediaType)
                    .HasMaxLength(50);

                b.Property(x => x.ContentType)
                    .HasMaxLength(255);

                b.Property(x => x.FileExtensionType)
                    .HasMaxLength(20);

                b.Property(x => x.StorageProvider)
                    .HasMaxLength(100);

                b.Property(x => x.StorageBucket)
                    .HasMaxLength(255);

                b.Property(x => x.StorageKey)
                    .HasMaxLength(1024);

                b.Property(x => x.FileUrl)
                    .HasMaxLength(2048);

                b.Property(x => x.OrjFileUrl)
                    .HasMaxLength(2048);

                b.Property(x => x.FilePhysicalPathRoad)
                    .HasMaxLength(2048);

                b.Property(x => x.OrjFilePhysicalPathRoad)
                    .HasMaxLength(2048);

                b.Property(x => x.FileUrl_Ratio_1_2)
                    .HasMaxLength(2048);

                b.Property(x => x.FileUrl_Ratio_1_4)
                    .HasMaxLength(2048);

                b.Property(x => x.FileUrl_Ratio_1_8)
                    .HasMaxLength(2048);

                b.Property(x => x.FileUrl_Ratio_1_16)
                    .HasMaxLength(2048);

                b.Property(x => x.Sha256)
                    .HasMaxLength(64);

                b.Property(x => x.ETag)
                    .HasMaxLength(255);

                b.Property(x => x.BlurHash)
                    .HasMaxLength(512);

                b.Property(x => x.DominantColor)
                    .HasMaxLength(32);

                b.Property(x => x.ExternalUrl)
                    .HasMaxLength(2048);

                b.Property(x => x.ExternalProvider)
                    .HasMaxLength(100);

                b.Property(x => x.ProcessingStatus)
                    .HasMaxLength(50);

                // Aynı içeriğin tekrar yüklenmesini hızlı tespit etmek için.
                b.HasIndex(x => x.Sha256);

                // Kullanıcının kendi medya kütüphanesini hızlı getirmek için.
                b.HasIndex(x => x.UserId);

                // İşleme kuyruğu için.
                b.HasIndex(x => x.ProcessingStatus);

                // Medya türüne göre filtreleme için.
                b.HasIndex(x => x.MediaType);

                // Son oluşturulan medyaları hızlı almak için.
                b.HasIndex(x => x.CreatedAt);

                // Medyanın sahibi kullanıcıdır; kullanıcı silindiğinde medya zorunlu
                // olarak fiziksel olarak silinmemelidir.
                b.HasOne(typeof(Users))
                    .WithMany()
                    .HasForeignKey(nameof(Media.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ================================================================
            // MediaItems
            // Medyanın hangi entity içerisinde ve hangi amaçla kullanıldığını tutar.
            // ================================================================
            modelBuilder.Entity<MediaItems>(b =>
            {
                b.HasKey(x => x.Id);

                b.Property(x => x.ItemType)
                    .HasMaxLength(100)
                    .IsRequired();

                b.Property(x => x.MediaRole)
                    .HasMaxLength(50);

                b.Property(x => x.AltText)
                    .HasMaxLength(1000);

                b.Property(x => x.Caption)
                    .HasMaxLength(4000);

                b.Property(x => x.Title)
                    .HasMaxLength(500);

                b.Property(x => x.LinkUrl)
                    .HasMaxLength(2048);

                // Ürün medya galerisi sorgusu.
                b.HasIndex(x => new
                {
                    x.ItemType,
                    x.ItemId,
                    x.SortOrder
                });

                // Ürün/varyant/profil/gönderi gibi entity bazında medya sorguları.
                b.HasIndex(x => new
                {
                    x.ItemType,
                    x.ItemId
                });

                // Belirli bir medya kaydının nerelerde kullanıldığını hızlı bulmak için.
                b.HasIndex(x => x.MediaId);

                // Kullanıcı medya sorguları.
                b.HasIndex(x => x.UserId);

                // Primary medya sorguları.
                b.HasIndex(x => new
                {
                    x.ItemType,
                    x.ItemId,
                    x.IsPrimary
                });

                // Aynı medya aynı entity'ye aynı rol ile ikinci kez bağlanmasın.
                b.HasIndex(x => new
                {
                    x.ItemType,
                    x.ItemId,
                    x.MediaId,
                    x.MediaRole
                })
                .IsUnique()
                .HasFilter(SoftDeleteFilter);

                // MediaItems → Media
                // Media silindiğinde ilişkisel kayıt otomatik silinmez.
                b.HasOne(typeof(Media))
                    .WithMany()
                    .HasForeignKey(nameof(MediaItems.MediaId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}