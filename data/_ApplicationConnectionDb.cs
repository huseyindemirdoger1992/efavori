using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace data
{
    public class ApplicationConnectionDb : DbContext
    {
        public ApplicationConnectionDb(DbContextOptions<ApplicationConnectionDb> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=mssql02.trwww.com,1433;Database=efavoric_efavori;User Id=efavori;Password=Sx10Kg=X,t2cixiI;TrustServerCertificate=True;MultipleActiveResultSets=True;Encrypt=False;"
                );
            }
        }

        // Media
        public DbSet<Media> Media { get; set; }
        public DbSet<ItemGallery> ItemGallery { get; set; }

        // Logs
        public DbSet<Logs> Logs { get; set; }

        // Users
        public DbSet<Users> Users { get; set; }
        public DbSet<LoginTry> LoginTry { get; set; }
        public DbSet<UserShortcuts> UserShortcuts { get; set; }
        public DbSet<UsersRoles> UsersRoles { get; set; }

        // Languages & Localization
        public DbSet<Languages> Languages { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Cities> Cities { get; set; }
        public DbSet<Regions> Regions { get; set; }
        public DbSet<States> States { get; set; }

        // Categories & Products
        public DbSet<CategoriesAz> CategoriesAz { get; set; }
        public DbSet<CategoriesDe> CategoriesDe { get; set; }
        public DbSet<CategoriesEn> CategoriesEn { get; set; }
        public DbSet<CategoriesEs> CategoriesEs { get; set; }
        public DbSet<CategoriesFr> CategoriesFr { get; set; }
        public DbSet<CategoriesHi> CategoriesHi { get; set; }
        public DbSet<CategoriesPt> CategoriesPt { get; set; }
        public DbSet<CategoriesRu> CategoriesRu { get; set; }
        public DbSet<CategoriesTr> CategoriesTr { get; set; }
        public DbSet<CategoriesZh> CategoriesZh { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Product> Product { get; set; }


    }
}
