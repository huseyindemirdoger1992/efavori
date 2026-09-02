using data._Attribute;
using data._BulkImportProducts;
using data._Carts;
using data._Categories;
using data._Chat;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace data
{
    /// <summary>
    /// efavori uygulama veritabanı bağlamı.
    ///
    /// MİMARİ KURAL: Entity sınıflarında navigation property BULUNMAZ. Tüm ilişkiler,
    /// indeksler, tekillik kuralları, CHECK kısıtları, silme davranışları, enum
    /// dönüşümleri ve decimal hassasiyetleri modül bazlı
    /// <c>_XxxModelConfiguration.Apply(modelBuilder)</c> sınıflarında Fluent API ile
    /// tanımlanır. Bu dosya yalnızca DbSet tanımlarını ve yapılandırma çağrılarını içerir.
    ///
    /// ÖNEMLİ DÜZELTME: Bu sürümden önce <c>OnModelCreating</c> yalnızca DÖRT modülü
    /// (Attribute, Product, ProductReview, BulkImport) uyguluyordu. Sipariş, ödeme,
    /// kargo, kampanya, iade, bildirim ve stok modüllerinin yapılandırmaları yazılmış
    /// ama HİÇ ÇAĞRILMAMIŞTI; dolayısıyla bu modüllerin FK'leri, indeksleri, decimal
    /// hassasiyetleri ve CHECK kısıtları veritabanına HİÇ UYGULANMIYORDU. Tüm modüller
    /// aşağıda bağlanmıştır.
    /// </summary>
    public class _ApplicationConnectionDb : DbContext
    {
        public DbSet<TryTable> TryTable { get; set; } = default!;


        // ═════════════════════════════════════════════════════════════════════
        //  _Galleries — MERKEZÎ MEDYA SİSTEMİ
        //  Platformdaki TEK fiziksel medya deposu (§14, §72).
        // ═════════════════════════════════════════════════════════════════════
        public DbSet<Media> Media { get; set; } = default!;
        public DbSet<MediaItems> MediaItems { get; set; } = default!;

        // ═════════════════════════════════════════════════════════════════════
        //  _Users — KİMLİK / PROFİL / AYAR / GİZLİLİK / GÜVENLİK
        // ═════════════════════════════════════════════════════════════════════
        public DbSet<Users> Users { get; set; } = default!;
        public DbSet<UserProfiles> UserProfiles { get; set; } = default!;
        public DbSet<UserSettings> UserSettings { get; set; } = default!;
        public DbSet<UserPrivacySettings> UserPrivacySettings { get; set; } = default!;
        public DbSet<UserSecurity> UserSecurity { get; set; } = default!;
        public DbSet<UserAddress> UserAddress { get; set; } = default!;
        public DbSet<UserShortcuts> UserShortcuts { get; set; } = default!;
        public DbSet<EmailHistory> EmailHistory { get; set; } = default!;
        public DbSet<LoginTry> LoginTry { get; set; } = default!;
        // KALDIRILDI: UserPayment — tam kart numarası tutuyordu (PCI DSS ihlali).
        //             Yerine PCI uyumlu data._Payments.UserPaymentMethods kullanılır (§35).
        // KALDIRILDI: ChatMessage — yerine data._Chat modülü kullanılır (§33).

        // ═════════════════════════════════════════════════════════════════════
        //  _Follows — SOSYAL GRAF
        // ═════════════════════════════════════════════════════════════════════
        public DbSet<Friendships> Friendships { get; set; } = default!;
        public DbSet<UserBlocks> UserBlocks { get; set; } = default!;
        public DbSet<UserFollows> UserFollows { get; set; } = default!;
        public DbSet<StoreFollowers> StoreFollowers { get; set; } = default!;
        // KALDIRILDI: FriendShip — bool Status + bool Block modeli yetersizdi (§7).
        // KALDIRILDI: StoreShip  — tip güvensiz iki Guid ile modellenmişti (§10).

        // ═════════════════════════════════════════════════════════════════════
        //  _Shares — SOSYAL İÇERİK
        // ═════════════════════════════════════════════════════════════════════
        public DbSet<Posts> Posts { get; set; } = default!;
        public DbSet<PostMedia> PostMedia { get; set; } = default!;
        public DbSet<PostAudienceUsers> PostAudienceUsers { get; set; } = default!;
        public DbSet<PostAudienceRules> PostAudienceRules { get; set; } = default!;
        public DbSet<PostReactions> PostReactions { get; set; } = default!;
        public DbSet<PostComments> PostComments { get; set; } = default!;
        public DbSet<CommentReactions> CommentReactions { get; set; } = default!;
        public DbSet<PostShares> PostShares { get; set; } = default!;
        public DbSet<PostReposts> PostReposts { get; set; } = default!;
        public DbSet<SavedPosts> SavedPosts { get; set; } = default!;
        public DbSet<PostMentions> PostMentions { get; set; } = default!;
        public DbSet<CommentMentions> CommentMentions { get; set; } = default!;
        public DbSet<PostProductTags> PostProductTags { get; set; } = default!;
        public DbSet<Hashtags> Hashtags { get; set; } = default!;
        public DbSet<PostHashtags> PostHashtags { get; set; } = default!;
        public DbSet<Articles> Articles { get; set; } = default!;
        public DbSet<ContentReports> ContentReports { get; set; } = default!;
        public DbSet<ContentModerationActions> ContentModerationActions { get; set; } = default!;
        public DbSet<ContentViewEvents> ContentViewEvents { get; set; } = default!;
        public DbSet<ContentViewDailyAggregates> ContentViewDailyAggregates { get; set; } = default!;

        // ═════════════════════════════════════════════════════════════════════
        //  _Chat — MESAJLAŞMA
        // ═════════════════════════════════════════════════════════════════════
        public DbSet<ChatConversations> ChatConversations { get; set; } = default!;
        public DbSet<ChatParticipants> ChatParticipants { get; set; } = default!;
        public DbSet<ChatMessages> ChatMessages { get; set; } = default!;
        public DbSet<ChatMessageReads> ChatMessageReads { get; set; } = default!;
        public DbSet<ChatMessageMedia> ChatMessageMedia { get; set; } = default!;

        // ═════════════════════════════════════════════════════════════════════
        //  _Store — MAĞAZA
        // ═════════════════════════════════════════════════════════════════════
        public DbSet<Store> Store { get; set; } = default!;
        public DbSet<StoreDocuments> StoreDocuments { get; set; } = default!;
        public DbSet<StoreFeaturedItems> StoreFeaturedItems { get; set; } = default!;
        public DbSet<StoreBlockingInfos> StoreBlockingInfos { get; set; } = default!;
        public DbSet<StoreIntegration> StoreIntegration { get; set; } = default!;
        public DbSet<WareHouse> WareHouse { get; set; } = default!;

        // ═════════════════════════════════════════════════════════════════════
        //  _Carts — SEPET
        // ═════════════════════════════════════════════════════════════════════
        public DbSet<CartsProduct> CartsProduct { get; set; } = default!;
        // KALDIRILDI: CartsFavorite — sepet satırının kopyasıydı; favori kavramı
        //             ProductFavorites ve Wishlists tablolarına taşındı (§29, §71).

        // ═════════════════════════════════════════════════════════════════════
        //  _Categories — KATEGORİLER
        // ═════════════════════════════════════════════════════════════════════
        public DbSet<CategoriesArticle> CategoriesArticle { get; set; } = default!;
        public DbSet<CategoriesProduct> CategoriesProduct { get; set; } = default!;

        // ═════════════════════════════════════════════════════════════════════
        //  _Attribute — Enterprise Marketplace Attribute System V3
        // ═════════════════════════════════════════════════════════════════════
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
        public DbSet<AttributeTemplate> AttributeTemplates_V3 { get; set; } = default!;
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

        // ═════════════════════════════════════════════════════════════════════
        //  _Products — KATALOG
        // ═════════════════════════════════════════════════════════════════════
        public DbSet<Brands> Brands { get; set; } = default!;
        public DbSet<MoneyExchangeRate> MoneyExchangeRate { get; set; } = default!;
        public DbSet<Products> Products { get; set; } = default!;
        public DbSet<ProductTranslations> ProductTranslations { get; set; } = default!;
        public DbSet<ProductCategoryLinks> ProductCategoryLinks { get; set; } = default!;
        public DbSet<ProductVariants> ProductVariants { get; set; } = default!;
        public DbSet<ProductVariantAttributeValues> ProductVariantAttributeValues { get; set; } = default!;
        public DbSet<ProductAttributeValues> ProductAttributeValues { get; set; } = default!;
        public DbSet<ProductPrices> ProductPrices { get; set; } = default!;

        // _Products — Yorum / Puanlama
        public DbSet<ProductReviews> ProductReviews { get; set; } = default!;
        public DbSet<ProductReviewMedia> ProductReviewMedia { get; set; } = default!;
        public DbSet<ProductReviewVotes> ProductReviewVotes { get; set; } = default!;
        public DbSet<ProductReviewReports> ProductReviewReports { get; set; } = default!;
        public DbSet<ProductRatingSummary> ProductRatingSummary { get; set; } = default!;

        // _Products — Soru / Cevap
        public DbSet<ProductQuestions> ProductQuestions { get; set; } = default!;
        public DbSet<ProductQuestionMedia> ProductQuestionMedia { get; set; } = default!;
        public DbSet<ProductQuestionVotes> ProductQuestionVotes { get; set; } = default!;
        public DbSet<ProductQuestionReports> ProductQuestionReports { get; set; } = default!;

        // _Products — Sosyal etkileşim (§25, §29)
        public DbSet<ProductFavorites> ProductFavorites { get; set; } = default!;
        public DbSet<ProductReactions> ProductReactions { get; set; } = default!;
        public DbSet<ProductShares> ProductShares { get; set; } = default!;
        public DbSet<Wishlists> Wishlists { get; set; } = default!;
        public DbSet<WishlistItems> WishlistItems { get; set; } = default!;

        // _Products — Fiyat geçmişi
        public DbSet<ProductPriceHistory> ProductPriceHistory { get; set; } = default!;
        public DbSet<ProductPriceDailySnapshot> ProductPriceDailySnapshot { get; set; } = default!;
        public DbSet<PriceAlerts> PriceAlerts { get; set; } = default!;

        // ═════════════════════════════════════════════════════════════════════
        //  _BulkImportProducts — TOPLU ÜRÜN İÇE AKTARIM
        // ═════════════════════════════════════════════════════════════════════
        public DbSet<ImportProfile> ImportProfiles { get; set; } = default!;
        public DbSet<ImportCredential> ImportCredentials { get; set; } = default!;
        public DbSet<ImportFieldMapping> ImportFieldMappings { get; set; } = default!;
        public DbSet<ImportCategoryMapping> ImportCategoryMappings { get; set; } = default!;
        public DbSet<ImportJob> ImportJobs { get; set; } = default!;
        public DbSet<ImportRow> ImportRows { get; set; } = default!;
        public DbSet<ImportRowLog> ImportRowLogs { get; set; } = default!;

        // ── Yorum İçe Aktarım (Review Import V1) ────────────────────────────
        public DbSet<ImportReviewRow> ImportReviewRows { get; set; } = default!;
        public DbSet<ImportReviewRowLog> ImportReviewRowLogs { get; set; } = default!;

        // ═════════════════════════════════════════════════════════════════════
        //  _Inventory — STOK VE ÇOKLU DEPO
        // ═════════════════════════════════════════════════════════════════════
        public DbSet<VariantWarehouseStock> VariantWarehouseStock { get; set; } = default!;
        public DbSet<StockMovements> StockMovements { get; set; } = default!;
        public DbSet<StockReservations> StockReservations { get; set; } = default!;
        public DbSet<StockTransfers> StockTransfers { get; set; } = default!;
        public DbSet<StockTransferItems> StockTransferItems { get; set; } = default!;

        // ═════════════════════════════════════════════════════════════════════
        //  _Orders — SİPARİŞ
        // ═════════════════════════════════════════════════════════════════════
        public DbSet<Orders> Orders { get; set; } = default!;
        public DbSet<SubOrders> SubOrders { get; set; } = default!;
        public DbSet<OrderItems> OrderItems { get; set; } = default!;
        public DbSet<OrderStatusHistory> OrderStatusHistory { get; set; } = default!;
        public DbSet<OrderInvoices> OrderInvoices { get; set; } = default!;
        public DbSet<CheckoutSessions> CheckoutSessions { get; set; } = default!;
        public DbSet<OrderNumberSequences> OrderNumberSequences { get; set; } = default!;

        // ═════════════════════════════════════════════════════════════════════
        //  _Payments — ÖDEME VE HAKEDİŞ
        // ═════════════════════════════════════════════════════════════════════
        public DbSet<PaymentProviders> PaymentProviders { get; set; } = default!;
        public DbSet<PaymentTransactions> PaymentTransactions { get; set; } = default!;
        public DbSet<UserPaymentMethods> UserPaymentMethods { get; set; } = default!;
        public DbSet<CommissionRates> CommissionRates { get; set; } = default!;
        public DbSet<SellerLedgerEntries> SellerLedgerEntries { get; set; } = default!;
        public DbSet<SellerPayouts> SellerPayouts { get; set; } = default!;
        public DbSet<Refunds> Refunds { get; set; } = default!;

        // ═════════════════════════════════════════════════════════════════════
        //  _Shipping — KARGO VE TESLİMAT
        // ═════════════════════════════════════════════════════════════════════
        public DbSet<Carriers> Carriers { get; set; } = default!;
        public DbSet<StoreCarrierAccounts> StoreCarrierAccounts { get; set; } = default!;
        public DbSet<ShippingZones> ShippingZones { get; set; } = default!;
        public DbSet<ShippingZoneAreas> ShippingZoneAreas { get; set; } = default!;
        public DbSet<ShippingRateRules> ShippingRateRules { get; set; } = default!;
        public DbSet<Shipments> Shipments { get; set; } = default!;
        public DbSet<ShipmentItems> ShipmentItems { get; set; } = default!;
        public DbSet<ShipmentTrackingEvents> ShipmentTrackingEvents { get; set; } = default!;

        // ═════════════════════════════════════════════════════════════════════
        //  _Promotions — KAMPANYA VE KUPON
        // ═════════════════════════════════════════════════════════════════════
        public DbSet<Coupons> Coupons { get; set; } = default!;
        public DbSet<CouponScopes> CouponScopes { get; set; } = default!;
        public DbSet<CouponUsages> CouponUsages { get; set; } = default!;
        public DbSet<Campaigns> Campaigns { get; set; } = default!;
        public DbSet<CampaignScopes> CampaignScopes { get; set; } = default!;

        // ═════════════════════════════════════════════════════════════════════
        //  _Returns — İADE VE İHTİLAF
        // ═════════════════════════════════════════════════════════════════════
        public DbSet<ReturnRequests> ReturnRequests { get; set; } = default!;
        public DbSet<ReturnRequestItems> ReturnRequestItems { get; set; } = default!;
        public DbSet<ReturnRequestMedia> ReturnRequestMedia { get; set; } = default!;
        public DbSet<ReturnStatusHistory> ReturnStatusHistory { get; set; } = default!;
        public DbSet<Disputes> Disputes { get; set; } = default!;
        public DbSet<DisputeMessages> DisputeMessages { get; set; } = default!;
        public DbSet<DisputeAttachments> DisputeAttachments { get; set; } = default!;
        public DbSet<ReturnPolicies> ReturnPolicies { get; set; } = default!;

        // ═════════════════════════════════════════════════════════════════════
        //  _Notifications — BİLDİRİM
        // ═════════════════════════════════════════════════════════════════════
        public DbSet<NotificationTemplates> NotificationTemplates { get; set; } = default!;
        public DbSet<NotificationTemplateTranslations> NotificationTemplateTranslations { get; set; } = default!;
        public DbSet<Notifications> Notifications { get; set; } = default!;
        public DbSet<NotificationDeliveries> NotificationDeliveries { get; set; } = default!;
        public DbSet<UserNotificationPreferences> UserNotificationPreferences { get; set; } = default!;

        // ═════════════════════════════════════════════════════════════════════
        //  _Locations — COĞRAFİ REFERANS TABLOLARI (int PK)
        // ═════════════════════════════════════════════════════════════════════
        public DbSet<Country> Country { get; set; } = default!;
        public DbSet<States> States { get; set; } = default!;
        public DbSet<Cities> Cities { get; set; } = default!;
        public DbSet<Regions> Regions { get; set; } = default!;

        // ═════════════════════════════════════════════════════════════════════
        //  _Helper / _Systems / _Tasks
        // ═════════════════════════════════════════════════════════════════════
        public DbSet<SupportTickets> SupportTickets { get; set; } = default!;
        public DbSet<AuditLogs> AuditLogs { get; set; } = default!;
        public DbSet<AccountPermissions> AccountPermissions { get; set; } = default!;
        public DbSet<AllBackgroundServicesFrequencyRate> AllBackgroundServicesFrequencyRate { get; set; } = default!;
        public DbSet<Logs> Logs { get; set; } = default!;
        public DbSet<MainCssJs> MainCssJs { get; set; } = default!;
        // KALDIRILDI: TryTableSingle — üretimde karşılığı olmayan deneme tablosuydu (§51).

        public DbSet<TaskFramework> TaskFramework { get; set; } = default!;
        public DbSet<TaskCategories> TaskCategories { get; set; } = default!;
        public DbSet<data._Tasks.TaskStatus> TaskStatus { get; set; } = default!;
        public DbSet<TaskKeeperJoint> TaskKeeperJoint { get; set; } = default!;
        public DbSet<TaskNotes> TaskNotes { get; set; } = default!;

        //---------------- Constructor'lar ----------------//

        public _ApplicationConnectionDb(DbContextOptions<_ApplicationConnectionDb> options)
            : base(options)
        {
        }

        public _ApplicationConnectionDb()
        {
        }

        //---------------- Model yapılandırması ----------------//

        /// <summary>
        /// Modül yapılandırmalarını uygular.
        ///
        /// SIRA ÖNEMLİDİR:
        ///  1) Medya en başta gelir — diğer modüllerin çoğu Media'ya FK verir.
        ///  2) Katalog (Attribute → Product → Review) kendi içinde sıralıdır.
        ///  3) Kimlik ve mağaza, sosyal modüllerden ÖNCE gelir.
        ///  4) Sosyal ve sohbet modülleri en sona yakın gelir (çapraz FK'ler bunlara bakar).
        ///  5) Global soft-delete filtresi (etkinse) EN SON uygulanır.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── 1) Merkezî medya (§14, §72) ──────────────────────────────────
            // Media ve MediaItems yapılandırması, ürün modülünden buraya taşındı;
            // medya tüm domainlere hizmet ettiği için tek bir yerde tanımlanır.
            _MediaModelConfiguration.Apply(modelBuilder);

            // ── 2) Katalog ───────────────────────────────────────────────────
            _AttributeModelConfiguration.Apply(modelBuilder);
            _ProductModelConfiguration.Apply(modelBuilder);
            _ProductReviewModelConfiguration.Apply(modelBuilder);
            _ProductPriceHistoryModelConfiguration.Apply(modelBuilder);
            _BulkImportModelConfiguration.Apply(modelBuilder);

            // ── 3) Kimlik ve mağaza ──────────────────────────────────────────
            _UserModelConfiguration.Apply(modelBuilder);
            _StoreModelConfiguration.Apply(modelBuilder);

            // ── 4) Ticaret ───────────────────────────────────────────────────
            _InventoryModelConfiguration.Apply(modelBuilder);
            _CartModelConfiguration.Apply(modelBuilder);
            _OrderModelConfiguration.Apply(modelBuilder);
            _PaymentModelConfiguration.Apply(modelBuilder);
            _ShippingModelConfiguration.Apply(modelBuilder);
            _PromotionModelConfiguration.Apply(modelBuilder);
            _ReturnModelConfiguration.Apply(modelBuilder);

            // ── 5) Sosyal ağ ─────────────────────────────────────────────────
            _FollowModelConfiguration.Apply(modelBuilder);
            _ChatModelConfiguration.Apply(modelBuilder);
            _SocialModelConfiguration.Apply(modelBuilder);
            _ProductSocialModelConfiguration.Apply(modelBuilder);

            // ── 6) Bildirim ve sistem ────────────────────────────────────────
            _NotificationModelConfiguration.Apply(modelBuilder);
            _SystemModelConfiguration.Apply(modelBuilder);

            // ── 7) Global soft-delete filtreleri (varsayılan KAPALI) ─────────
            // Gerekçeler ve açma yöntemi için bkz. _SoftDeleteConfiguration.
            _SoftDeleteConfiguration.Apply(modelBuilder);
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

        /// <summary>
        /// Tasarım zamanı (dotnet ef) bağlam üreticisi.
        /// </summary>
        public class _ApplicationConnectionDbFactory : IDesignTimeDbContextFactory<_ApplicationConnectionDb>
        {
            /// <summary>appsettings.json içindeki DefaultConnection ile bağlam oluşturur.</summary>
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
                    throw new InvalidOperationException(
                        $"ERROR: Connection string 'DefaultConnection' not found in appsettings.json. " +
                        $"Searched path: {Path.Combine(basePath, "appsettings.json")}");
                }

                optionsBuilder.UseSqlServer(connectionString);

                return new _ApplicationConnectionDb(optionsBuilder.Options);
            }
        }
    }
}
