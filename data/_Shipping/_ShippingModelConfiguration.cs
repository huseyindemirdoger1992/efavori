using Microsoft.EntityFrameworkCore;

namespace data._Shipping
{
    /// <summary>
    /// Kargo ve Teslimat Sistemi V1 — tüm FK, indeks, silme davranışı ve decimal precision tanımları.
    /// Mevcut _ProductModelConfiguration / _OrderModelConfiguration deseni ile birebir aynıdır.
    /// Kullanım: OnModelCreating içinde tek satır → _ShippingModelConfiguration.Apply(modelBuilder);
    /// </summary>
    public static class _ShippingModelConfiguration
    {
        /// <summary>Soft-delete filtreli tekil indeks sabiti (mevcut _ProductModelConfiguration'daki sabitle aynı tutulmalıdır).</summary>
        private const string SoftDeleteFilter = "[IsDeleted_Value] = 0";

        public static void Apply(ModelBuilder modelBuilder)
        {
            // =====================================================================
            // Carriers — kargo firmaları referans tablosu
            // =====================================================================
            modelBuilder.Entity<Carriers>(e =>
            {
                e.ToTable("Carriers");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.Code).HasMaxLength(50).IsRequired();
                e.Property(x => x.Name).HasMaxLength(150).IsRequired();
                e.Property(x => x.TrackingUrlTemplate).HasMaxLength(500);
                e.Property(x => x.LogoUrl).HasMaxLength(1000);

                e.HasIndex(x => x.Code).IsUnique().HasFilter(SoftDeleteFilter);
                e.HasIndex(x => new { x.IsActive, x.SortOrder });                               // checkout kargo listesi
            });

            // =====================================================================
            // StoreCarrierAccounts — satıcının anlaşmalı kargo hesabı (şifreli credential)
            // =====================================================================
            modelBuilder.Entity<StoreCarrierAccounts>(e =>
            {
                e.ToTable("StoreCarrierAccounts");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.AccountName).HasMaxLength(150).IsRequired();
                e.Property(x => x.EncryptedCredentials).IsRequired();                           // nvarchar(max) — şifreli paket
                e.Property(x => x.EncryptionKeyVersion).HasMaxLength(20);
                e.Property(x => x.MaskedAccountCode).HasMaxLength(50);
                e.Property(x => x.LastErrorMessage).HasMaxLength(1000);

                e.HasOne(typeof(data.Store)).WithMany().HasForeignKey(nameof(StoreCarrierAccounts.StoreId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(Carriers)).WithMany().HasForeignKey(nameof(StoreCarrierAccounts.CarrierId)).OnDelete(DeleteBehavior.Restrict);

                // Aynı mağaza + firma için aynı isimde ikinci hesap açılamaz.
                e.HasIndex(x => new { x.StoreId, x.CarrierId, x.AccountName })
                    .IsUnique().HasFilter(SoftDeleteFilter);
                e.HasIndex(x => new { x.StoreId, x.IsActive, x.IsDefault });                    // varsayılan hesap çözümü
            });

            // =====================================================================
            // ShippingZones — kargo bölgesi başlığı
            // =====================================================================
            modelBuilder.Entity<ShippingZones>(e =>
            {
                e.ToTable("ShippingZones");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.Name).HasMaxLength(150).IsRequired();
                e.Property(x => x.Code).HasMaxLength(50).IsRequired();
                e.Property(x => x.Description).HasMaxLength(500);

                e.HasOne(typeof(data.Store)).WithMany().HasForeignKey(nameof(ShippingZones.StoreId)).OnDelete(DeleteBehavior.Restrict);

                // Kod, sahip (mağaza veya platform) kapsamında tekildir.
                e.HasIndex(x => new { x.StoreId, x.Code }).IsUnique().HasFilter(SoftDeleteFilter);
                e.HasIndex(x => new { x.StoreId, x.IsActive, x.Priority });                     // bölge çözüm sorgusu
            });

