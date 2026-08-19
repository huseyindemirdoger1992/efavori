using data;
using data._Attribute;
using data._BulkImportProducts;
using data._Carts;
using data._Categories;
using data._Follows;
using data._Galleries;
using data._Helper;
using data._Inventory;
using data._Locations;
using data._Notifications;
using data._Orders;
using data._Payments;
using data._Products;
using data._Promotions;
using data._Returns;
using data._Shares;
using data._Shipping;
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
        public DbSet<AttributeGroup> AttributeGroups { get; set; } = default!;
        public DbSet<AttributeGroupTranslation> AttributeGroupTranslations { get; set; } = default!;
        public DbSet<AttributeDefinition> AttributeDefinitions { get; set; } = default!;
        public DbSet<AttributeTranslation> AttributeTranslations { get; set; } = default!;
        public DbSet<AttributeAlias> AttributeAliases { get; set; } = default!;
        public DbSet<AttributeSynonym> AttributeSynonyms { get; set; } = default!;
        public DbSet<AttributeOption> AttributeOptions { get; set; } = default!;
        public DbSet<AttributeOptionTranslation> AttributeOptionTranslations { get; set; } = default!;
        public DbSet<AttributeOptionAlias> AttributeOptionAliases { get; set; } = default!;
        public DbSet<AttributeOptionSynonym> AttributeOptionSynonyms { get; set; } = default!;
        public DbSet<UnitGroup> UnitGroups { get; set; } = default!;
        public DbSet<UnitGroupTranslation> UnitGroupTranslations { get; set; } = default!;
        public DbSet<Unit> Units { get; set; } = default!;
        public DbSet<UnitTranslation> UnitTranslations { get; set; } = default!;
        public DbSet<AttributeTemplate> AttributeTemplates_V3 { get; set; } = default!; // adı mevcut ile çakışmasın diye _V3
        public DbSet<AttributeTemplateTranslation> AttributeTemplateTranslations { get; set; } = default!;
        public DbSet<TemplateAttribute> TemplateAttributes { get; set; } = default!;
        public DbSet<TemplateCategory> TemplateCategories { get; set; } = default!;
        public DbSet<CategoryAttribute> CategoryAttributes { get; set; } = default!;
        public DbSet<AttributeDependency> AttributeDependencies { get; set; } = default!;
        public DbSet<NormalizationRule> NormalizationRules { get; set; } = default!;
        public DbSet<AiGenerationJob> AiGenerationJobs { get; set; } = default!;
        public DbSet<AiGenerationHistory> AiGenerationHistories { get; set; } = default!;
        public DbSet<IntegrationPlatform> IntegrationPlatforms { get; set; } = default!;
        public DbSet<AttributeMapping> AttributeMappings { get; set; } = default!;
        public DbSet<AttributeOptionMapping> AttributeOptionMappings { get; set; } = default!;


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
        public DbSet<Products> Products { get; set; } = default!;
        public DbSet<ProductTranslations> ProductTranslations { get; set; } = default!;
        public DbSet<ProductCategoryLinks> ProductCategoryLinks { get; set; } = default!;
        public DbSet<ProductVariants> ProductVariants { get; set; } = default!;
        public DbSet<ProductVariantAttributeValues> ProductVariantAttributeValues { get; set; } = default!;
        public DbSet<ProductAttributeValues> ProductAttributeValues { get; set; } = default!;
        public DbSet<ProductPrices> ProductPrices { get; set; } = default!;
        public DbSet<ProductMedia> ProductMedia { get; set; } = default!;
        public DbSet<ProductMediaTranslations> ProductMediaTranslations { get; set; } = default!;

        // === _Products (Yorum / Puanlama) ===
        public DbSet<ProductReviews> ProductReviews { get; set; } = default!;
        public DbSet<ProductReviewVotes> ProductReviewVotes { get; set; } = default!;
        public DbSet<ProductReviewReports> ProductReviewReports { get; set; } = default!;
        public DbSet<ProductRatingSummary> ProductRatingSummary { get; set; } = default!;

        // === _Products (Soru-Cevap) ===
        public DbSet<ProductQuestions> ProductQuestions { get; set; } = default!;
        public DbSet<ProductQuestionVotes> ProductQuestionVotes { get; set; } = default!;
        public DbSet<ProductQuestionReports> ProductQuestionReports { get; set; } = default!;

        // === _BulkImportProducts (Toplu Ürün İçe Aktarım) ===
        public DbSet<ImportProfile> ImportProfiles { get; set; } = default!;
        public DbSet<ImportCredential> ImportCredentials { get; set; } = default!;
        public DbSet<ImportFieldMapping> ImportFieldMappings { get; set; } = default!;
        public DbSet<ImportCategoryMapping> ImportCategoryMappings { get; set; } = default!;
        public DbSet<ImportJob> ImportJobs { get; set; } = default!;
        public DbSet<ImportRow> ImportRows { get; set; } = default!;
        public DbSet<ImportRowLog> ImportRowLogs { get; set; } = default!;

        // ================= Fiyat Geçmişi V1 (data._Products ek) =================
        public DbSet<data._Products.ProductPriceHistory> ProductPriceHistory { get; set; } = default!;
        public DbSet<data._Products.ProductPriceDailySnapshot> ProductPriceDailySnapshot { get; set; } = default!;
        public DbSet<data._Products.PriceAlerts> PriceAlerts { get; set; } = default!;

        // ================= Stok ve Çoklu Depo V1 (data._Inventory) =================
        public DbSet<data._Inventory.VariantWarehouseStock> VariantWarehouseStock { get; set; } = default!;
        public DbSet<data._Inventory.StockMovements> StockMovements { get; set; } = default!;
        public DbSet<data._Inventory.StockReservations> StockReservations { get; set; } = default!;
        public DbSet<data._Inventory.StockTransfers> StockTransfers { get; set; } = default!;
        public DbSet<data._Inventory.StockTransferItems> StockTransferItems { get; set; } = default!;

        // === _Shares ===
        public DbSet<Articles> Articles { get; set; } = default!;
        public DbSet<Posts> Posts { get; set; } = default!;

        // === _Store ===
        public DbSet<Store> Store { get; set; } = default!;
        public DbSet<StoreBlockingInfos> StoreBlockingInfos { get; set; } = default!;
        public DbSet<StoreIntegration> StoreIntegration { get; set; } = default!;
        public DbSet<data._Store.WareHouse> WareHouse { get; set; } = default!;

        // ================= Sipariş Sistemi V1 (data._Orders) =================
        public DbSet<data._Orders.Orders> Orders { get; set; } = default!;
        public DbSet<data._Orders.SubOrders> SubOrders { get; set; } = default!;
        public DbSet<data._Orders.OrderItems> OrderItems { get; set; } = default!;
        public DbSet<data._Orders.OrderStatusHistory> OrderStatusHistory { get; set; } = default!;
        public DbSet<data._Orders.OrderInvoices> OrderInvoices { get; set; } = default!;
        public DbSet<data._Orders.CheckoutSessions> CheckoutSessions { get; set; } = default!;
        public DbSet<data._Orders.OrderNumberSequences> OrderNumberSequences { get; set; } = default!;

        // ================= Kargo ve Teslimat V1 (data._Shipping) =================
        public DbSet<data._Shipping.Carriers> Carriers { get; set; } = default!;
        public DbSet<data._Shipping.StoreCarrierAccounts> StoreCarrierAccounts { get; set; } = default!;
        public DbSet<data._Shipping.ShippingZones> ShippingZones { get; set; } = default!;
        public DbSet<data._Shipping.ShippingZoneAreas> ShippingZoneAreas { get; set; } = default!;
        public DbSet<data._Shipping.ShippingRateRules> ShippingRateRules { get; set; } = default!;
        public DbSet<data._Shipping.Shipments> Shipments { get; set; } = default!;
        public DbSet<data._Shipping.ShipmentItems> ShipmentItems { get; set; } = default!;
        public DbSet<data._Shipping.ShipmentTrackingEvents> ShipmentTrackingEvents { get; set; } = default!;

        // ================= Kampanya ve Kupon V1 (data._Promotions) =================
        public DbSet<data._Promotions.Coupons> Coupons { get; set; } = default!;
        public DbSet<data._Promotions.CouponScopes> CouponScopes { get; set; } = default!;
        public DbSet<data._Promotions.CouponUsages> CouponUsages { get; set; } = default!;
        public DbSet<data._Promotions.Campaigns> Campaigns { get; set; } = default!;
        public DbSet<data._Promotions.CampaignScopes> CampaignScopes { get; set; } = default!;

        // ================= Bildirim V1 (data._Notifications) =================
        public DbSet<data._Notifications.NotificationTemplates> NotificationTemplates { get; set; } = default!;
        public DbSet<data._Notifications.NotificationTemplateTranslations> NotificationTemplateTranslations { get; set; } = default!;
        public DbSet<data._Notifications.Notifications> Notifications { get; set; } = default!;
        public DbSet<data._Notifications.NotificationDeliveries> NotificationDeliveries { get; set; } = default!;
        public DbSet<data._Notifications.UserNotificationPreferences> UserNotificationPreferences { get; set; } = default!;

        // ================= İade ve İhtilaf V1 (data._Returns) =================
        public DbSet<data._Returns.ReturnRequests> ReturnRequests { get; set; } = default!;
        public DbSet<data._Returns.ReturnRequestItems> ReturnRequestItems { get; set; } = default!;
        public DbSet<data._Returns.ReturnRequestMedia> ReturnRequestMedia { get; set; } = default!;
        public DbSet<data._Returns.ReturnStatusHistory> ReturnStatusHistory { get; set; } = default!;
        public DbSet<data._Returns.Disputes> Disputes { get; set; } = default!;
        public DbSet<data._Returns.DisputeMessages> DisputeMessages { get; set; } = default!;
        public DbSet<data._Returns.DisputeAttachments> DisputeAttachments { get; set; } = default!;
        public DbSet<data._Returns.ReturnPolicies> ReturnPolicies { get; set; } = default!;


        // ================= Ödeme ve Hakediş Sistemi V1 (data._Payments) =================
        public DbSet<data._Payments.PaymentProviders> PaymentProviders { get; set; } = default!;
        public DbSet<data._Payments.PaymentTransactions> PaymentTransactions { get; set; } = default!;
        public DbSet<data._Payments.UserPaymentMethods> UserPaymentMethods { get; set; } = default!;
        public DbSet<data._Payments.CommissionRates> CommissionRates { get; set; } = default!;
        public DbSet<data._Payments.SellerLedgerEntries> SellerLedgerEntries { get; set; } = default!;
        public DbSet<data._Payments.SellerPayouts> SellerPayouts { get; set; } = default!;
        public DbSet<data._Payments.Refunds> Refunds { get; set; } = default!;

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