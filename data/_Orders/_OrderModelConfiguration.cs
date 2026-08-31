using Microsoft.EntityFrameworkCore;
using data._Users;

namespace data._Orders
{
    /// <summary>
    /// Sipariş Sistemi V1 — tüm FK, indeks, silme davranışı ve decimal precision tanımları.
    /// Mevcut _ProductModelConfiguration deseni ile birebir aynı yaklaşımdır:
    /// - Navigation property yok; HasOne(typeof(T)).WithMany().HasForeignKey(...) biçimi.
    /// - Tekil indeksler soft-delete filtrelidir.
    /// - Sipariş/finans kayıtları TARİHSEL KANIT olduğundan silme davranışı ağırlıkla Restrict'tir
    ///   (gerekçe tablosu README_Orders.md'dedir).
    /// Kullanım: OnModelCreating içinde tek satır → _OrderModelConfiguration.Apply(modelBuilder);
    /// </summary>
    public static class _OrderModelConfiguration
    {
        /// <summary>Soft-delete filtreli tekil indeks sabiti.
        /// NOT: Mevcut _ProductModelConfiguration'daki sabitle AYNI dizge kullanılmalıdır.</summary>
        private const string SoftDeleteFilter = "[IsDeleted_IsDeletedStatu] = 0";

        public static void Apply(ModelBuilder modelBuilder)
        {
            // =====================================================================
            // Orders — üst sipariş
            // =====================================================================
            modelBuilder.Entity<Orders>(e =>
            {
                e.ToTable("Orders");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.OrderNumber).HasMaxLength(32).IsRequired();
                e.Property(x => x.GuestEmail).HasMaxLength(256);
                e.Property(x => x.GuestPhone).HasMaxLength(32);
                e.Property(x => x.CustomerNote).HasMaxLength(1000);

                e.Property(x => x.ItemsTotal).HasPrecision(18, 2);
                e.Property(x => x.DiscountTotal).HasPrecision(18, 2);
                e.Property(x => x.CouponTotal).HasPrecision(18, 2);
                e.Property(x => x.ShippingTotal).HasPrecision(18, 2);
                e.Property(x => x.TaxTotal).HasPrecision(18, 2);
                e.Property(x => x.GrandTotal).HasPrecision(18, 2);

                // Adres snapshot alanları
                e.Property(x => x.ShippingFullName).HasMaxLength(200).IsRequired();
                e.Property(x => x.ShippingPhone).HasMaxLength(32).IsRequired();
                e.Property(x => x.ShippingCountryName).HasMaxLength(100).IsRequired();
                e.Property(x => x.ShippingStateName).HasMaxLength(100);
                e.Property(x => x.ShippingCityName).HasMaxLength(100);
                e.Property(x => x.ShippingAddressLine).HasMaxLength(500).IsRequired();
                e.Property(x => x.ShippingPostalCode).HasMaxLength(20);
                e.Property(x => x.BillingFullName).HasMaxLength(200).IsRequired();
                e.Property(x => x.BillingPhone).HasMaxLength(32);
                e.Property(x => x.BillingCountryName).HasMaxLength(100).IsRequired();
                e.Property(x => x.BillingStateName).HasMaxLength(100);
                e.Property(x => x.BillingCityName).HasMaxLength(100);
                e.Property(x => x.BillingAddressLine).HasMaxLength(500).IsRequired();
                e.Property(x => x.BillingPostalCode).HasMaxLength(20);
                e.Property(x => x.BillingCompanyName).HasMaxLength(300);
                e.Property(x => x.BillingTaxNumber).HasMaxLength(20);
                e.Property(x => x.BillingTaxOffice).HasMaxLength(100);

                // FK'ler — hepsi Restrict: sipariş tarihsel kanıttır, üst kayıt silinince yok olamaz.
                e.HasOne(typeof(data._Users.Users)).WithMany().HasForeignKey(nameof(Orders.UserId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(CheckoutSessions)).WithMany().HasForeignKey(nameof(Orders.CheckoutSessionId)).OnDelete(DeleteBehavior.Restrict);
                // Adres/lokasyon FK'ları yalnızca iz olduğundan kısıt konulmaz (SetNull yerine izli Guid?):
                // ShippingUserAddressId, BillingUserAddressId, ShippingCountryId/StateId/CityId → FK tanımlanmaz,
                // çünkü kaynak kayıt soft-delete edilse bile snapshot geçerlidir. (Bilinçli tasarım kararı.)

                // İndeksler
                e.HasIndex(x => x.OrderNumber).IsUnique().HasFilter(SoftDeleteFilter);          // insan-okur tekil no
                e.HasIndex(x => new { x.UserId, x.CreatedAtUtc });                              // "Siparişlerim" listesi
                e.HasIndex(x => new { x.Status, x.CreatedAtUtc });                              // admin durum panosu
                e.HasIndex(x => x.GuestEmail);                                                  // misafir sipariş takibi
                e.HasIndex(x => x.CheckoutSessionId);                                           // idempotency izi
            });

            // =====================================================================
            // SubOrders — mağaza bazlı alt sipariş
            // =====================================================================
            modelBuilder.Entity<SubOrders>(e =>
            {
                e.ToTable("SubOrders");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.SubOrderNumber).HasMaxLength(40).IsRequired();
                e.Property(x => x.CancellationNote).HasMaxLength(1000);

                e.Property(x => x.ItemsTotal).HasPrecision(18, 2);
                e.Property(x => x.DiscountTotal).HasPrecision(18, 2);
                e.Property(x => x.CouponTotal).HasPrecision(18, 2);
                e.Property(x => x.ShippingTotal).HasPrecision(18, 2);
                e.Property(x => x.TaxTotal).HasPrecision(18, 2);
                e.Property(x => x.GrandTotal).HasPrecision(18, 2);
                e.Property(x => x.CommissionTotal).HasPrecision(18, 2);

                e.HasOne(typeof(Orders)).WithMany().HasForeignKey(nameof(SubOrders.OrderId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Store.Store)).WithMany().HasForeignKey(nameof(SubOrders.StoreId)).OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => x.SubOrderNumber).IsUnique().HasFilter(SoftDeleteFilter);
                e.HasIndex(x => x.OrderId);                                                     // üst siparişin parçaları
                e.HasIndex(x => new { x.StoreId, x.Status, x.CreatedAtUtc });                   // satıcı sipariş paneli
                e.HasIndex(x => new { x.Status, x.DeliveredAtUtc });                            // escrow release taraması (Faz 2)
            });

