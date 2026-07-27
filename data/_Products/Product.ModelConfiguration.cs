using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using data.Owned;        // ortak soft-delete owned tipi (data.Owned.IsDeleted)
using data._Attribute;   // AttributeDefinition, AttributeOption, Unit (V3 sözlüğü)
using data._Categories;  // CategoriesProduct
using data._Galleries;   // Media
using data._Store;       // Store
using data._Users;       // Users

namespace data._Products
{
    /// <summary>
    /// Manuel Ürün Yönetim Sistemi için EF Core Fluent yapılandırması.
    /// _AttributeModelConfiguration desenini birebir izler.
    ///
    /// _ApplicationConnectionDb'ye eklenecekler:
    ///
    ///     // === _Products (Manuel Ürün Yönetim Sistemi) ===
    ///     public DbSet&lt;Products&gt; Products => Set&lt;Products&gt;();
    ///     public DbSet&lt;ProductTranslations&gt; ProductTranslations => Set&lt;ProductTranslations&gt;();
    ///     public DbSet&lt;ProductCategoryLinks&gt; ProductCategoryLinks => Set&lt;ProductCategoryLinks&gt;();
    ///     public DbSet&lt;ProductVariants&gt; ProductVariants => Set&lt;ProductVariants&gt;();
    ///     public DbSet&lt;ProductVariantAttributeValues&gt; ProductVariantAttributeValues => Set&lt;ProductVariantAttributeValues&gt;();
    ///     public DbSet&lt;ProductAttributeValues&gt; ProductAttributeValues => Set&lt;ProductAttributeValues&gt;();
    ///     public DbSet&lt;ProductPrices&gt; ProductPrices => Set&lt;ProductPrices&gt;();
    ///     public DbSet&lt;ProductMedia&gt; ProductMedia => Set&lt;ProductMedia&gt;();
    ///     public DbSet&lt;ProductMediaTranslations&gt; ProductMediaTranslations => Set&lt;ProductMediaTranslations&gt;();
    ///
    ///     protected override void OnModelCreating(ModelBuilder modelBuilder)
    ///     {
    ///         base.OnModelCreating(modelBuilder);
    ///         _AttributeModelConfiguration.Apply(modelBuilder);
    ///         _ProductModelConfiguration.Apply(modelBuilder);   // ← bu satır
    ///     }
    ///
    /// Konvansiyon: navigation property YOK; ilişkiler HasOne(typeof(X)).WithMany()
    /// .HasForeignKey("...") ile yalnızca FK üzerinden kurulur.
    /// </summary>
    public static class _ProductModelConfiguration
    {
        /// <summary>
        /// Soft-delete filtresi — benzersiz indekslerin soft-delete edilmiş
        /// kayıtlarla çakışmaması için (aynı SKU/slug yeniden kullanılabilsin).
        /// Attribute System V3'teki sabitle aynı kolon adlandırmasını kullanır.
        /// </summary>
        private const string SoftDeleteFilter = "[IsDeleted_IsDeletedStatu] = 0";

        /// <summary>Tüm ürün sistemi yapılandırmasını uygular.</summary>
        public static void Apply(ModelBuilder modelBuilder)
        {
            // Önce entity'ler modele kaydedilir (Entity<T> çağrıları) ...
            ApplyProducts(modelBuilder);
            ApplyTranslations(modelBuilder);
            ApplyCategoryLinks(modelBuilder);
            ApplyVariants(modelBuilder);
            ApplyAttributeValues(modelBuilder);
            ApplyPrices(modelBuilder);
            ApplyMedia(modelBuilder);

            // ... sonra ortak konvansiyonlar (owned IsDeleted, RowVersion) tüm
            // kayıtlı entity'lere tek yerden uygulanır. Sıra ÖNEMLİDİR.
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

                var b = modelBuilder.Entity(clr);

                // Soft-delete owned tipi (mevcut ortak IsDeleted tipi)
                b.OwnsOne(typeof(IsDeleted), nameof(ProductEntityBase.IsDeleted));

                // İyimser eşzamanlılık damgası
                b.Property(nameof(ProductEntityBase.RowVersion)).IsRowVersion();
            }
        }

