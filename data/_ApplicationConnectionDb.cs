using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace data
{
    public class _ApplicationConnectionDb : DbContext
    {
        private const string ConnectionString =
            "Server=mssql02.trwww.com,1433;Database=efavoric_efavori;User Id=efavori;Password=Sx10Kg=X,t2cixiI;TrustServerCertificate=True;MultipleActiveResultSets=True;Encrypt=False;";

        public class _ApplicationConnectionDbFactory : IDesignTimeDbContextFactory<_ApplicationConnectionDb>
        {
            public _ApplicationConnectionDb CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<_ApplicationConnectionDb>();
                optionsBuilder.UseSqlServer(ConnectionString);

                return new _ApplicationConnectionDb(optionsBuilder.Options);
            }
        }

        // Parametresiz constructor eklendi
        public _ApplicationConnectionDb()
            : base()
        {
        }

        public _ApplicationConnectionDb(DbContextOptions<_ApplicationConnectionDb> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(ConnectionString);
        }

        // Media
        // public DbSet<Media> Media { get; set; } = default!;
        // public DbSet<ItemGallery> ItemGallery { get; set; } = default!;

        // Logs
        // public DbSet<Logs> Logs { get; set; } = default!;

        // Users
        // public DbSet<Users> Users { get; set; } = default!;
        // public DbSet<LoginTry> LoginTry { get; set; } = default!;
        // public DbSet<UserShortcuts> UserShortcuts { get; set; } = default!;
        // public DbSet<UsersRoles> UsersRoles { get; set; } = default!;

        // Languages & Localization
        // public DbSet<Languages> Languages { get; set; } = default!;
        public DbSet<Country> Country { get; set; } = default!;
        public DbSet<Cities> Cities { get; set; } = default!;
        public DbSet<Regions> Regions { get; set; } = default!;
        public DbSet<States> States { get; set; } = default!;

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
        // public DbSet<Store> Stores { get; set; } = default!;
        // public DbSet<Product> Product { get; set; } = default!;

    }
}
