using data;
using data.Owned;
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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using data._Attributes;

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

        // === _Attributes ===
        public DbSet<AttributeDefinition> AttributeDefinition { get; set; } = default!;
        public DbSet<AttributeGroup> AttributeGroup { get; set; } = default!;
        public DbSet<AttributeCategoryJoint> AttributeCategoryJoint { get; set; } = default!;
        public DbSet<AttributeOption> AttributeOption { get; set; } = default!;
        public DbSet<AttributeValue> AttributeValue { get; set; } = default!;
        public DbSet<AttributeMapping> AttributeMapping { get; set; } = default!;
        public DbSet<AttributeUnit> AttributeUnit { get; set; } = default!;

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

        // === _Products ===
        public DbSet<Brands> Brands { get; set; } = default!;
        public DbSet<MoneyExchangeRate> MoneyExchangeRate { get; set; } = default!;

        // === _Shares ===
        public DbSet<Articles> Articles { get; set; } = default!;
        public DbSet<Posts> Posts { get; set; } = default!;

        // === _Store ===
        public DbSet<Store> Store { get; set; } = default!;
        public DbSet<StoreBlockingInfos> StoreBlockingInfos { get; set; } = default!;
        public DbSet<StoreIntegration> StoreIntegration { get; set; } = default!;

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