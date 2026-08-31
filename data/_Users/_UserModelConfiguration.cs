using System;
using Microsoft.EntityFrameworkCore;
using data._Galleries;
using data._Locations;
using data.Owned;

namespace data._Users
{
    /// <summary>
    /// Kimlik / Profil / Ayar / Gizlilik / Güvenlik modülünün EF Core Fluent yapılandırması.
    /// Mevcut <c>_ProductModelConfiguration</c> desenini birebir izler.
    ///
    /// _ApplicationConnectionDb.OnModelCreating içinde şu satırla uygulanır:
    ///     _UserModelConfiguration.Apply(modelBuilder);
    ///
    /// KONVANSİYON: navigation property YOK; ilişkiler yalnızca FK üzerinden kurulur.
    /// Tekil indeksler soft-delete filtrelidir; böylece silinmiş bir kaydın e-postası
    /// veya kullanıcı adı yeniden kullanılabilir.
    /// </summary>
    public static class _UserModelConfiguration
    {
        /// <summary>Soft-delete edilmemiş satırları hedefleyen filtreli indeks koşulu.</summary>
        private const string SoftDeleteFilter = "[IsDeleted_IsDeletedStatu] = 0";

        /// <summary>Tüm kimlik/profil yapılandırmasını uygular.</summary>
        public static void Apply(ModelBuilder modelBuilder)
        {
            ApplyBaseConventions(modelBuilder);

            ApplyUsers(modelBuilder);
            ApplyUserProfiles(modelBuilder);
            ApplyUserSettings(modelBuilder);
            ApplyUserPrivacySettings(modelBuilder);
            ApplyUserSecurity(modelBuilder);
            ApplyUserAddress(modelBuilder);
            ApplyUserShortcuts(modelBuilder);
            ApplyEmailHistory(modelBuilder);
            ApplyLoginTry(modelBuilder);
        }

        // ── Ortak konvansiyonlar: owned soft-delete + rowversion ──────────────
        private static void ApplyBaseConventions(ModelBuilder modelBuilder)
        {
            ConfigureBase<Users>(modelBuilder);
            ConfigureBase<UserProfiles>(modelBuilder);
            ConfigureBase<UserSettings>(modelBuilder);
            ConfigureBase<UserPrivacySettings>(modelBuilder);
            ConfigureBase<UserSecurity>(modelBuilder);
            ConfigureBase<UserAddress>(modelBuilder);
            ConfigureBase<UserShortcuts>(modelBuilder);
        }

        private static void ConfigureBase<TEntity>(ModelBuilder modelBuilder)
            where TEntity : UserEntityBase
        {
            modelBuilder.Entity<TEntity>(b =>
            {
                b.HasKey(x => x.Id);
                b.OwnsOne(x => x.IsDeleted);
                b.Property(x => x.RowVersion).IsRowVersion();
                b.HasIndex(x => x.CreatedAtUtc);
            });
        }

