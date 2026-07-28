using Microsoft.EntityFrameworkCore;

namespace data._Returns
{
    /// <summary>
    /// İade ve İhtilaf Sistemi V1 — tüm FK, indeks, silme davranışı ve decimal precision tanımları.
    /// Mevcut _ProductModelConfiguration / _OrderModelConfiguration deseni ile birebir aynıdır.
    /// İade/ihtilaf kayıtları HUKUKİ KANIT olduğundan silme davranışı tamamen Restrict'tir.
    /// Kullanım: OnModelCreating içinde tek satır → _ReturnModelConfiguration.Apply(modelBuilder);
    /// </summary>
    public static class _ReturnModelConfiguration
    {
        /// <summary>Soft-delete filtreli tekil indeks sabiti (mevcut _ProductModelConfiguration'daki sabitle aynı tutulmalıdır).</summary>
        private const string SoftDeleteFilter = "[IsDeleted_Value] = 0";

        public static void Apply(ModelBuilder modelBuilder)
        {
            // =====================================================================
            // ReturnRequests — iade talebi başlığı
            // =====================================================================
            modelBuilder.Entity<ReturnRequests>(e =>
            {
                e.ToTable("ReturnRequests");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.ReturnNumber).HasMaxLength(32).IsRequired();
                e.Property(x => x.GuestEmail).HasMaxLength(256);
                e.Property(x => x.CustomerNote).HasMaxLength(2000);
                e.Property(x => x.SellerNote).HasMaxLength(2000);
                e.Property(x => x.AdminNote).HasMaxLength(2000);
                e.Property(x => x.IdempotencyKey).HasMaxLength(100).IsRequired();

                e.Property(x => x.RequestedAmount).HasPrecision(18, 2);
                e.Property(x => x.ApprovedAmount).HasPrecision(18, 2);

                e.HasOne(typeof(data._Orders.SubOrders)).WithMany().HasForeignKey(nameof(ReturnRequests.SubOrderId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Orders.Orders)).WithMany().HasForeignKey(nameof(ReturnRequests.OrderId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data.Store)).WithMany().HasForeignKey(nameof(ReturnRequests.StoreId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data.Users)).WithMany().HasForeignKey(nameof(ReturnRequests.UserId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Shipping.Shipments)).WithMany().HasForeignKey(nameof(ReturnRequests.ReturnShipmentId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data.WareHouse)).WithMany().HasForeignKey(nameof(ReturnRequests.TargetWarehouseId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(ReturnPolicies)).WithMany().HasForeignKey(nameof(ReturnRequests.AppliedReturnPolicyId)).OnDelete(DeleteBehavior.Restrict);
                // DisputeId: dairesel bağımlılık (Disputes → ReturnRequests FK'sı zaten var) oluşmaması
                // için bu yönde FK kısıtı konulmaz; iz alanıdır.

                e.HasIndex(x => x.ReturnNumber).IsUnique().HasFilter(SoftDeleteFilter);
                e.HasIndex(x => x.IdempotencyKey).IsUnique().HasFilter(SoftDeleteFilter);         // çift talep engeli
                e.HasIndex(x => new { x.StoreId, x.Status, x.CreatedAtUtc });                     // satıcı iade paneli
                e.HasIndex(x => new { x.UserId, x.CreatedAtUtc });                                // "İadelerim" sayfası
                e.HasIndex(x => x.SubOrderId);                                                    // siparişin iadeleri
                e.HasIndex(x => new { x.Status, x.SellerResponseDeadlineUtc });                   // otomatik onay sweeper'ı
                e.HasIndex(x => new { x.Status, x.CreatedAtUtc });                                // admin iade panosu
            });

            // =====================================================================
            // ReturnRequestItems — iade kalemleri
            // =====================================================================
            modelBuilder.Entity<ReturnRequestItems>(e =>
            {
                e.ToTable("ReturnRequestItems", t =>
                {
                    t.HasCheckConstraint("CK_ReturnRequestItems_Quantity",
                        "[Quantity] > 0 AND ([ApprovedQuantity] IS NULL OR [ApprovedQuantity] >= 0)");
                });
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.Description).HasMaxLength(2000);
                e.Property(x => x.DeductionNote).HasMaxLength(1000);
                e.Property(x => x.ProductNameSnapshot).HasMaxLength(500);
                e.Property(x => x.SkuSnapshot).HasMaxLength(100);

                e.Property(x => x.RequestedAmount).HasPrecision(18, 2);
                e.Property(x => x.ApprovedAmount).HasPrecision(18, 2);

                e.HasOne(typeof(ReturnRequests)).WithMany().HasForeignKey(nameof(ReturnRequestItems.ReturnRequestId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Orders.OrderItems)).WithMany().HasForeignKey(nameof(ReturnRequestItems.OrderItemId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Payments.Refunds)).WithMany().HasForeignKey(nameof(ReturnRequestItems.RefundId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Inventory.StockMovements)).WithMany().HasForeignKey(nameof(ReturnRequestItems.StockMovementId)).OnDelete(DeleteBehavior.Restrict);
                // VariantId / ProductId: iz/denormalize alanlar — ürün yaşam döngüsü iadeyi kilitlemesin.

                // Aynı talepte aynı sipariş kalemi iki kez satırlanamaz (adet birleştirilir).
                e.HasIndex(x => new { x.ReturnRequestId, x.OrderItemId })
                    .IsUnique().HasFilter(SoftDeleteFilter);
                e.HasIndex(x => x.OrderItemId);                                                   // kalemin iade toplamı doğrulaması
                e.HasIndex(x => new { x.ProductId, x.CreatedAtUtc });                             // ürün iade oranı (Faz 8 metriği)
                e.HasIndex(x => new { x.VariantId, x.InspectionResult });                         // stoğa dönen ürün raporu
            });

