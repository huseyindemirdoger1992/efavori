using Microsoft.EntityFrameworkCore;

namespace data._Payments
{
    /// <summary>
    /// Ödeme ve Hakediş Sistemi V1 — tüm FK, indeks, silme davranışı ve decimal precision tanımları.
    /// Mevcut _ProductModelConfiguration / _OrderModelConfiguration deseni ile birebir aynıdır.
    /// Finansal kayıtlar TARİHSEL KANIT olduğundan silme davranışı tamamen Restrict'tir.
    /// Kullanım: OnModelCreating içinde tek satır → _PaymentModelConfiguration.Apply(modelBuilder);
    /// </summary>
    public static class _PaymentModelConfiguration
    {
        /// <summary>Soft-delete filtreli tekil indeks sabiti (mevcut _ProductModelConfiguration'daki sabitle aynı tutulmalıdır).</summary>
        private const string SoftDeleteFilter = "[IsDeleted_Value] = 0";

        public static void Apply(ModelBuilder modelBuilder)
        {
            // =====================================================================
            // PaymentProviders — referans tablosu
            // =====================================================================
            modelBuilder.Entity<PaymentProviders>(e =>
            {
                e.ToTable("PaymentProviders");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.Code).HasMaxLength(50).IsRequired();
                e.Property(x => x.Name).HasMaxLength(100).IsRequired();
                e.Property(x => x.LogoUrl).HasMaxLength(1000);

                e.HasIndex(x => x.Code).IsUnique().HasFilter(SoftDeleteFilter);                 // makine-okur tekil kod
                e.HasIndex(x => new { x.IsActive, x.SortOrder });                               // ödeme sayfası listesi
            });

            // =====================================================================
            // PaymentTransactions — ödeme işlemleri
            // =====================================================================
            modelBuilder.Entity<PaymentTransactions>(e =>
            {
                e.ToTable("PaymentTransactions");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.Amount).HasPrecision(18, 2);
                e.Property(x => x.IdempotencyKey).HasMaxLength(100).IsRequired();
                e.Property(x => x.ProviderTransactionId).HasMaxLength(100);
                e.Property(x => x.ProviderReferenceCode).HasMaxLength(100);
                e.Property(x => x.FailureCode).HasMaxLength(50);
                e.Property(x => x.FailureMessage).HasMaxLength(1000);

                e.HasOne(typeof(data._Orders.Orders)).WithMany().HasForeignKey(nameof(PaymentTransactions.OrderId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(PaymentTransactions)).WithMany().HasForeignKey(nameof(PaymentTransactions.ParentTransactionId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(PaymentProviders)).WithMany().HasForeignKey(nameof(PaymentTransactions.PaymentProviderId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(UserPaymentMethods)).WithMany().HasForeignKey(nameof(PaymentTransactions.UserPaymentMethodId)).OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => x.IdempotencyKey).IsUnique().HasFilter(SoftDeleteFilter);       // çift gönderim DB engeli
                e.HasIndex(x => new { x.OrderId, x.CreatedAtUtc });                             // sipariş ödeme geçmişi
                e.HasIndex(x => new { x.PaymentProviderId, x.ProviderTransactionId });          // webhook eşlemesi
                e.HasIndex(x => new { x.Status, x.CreatedAtUtc });                              // askıda işlem taraması (poll)
            });

            // =====================================================================
            // UserPaymentMethods — PCI uyumlu kayıtlı kartlar
            // =====================================================================
            modelBuilder.Entity<UserPaymentMethods>(e =>
            {
                e.ToTable("UserPaymentMethods");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.ProviderToken).HasMaxLength(200).IsRequired();
                e.Property(x => x.ProviderCustomerReference).HasMaxLength(200);
                e.Property(x => x.CardBrand).HasMaxLength(30).IsRequired();
                e.Property(x => x.Last4).HasMaxLength(4).IsRequired();
                e.Property(x => x.CardAlias).HasMaxLength(100);

                e.HasOne(typeof(data._Users.Users)).WithMany().HasForeignKey(nameof(UserPaymentMethods.UserId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(PaymentProviders)).WithMany().HasForeignKey(nameof(UserPaymentMethods.PaymentProviderId)).OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.UserId, x.PaymentProviderId, x.ProviderToken })
                    .IsUnique().HasFilter(SoftDeleteFilter);                                    // aynı kartın çift kaydını önler
                e.HasIndex(x => new { x.UserId, x.IsDefault });                                 // varsayılan kart çözümü
            });