        // ── Users — kimlik çekirdeği ──────────────────────────────────────────
        private static void ApplyUsers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>(b =>
            {
                b.ToTable("Users", t =>
                {
                    // Kullanıcı kendi sponsoru olamaz.
                    t.HasCheckConstraint("CK_Users_SponsorNotSelf", "[SponsorUserId] IS NULL OR [SponsorUserId] <> [Id]");
                });

                b.Property(x => x.Email).HasMaxLength(320).IsRequired();
                b.Property(x => x.NormalizedEmail).HasMaxLength(320).IsRequired();
                b.Property(x => x.PhoneCountryCode).HasMaxLength(8);
                b.Property(x => x.PhoneNumber).HasMaxLength(32);
                b.Property(x => x.NormalizedPhoneNumber).HasMaxLength(40);
                b.Property(x => x.AccountStatusReason).HasMaxLength(1024);
                b.Property(x => x.RegistrationIpAddress).HasMaxLength(64);
                b.Property(x => x.LastLoginIpAddress).HasMaxLength(64);
                b.Property(x => x.TermsVersion).HasMaxLength(64);

                b.Property(x => x.AccountStatus).HasConversion<byte>();
                b.Property(x => x.UserType).HasConversion<byte>();
                b.Property(x => x.VendorCapability).HasConversion<byte>();

                // Giriş kimliği: aynı e-posta ile ikinci AKTİF hesap açılamaz.
                b.HasIndex(x => x.NormalizedEmail).IsUnique().HasFilter(SoftDeleteFilter);

                // Telefonla giriş/doğrulama: null olmayan numaralar tekildir.
                b.HasIndex(x => x.NormalizedPhoneNumber)
                    .IsUnique()
                    .HasFilter("[NormalizedPhoneNumber] IS NOT NULL AND [IsDeleted_IsDeletedStatu] = 0");

                // Yönetim panelinde durum/rol bazlı listeleme.
                b.HasIndex(x => new { x.AccountStatus, x.CreatedAtUtc });
                b.HasIndex(x => new { x.UserType, x.AccountStatus });
                // Satıcı başvuru kuyruğu.
                b.HasIndex(x => x.VendorCapability);
                // "Çevrimiçi kullanıcılar" ve son görülme sorguları.
                b.HasIndex(x => x.LastSeenAtUtc);
                // KVKK anonimleştirme kuyruğu (arka plan servisi).
                b.HasIndex(x => x.ScheduledAnonymizationAtUtc);

                // Sponsor kendine referans — döngü riskine karşı NoAction.
                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(Users.SponsorUserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── UserProfiles — sosyal profil (1:1) ────────────────────────────────
        private static void ApplyUserProfiles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserProfiles>(b =>
            {
                b.Property(x => x.Username).HasMaxLength(32).IsRequired();
                b.Property(x => x.NormalizedUsername).HasMaxLength(32).IsRequired();
                b.Property(x => x.DisplayName).HasMaxLength(128);
                b.Property(x => x.FirstName).HasMaxLength(128);
                b.Property(x => x.LastName).HasMaxLength(128);
                b.Property(x => x.Bio).HasMaxLength(2000);
                b.Property(x => x.Website).HasMaxLength(512);
                b.Property(x => x.LocationText).HasMaxLength(256);

                b.Property(x => x.ProfileVisibility).HasConversion<byte>();
                b.Property(x => x.VerificationStatus).HasConversion<byte>();
                b.Property(x => x.Gender).HasConversion<byte>();

                // Sosyal bağlantılar owned tip olarak aynı tabloya açılır.
                b.OwnsOne(x => x.ContactInformation);

                // Users ile 1:1.
                b.HasIndex(x => x.UserId).IsUnique().HasFilter(SoftDeleteFilter);

                // "@kullanici" çözümlemesi ve mention eşlemesi — GLOBAL TEKİL.
                b.HasIndex(x => x.NormalizedUsername).IsUnique().HasFilter(SoftDeleteFilter);

                // Kişi arama ekranı.
                b.HasIndex(x => x.DisplayName);
                // "Doğrulanmış hesaplar" vitrini.
                b.HasIndex(x => new { x.VerificationStatus, x.FollowersCount });
                // Keşfet/öneri sıralaması.
                b.HasIndex(x => new { x.ProfileVisibility, x.FollowersCount });

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(UserProfiles.UserId))
                    .OnDelete(DeleteBehavior.Cascade);

                // Medya merkezî depodadır; profil silinse bile asset silinmez.
                b.HasOne(typeof(Media)).WithMany()
                    .HasForeignKey(nameof(UserProfiles.AvatarMediaId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Media)).WithMany()
                    .HasForeignKey(nameof(UserProfiles.CoverMediaId))
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(typeof(Country)).WithMany()
                    .HasForeignKey(nameof(UserProfiles.CountryId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Cities)).WithMany()
                    .HasForeignKey(nameof(UserProfiles.CityId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── UserSettings (1:1) ────────────────────────────────────────────────
        private static void ApplyUserSettings(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserSettings>(b =>
            {
                b.Property(x => x.TimeZoneId).HasMaxLength(64);
                b.Property(x => x.DateFormat).HasMaxLength(32);

                b.Property(x => x.Language).HasConversion<byte>();
                b.Property(x => x.Currency).HasConversion<byte>();
                b.Property(x => x.FeedSort).HasConversion<byte>();

                b.HasIndex(x => x.UserId).IsUnique().HasFilter(SoftDeleteFilter);

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(UserSettings.UserId))
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(typeof(Country)).WithMany()
                    .HasForeignKey(nameof(UserSettings.PreferredCountryId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── UserPrivacySettings (1:1) ─────────────────────────────────────────
        private static void ApplyUserPrivacySettings(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserPrivacySettings>(b =>
            {
                b.Property(x => x.WhoCanMessage).HasConversion<byte>();
                b.Property(x => x.WhoCanSendFriendRequest).HasConversion<byte>();
                b.Property(x => x.WhoCanFollow).HasConversion<byte>();
                b.Property(x => x.WhoCanVoiceCall).HasConversion<byte>();
                b.Property(x => x.WhoCanVideoCall).HasConversion<byte>();
                b.Property(x => x.WhoCanComment).HasConversion<byte>();
                b.Property(x => x.WhoCanTag).HasConversion<byte>();
                b.Property(x => x.WhoCanMention).HasConversion<byte>();
                b.Property(x => x.WhoCanShareMyPosts).HasConversion<byte>();
                b.Property(x => x.WhoCanSeeFollowerList).HasConversion<byte>();
                b.Property(x => x.WhoCanSeeFollowingList).HasConversion<byte>();
                b.Property(x => x.WhoCanSeeFriendList).HasConversion<byte>();
                b.Property(x => x.WhoCanSeeBirthDate).HasConversion<byte>();
                b.Property(x => x.WhoCanSeeEmail).HasConversion<byte>();
                b.Property(x => x.WhoCanSeePhone).HasConversion<byte>();
                b.Property(x => x.WhoCanSeeLocation).HasConversion<byte>();

                b.HasIndex(x => x.UserId).IsUnique().HasFilter(SoftDeleteFilter);

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(UserPrivacySettings.UserId))
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        // ── UserSecurity (1:1) ────────────────────────────────────────────────
        private static void ApplyUserSecurity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserSecurity>(b =>
            {
                b.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
                b.Property(x => x.PasswordHashAlgorithm).HasMaxLength(128);
                b.Property(x => x.SecurityStamp).HasMaxLength(64).IsRequired();
                b.Property(x => x.PasswordResetTokenHash).HasMaxLength(128);
                b.Property(x => x.ActivationTokenHash).HasMaxLength(128);
                b.Property(x => x.EmailChangeTokenHash).HasMaxLength(128);
                b.Property(x => x.PendingEmail).HasMaxLength(320);
                b.Property(x => x.PhoneVerificationCodeHash).HasMaxLength(128);
                b.Property(x => x.TwoFactorSecret).HasMaxLength(512);
                b.Property(x => x.LastPasswordChangeIpAddress).HasMaxLength(64);

                b.Property(x => x.TwoFactorMethod).HasConversion<byte>();

                b.HasIndex(x => x.UserId).IsUnique().HasFilter(SoftDeleteFilter);
                // Parola sıfırlama akışında token hash'i ile arama.
                b.HasIndex(x => x.PasswordResetTokenHash);
                b.HasIndex(x => x.ActivationTokenHash);

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(UserSecurity.UserId))
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        // ── UserAddress ───────────────────────────────────────────────────────
        private static void ApplyUserAddress(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAddress>(b =>
            {
                b.Property(x => x.AddressName).HasMaxLength(128).IsRequired();
                b.Property(x => x.ReceiverFirstName).HasMaxLength(128).IsRequired();
                b.Property(x => x.ReceiverLastName).HasMaxLength(128).IsRequired();
                b.Property(x => x.ReceiverEmail).HasMaxLength(320);
                b.Property(x => x.ReceiverPhoneCountryCode).HasMaxLength(8);
                b.Property(x => x.ReceiverPhoneNumber).HasMaxLength(32).IsRequired();
                b.Property(x => x.CountryName).HasMaxLength(128);
                b.Property(x => x.StateName).HasMaxLength(128);
                b.Property(x => x.CityName).HasMaxLength(128);
                b.Property(x => x.AddressLine1).HasMaxLength(512).IsRequired();
                b.Property(x => x.AddressLine2).HasMaxLength(512);
                b.Property(x => x.PostalCode).HasMaxLength(20);
                b.Property(x => x.DeliveryNote).HasMaxLength(1000);
                b.Property(x => x.CompanyName).HasMaxLength(256);
                b.Property(x => x.TaxOffice).HasMaxLength(128);
                b.Property(x => x.TaxNumber).HasMaxLength(32);

                b.Property(x => x.AddressType).HasConversion<byte>();
                b.Property(x => x.Latitude).HasPrecision(9, 6);
                b.Property(x => x.Longitude).HasPrecision(9, 6);

                // Adres defteri listesi ve varsayılan adres çözümü.
                b.HasIndex(x => new { x.UserId, x.IsDefaultShipping });
                b.HasIndex(x => new { x.UserId, x.IsDefaultBilling });
                b.HasIndex(x => new { x.CityId, x.PostalCode });

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(UserAddress.UserId))
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(typeof(Country)).WithMany()
                    .HasForeignKey(nameof(UserAddress.CountryId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(States)).WithMany()
                    .HasForeignKey(nameof(UserAddress.StateId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Cities)).WithMany()
                    .HasForeignKey(nameof(UserAddress.CityId))
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(typeof(Regions)).WithMany()
                    .HasForeignKey(nameof(UserAddress.RegionId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── UserShortcuts ─────────────────────────────────────────────────────
        private static void ApplyUserShortcuts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserShortcuts>(b =>
            {
                b.Property(x => x.ShortcutName).HasMaxLength(128).IsRequired();
                b.Property(x => x.ShortcutUrl).HasMaxLength(2048).IsRequired();
                b.Property(x => x.ShortcutIcon).HasMaxLength(128);

                b.HasIndex(x => new { x.UserId, x.DisplayOrder });

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(UserShortcuts.UserId))
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        // ── EmailHistory (arşiv — soft delete yok) ────────────────────────────
        private static void ApplyEmailHistory(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmailHistory>(b =>
            {
                b.HasKey(x => x.Id);

                b.Property(x => x.FromAddress).HasMaxLength(320).IsRequired();
                b.Property(x => x.ToAddress).HasMaxLength(320).IsRequired();
                b.Property(x => x.CcAddresses).HasMaxLength(2048);
                b.Property(x => x.BccAddresses).HasMaxLength(2048);
                b.Property(x => x.Subject).HasMaxLength(512).IsRequired();
                b.Property(x => x.TraceId).HasMaxLength(128);
                b.Property(x => x.ProviderMessageId).HasMaxLength(256);

                b.HasIndex(x => new { x.UserId, x.SentAtUtc });
                b.HasIndex(x => x.SentAtUtc);
                b.HasIndex(x => x.TraceId);
                b.HasIndex(x => x.ProviderMessageId);

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(EmailHistory.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // ── LoginTry (append-only güvenlik kaydı) ─────────────────────────────
        private static void ApplyLoginTry(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoginTry>(b =>
            {
                b.HasKey(x => x.Id);

                b.Property(x => x.AttemptedIdentifier).HasMaxLength(320);
                b.Property(x => x.FailureReason).HasMaxLength(128);
                b.Property(x => x.IpAddress).HasMaxLength(64);
                b.Property(x => x.UserAgent).HasMaxLength(1024);
                b.Property(x => x.Platform).HasMaxLength(64);
                b.Property(x => x.Browser).HasMaxLength(128);
                b.Property(x => x.DeviceFingerprint).HasMaxLength(128);
                b.Property(x => x.CountryCode).HasMaxLength(2);
                b.Property(x => x.RequestPath).HasMaxLength(1024);

                // "Bu kullanıcının son girişleri" ekranı.
                b.HasIndex(x => new { x.UserId, x.AttemptedAtUtc });
                // Brute-force tespiti: aynı IP'den kısa sürede çok deneme.
                b.HasIndex(x => new { x.IpAddress, x.AttemptedAtUtc });
                // Saklama süresi dolan kayıtların toplu temizliği.
                b.HasIndex(x => x.AttemptedAtUtc);

                b.HasOne(typeof(Users)).WithMany()
                    .HasForeignKey(nameof(LoginTry.UserId))
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
