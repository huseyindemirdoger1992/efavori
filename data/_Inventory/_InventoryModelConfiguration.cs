using Microsoft.EntityFrameworkCore;

namespace data._Inventory
{
    /// <summary>
    /// Stok Defteri ve Çoklu Depo Sistemi V1 — tüm FK, indeks, silme davranışı ve CHECK kısıtları.
    /// Mevcut _ProductModelConfiguration / _OrderModelConfiguration deseni ile birebir aynıdır.
    /// Kullanım: OnModelCreating içinde tek satır → _InventoryModelConfiguration.Apply(modelBuilder);
    /// </summary>
    public static class _InventoryModelConfiguration
    {
        /// <summary>Soft-delete filtreli tekil indeks sabiti (mevcut _ProductModelConfiguration'daki sabitle aynı tutulmalıdır).</summary>
        private const string SoftDeleteFilter = "[IsDeleted_Value] = 0";

        public static void Apply(ModelBuilder modelBuilder)
        {
            // =====================================================================
            // VariantWarehouseStock — canlı bakiye (tek doğruluk kaynağı)
            // =====================================================================
            modelBuilder.Entity<VariantWarehouseStock>(e =>
            {
                e.ToTable("VariantWarehouseStock", t =>
                {
                    // Bakiye asla negatife düşemez: koşullu UPDATE'ler zaten korur,
                    // bunlar hatalı bir kod yolunun sessizce bozmasına karşı son emniyet kemeridir.
                    t.HasCheckConstraint("CK_VariantWarehouseStock_OnHand", "[OnHand] >= 0");
                    t.HasCheckConstraint("CK_VariantWarehouseStock_Reserved", "[Reserved] >= 0");
                    t.HasCheckConstraint("CK_VariantWarehouseStock_Available", "[Reserved] <= [OnHand]");
                });
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.BinLocation).HasMaxLength(50);

                e.HasOne(typeof(data._Products.ProductVariants)).WithMany().HasForeignKey(nameof(VariantWarehouseStock.VariantId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Store.WareHouse)).WithMany().HasForeignKey(nameof(VariantWarehouseStock.WarehouseId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Products.Products)).WithMany().HasForeignKey(nameof(VariantWarehouseStock.ProductId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Store.Store)).WithMany().HasForeignKey(nameof(VariantWarehouseStock.StoreId)).OnDelete(DeleteBehavior.Restrict);

                // (VariantId, WarehouseId) TEKİL — modülün temel değişmezi.
                e.HasIndex(x => new { x.VariantId, x.WarehouseId })
                    .IsUnique().HasFilter(SoftDeleteFilter);

                e.HasIndex(x => new { x.VariantId, x.IsSellable })
                    .IncludeProperties(x => new { x.OnHand, x.Reserved });                      // satılabilir toplam (checkout kontrolü)
                e.HasIndex(x => new { x.WarehouseId, x.OnHand });                               // depo stok listesi
                e.HasIndex(x => new { x.StoreId, x.OnHand, x.ReorderPoint });                   // düşük stok taraması (StockAlert)
                e.HasIndex(x => x.ProductId);                                                   // ürün toplam stoğu
            });