            // =====================================================================
            // OrderItems — sipariş kalemi (snapshot)
            // =====================================================================
            modelBuilder.Entity<OrderItems>(e =>
            {
                e.ToTable("OrderItems");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.ProductName).HasMaxLength(500).IsRequired();
                e.Property(x => x.Sku).HasMaxLength(100);
                e.Property(x => x.Barcode).HasMaxLength(100);
                e.Property(x => x.ImageUrl).HasMaxLength(1000);
                e.Property(x => x.VariantSummary).HasMaxLength(500);

                e.Property(x => x.UnitPrice).HasPrecision(18, 2);
                e.Property(x => x.UnitListPrice).HasPrecision(18, 2);
                e.Property(x => x.VatRate).HasPrecision(5, 2);
                e.Property(x => x.DiscountAmount).HasPrecision(18, 2);
                e.Property(x => x.CouponAmount).HasPrecision(18, 2);
                e.Property(x => x.CommissionRate).HasPrecision(5, 2);
                e.Property(x => x.CommissionAmount).HasPrecision(18, 2);
                e.Property(x => x.LineTotal).HasPrecision(18, 2);

                e.HasOne(typeof(SubOrders)).WithMany().HasForeignKey(nameof(OrderItems.SubOrderId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(Orders)).WithMany().HasForeignKey(nameof(OrderItems.OrderId)).OnDelete(DeleteBehavior.Restrict);
                // ProductId / VariantId / StoreId: yalnızca iz — ürün soft-delete edilse bile kalem
                // yaşamalı; snapshot esas olduğundan FK kısıtı bilinçli olarak konulmaz.

                e.HasIndex(x => x.SubOrderId);                                                  // alt sipariş kalemleri
                e.HasIndex(x => x.OrderId);                                                     // müşteri sipariş detayı
                e.HasIndex(x => new { x.ProductId, x.CreatedAtUtc });                           // SoldCount hesabı (BackgroundService)
                e.HasIndex(x => new { x.VariantId, x.CreatedAtUtc });                           // varyant satış raporu
                e.HasIndex(x => new { x.StoreId, x.CreatedAtUtc });                             // satıcı satış raporu
            });

