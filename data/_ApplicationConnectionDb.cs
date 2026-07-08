using data._Product;
using data._Product.BulkWordPressProductImport;
using data._Product.ProductHistory;
using data._Shared;
using data.Articles;
using data.FavoriteCart;
using data.ShoppingCart;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace data
{
    public class _ApplicationConnectionDb : DbContext
    {
        // CSS-JS
        public DbSet<MainCssJs> MainCssJs { get; set; } = default!;

        // Deneme Tabloları
        public DbSet<TryTableSingle> TryTableSingle { get; set; } = default!;

        // Oturum izinleri
        public DbSet<AccountPermissions> AccountPermissions { get; set; } = default!;

        // Arkadaşlıklar / Takipleşmeler
        public DbSet<FriendShip> FriendShip { get; set; } = default!;
        public DbSet<WorkstationEmployeeGroup> WorkstationEmployeeGroup { get; set; } = default!;

        // Shared
        public DbSet<Posts> Posts { get; set; } = default!;


        // Chat
        public DbSet<ChatMessage> ChatMessage { get; set; } = default!;

        // Tasks
        public DbSet<TaskFramework> TaskFramework { get; set; } = default!;
        public DbSet<TaskCategories> TaskCategories { get; set; } = default!;
        public DbSet<TaskStatus> TaskStatus { get; set; } = default!;
        public DbSet<TaskNotes> TaskNotes { get; set; } = default!;
        public DbSet<TaskKeeperJoint> TaskKeeperJoint { get; set; } = default!;

        // Media
        public DbSet<Media> Media { get; set; } = default!;
        public DbSet<ItemGallery> ItemGallery { get; set; } = default!;

        // Logs
        public DbSet<Logs> Logs { get; set; } = default!;
        public DbSet<EmailHistory> EmailHistory { get; set; } = default!;

        // Users
        public DbSet<Users> Users { get; set; } = default!;
        public DbSet<LoginTry> LoginTry { get; set; } = default!;
        public DbSet<UserShortcuts> UserShortcuts { get; set; } = default!;
        public DbSet<UsersRoles> UsersRoles { get; set; } = default!;
        public DbSet<UserPaymentMethod> UserPaymentMethod { get; set; } = default!;
        public DbSet<UserAddressMethod> UserAddressMethod { get; set; } = default!;

        // Languages
        public DbSet<Languages> Languages { get; set; } = default!;

        //Localization
        public DbSet<Country> Country { get; set; } = default!;
        public DbSet<Cities> Cities { get; set; } = default!;
        public DbSet<Regions> Regions { get; set; } = default!;
        public DbSet<States> States { get; set; } = default!;
        public DbSet<Addresses> Addresses { get; set; } = default!;

        // Products Categories
        public DbSet<CategoriesAz> CategoriesAz { get; set; } = default!;
        public DbSet<CategoriesDe> CategoriesDe { get; set; } = default!;
        public DbSet<CategoriesEn> CategoriesEn { get; set; } = default!;
        public DbSet<CategoriesEs> CategoriesEs { get; set; } = default!;
        public DbSet<CategoriesFr> CategoriesFr { get; set; } = default!;
        public DbSet<CategoriesHi> CategoriesHi { get; set; } = default!;
        public DbSet<CategoriesPt> CategoriesPt { get; set; } = default!;
        public DbSet<CategoriesRu> CategoriesRu { get; set; } = default!;
        public DbSet<CategoriesTr> CategoriesTr { get; set; } = default!;
        public DbSet<CategoriesZh> CategoriesZh { get; set; } = default!;


        // Articles
        public DbSet<AiTitlesForArticle> AiTitlesForArticle { get; set; } = default!;
        public DbSet<Article> Article { get; set; } = default!;



        // Articles Categories
        public DbSet<ArticlesCategoriesAz> ArticlesCategoriesAz { get; set; } = default!;
        public DbSet<ArticlesCategoriesDe> ArticlesCategoriesDe { get; set; } = default!;
        public DbSet<ArticlesCategoriesEn> ArticlesCategoriesEn { get; set; } = default!;
        public DbSet<ArticlesCategoriesEs> ArticlesCategoriesEs { get; set; } = default!;
        public DbSet<ArticlesCategoriesFr> ArticlesCategoriesFr { get; set; } = default!;
        public DbSet<ArticlesCategoriesHi> ArticlesCategoriesHi { get; set; } = default!;
        public DbSet<ArticlesCategoriesPt> ArticlesCategoriesPt { get; set; } = default!;
        public DbSet<ArticlesCategoriesRu> ArticlesCategoriesRu { get; set; } = default!;
        public DbSet<ArticlesCategoriesTr> ArticlesCategoriesTr { get; set; } = default!;
        public DbSet<ArticlesCategoriesZh> ArticlesCategoriesZh { get; set; } = default!;

        // Stores
        public DbSet<Store> Stores { get; set; } = default!;
        public DbSet<StoreShip> StoreShip { get; set; } = default!;
        public DbSet<StoreBlockingInfos> StoreBlockingInfos { get; set; } = default!;
        public DbSet<StoreIntegration> StoreIntegration { get; set; } = default!;



        // Products

        public DbSet<MoneyExchangeRate> MoneyExchangeRate { get; set; } = default!;

        public DbSet<ProductVariantValues> ProductVariantValues { get; set; } = default!;
        public DbSet<ProductVariants> ProductVariants { get; set; } = default!;
        public DbSet<ProductStocks> ProductStocks { get; set; } = default!;
        public DbSet<ProductSpecifications> ProductSpecifications { get; set; } = default!;
        public DbSet<ProductSeo> ProductSeo { get; set; } = default!;
        public DbSet<Products> Products { get; set; } = default!;
        public DbSet<ProductPrices> ProductPrices { get; set; } = default!;
        public DbSet<ProductMarketplaceListings> ProductMarketplaceListings { get; set; } = default!;
        public DbSet<ProductImportProfiles> ProductImportProfiles { get; set; } = default!;
        public DbSet<ProductImageVariantGroups> ProductImageVariantGroups { get; set; } = default!;
        public DbSet<ProductExternalMedias> ProductExternalMedias { get; set; } = default!;
        public DbSet<ProductDigitalAssets> ProductDigitalAssets { get; set; } = default!;
        public DbSet<ProductCategories> ProductCategories { get; set; } = default!;
        public DbSet<ProductBundleItems> ProductBundleItems { get; set; } = default!;
        public DbSet<ProductAttributeValues> ProductAttributeValues { get; set; } = default!;
        public DbSet<ProductAttributes> ProductAttributes { get; set; } = default!;
        public DbSet<ProductAttributeMappings> ProductAttributeMappings { get; set; } = default!;
        public DbSet<Marketplaces> Marketplaces { get; set; } = default!;
        public DbSet<MarketplaceCategoryMappings> MarketplaceCategoryMappings { get; set; } = default!;
        public DbSet<MarketplaceAttributeMappings> MarketplaceAttributeMappings { get; set; } = default!;
        public DbSet<Brands> Brands { get; set; } = default!;
        public DbSet<AttributeTemplates> AttributeTemplates { get; set; } = default!;
        public DbSet<AttributeTemplateItems> AttributeTemplateItems { get; set; } = default!;
        public DbSet<AttributeTemplateCategories> AttributeTemplateCategories { get; set; } = default!;
        public DbSet<Warehouse> Warehouse { get; set; } = default!;
        public DbSet<ProductHistory> ProductHistory { get; set; } = default!;
        public DbSet<ProductReview> ProductReview { get; set; } = default!;

        // ShoppingCart
        public DbSet<ProductCart> ProductCart { get; set; } = default!;


        // FavoriteCart
        public DbSet<FavoriteProductCart> FavoriteProductCart { get; set; } = default!;



        public DbSet<ProductImportJobs> ProductImportJobs { get; set; } = default!;
        public DbSet<ProductImportMappings> ProductImportMappings { get; set; } = default!;
        public DbSet<ProductImportRows> ProductImportRows { get; set; } = default!;
        // public DbSet<UrlImportFormFile> UrlImportFormFile { get; set; } = default!;





        //---------------- Constructor'lar ----------------//

        // Program.cs'den gelen ayarlar için
        public _ApplicationConnectionDb(DbContextOptions<_ApplicationConnectionDb> options)
            : base(options)
        {
        }

        // CSHTML'deki "new _ApplicationConnectionDb()" kullanımı için boş constructor
        public _ApplicationConnectionDb()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // EĞER options dışarıdan (Program.cs'den) gelmemişse, manuel yapılandır
            if (!optionsBuilder.IsConfigured)
            {
                // ========================================================================
                // FIX: Production environment için path resolution düzeltildi
                // ========================================================================
                string basePath = AppContext.BaseDirectory;

                // appsettings.json'ı bul - önce mevcut dizinde, sonra üst dizinlerde ara
                if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
                {
                    // Development ortamı için fallback
                    var devPath = Path.Combine(Directory.GetCurrentDirectory(), "../web");
                    if (Directory.Exists(devPath) && File.Exists(Path.Combine(devPath, "appsettings.json")))
                    {
                        basePath = devPath;
                    }
                    else
                    {
                        // Production: Mevcut dizini kullan
                        basePath = Directory.GetCurrentDirectory();
                    }
                }

                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException(
                        $"Connection string 'DefaultConnection' not found. " +
                        $"Searched in: {Path.Combine(basePath, "appsettings.json")}");
                }

                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        public class _ApplicationConnectionDbFactory : IDesignTimeDbContextFactory<_ApplicationConnectionDb>
        {
            public _ApplicationConnectionDb CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<_ApplicationConnectionDb>();

                // ========================================================================
                // FIX: Design-time için path resolution düzeltildi
                // ========================================================================
                string basePath = AppContext.BaseDirectory;

                // appsettings.json'ı bul
                if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
                {
                    // Eğer dosya direkt base'de yoksa, development path'i dene
                    var devPath = Path.Combine(Directory.GetCurrentDirectory(), "../web");
                    if (Directory.Exists(devPath))
                    {
                        basePath = devPath;
                    }
                    else
                    {
                        basePath = Directory.GetCurrentDirectory();
                    }
                }

                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception(
                        $"ERROR: Connection string 'DefaultConnection' not found in appsettings.json. " +
                        $"Searched path: {Path.Combine(basePath, "appsettings.json")}");
                }

                optionsBuilder.UseSqlServer(connectionString);

                return new _ApplicationConnectionDb(optionsBuilder.Options);
            }
        }
    }
}