        // ── Products (ana ürün) ───────────────────────────────────────────────
        private static void ApplyProducts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Products>(b =>
            {
                b.Property(x => x.ModelCode).HasMaxLength(128);
                b.Property(x => x.Mpn).HasMaxLength(128);
                b.Property(x => x.Gtin).HasMaxLength(64);
                b.Property(x => x.CountryOfOriginCode).HasMaxLength(2);
                b.Property(x => x.HsCode).HasMaxLength(32);
                b.Property(x => x.RejectionReason).HasMaxLength(2048);

                b.Property(x => x.VatRate).HasPrecision(5, 2);
                b.Property(x => x.WeightValue).HasPrecision(18, 3);
                b.Property(x => x.LengthValue).HasPrecision(18, 3);
                b.Property(x => x.WidthValue).HasPrecision(18, 3);
                b.Property(x => x.HeightValue).HasPrecision(18, 3);
                b.Property(x => x.Desi).HasPrecision(18, 3);

                // Vitrin sorgularının ana indeksi: yayın penceresi taraması
                b.HasIndex(x => new { x.Status, x.IsPublished, x.PublishStartDate, x.PublishEndDate });
                b.HasIndex(x => x.StoreId);
                b.HasIndex(x => x.UserId);
                b.HasIndex(x => x.PrimaryCategoryId);
                b.HasIndex(x => x.BrandId);
                b.HasIndex(x => x.Gtin);
                b.HasIndex(x => x.Mpn);
                b.HasIndex(x => new { x.StoreId, x.ModelCode });

                b.HasOne(typeof(Store)).WithMany()
                    .HasForeignKey(nameof(Products.StoreId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(Products.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Brands)).WithMany()
                    .HasForeignKey(nameof(Products.BrandId))
                    .OnDelete(DeleteBehavior.SetNull);
                b.HasOne(typeof(CategoriesProduct)).WithMany()
                    .HasForeignKey(nameof(Products.PrimaryCategoryId))
                    .OnDelete(DeleteBehavior.NoAction);
                // NOT: WareHouse şu an _ApplicationConnectionDb'de DbSet olarak kayıtlı
                // DEĞİL; FK kısıtı tanımlamak tabloyu istemeden EF modeline dâhil ederdi.
                // ShipsFromWarehouseId bu yüzden (mevcut proje konvansiyonuna uygun olarak)
                // yalnızca ID referansı olarak tutulur. WareHouse context'e eklenirse
                // buraya HasOne(typeof(WareHouse)) + SetNull kısıtı eklenebilir.
                b.HasIndex(x => x.ShipsFromWarehouseId);

                // Products ↔ ProductVariants arasında FK döngüsü oluşmaması için NoAction.
                b.HasOne(typeof(ProductVariants)).WithMany()
                    .HasForeignKey(nameof(Products.DefaultVariantId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── ProductTranslations (10 dil + AI çeviri yönetişimi) ──────────────
        private static void ApplyTranslations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductTranslations>(b =>
            {
                b.Property(x => x.Name).HasMaxLength(512).IsRequired();
                b.Property(x => x.SubTitle).HasMaxLength(512);
                b.Property(x => x.ShortDescription).HasMaxLength(2048);
                // DescriptionHtml, BulletPointsJson → nvarchar(max) (varsayılan)
                b.Property(x => x.SearchKeywords).HasMaxLength(2048);
                b.Property(x => x.Slug).HasMaxLength(512).IsRequired();
                b.Property(x => x.MetaTitle).HasMaxLength(512);
                b.Property(x => x.MetaDescription).HasMaxLength(1024);
                b.Property(x => x.FocusKeywords).HasMaxLength(1024);
                b.Property(x => x.CanonicalUrl).HasMaxLength(1024);
                b.Property(x => x.OgTitle).HasMaxLength(512);
                b.Property(x => x.OgDescription).HasMaxLength(1024);
                b.Property(x => x.RobotsIndex).HasMaxLength(64);
                b.Property(x => x.AiModel).HasMaxLength(128);
                b.Property(x => x.ErrorMessage).HasMaxLength(2048);
                b.Property(x => x.AiConfidence).HasPrecision(5, 4);

                // Bir ürün + dil için tek çeviri satırı
                b.HasIndex(x => new { x.ProductId, x.Language }).IsUnique().HasFilter(SoftDeleteFilter);
                // Slug her dil içinde tekil (SEO URL çakışması engellenir)
                b.HasIndex(x => new { x.Language, x.Slug }).IsUnique().HasFilter(SoftDeleteFilter);
                // AI çeviri BackgroundService'inin kuyruk taraması (Pending satırlar)
                b.HasIndex(x => x.Status);

                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(ProductTranslations.ProductId))
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        // ── ProductCategoryLinks (çoklu kategori) ────────────────────────────
        private static void ApplyCategoryLinks(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductCategoryLinks>(b =>
            {
                b.HasIndex(x => new { x.ProductId, x.CategoryId }).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => x.CategoryId); // kategori sayfası listelemeleri

                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(ProductCategoryLinks.ProductId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(CategoriesProduct)).WithMany()
                    .HasForeignKey(nameof(ProductCategoryLinks.CategoryId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── ProductVariants + eksen değerleri ────────────────────────────────
        private static void ApplyVariants(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductVariants>(b =>
            {
                b.Property(x => x.Sku).HasMaxLength(128).IsRequired();
                b.Property(x => x.Barcode).HasMaxLength(64);
                b.Property(x => x.Gtin).HasMaxLength(64);
                b.Property(x => x.Mpn).HasMaxLength(128);
                b.Property(x => x.WeightValue).HasPrecision(18, 3);
                b.Property(x => x.LengthValue).HasPrecision(18, 3);
                b.Property(x => x.WidthValue).HasPrecision(18, 3);
                b.Property(x => x.HeightValue).HasPrecision(18, 3);
                b.Property(x => x.Desi).HasPrecision(18, 3);

                // SKU sistem genelinde tekil (soft-delete edilmişler hariç)
                b.HasIndex(x => x.Sku).IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => x.ProductId);
                b.HasIndex(x => x.Barcode);
                b.HasIndex(x => x.Gtin);

                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(ProductVariants.ProductId))
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProductVariantAttributeValues>(b =>
            {
                // Bir varyantta aynı eksen (attribute) yalnızca bir kez yer alır
                b.HasIndex(x => new { x.ProductVariantId, x.AttributeDefinitionId })
                    .IsUnique().HasFilter(SoftDeleteFilter);
                // Facet/varyant-seçici sorguları: "bu üründe Renk ekseninde hangi option'lar var?"
                b.HasIndex(x => new { x.ProductId, x.AttributeDefinitionId, x.AttributeOptionId });

                // Çoklu cascade yolu hatasına karşı: silme yalnızca varyant üzerinden akar.
                b.HasOne(typeof(ProductVariants)).WithMany()
                    .HasForeignKey(nameof(ProductVariantAttributeValues.ProductVariantId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(ProductVariantAttributeValues.ProductId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(AttributeDefinition)).WithMany()
                    .HasForeignKey(nameof(ProductVariantAttributeValues.AttributeDefinitionId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(AttributeOption)).WithMany()
                    .HasForeignKey(nameof(ProductVariantAttributeValues.AttributeOptionId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── ProductAttributeValues (teknik özellikler) ───────────────────────
        private static void ApplyAttributeValues(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductAttributeValues>(b =>
            {
                b.Property(x => x.ValueText).HasMaxLength(4000);
                b.Property(x => x.ValueNumeric).HasPrecision(38, 10);
                b.Property(x => x.ValueNumericInBaseUnit).HasPrecision(38, 10);
                // ValueJson → nvarchar(max) (varsayılan)

                b.HasIndex(x => new { x.ProductId, x.AttributeDefinitionId });
                // Facet filtreleri: option bazlı ("Renk = Siyah olan ürünler")
                b.HasIndex(x => new { x.AttributeDefinitionId, x.AttributeOptionId });
                // Aralık filtreleri: baz birime normalize sayısal değer üzerinden
                b.HasIndex(x => new { x.AttributeDefinitionId, x.ValueNumericInBaseUnit });

                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(ProductAttributeValues.ProductId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(AttributeDefinition)).WithMany()
                    .HasForeignKey(nameof(ProductAttributeValues.AttributeDefinitionId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(AttributeOption)).WithMany()
                    .HasForeignKey(nameof(ProductAttributeValues.AttributeOptionId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Unit)).WithMany()
                    .HasForeignKey(nameof(ProductAttributeValues.UnitId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── ProductPrices (4 para birimi) ────────────────────────────────────
        private static void ApplyPrices(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductPrices>(b =>
            {
                b.Property(x => x.ListPrice).HasPrecision(18, 4);
                b.Property(x => x.SalePrice).HasPrecision(18, 4);
                b.Property(x => x.DiscountedPrice).HasPrecision(18, 4);
                b.Property(x => x.ConversionRateUsed).HasPrecision(18, 6);

                // Aynı kapsam (ürün geneli VEYA belirli varyant) + para birimi → tek satır.
                // Not: SQL Server benzersiz indekste NULL'u tek satırla sınırlar; bu,
                // "ürün geneli (VariantId = null)" satırının da para birimi başına
                // tekil olmasını sağlar — tam istenen davranış.
                b.HasIndex(x => new { x.ProductId, x.ProductVariantId, x.Currency })
                    .IsUnique().HasFilter(SoftDeleteFilter);
                b.HasIndex(x => x.ProductVariantId);

                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(ProductPrices.ProductId))
                    .OnDelete(DeleteBehavior.Cascade);
                // Çoklu cascade yolu (Products→Variants→Prices ve Products→Prices) → NoAction.
                b.HasOne(typeof(ProductVariants)).WithMany()
                    .HasForeignKey(nameof(ProductPrices.ProductVariantId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── ProductMedia + çevirileri ────────────────────────────────────────
        private static void ApplyMedia(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductMedia>(b =>
            {
                b.Property(x => x.ExternalUrl).HasMaxLength(1024);

                b.HasIndex(x => new { x.ProductId, x.DisplayOrder });
                b.HasIndex(x => x.ProductVariantId);
                b.HasIndex(x => x.MediaId);

                b.HasOne(typeof(Products)).WithMany()
                    .HasForeignKey(nameof(ProductMedia.ProductId))
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(ProductVariants)).WithMany()
                    .HasForeignKey(nameof(ProductMedia.ProductVariantId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Media)).WithMany()
                    .HasForeignKey(nameof(ProductMedia.MediaId))
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ProductMediaTranslations>(b =>
            {
                b.Property(x => x.AltText).HasMaxLength(512);
                b.Property(x => x.Title).HasMaxLength(512);
                b.Property(x => x.Caption).HasMaxLength(1024);

                b.HasIndex(x => new { x.ProductMediaId, x.Language }).IsUnique().HasFilter(SoftDeleteFilter);

                b.HasOne(typeof(ProductMedia)).WithMany()
                    .HasForeignKey(nameof(ProductMediaTranslations.ProductMediaId))
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