            // =====================================================================
            // ReturnRequestMedia — kanıt görselleri
            // =====================================================================
            modelBuilder.Entity<ReturnRequestMedia>(e =>
            {
                e.ToTable("ReturnRequestMedia");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.Caption).HasMaxLength(500);

                e.HasOne(typeof(ReturnRequests)).WithMany().HasForeignKey(nameof(ReturnRequestMedia.ReturnRequestId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(ReturnRequestItems)).WithMany().HasForeignKey(nameof(ReturnRequestMedia.ReturnRequestItemId)).OnDelete(DeleteBehavior.Restrict);
                // MediaItemId: iz alanı — kanıt dosyası ayrı yaşam döngüsüne sahiptir.

                e.HasIndex(x => new { x.ReturnRequestId, x.SortOrder });
                e.HasIndex(x => x.ReturnRequestItemId);
            });

            // =====================================================================
            // ReturnStatusHistory — append-only tarihçe
            // =====================================================================
            modelBuilder.Entity<ReturnStatusHistory>(e =>
            {
                e.ToTable("ReturnStatusHistory");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.Note).HasMaxLength(2000);

                e.HasOne(typeof(ReturnRequests)).WithMany().HasForeignKey(nameof(ReturnStatusHistory.ReturnRequestId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(Disputes)).WithMany().HasForeignKey(nameof(ReturnStatusHistory.DisputeId)).OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.ReturnRequestId, x.CreatedAtUtc });                       // iade zaman çizelgesi
            });

            // =====================================================================
            // Disputes — platform hakemliği
            // =====================================================================
            modelBuilder.Entity<Disputes>(e =>
            {
                e.ToTable("Disputes");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.DisputeNumber).HasMaxLength(32).IsRequired();
                e.Property(x => x.GuestEmail).HasMaxLength(256);
                e.Property(x => x.OpeningClaim).HasMaxLength(4000).IsRequired();
                e.Property(x => x.DecisionReason).HasMaxLength(4000);
                e.Property(x => x.IdempotencyKey).HasMaxLength(100).IsRequired();

                e.Property(x => x.ClaimedAmount).HasPrecision(18, 2);
                e.Property(x => x.DecidedRefundAmount).HasPrecision(18, 2);

                e.HasOne(typeof(ReturnRequests)).WithMany().HasForeignKey(nameof(Disputes.ReturnRequestId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Orders.SubOrders)).WithMany().HasForeignKey(nameof(Disputes.SubOrderId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Orders.Orders)).WithMany().HasForeignKey(nameof(Disputes.OrderId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data.Store)).WithMany().HasForeignKey(nameof(Disputes.StoreId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data.Users)).WithMany().HasForeignKey(nameof(Disputes.UserId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Payments.Refunds)).WithMany().HasForeignKey(nameof(Disputes.RefundId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Payments.SellerLedgerEntries)).WithMany().HasForeignKey(nameof(Disputes.PenaltyLedgerEntryId)).OnDelete(DeleteBehavior.Restrict);
                // DecidedByUserId: Users'a ikinci FK — EF çoklu FK'de gölge kolon adı çakışabileceğinden
                // yalnızca iz alanı olarak bırakılmıştır.

                e.HasIndex(x => x.DisputeNumber).IsUnique().HasFilter(SoftDeleteFilter);
                e.HasIndex(x => x.IdempotencyKey).IsUnique().HasFilter(SoftDeleteFilter);         // çift ihtilaf engeli
                e.HasIndex(x => new { x.Status, x.CreatedAtUtc });                                // hakem çalışma kuyruğu
                e.HasIndex(x => new { x.StoreId, x.Status, x.CreatedAtUtc });                     // satıcı ihtilaf oranı (Faz 8)
                e.HasIndex(x => new { x.UserId, x.CreatedAtUtc });                                // müşteri ihtilaf listesi
                e.HasIndex(x => new { x.Status, x.SellerResponseDeadlineUtc });                   // süre aşımı sweeper'ı
                e.HasIndex(x => new { x.Status, x.EvidenceDeadlineUtc });                         // kanıt süresi sweeper'ı
                e.HasIndex(x => x.ReturnRequestId);
            });

            // =====================================================================
            // DisputeMessages — append-only yazışma
            // =====================================================================
            modelBuilder.Entity<DisputeMessages>(e =>
            {
                e.ToTable("DisputeMessages");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.Message).HasMaxLength(4000).IsRequired();

                e.HasOne(typeof(Disputes)).WithMany().HasForeignKey(nameof(DisputeMessages.DisputeId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data.Users)).WithMany().HasForeignKey(nameof(DisputeMessages.SenderUserId)).OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.DisputeId, x.CreatedAtUtc });                             // yazışma akışı
                e.HasIndex(x => new { x.DisputeId, x.IsInternalNote });                           // taraflara gösterilecek mesajlar
            });

            // =====================================================================
            // DisputeAttachments — kanıt ekleri
            // =====================================================================
            modelBuilder.Entity<DisputeAttachments>(e =>
            {
                e.ToTable("DisputeAttachments");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.Caption).HasMaxLength(500);

                e.HasOne(typeof(Disputes)).WithMany().HasForeignKey(nameof(DisputeAttachments.DisputeId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(DisputeMessages)).WithMany().HasForeignKey(nameof(DisputeAttachments.DisputeMessageId)).OnDelete(DeleteBehavior.Restrict);
                // MediaItemId: iz alanı.

                e.HasIndex(x => new { x.DisputeId, x.SortOrder });
                e.HasIndex(x => x.DisputeMessageId);
            });

            // =====================================================================
            // ReturnPolicies — kapsam bazlı iade politikaları
            // =====================================================================
            modelBuilder.Entity<ReturnPolicies>(e =>
            {
                e.ToTable("ReturnPolicies", t =>
                {
                    t.HasCheckConstraint("CK_ReturnPolicies_Window", "[ReturnWindowDays] >= 0");
                });
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.PolicyText).HasMaxLength(4000);
                e.Property(x => x.Note).HasMaxLength(500);

                e.HasOne(typeof(data.Store)).WithMany().HasForeignKey(nameof(ReturnPolicies.StoreId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data.CategoriesProduct)).WithMany().HasForeignKey(nameof(ReturnPolicies.CategoryId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data.Country)).WithMany().HasForeignKey(nameof(ReturnPolicies.CountryId)).OnDelete(DeleteBehavior.Restrict);

                // Kapsam çözümünün ana indeksi (README §6).
                e.HasIndex(x => new { x.StoreId, x.CategoryId, x.CountryId, x.ValidFromUtc });
                e.HasIndex(x => new { x.ScopeType, x.ValidToUtc });                               // geçerli politika taraması
            });
        }
    }
}