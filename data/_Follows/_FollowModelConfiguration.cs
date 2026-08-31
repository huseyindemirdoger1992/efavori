using System;
using Microsoft.EntityFrameworkCore;
using data._Store;
using data._Users;

namespace data._Follows
{
    /// <summary>
    /// Sosyal graf modülünün (arkadaşlık, engelleme, kullanıcı takibi, mağaza takibi)
    /// EF Core Fluent yapılandırması.
    ///
    /// _ApplicationConnectionDb.OnModelCreating içinde şu satırla uygulanır:
    ///     _FollowModelConfiguration.Apply(modelBuilder);
    ///
    /// TASARIM NOTLARI:
    ///  • İş kurallarının bir kısmı (kendini takip etme, kendini engelleme, kendine
    ///    arkadaşlık isteği) CHECK KISITI olarak veritabanına gömülmüştür. Uygulama
    ///    katmanı hatalı da olsa bu veriler asla oluşamaz.
    ///  • Aynı kullanıcı tablosuna iki FK veren entity'lerde (Friendships, UserBlocks,
    ///    UserFollows) her iki FK de NoAction'dır — SQL Server aynı tabloya birden çok
    ///    cascade yolunu reddeder.
    ///  • Arkadaşlık tekilliği hesaplanmış (persisted computed) kolon çifti üzerindedir;
    ///    ayrıntı için bkz. <see cref="Friendships"/>.
    /// </summary>
    public static class _FollowModelConfiguration
    {
        private const string SoftDeleteFilter = "[IsDeleted_IsDeletedStatu] = 0";

        /// <summary>Tüm sosyal graf yapılandırmasını uygular.</summary>
        public static void Apply(ModelBuilder modelBuilder)
        {
            ApplyBaseConventions(modelBuilder);

            ApplyFriendships(modelBuilder);
            ApplyUserBlocks(modelBuilder);
            ApplyUserFollows(modelBuilder);
            ApplyStoreFollowers(modelBuilder);
        }

        private static void ApplyBaseConventions(ModelBuilder modelBuilder)
        {
            ConfigureBase<Friendships>(modelBuilder);
            ConfigureBase<UserBlocks>(modelBuilder);
            ConfigureBase<UserFollows>(modelBuilder);
            ConfigureBase<StoreFollowers>(modelBuilder);
        }

        private static void ConfigureBase<TEntity>(ModelBuilder modelBuilder)
            where TEntity : FollowEntityBase
        {
            modelBuilder.Entity<TEntity>(b =>
            {
                b.HasKey(x => x.Id);
                b.OwnsOne(x => x.IsDeleted);
                b.Property(x => x.RowVersion).IsRowVersion();
            });
        }

