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
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../web"))
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        public class _ApplicationConnectionDbFactory : IDesignTimeDbContextFactory<_ApplicationConnectionDb>
        {
            public _ApplicationConnectionDb CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<_ApplicationConnectionDb>();

                // Mevcut çalışma dizinini al
                string basePath = AppContext.BaseDirectory;

                // Eğer yerelde geliştirme yapıyorsan ve appsettings.json "web" klasöründeyse
                // canlıda ise direkt ana dizindeyse bu kontrol hayat kurtarır:
                if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
                {
                    // Eğer dosya direkt base'de yoksa, yerel geliştirme ortamındaki "../web" yolunu dene
                    basePath = Path.Combine(Directory.GetCurrentDirectory(), "../web");
                }

                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("A (ConnectionString) error occurred! Please check the appsettings.json values.");
                }

                optionsBuilder.UseSqlServer(connectionString);

                return new _ApplicationConnectionDb(optionsBuilder.Options);
            }
        }
    }
}
