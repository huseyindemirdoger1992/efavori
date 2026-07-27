using data;
using data._Attribute;
using data._BulkImportProducts;
using data._Carts;
using data._Categories;
using data._Follows;
using data._Galleries;
using data._Helper;
using data._Locations;
using data._Products;
using data._Shares;
using data._Store;
using data._Systems;
using data._Tasks;
using data._Users;
using data.Owned;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace data
{
    public class _ApplicationConnectionDb : DbContext
    {
        // === _Carts ===
        public DbSet<CartsFavorite> CartsFavorite { get; set; } = default!;
        public DbSet<CartsProduct> CartsProduct { get; set; } = default!;

        // === _Categories ===
        public DbSet<CategoriesArticle> CategoriesArticle { get; set; } = default!;
        public DbSet<CategoriesProduct> CategoriesProduct { get; set; } = default!;

        // === _Attribute (Enterprise Marketplace Attribute System V3) ===
        public DbSet<AttributeGroup> AttributeGroups => Set<AttributeGroup>();
        public DbSet<AttributeGroupTranslation> AttributeGroupTranslations => Set<AttributeGroupTranslation>();
        public DbSet<AttributeDefinition> AttributeDefinitions => Set<AttributeDefinition>();
        public DbSet<AttributeTranslation> AttributeTranslations => Set<AttributeTranslation>();
        public DbSet<AttributeAlias> AttributeAliases => Set<AttributeAlias>();
        public DbSet<AttributeSynonym> AttributeSynonyms => Set<AttributeSynonym>();
        public DbSet<AttributeOption> AttributeOptions => Set<AttributeOption>();
        public DbSet<AttributeOptionTranslation> AttributeOptionTranslations => Set<AttributeOptionTranslation>();
        public DbSet<AttributeOptionAlias> AttributeOptionAliases => Set<AttributeOptionAlias>();
        public DbSet<AttributeOptionSynonym> AttributeOptionSynonyms => Set<AttributeOptionSynonym>();
        public DbSet<UnitGroup> UnitGroups => Set<UnitGroup>();
        public DbSet<UnitGroupTranslation> UnitGroupTranslations => Set<UnitGroupTranslation>();
        public DbSet<Unit> Units => Set<Unit>();
        public DbSet<UnitTranslation> UnitTranslations => Set<UnitTranslation>();
        public DbSet<AttributeTemplate> AttributeTemplates_V3 => Set<AttributeTemplate>(); // adı mevcut ile çakışmasın diye _V3
        public DbSet<AttributeTemplateTranslation> AttributeTemplateTranslations => Set<AttributeTemplateTranslation>();
        public DbSet<TemplateAttribute> TemplateAttributes => Set<TemplateAttribute>();
        public DbSet<TemplateCategory> TemplateCategories => Set<TemplateCategory>();
        public DbSet<CategoryAttribute> CategoryAttributes => Set<CategoryAttribute>();
        public DbSet<AttributeDependency> AttributeDependencies => Set<AttributeDependency>();
        public DbSet<NormalizationRule> NormalizationRules => Set<NormalizationRule>();
        public DbSet<AiGenerationJob> AiGenerationJobs => Set<AiGenerationJob>();
        public DbSet<AiGenerationHistory> AiGenerationHistories => Set<AiGenerationHistory>();
        public DbSet<IntegrationPlatform> IntegrationPlatforms => Set<IntegrationPlatform>();
        public DbSet<AttributeMapping> AttributeMappings => Set<AttributeMapping>();
        public DbSet<AttributeOptionMapping> AttributeOptionMappings => Set<AttributeOptionMapping>();


        // === _Follows ===
        public DbSet<FriendShip> FriendShip { get; set; } = default!;
        public DbSet<StoreShip> StoreShip { get; set; } = default!;

        // === _Galleries ===
        public DbSet<Media> Media { get; set; } = default!;
        public DbSet<MediaItems> MediaItems { get; set; } = default!;

        // === _Helper ===
        public DbSet<SupportTickets> SupportTickets { get; set; } = default!;

        // === _Locations ===
        public DbSet<Country> Country { get; set; } = default!;
        public DbSet<States> States { get; set; } = default!;
        public DbSet<Cities> Cities { get; set; } = default!;
        public DbSet<Regions> Regions { get; set; } = default!;

        // === _Products (Marka & Kur) ===
        public DbSet<Brands> Brands { get; set; } = default!;
        public DbSet<MoneyExchangeRate> MoneyExchangeRate { get; set; } = default!;

        // === _Products (Manuel Ürün Yönetim Sistemi — Product System V1) ===
        public DbSet<Products> Products => Set<Products>();
        public DbSet<ProductTranslations> ProductTranslations => Set<ProductTranslations>();
        public DbSet<ProductCategoryLinks> ProductCategoryLinks => Set<ProductCategoryLinks>();
        public DbSet<ProductVariants> ProductVariants => Set<ProductVariants>();
        public DbSet<ProductVariantAttributeValues> ProductVariantAttributeValues => Set<ProductVariantAttributeValues>();
        public DbSet<ProductAttributeValues> ProductAttributeValues => Set<ProductAttributeValues>();
        public DbSet<ProductPrices> ProductPrices => Set<ProductPrices>();
        public DbSet<ProductMedia> ProductMedia => Set<ProductMedia>();
        public DbSet<ProductMediaTranslations> ProductMediaTranslations => Set<ProductMediaTranslations>();

        // === _Products (Yorum / Puanlama) ===
        public DbSet<ProductReviews> ProductReviews => Set<ProductReviews>();
        public DbSet<ProductReviewVotes> ProductReviewVotes => Set<ProductReviewVotes>();
        public DbSet<ProductReviewReports> ProductReviewReports => Set<ProductReviewReports>();
        public DbSet<ProductRatingSummary> ProductRatingSummary => Set<ProductRatingSummary>();

        // === _Products (Soru-Cevap) ===
        public DbSet<ProductQuestions> ProductQuestions => Set<ProductQuestions>();
        public DbSet<ProductQuestionVotes> ProductQuestionVotes => Set<ProductQuestionVotes>();
        public DbSet<ProductQuestionReports> ProductQuestionReports => Set<ProductQuestionReports>();

        // === _BulkImportProducts (Toplu Ürün İçe Aktarım) ===
        public DbSet<ImportProfile> ImportProfiles => Set<ImportProfile>();
        public DbSet<ImportCredential> ImportCredentials => Set<ImportCredential>();
        public DbSet<ImportFieldMapping> ImportFieldMappings => Set<ImportFieldMapping>();
        public DbSet<ImportCategoryMapping> ImportCategoryMappings => Set<ImportCategoryMapping>();
        public DbSet<ImportJob> ImportJobs => Set<ImportJob>();
        public DbSet<ImportRow> ImportRows => Set<ImportRow>();
        public DbSet<ImportRowLog> ImportRowLogs => Set<ImportRowLog>();

        // === _Shares ===
        public DbSet<Articles> Articles { get; set; } = default!;
        public DbSet<Posts> Posts { get; set; } = default!;

        // === _Store ===
        public DbSet<Store> Store { get; set; } = default!;
        public DbSet<StoreBlockingInfos> StoreBlockingInfos { get; set; } = default!;
        public DbSet<StoreIntegration> StoreIntegration { get; set; } = default!;
        public DbSet<WareHouse> WareHouse { get; set; } = default!;

        // ================= Sipariş Sistemi V1 (data._Orders) =================
        public DbSet<data._Orders.Orders> Orders { get; set; }
        public DbSet<data._Orders.SubOrders> SubOrders { get; set; }
        public DbSet<data._Orders.OrderItems> OrderItems { get; set; }
        public DbSet<data._Orders.OrderStatusHistory> OrderStatusHistory { get; set; }
        public DbSet<data._Orders.OrderInvoices> OrderInvoices { get; set; }
        public DbSet<data._Orders.CheckoutSessions> CheckoutSessions { get; set; }
        public DbSet<data._Orders.OrderNumberSequences> OrderNumberSequences { get; set; }

        // ================= Ödeme ve Hakediş Sistemi V1 (data._Payments) =================
        public DbSet<data._Payments.PaymentProviders> PaymentProviders { get; set; }
        public DbSet<data._Payments.PaymentTransactions> PaymentTransactions { get; set; }
        public DbSet<data._Payments.UserPaymentMethods> UserPaymentMethods { get; set; }
        public DbSet<data._Payments.CommissionRates> CommissionRates { get; set; }
        public DbSet<data._Payments.SellerLedgerEntries> SellerLedgerEntries { get; set; }
        public DbSet<data._Payments.SellerPayouts> SellerPayouts { get; set; }
        public DbSet<data._Payments.Refunds> Refunds { get; set; }

        // === _Systems ===
        public DbSet<AccountPermissions> AccountPermissions { get; set; } = default!;
        public DbSet<AllBackgroundServicesFrequencyRate> AllBackgroundServicesFrequencyRate { get; set; } = default!;
        public DbSet<Logs> Logs { get; set; } = default!;
        public DbSet<MainCssJs> MainCssJs { get; set; } = default!;
        public DbSet<TryTableSingle> TryTableSingle { get; set; } = default!;

        // === _Tasks ===
        public DbSet<TaskFramework> TaskFramework { get; set; } = default!;
        public DbSet<TaskCategories> TaskCategories { get; set; } = default!;
        public DbSet<data._Tasks.TaskStatus> TaskStatus { get; set; } = default!;
        public DbSet<TaskKeeperJoint> TaskKeeperJoint { get; set; } = default!;
        public DbSet<TaskNotes> TaskNotes { get; set; } = default!;

        // === _Users ===
        public DbSet<Users> Users { get; set; } = default!;
        public DbSet<UserAddress> UserAddress { get; set; } = default!;
        public DbSet<UserPayment> UserPayment { get; set; } = default!;
        public DbSet<UserShortcuts> UserShortcuts { get; set; } = default!;
        public DbSet<ChatMessage> ChatMessage { get; set; } = default!;
        public DbSet<EmailHistory> EmailHistory { get; set; } = default!;
        public DbSet<LoginTry> LoginTry { get; set; } = default!;


        //---------------- Constructor'lar ----------------//

        public _ApplicationConnectionDb(DbContextOptions<_ApplicationConnectionDb> options)
            : base(options)
        {
        }

        public _ApplicationConnectionDb()
        {
        }

        //---------------- Model yapılandırması ----------------//

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Enterprise Marketplace Attribute System V3 — owned tipler (Ai/IsDeleted),
            // FK'ler, silme davranışları, tekil/bileşik indeksler, RowVersion ve
            // decimal precision yapılandırmalarının tamamını uygular.
            _AttributeModelConfiguration.Apply(modelBuilder);

            // Manuel Ürün Yönetim Sistemi (Product System V1) — ürün, çeviri (10 dil),
            // varyant/SKU, teknik özellik değerleri, 4 para birimli fiyat, medya ve
            // çoklu kategori atamalarının FK, indeks, silme davranışı, RowVersion ve
            // decimal precision yapılandırmalarını uygular.
            _ProductModelConfiguration.Apply(modelBuilder);

            // Ürün Yorum/Puanlama & Soru-Cevap sistemi — yorum ağacı, fayda oyları,
            // şikâyetler, türetilmiş puan özeti ve Q&A ağacının FK, indeks, silme
            // davranışı, RowVersion ve decimal precision yapılandırmalarını uygular.
            _ProductReviewModelConfiguration.Apply(modelBuilder);

            // Toplu Ürün İçe Aktarım Sistemi — kullanıcıya özel profil/kimlik,
            // alan & kategori eşleştirmeleri, lease/idempotency'li iş kuyruğu ve
            // staging satırlarının FK, indeks, silme davranışı, RowVersion ve
            // decimal precision yapılandırmalarını uygular.
            _BulkImportModelConfiguration.Apply(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string basePath = AppContext.BaseDirectory;

                if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
                {
                    var devPath = Path.Combine(Directory.GetCurrentDirectory(), "../web");
                    if (Directory.Exists(devPath) && File.Exists(Path.Combine(devPath, "appsettings.json")))
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

                string basePath = AppContext.BaseDirectory;

                if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
                {
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