            // =====================================================================
            // CommissionRates — tarihçeli komisyon oranları
            // =====================================================================
            modelBuilder.Entity<CommissionRates>(e =>
            {
                e.ToTable("CommissionRates");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.RatePercent).HasPrecision(5, 2);
                e.Property(x => x.FixedAmount).HasPrecision(18, 2);
                e.Property(x => x.Note).HasMaxLength(500);

                e.HasOne(typeof(data._Store.Store)).WithMany().HasForeignKey(nameof(CommissionRates.StoreId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Categories.CategoriesProduct)).WithMany().HasForeignKey(nameof(CommissionRates.CategoryId)).OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.StoreId, x.CategoryId, x.ValidFromUtc });               // kapsam çözümü (öncelik sırası sorgusu)
                e.HasIndex(x => x.ValidToUtc);                                                  // aktif oran taraması (NULL = geçerli)
            });

            // =====================================================================
            // SellerLedgerEntries — append-only satıcı defteri
            // =====================================================================
            modelBuilder.Entity<SellerLedgerEntries>(e =>
            {
                e.ToTable("SellerLedgerEntries");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.Amount).HasPrecision(18, 2);
                e.Property(x => x.Description).HasMaxLength(1000);
                e.Property(x => x.IdempotencyKey).HasMaxLength(100).IsRequired();

                e.HasOne(typeof(data._Store.Store)).WithMany().HasForeignKey(nameof(SellerLedgerEntries.StoreId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Orders.SubOrders)).WithMany().HasForeignKey(nameof(SellerLedgerEntries.SubOrderId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Orders.OrderItems)).WithMany().HasForeignKey(nameof(SellerLedgerEntries.OrderItemId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(Refunds)).WithMany().HasForeignKey(nameof(SellerLedgerEntries.RefundId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(PaymentTransactions)).WithMany().HasForeignKey(nameof(SellerLedgerEntries.PaymentTransactionId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(SellerPayouts)).WithMany().HasForeignKey(nameof(SellerLedgerEntries.PayoutId)).OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => x.IdempotencyKey).IsUnique().HasFilter(SoftDeleteFilter);       // aynı olayın çift yazımı engeli
                e.HasIndex(x => new { x.StoreId, x.Currency, x.CreatedAtUtc });                 // bakiye türetimi + ekstre
                e.HasIndex(x => new { x.StoreId, x.Currency, x.PayoutId, x.ReleaseAfterUtc });  // ödenebilir satır taraması (payout oluşturma)
                e.HasIndex(x => x.PayoutId);                                                    // payout detay dökümü
            });

            // =====================================================================
            // SellerPayouts — hakediş ödemeleri (kuyruk + lease)
            // =====================================================================
            modelBuilder.Entity<SellerPayouts>(e =>
            {
                e.ToTable("SellerPayouts");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.Amount).HasPrecision(18, 2);
                e.Property(x => x.IbanSnapshot).HasMaxLength(34).IsRequired();
                e.Property(x => x.AccountHolderSnapshot).HasMaxLength(200).IsRequired();
                e.Property(x => x.FailureMessage).HasMaxLength(1000);
                e.Property(x => x.LeasedBy).HasMaxLength(100);
                e.Property(x => x.IdempotencyKey).HasMaxLength(100).IsRequired();

                e.HasOne(typeof(data._Store.Store)).WithMany().HasForeignKey(nameof(SellerPayouts.StoreId)).OnDelete(DeleteBehavior.Restrict);
                // ReceiptMediaItemId: media temizliği dekont kaydını kilitlemesin diye FK kısıtı konulmaz (iz alanı).

                e.HasIndex(x => x.IdempotencyKey).IsUnique().HasFilter(SoftDeleteFilter);       // dönem başına tek payout garantisi
                e.HasIndex(x => new { x.StoreId, x.PeriodEndUtc });                             // mağaza hakediş geçmişi
                e.HasIndex(x => new { x.Status, x.NextRetryAtUtc, x.LeasedUntilUtc });          // kuyruk taraması (lease alma)
            });

            // =====================================================================
            // Refunds — iade finansal kayıtları
            // =====================================================================
            modelBuilder.Entity<Refunds>(e =>
            {
                e.ToTable("Refunds");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.Amount).HasPrecision(18, 2);
                e.Property(x => x.Note).HasMaxLength(1000);

                e.HasOne(typeof(data._Orders.OrderItems)).WithMany().HasForeignKey(nameof(Refunds.OrderItemId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Orders.SubOrders)).WithMany().HasForeignKey(nameof(Refunds.SubOrderId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Orders.Orders)).WithMany().HasForeignKey(nameof(Refunds.OrderId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Store.Store)).WithMany().HasForeignKey(nameof(Refunds.StoreId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(PaymentTransactions)).WithMany().HasForeignKey(nameof(Refunds.PaymentTransactionId)).OnDelete(DeleteBehavior.Restrict);
                // ReturnRequestId: Faz 7 tablosu henüz yok — iz alanı; FK Faz 7 konfigürasyonunda tanımlanacak.

                e.HasIndex(x => new { x.OrderItemId, x.CreatedAtUtc });                         // kalem iade geçmişi (kısmi iade toplamı)
                e.HasIndex(x => new { x.StoreId, x.Status, x.CreatedAtUtc });                   // satıcı iade paneli
                e.HasIndex(x => new { x.OrderId, x.CreatedAtUtc });                             // müşteri iade listesi
                e.HasIndex(x => new { x.Status, x.CreatedAtUtc });                              // askıda iade taraması
            });
        }
    }
}