            // =====================================================================
            // ShippingZoneAreas — bölge kapsam satırları
            // =====================================================================
            modelBuilder.Entity<ShippingZoneAreas>(e =>
            {
                e.ToTable("ShippingZoneAreas");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.PostalCodeFrom).HasMaxLength(20);
                e.Property(x => x.PostalCodeTo).HasMaxLength(20);

                e.HasOne(typeof(ShippingZones)).WithMany().HasForeignKey(nameof(ShippingZoneAreas.ShippingZoneId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data.Regions)).WithMany().HasForeignKey(nameof(ShippingZoneAreas.RegionId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data.Country)).WithMany().HasForeignKey(nameof(ShippingZoneAreas.CountryId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data.States)).WithMany().HasForeignKey(nameof(ShippingZoneAreas.StateId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data.Cities)).WithMany().HasForeignKey(nameof(ShippingZoneAreas.CityId)).OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => x.ShippingZoneId);
                // Bölge çözümü: adresten yukarı doğru (City → State → Country → Region) arama.
                e.HasIndex(x => new { x.CityId, x.IsExcluded });
                e.HasIndex(x => new { x.StateId, x.IsExcluded });
                e.HasIndex(x => new { x.CountryId, x.IsExcluded });
            });

            // =====================================================================
            // ShippingRateRules — ücret kuralları
            // =====================================================================
            modelBuilder.Entity<ShippingRateRules>(e =>
            {
                e.ToTable("ShippingRateRules", t =>
                {
                    t.HasCheckConstraint("CK_ShippingRateRules_Range",
                        "[RangeTo] IS NULL OR [RangeTo] > [RangeFrom]");
                });
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.RangeFrom).HasPrecision(18, 3);                               // desi/kg ondalığı fiyattan hassas
                e.Property(x => x.RangeTo).HasPrecision(18, 3);
                e.Property(x => x.Price).HasPrecision(18, 2);
                e.Property(x => x.PricePerExtraUnit).HasPrecision(18, 2);
                e.Property(x => x.FreeShippingThreshold).HasPrecision(18, 2);
                e.Property(x => x.Note).HasMaxLength(500);

                e.HasOne(typeof(data.Store)).WithMany().HasForeignKey(nameof(ShippingRateRules.StoreId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(Carriers)).WithMany().HasForeignKey(nameof(ShippingRateRules.CarrierId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(ShippingZones)).WithMany().HasForeignKey(nameof(ShippingRateRules.ShippingZoneId)).OnDelete(DeleteBehavior.Restrict);

                // Checkout'taki ücret çözümünün ana indeksi (README §5).
                e.HasIndex(x => new { x.ShippingZoneId, x.CarrierId, x.RateBasis, x.IsActive, x.RangeFrom });
                e.HasIndex(x => new { x.StoreId, x.IsActive });                                 // mağazanın kendi kuralları
                e.HasIndex(x => x.ValidToUtc);                                                  // geçerli kural taraması
            });

            // =====================================================================
            // Shipments — paketler
            // =====================================================================
            modelBuilder.Entity<Shipments>(e =>
            {
                e.ToTable("Shipments");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.TrackingNumber).HasMaxLength(100);
                e.Property(x => x.TrackingUrl).HasMaxLength(1000);
                e.Property(x => x.DeliveredToName).HasMaxLength(200);
                e.Property(x => x.DeliveryLocationInfo).HasMaxLength(500);
                e.Property(x => x.Note).HasMaxLength(1000);
                e.Property(x => x.LeasedBy).HasMaxLength(100);
                e.Property(x => x.IdempotencyKey).HasMaxLength(100).IsRequired();

                e.Property(x => x.WidthCm).HasPrecision(10, 2);
                e.Property(x => x.LengthCm).HasPrecision(10, 2);
                e.Property(x => x.HeightCm).HasPrecision(10, 2);
                e.Property(x => x.WeightKg).HasPrecision(10, 3);
                e.Property(x => x.Desi).HasPrecision(10, 3);
                e.Property(x => x.CalculatedCost).HasPrecision(18, 2);
                e.Property(x => x.ActualCost).HasPrecision(18, 2);

                e.HasOne(typeof(data._Orders.SubOrders)).WithMany().HasForeignKey(nameof(Shipments.SubOrderId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Orders.Orders)).WithMany().HasForeignKey(nameof(Shipments.OrderId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data.Store)).WithMany().HasForeignKey(nameof(Shipments.StoreId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(Carriers)).WithMany().HasForeignKey(nameof(Shipments.CarrierId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(StoreCarrierAccounts)).WithMany().HasForeignKey(nameof(Shipments.StoreCarrierAccountId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data.WareHouse)).WithMany().HasForeignKey(nameof(Shipments.SourceWarehouseId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Inventory.StockTransfers)).WithMany().HasForeignKey(nameof(Shipments.StockTransferId)).OnDelete(DeleteBehavior.Restrict);
                // LabelMediaItemId / ProofMediaItemId / AppliedRateRuleId / ReturnRequestId: iz alanlarıdır.
                // Media temizliği veya kural değişimi sevkiyat kaydını kilitlememelidir.

                e.HasIndex(x => x.IdempotencyKey).IsUnique().HasFilter(SoftDeleteFilter);       // çift paket engeli
                e.HasIndex(x => new { x.SubOrderId, x.PackageIndex });                          // alt siparişin paketleri
                e.HasIndex(x => new { x.CarrierId, x.TrackingNumber });                         // webhook eşlemesi
                e.HasIndex(x => new { x.StoreId, x.Status, x.CreatedAtUtc });                   // satıcı kargo paneli
                e.HasIndex(x => new { x.Status, x.NextPollAtUtc, x.LeasedUntilUtc });           // takip sorgulama kuyruğu
                e.HasIndex(x => x.OrderId);                                                     // müşteri kargo takibi
                e.HasIndex(x => new { x.Direction, x.ReturnRequestId });                        // iade sevkiyatları (Faz 7)
            });

            // =====================================================================
            // ShipmentItems — paket içerikleri
            // =====================================================================
            modelBuilder.Entity<ShipmentItems>(e =>
            {
                e.ToTable("ShipmentItems", t =>
                {
                    t.HasCheckConstraint("CK_ShipmentItems_Quantity", "[Quantity] > 0");
                });
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.ProductNameSnapshot).HasMaxLength(500);
                e.Property(x => x.SkuSnapshot).HasMaxLength(100);

                e.HasOne(typeof(Shipments)).WithMany().HasForeignKey(nameof(ShipmentItems.ShipmentId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Orders.OrderItems)).WithMany().HasForeignKey(nameof(ShipmentItems.OrderItemId)).OnDelete(DeleteBehavior.Restrict);
                // VariantId: iz/denormalize alan — ürün yaşam döngüsü sevkiyatı kilitlemesin.

                e.HasIndex(x => x.ShipmentId);                                                  // paket içerik listesi (irsaliye)
                e.HasIndex(x => x.OrderItemId);                                                 // kalemin hangi paketlerde olduğu
            });

            // =====================================================================
            // ShipmentTrackingEvents — append-only takip olayları
            // =====================================================================
            modelBuilder.Entity<ShipmentTrackingEvents>(e =>
            {
                e.ToTable("ShipmentTrackingEvents");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.RawStatusCode).HasMaxLength(50);
                e.Property(x => x.Description).HasMaxLength(1000);
                e.Property(x => x.LocationName).HasMaxLength(200);
                e.Property(x => x.CarrierEventId).HasMaxLength(100);
                e.Property(x => x.IdempotencyKey).HasMaxLength(150).IsRequired();

                e.HasOne(typeof(Shipments)).WithMany().HasForeignKey(nameof(ShipmentTrackingEvents.ShipmentId)).OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => x.IdempotencyKey).IsUnique().HasFilter(SoftDeleteFilter);       // webhook tekrarı engeli
                e.HasIndex(x => new { x.ShipmentId, x.OccurredAtUtc });                         // takip zaman çizelgesi (müşteri ekranı)
                e.HasIndex(x => new { x.EventType, x.OccurredAtUtc });                          // "bu hafta kaç teslim başarısız" raporu
            });
        }
    }
}