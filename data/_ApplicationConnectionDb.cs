using data._Shared;
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
        public DbSet<WorkstationEmployeeGroup> WorkstationEmployeeGroup { get; set; } = default!;

        // Chat
        public DbSet<ChatMessage> ChatMessage { get; set; } = default!;

        // Tasks
        public DbSet<TaskCategories> TaskCategories { get; set; } = default!;
        public DbSet<TaskStatus> TaskStatus { get; set; } = default!;
        public DbSet<TaskNotes> TaskNotes { get; set; } = default!;

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

        // Languages
        public DbSet<Languages> Languages { get; set; } = default!;

        //Localization
        public DbSet<Country> Country { get; set; } = default!;
        public DbSet<Cities> Cities { get; set; } = default!;
        public DbSet<Regions> Regions { get; set; } = default!;
        public DbSet<States> States { get; set; } = default!;
        public DbSet<Addresses> Addresses { get; set; } = default!;

        // Categories
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

        // Products & Stores
        public DbSet<Store> Stores { get; set; } = default!;
        public DbSet<Pricing> Pricing { get; set; } = default!;

        // public DbSet<Product> Product { get; set; } = default!;


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