        // ── Friendships ───────────────────────────────────────────────────────
        private static void ApplyFriendships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Friendships>(b =>
            {
                b.ToTable("Friendships", t =>
                {
                    // Kullanıcı kendisine arkadaşlık isteği gönderemez.
                    t.HasCheckConstraint(
                        "CK_Friendships_NotSelf",
                        "[RequestorUserId] <> [ReceiverUserId]");
                });

                b.Property(x => x.Status).HasConversion<byte>();
                b.Property(x => x.RequestMessage).HasMaxLength(512);

                // ── Çift yönlü veri probleminin çözümü ────────────────────────
                // İki kimlikten küçüğü / büyüğü SQL Server tarafından üretilir ve
                // fiziksel olarak saklanır (PERSISTED); böylece indekslenebilir.
                b.Property(x => x.UserPairLowId)
                    .HasComputedColumnSql(
                        "(CASE WHEN [RequestorUserId] < [ReceiverUserId] THEN [RequestorUserId] ELSE [ReceiverUserId] END)",
                        stored: true);

                b.Property(x => x.UserPairHighId)
                    .HasComputedColumnSql(
                        "(CASE WHEN [RequestorUserId] < [ReceiverUserId] THEN [ReceiverUserId] ELSE [RequestorUserId] END)",
                        stored: true);

                // İKİ KULLANICI ARASINDA AYNI ANDA TEK AKTİF İLİŞKİ.
                // Filtre yalnızca "canlı" durumları (1 = Pending, 2 = Accepted) kapsar;
                // reddedilmiş/iptal edilmiş/sonlandırılmış geçmiş kayıtlar birikebilir
                // ve yeni istek gönderilmesini engellemez.
                b.HasIndex(x => new { x.UserPairLowId, x.UserPairHighId })
                    .IsUnique()
                    .HasDatabaseName("UX_Friendships_ActivePair")
                    .HasFilter("[Status] IN (1, 2) AND [IsDeleted_IsDeletedStatu] = 0");

                // Simetrik arkadaş listesi sorguları (her iki yön için ayrı indeks).
                b.HasIndex(x => new { x.UserPairLowId, x.Status });
                b.HasIndex(x => new { x.UserPairHighId, x.Status });

                // "Bana gelen istekler" ve "gönderdiğim istekler" ekranları.
                b.HasIndex(x => new { x.ReceiverUserId, x.Status, x.CreatedAtUtc });
                b.HasIndex(x => new { x.RequestorUserId, x.Status, x.CreatedAtUtc });

                // Süresi dolan istekleri kapatan arka plan servisi.
                b.HasIndex(x => x.ExpiresAtUtc);

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(Friendships.RequestorUserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(Friendships.ReceiverUserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(Friendships.UnfriendedByUserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── UserBlocks ────────────────────────────────────────────────────────
        private static void ApplyUserBlocks(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserBlocks>(b =>
            {
                b.ToTable("UserBlocks", t =>
                {
                    // Kullanıcı kendisini engelleyemez.
                    t.HasCheckConstraint(
                        "CK_UserBlocks_NotSelf",
                        "[BlockingUserId] <> [BlockedUserId]");
                });

                b.Property(x => x.Reason).HasConversion<byte>();
                b.Property(x => x.Description).HasMaxLength(1024);

                // Aynı çift için ikinci bir AKTİF engel kaydı oluşamaz.
                b.HasIndex(x => new { x.BlockingUserId, x.BlockedUserId })
                    .IsUnique()
                    .HasDatabaseName("UX_UserBlocks_ActivePair")
                    .HasFilter("[IsActive] = 1 AND [IsDeleted_IsDeletedStatu] = 0");

                // BlockGuard'ın en sıcak sorgusu: "bu iki kullanıcı arasında engel var mı?"
                // Her iki yön de tek indeks aramasıyla çözülebilmelidir.
                b.HasIndex(x => new { x.BlockingUserId, x.IsActive });
                b.HasIndex(x => new { x.BlockedUserId, x.IsActive });

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(UserBlocks.BlockingUserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(UserBlocks.BlockedUserId))
                    .OnDelete(DeleteBehavior.NoAction);
                // SourceContentReportId için FK, modüller arası döngüsel bağımlılık
                // oluşturmamak adına _SocialModelConfiguration içinde tanımlanır.
            });
        }

        // ── UserFollows ───────────────────────────────────────────────────────
        private static void ApplyUserFollows(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserFollows>(b =>
            {
                b.ToTable("UserFollows", t =>
                {
                    // Kullanıcı kendisini takip edemez.
                    t.HasCheckConstraint(
                        "CK_UserFollows_NotSelf",
                        "[FollowerUserId] <> [FollowingUserId]");
                });

                b.Property(x => x.Status).HasConversion<byte>();
                b.Property(x => x.SourceContext).HasMaxLength(64);

                // Aynı takip iki kez oluşamaz (aktif kayıtlar arasında).
                b.HasIndex(x => new { x.FollowerUserId, x.FollowingUserId })
                    .IsUnique()
                    .HasDatabaseName("UX_UserFollows_ActivePair")
                    .HasFilter("[IsActive] = 1 AND [IsDeleted_IsDeletedStatu] = 0");

                // "Takip ettiklerim" listesi ve akış (feed) kaynak sorgusu.
                b.HasIndex(x => new { x.FollowerUserId, x.Status, x.CreatedAtUtc });
                // "Takipçilerim" listesi.
                b.HasIndex(x => new { x.FollowingUserId, x.Status, x.CreatedAtUtc });
                // Gizli hesaplarda onay bekleyen takip istekleri kuyruğu.
                b.HasIndex(x => new { x.FollowingUserId, x.Status });

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(UserFollows.FollowerUserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(UserFollows.FollowingUserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── StoreFollowers ────────────────────────────────────────────────────
        private static void ApplyStoreFollowers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreFollowers>(b =>
            {
                b.Property(x => x.SourceContext).HasMaxLength(64);

                // Bir kullanıcı bir mağazayı iki kez takip edemez.
                b.HasIndex(x => new { x.UserId, x.StoreId })
                    .IsUnique()
                    .HasDatabaseName("UX_StoreFollowers_ActivePair")
                    .HasFilter("[IsActive] = 1 AND [IsDeleted_IsDeletedStatu] = 0");

                // "Takip ettiğim mağazalar" ve akış kaynak sorgusu.
                b.HasIndex(x => new { x.UserId, x.IsActive, x.CreatedAtUtc });
                // Mağaza takipçi listesi ve bildirim yayını (fan-out).
                b.HasIndex(x => new { x.StoreId, x.IsActive, x.CreatedAtUtc });
                // Kampanya bildirimi hedef kitlesi.
                b.HasIndex(x => new { x.StoreId, x.NotificationEnabled, x.NotifyOnCampaign });
                // Yeni ürün bildirimi hedef kitlesi.
                b.HasIndex(x => new { x.StoreId, x.NotificationEnabled, x.NotifyOnNewProduct });

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(StoreFollowers.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Store)).WithMany()
                    .HasForeignKey(nameof(StoreFollowers.StoreId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
