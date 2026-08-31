using System;
using Microsoft.EntityFrameworkCore;
using data._Store;
using data._Users;

namespace data._Systems
{
    /// <summary>
    /// Sistem modülünün (denetim kaydı, loglar, hesap izinleri, arka plan servis
    /// ayarları, tema kaynakları) EF Core Fluent yapılandırması.
    ///
    /// _ApplicationConnectionDb.OnModelCreating içinde şu satırla uygulanır:
    ///     _SystemModelConfiguration.Apply(modelBuilder);
    /// </summary>
    public static class _SystemModelConfiguration
    {
        /// <summary>Tüm sistem modülü yapılandırmasını uygular.</summary>
        public static void Apply(ModelBuilder modelBuilder)
        {
            ApplyAuditLogs(modelBuilder);
            ApplyLogs(modelBuilder);
            ApplySupportTickets(modelBuilder);
        }

        // ── AuditLogs (append-only) ───────────────────────────────────────────
        private static void ApplyAuditLogs(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditLogs>(b =>
            {
                b.HasKey(x => x.Id);

                b.Property(x => x.EntityName).HasMaxLength(128).IsRequired();
                b.Property(x => x.Action).HasConversion<byte>();
                b.Property(x => x.SystemActor).HasMaxLength(128);
                b.Property(x => x.Reason).HasMaxLength(2000);
                b.Property(x => x.IpAddress).HasMaxLength(64);
                b.Property(x => x.UserAgent).HasMaxLength(1024);
                b.Property(x => x.RequestPath).HasMaxLength(1024);
                b.Property(x => x.TraceId).HasMaxLength(128);

                // "Bu kaydın değişiklik geçmişi" — en sık kullanılan sorgu.
                b.HasIndex(x => new { x.EntityName, x.EntityId, x.CreatedAtUtc });
                // "Bu kullanıcı ne yaptı" — güvenlik incelemesi.
                b.HasIndex(x => new { x.UserId, x.CreatedAtUtc });
                // Mağaza bazlı denetim.
                b.HasIndex(x => new { x.StoreId, x.CreatedAtUtc });
                // Toplu işlem gruplaması.
                b.HasIndex(x => x.BatchId);
                // Saklama süresi dolan kayıtların toplu temizliği/arşivi.
                b.HasIndex(x => x.CreatedAtUtc);
                // İşlem türüne göre rapor.
                b.HasIndex(x => new { x.Action, x.CreatedAtUtc });

                // FK TANIMLANMAZ (EntityName + EntityId): denetim kaydı, izlediği
                // kayıt silinse bile ayakta kalmalıdır. UserId/StoreId için de
                // FK kurulmaz — kullanıcı anonimleştirilse bile iz korunur.
            });
        }

        // ── Logs (uygulama günlüğü) ───────────────────────────────────────────
        private static void ApplyLogs(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Logs>(b =>
            {
                b.HasKey(x => x.Id);

                // Hata inceleme ekranı: zaman ve seviye bazlı tarama.
                b.HasIndex(x => x.Date);
                b.HasIndex(x => x.UserId);
                b.HasIndex(x => new { x.PageNameSpaceTitle, x.Date });
            });
        }

        // ── SupportTickets ────────────────────────────────────────────────────
        private static void ApplySupportTickets(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<data._Helper.SupportTickets>(b =>
            {
                b.HasKey(x => x.Id);
                b.OwnsOne(x => x.IsDeleted);

                b.HasIndex(x => x.UserId);
                b.HasIndex(x => x.CreatedAt);

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(data._Helper.SupportTickets.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
