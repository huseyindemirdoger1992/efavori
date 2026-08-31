using System;
using Microsoft.EntityFrameworkCore;
using data._Store;
using data._Users;

namespace data._Galleries
{
    /// <summary>
    /// Merkezî medya sisteminin EF Core Fluent yapılandırması.
    ///
    /// Bu yapılandırma daha önce <c>_ProductModelConfiguration.ApplyMedia</c> içindeydi.
    /// Medya artık ürüne değil TÜM domainlere hizmet ettiği için kendi modülüne taşındı;
    /// ürün yapılandırması artık medya tablolarına dokunmaz (tek bir yerde tanımlanır,
    /// çift yapılandırma riski ortadan kalkar).
    ///
    /// _ApplicationConnectionDb.OnModelCreating içinde şu satırla uygulanır:
    ///     _MediaModelConfiguration.Apply(modelBuilder);
    /// ve _ProductModelConfiguration.Apply'dan ÖNCE çağrılmalıdır.
    /// </summary>
    public static class _MediaModelConfiguration
    {
        private const string SoftDeleteFilter = "[IsDeleted_IsDeletedStatu] = 0";

        /// <summary>Media ve MediaItems yapılandırmasını uygular.</summary>
        public static void Apply(ModelBuilder modelBuilder)
        {
            ApplyMedia(modelBuilder);
            ApplyMediaItems(modelBuilder);
        }

        // ── Media — fiziksel asset ────────────────────────────────────────────
        private static void ApplyMedia(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Media>(b =>
            {
                b.HasKey(x => x.Id);
                b.OwnsOne(x => x.IsDeleted);
                b.Property(x => x.RowVersion).IsRowVersion();

                b.Property(x => x.FileName).HasMaxLength(512);
                b.Property(x => x.FileStoredName).HasMaxLength(512);
                b.Property(x => x.ContentType).HasMaxLength(255);
                b.Property(x => x.FileExtensionType).HasMaxLength(20);
                b.Property(x => x.StorageProvider).HasMaxLength(100);
                b.Property(x => x.StorageBucket).HasMaxLength(255);
                b.Property(x => x.StorageKey).HasMaxLength(1024);
                b.Property(x => x.FileUrl).HasMaxLength(2048);
                b.Property(x => x.OrjFileUrl).HasMaxLength(2048);
                b.Property(x => x.FilePhysicalPathRoad).HasMaxLength(2048);
                b.Property(x => x.OrjFilePhysicalPathRoad).HasMaxLength(2048);
                b.Property(x => x.FileUrl_Ratio_1_2).HasMaxLength(2048);
                b.Property(x => x.FileUrl_Ratio_1_4).HasMaxLength(2048);
                b.Property(x => x.FileUrl_Ratio_1_8).HasMaxLength(2048);
                b.Property(x => x.FileUrl_Ratio_1_16).HasMaxLength(2048);
                b.Property(x => x.Sha256).HasMaxLength(64);
                b.Property(x => x.ETag).HasMaxLength(255);
                b.Property(x => x.Codec).HasMaxLength(100);
                b.Property(x => x.BlurHash).HasMaxLength(512);
                b.Property(x => x.DominantColor).HasMaxLength(32);
                b.Property(x => x.ExternalUrl).HasMaxLength(2048);
                b.Property(x => x.ExternalProvider).HasMaxLength(100);
                b.Property(x => x.ExternalVideoId).HasMaxLength(255);
                b.Property(x => x.ProcessingError).HasMaxLength(4000);

                b.Property(x => x.MediaType).HasConversion<byte>();
                b.Property(x => x.ProcessingStatus).HasConversion<byte>();
                b.Property(x => x.Visibility).HasConversion<byte>();

                b.Property(x => x.FrameRate).HasPrecision(9, 3);
                b.Property(x => x.ModerationScore).HasPrecision(5, 4);

                // Aynı içeriğin tekrar yüklenmesini tespit (deduplication).
                b.HasIndex(x => x.Sha256);
                // Kullanıcı ve mağaza medya kütüphanesi.
                b.HasIndex(x => new { x.UserId, x.CreatedAtUtc });
                b.HasIndex(x => new { x.OwnerStoreId, x.CreatedAtUtc });
                // Arka plan işleme kuyruğu.
                b.HasIndex(x => new { x.ProcessingStatus, x.CreatedAtUtc });
                // Tür bazlı filtreleme (medya seçici ekranı).
                b.HasIndex(x => new { x.MediaType, x.CreatedAtUtc });
                // Erişim düzeyi denetimi.
                b.HasIndex(x => x.Visibility);
                // Kullanılmayan asset temizliği (UsageCount = 0).
                b.HasIndex(x => x.UsageCount);
                // Moderasyon kuyruğu.
                b.HasIndex(x => x.IsFlaggedByModeration);

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(Media.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Store)).WithMany()
                    .HasForeignKey(nameof(Media.OwnerStoreId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── MediaItems — kullanım katmanı ─────────────────────────────────────
        private static void ApplyMediaItems(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MediaItems>(b =>
            {
                b.HasKey(x => x.Id);
                b.OwnsOne(x => x.IsDeleted);
                b.Property(x => x.RowVersion).IsRowVersion();

                b.Property(x => x.AltText).HasMaxLength(1000);
                b.Property(x => x.Caption).HasMaxLength(4000);
                b.Property(x => x.Title).HasMaxLength(500);
                b.Property(x => x.LinkUrl).HasMaxLength(2048);

                b.Property(x => x.ItemType).HasConversion<byte>();
                b.Property(x => x.MediaRole).HasConversion<byte>();
                b.Property(x => x.Visibility).HasConversion<byte>();
                b.Property(x => x.Language).HasConversion<byte>();

                b.Property(x => x.FocalPointX).HasPrecision(5, 4);
                b.Property(x => x.FocalPointY).HasPrecision(5, 4);

                // Varlık galerisi — sıralı listeleme (en sıcak sorgu).
                b.HasIndex(x => new { x.ItemType, x.ItemId, x.SortOrder });
                // Varlığın kapak/ana medyası.
                b.HasIndex(x => new { x.ItemType, x.ItemId, x.IsPrimary });
                // Rol bazlı erişim ("bu ürünün 3B modeli var mı?").
                b.HasIndex(x => new { x.ItemType, x.ItemId, x.MediaRole });
                // "Bu asset nerelerde kullanılıyor?" — silme öncesi güvenlik kontrolü.
                b.HasIndex(x => x.MediaId);
                // Kullanıcı bazlı medya kullanım listesi.
                b.HasIndex(x => x.UserId);

                // Aynı asset aynı varlığa aynı rol ve dille ikinci kez bağlanamaz.
                b.HasIndex(x => new { x.ItemType, x.ItemId, x.MediaId, x.MediaRole, x.Language })
                    .IsUnique()
                    .HasDatabaseName("UX_MediaItems_Usage")
                    .HasFilter(SoftDeleteFilter);

                // Kullanım kaydı silinse bile asset silinmez.
                b.HasOne(typeof(Media)).WithMany()
                    .HasForeignKey(nameof(MediaItems.MediaId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(MediaItems.UserId))
                    .OnDelete(DeleteBehavior.NoAction);

                // ItemId için FK TANIMLANMAZ: hedef tablo ItemType'a göre değişen
                // bilinçli polimorfik ilişkidir (§42 istisnası). Referans bütünlüğü
                // gereken sıcak yollar için PostMedia / ProductReviewMedia /
                // ChatMessageMedia / StoreDocuments tabloları kullanılır.
            });
        }
    }
}