            // =====================================================================
            // StockMovements — append-only hareket defteri
            // =====================================================================
            modelBuilder.Entity<StockMovements>(e =>
            {
                e.ToTable("StockMovements");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.Description).HasMaxLength(1000);
                e.Property(x => x.IdempotencyKey).HasMaxLength(100).IsRequired();

                e.HasOne(typeof(data._Products.ProductVariants)).WithMany().HasForeignKey(nameof(StockMovements.VariantId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Store.WareHouse)).WithMany().HasForeignKey(nameof(StockMovements.WarehouseId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Orders.SubOrders)).WithMany().HasForeignKey(nameof(StockMovements.SubOrderId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Orders.OrderItems)).WithMany().HasForeignKey(nameof(StockMovements.OrderItemId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(StockReservations)).WithMany().HasForeignKey(nameof(StockMovements.StockReservationId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(StockTransfers)).WithMany().HasForeignKey(nameof(StockMovements.StockTransferId)).OnDelete(DeleteBehavior.Restrict);
                // ProductId / StoreId / ImportRowId / ReturnRequestId: iz alanlarıdır.
                // Defter kaydı, kaynak kayıt arşivlenince kilitlenmemeli veya silinmemelidir.

                e.HasIndex(x => x.IdempotencyKey).IsUnique().HasFilter(SoftDeleteFilter);       // retry güvenliği (çift hareket engeli)
                e.HasIndex(x => new { x.VariantId, x.WarehouseId, x.CreatedAtUtc });            // bakiye mutabakatı + hareket dökümü
                e.HasIndex(x => new { x.WarehouseId, x.CreatedAtUtc });                         // depo günlük hareket raporu
                e.HasIndex(x => new { x.MovementType, x.CreatedAtUtc });                        // "bu ay ne kadar fire" tipi raporlar
                e.HasIndex(x => x.SubOrderId);                                                  // siparişin stok izi
                e.HasIndex(x => x.StockTransferId);                                             // transferin stok izi
            });

            // =====================================================================
            // StockReservations — rezervasyon kayıtları
            // =====================================================================
            modelBuilder.Entity<StockReservations>(e =>
            {
                e.ToTable("StockReservations", t =>
                {
                    t.HasCheckConstraint("CK_StockReservations_Quantity", "[Quantity] > 0");
                });
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.IdempotencyKey).HasMaxLength(100).IsRequired();

                e.HasOne(typeof(data._Products.ProductVariants)).WithMany().HasForeignKey(nameof(StockReservations.VariantId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Store.WareHouse)).WithMany().HasForeignKey(nameof(StockReservations.WarehouseId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Orders.CheckoutSessions)).WithMany().HasForeignKey(nameof(StockReservations.CheckoutSessionId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Orders.SubOrders)).WithMany().HasForeignKey(nameof(StockReservations.SubOrderId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Orders.OrderItems)).WithMany().HasForeignKey(nameof(StockReservations.OrderItemId)).OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => x.IdempotencyKey).IsUnique().HasFilter(SoftDeleteFilter);       // çift rezervasyon engeli
                e.HasIndex(x => new { x.Status, x.ExpiresAtUtc });                              // süresi dolan rezervasyon sweeper'ı
                e.HasIndex(x => new { x.VariantId, x.WarehouseId, x.Status });                  // Reserved bakiyesinin mutabakatı
                e.HasIndex(x => x.CheckoutSessionId);
                e.HasIndex(x => x.SubOrderId);
            });

            // =====================================================================
            // StockTransfers — depolar arası transfer başlığı
            // =====================================================================
            modelBuilder.Entity<StockTransfers>(e =>
            {
                e.ToTable("StockTransfers", t =>
                {
                    t.HasCheckConstraint("CK_StockTransfers_DifferentWarehouses",
                        "[SourceWarehouseId] <> [TargetWarehouseId]");
                });
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.TransferNumber).HasMaxLength(32).IsRequired();
                e.Property(x => x.TrackingNumber).HasMaxLength(100);
                e.Property(x => x.RejectionReason).HasMaxLength(1000);
                e.Property(x => x.Note).HasMaxLength(1000);

                e.HasOne(typeof(data._Store.Store)).WithMany().HasForeignKey(nameof(StockTransfers.StoreId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Store.WareHouse)).WithMany().HasForeignKey(nameof(StockTransfers.SourceWarehouseId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Store.WareHouse)).WithMany().HasForeignKey(nameof(StockTransfers.TargetWarehouseId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Users.Users)).WithMany().HasForeignKey(nameof(StockTransfers.ApprovedByUserId)).OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => x.TransferNumber).IsUnique().HasFilter(SoftDeleteFilter);
                e.HasIndex(x => new { x.StoreId, x.Status, x.CreatedAtUtc });                   // satıcı transfer paneli
                e.HasIndex(x => new { x.SourceWarehouseId, x.Status });                         // çıkış deposu bekleyen sevkiyatlar
                e.HasIndex(x => new { x.TargetWarehouseId, x.Status });                         // varış deposu bekleyen kabuller
            });

            // =====================================================================
            // StockTransferItems — transfer kalemleri
            // =====================================================================
            modelBuilder.Entity<StockTransferItems>(e =>
            {
                e.ToTable("StockTransferItems", t =>
                {
                    t.HasCheckConstraint("CK_StockTransferItems_Quantities",
                        "[RequestedQuantity] > 0 AND [ShippedQuantity] >= 0 AND [ReceivedQuantity] >= 0 AND [DamagedQuantity] >= 0");
                });
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.SkuSnapshot).HasMaxLength(100);
                e.Property(x => x.Note).HasMaxLength(500);

                e.HasOne(typeof(StockTransfers)).WithMany().HasForeignKey(nameof(StockTransferItems.StockTransferId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Products.ProductVariants)).WithMany().HasForeignKey(nameof(StockTransferItems.VariantId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Products.Products)).WithMany().HasForeignKey(nameof(StockTransferItems.ProductId)).OnDelete(DeleteBehavior.Restrict);

                // Aynı transferde aynı varyant iki kez satırlanamaz (adet birleştirilir).
                e.HasIndex(x => new { x.StockTransferId, x.VariantId })
                    .IsUnique().HasFilter(SoftDeleteFilter);
                e.HasIndex(x => x.VariantId);                                                   // varyantın transfer geçmişi
            });
        }
    }
}