            // =====================================================================
            // OrderStatusHistory — append-only tarihçe
            // =====================================================================
            modelBuilder.Entity<OrderStatusHistory>(e =>
            {
                e.ToTable("OrderStatusHistory");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.Note).HasMaxLength(1000);

                e.HasOne(typeof(Orders)).WithMany().HasForeignKey(nameof(OrderStatusHistory.OrderId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(SubOrders)).WithMany().HasForeignKey(nameof(OrderStatusHistory.SubOrderId)).OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.OrderId, x.CreatedAtUtc });                             // sipariş zaman çizelgesi
                e.HasIndex(x => new { x.SubOrderId, x.CreatedAtUtc });                          // alt sipariş zaman çizelgesi
            });

            // =====================================================================
            // OrderInvoices — SubOrder bazlı fatura
            // =====================================================================
            modelBuilder.Entity<OrderInvoices>(e =>
            {
                e.ToTable("OrderInvoices");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.InvoiceSeries).HasMaxLength(10).IsRequired();
                e.Property(x => x.InvoiceFullNumber).HasMaxLength(30).IsRequired();
                e.Property(x => x.EInvoiceUuid).HasMaxLength(64);
                e.Property(x => x.TotalAmount).HasPrecision(18, 2);

                e.HasOne(typeof(SubOrders)).WithMany().HasForeignKey(nameof(OrderInvoices.SubOrderId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Store.Store)).WithMany().HasForeignKey(nameof(OrderInvoices.StoreId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Store.StoreIntegration)).WithMany().HasForeignKey(nameof(OrderInvoices.StoreIntegrationId)).OnDelete(DeleteBehavior.Restrict);
                // PdfMediaItemId: media temizliği faturayı kilitlemesin diye FK kısıtı konulmaz (iz alanı).

                e.HasIndex(x => new { x.StoreId, x.InvoiceSeries, x.InvoiceNumber })
                    .IsUnique().HasFilter(SoftDeleteFilter);                                    // mağaza içi seri/no tekilliği
                e.HasIndex(x => x.SubOrderId);
                e.HasIndex(x => new { x.EInvoiceStatus, x.CreatedAtUtc });                      // e-fatura kuyruğu taraması
            });

            // =====================================================================
            // CheckoutSessions — idempotency + sepet kilidi
            // =====================================================================
            modelBuilder.Entity<CheckoutSessions>(e =>
            {
                e.ToTable("CheckoutSessions");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.IdempotencyKey).HasMaxLength(64).IsRequired();
                e.Property(x => x.GuestEmail).HasMaxLength(256);
                e.Property(x => x.CartHash).HasMaxLength(64).IsRequired();
                e.Property(x => x.GrandTotal).HasPrecision(18, 2);

                e.HasOne(typeof(data._Users.Users)).WithMany().HasForeignKey(nameof(CheckoutSessions.UserId)).OnDelete(DeleteBehavior.Restrict);
                // OrderId: dairesel bağımlılık (Orders → CheckoutSessions FK'sı zaten var) oluşmaması
                // için bu yönde FK kısıtı konulmaz; iz alanıdır.

                e.HasIndex(x => x.IdempotencyKey).IsUnique().HasFilter(SoftDeleteFilter);       // idempotency'nin DB garantisi
                e.HasIndex(x => new { x.UserId, x.Status });                                    // kullanıcının açık oturumu
                e.HasIndex(x => new { x.Status, x.ExpiresAtUtc });                              // expiry sweeper (BackgroundService)
            });

            // =====================================================================
            // OrderNumberSequences — yıl bazlı sıra tablosu
            // =====================================================================
            modelBuilder.Entity<OrderNumberSequences>(e =>
            {
                e.ToTable("OrderNumberSequences");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.HasIndex(x => x.Year).IsUnique();                                             // yıl başına tek satır (filtre YOK: sıra satırı asla silinmez)
            });
        }
    }
}
