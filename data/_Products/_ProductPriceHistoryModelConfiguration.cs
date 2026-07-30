using Microsoft.EntityFrameworkCore;

namespace data._Products
{
    /// <summary>
    /// Fiyat Geçmişi V1 — FK, indeks, silme davranışı ve decimal precision tanımları.
    /// MEVCUT _ProductModelConfiguration DOSYASINA DOKUNULMAZ; bu faz kendi konfigürasyon
    /// sınıfını getirir ve OnModelCreating'e AYRI tek satır olarak eklenir.
    /// Kullanım: _ProductPriceHistoryModelConfiguration.Apply(modelBuilder);
    /// </summary>
    public static class _ProductPriceHistoryModelConfiguration
    {
        /// <summary>
        /// Soft-delete filtreli tekil indeks sabiti.
        /// Owned tip data.Owned.IsDeleted içindeki property adı 'IsDeletedStatu' olduğundan
        /// EF'in ürettiği kolon adı 'IsDeleted_IsDeletedStatu'dur.
        /// (_AttributeModelConfiguration ve _ProductModelConfiguration ile aynı sabit.)
        /// </summary>
        private const string SoftDeleteFilter = "[IsDeleted_IsDeletedStatu] = 0";

        public static void Apply(ModelBuilder modelBuilder)
        {
            // =====================================================================
            // ProductPriceHistory — append-only fiyat defteri
            // =====================================================================
            modelBuilder.Entity<ProductPriceHistory>(e =>
            {
                e.ToTable("ProductPriceHistory");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.OldListPrice).HasPrecision(18, 2);
                e.Property(x => x.NewListPrice).HasPrecision(18, 2);
                e.Property(x => x.OldSalePrice).HasPrecision(18, 2);
                e.Property(x => x.NewSalePrice).HasPrecision(18, 2);
                e.Property(x => x.OldDiscountedPrice).HasPrecision(18, 2);
                e.Property(x => x.NewDiscountedPrice).HasPrecision(18, 2);
                e.Property(x => x.UsedExchangeRate).HasPrecision(18, 6);   // kur hassasiyeti fiyattan yüksektir
                e.Property(x => x.Note).HasMaxLength(500);

                e.HasOne(typeof(Products)).WithMany().HasForeignKey(nameof(ProductPriceHistory.ProductId)).OnDelete(DeleteBehavior.Restrict);
                // VariantId nullable olduğundan bu ilişki OPSİYONEL FK'dir: ürün geneli
                // (varyantsız) fiyat satırlarının geçmişi de yazılabilir. Ek yapılandırma gerekmez.
                e.HasOne(typeof(ProductVariants)).WithMany().HasForeignKey(nameof(ProductPriceHistory.VariantId)).OnDelete(DeleteBehavior.Restrict);
                // ProductPriceId / MoneyExchangeRateId / ImportRowId / CampaignId: iz alanlarıdır.
                // Fiyat satırı, kur satırı veya import satırı temizlenince GEÇMİŞ SİLİNMEMELİ,
                // bu yüzden FK kısıtı bilinçli olarak konulmaz (mevzuat kanıtı korunur).

                // ŞARTNAME İNDEKSİ: (ProductId, Currency, CreatedAtUtc)
                e.HasIndex(x => new { x.ProductId, x.Currency, x.CreatedAtUtc });               // ürün fiyat grafiği + Omnibus ham sorgu
                e.HasIndex(x => new { x.VariantId, x.Currency, x.CreatedAtUtc });               // varyant bazlı fiyat izi
                e.HasIndex(x => new { x.ChangeSource, x.CreatedAtUtc });                        // "kur kaynaklı değişimler" denetim raporu
                e.HasIndex(x => x.CampaignId);                                                  // kampanyanın fiyat etkisi dökümü
            });

            // =====================================================================
            // ProductPriceDailySnapshot — günlük özet (Omnibus hızlandırıcı)
            // =====================================================================
            modelBuilder.Entity<ProductPriceDailySnapshot>(e =>
            {
                e.ToTable("ProductPriceDailySnapshot");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.MinPrice).HasPrecision(18, 2);
                e.Property(x => x.MaxPrice).HasPrecision(18, 2);
                e.Property(x => x.ClosePrice).HasPrecision(18, 2);
                e.Property(x => x.PriceDate).HasColumnType("date");                             // saat bileşeni yok → indeks daralır

                e.HasOne(typeof(Products)).WithMany().HasForeignKey(nameof(ProductPriceDailySnapshot.ProductId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(ProductVariants)).WithMany().HasForeignKey(nameof(ProductPriceDailySnapshot.VariantId)).OnDelete(DeleteBehavior.Restrict);

                // Gün başına TEK satır: BackgroundService UPSERT'i bu indekse dayanır.
                // NOT: VariantId burada non-nullable'dır. Özet job'ı ürün geneli satırları da
                // özetleyecek şekilde genişletilirse alan 'Guid?' yapılmalı ve filtreye
                // "AND [VariantId] IS NOT NULL" eklenmelidir (SQL Server NULL'ları tekil sayar).
                e.HasIndex(x => new { x.ProductId, x.VariantId, x.Currency, x.PriceDate })
                    .IsUnique().HasFilter(SoftDeleteFilter);

                // Omnibus sorgusu: son 30 günün MIN(ClosePrice) — kapsayıcı indeksle tek seek.
                e.HasIndex(x => new { x.VariantId, x.Currency, x.PriceDate })
                    .IncludeProperties(x => new { x.MinPrice, x.ClosePrice });
            });

            // =====================================================================
            // PriceAlerts — fiyat düşüşü alarmları
            // =====================================================================
            modelBuilder.Entity<PriceAlerts>(e =>
            {
                e.ToTable("PriceAlerts");
                e.HasKey(x => x.Id);
                e.Property(x => x.RowVersion).IsRowVersion();
                e.OwnsOne(x => x.IsDeleted);

                e.Property(x => x.TargetPrice).HasPrecision(18, 2);
                e.Property(x => x.PriceAtCreation).HasPrecision(18, 2);
                e.Property(x => x.TriggeredPrice).HasPrecision(18, 2);

                e.HasOne(typeof(data._Users.Users)).WithMany().HasForeignKey(nameof(PriceAlerts.UserId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(Products)).WithMany().HasForeignKey(nameof(PriceAlerts.ProductId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(ProductVariants)).WithMany().HasForeignKey(nameof(PriceAlerts.VariantId)).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(typeof(data._Carts.CartsFavorite)).WithMany().HasForeignKey(nameof(PriceAlerts.CartsFavoriteId)).OnDelete(DeleteBehavior.Restrict);
                // NotificationId: Faz 6 tablosu henüz yok — iz alanı; FK Faz 6 konfigürasyonunda tanımlanacak.

                // Aynı kullanıcı aynı varyant için tek AKTİF alarm kurabilir.
                // NOT: Status kolonu filtreye dâhil edilemediğinden (SQL Server filtered index sabit
                // karşılaştırma ister) filtre "IsDeleted + Status=0(Active)" biçiminde yazılır.
                e.HasIndex(x => new { x.UserId, x.ProductId, x.VariantId, x.Currency })
                    .IsUnique().HasFilter($"{SoftDeleteFilter} AND [Status] = 0");

                e.HasIndex(x => new { x.UserId, x.Status });                                    // "Fiyat alarmlarım" sayfası
                e.HasIndex(x => new { x.Status, x.ProductId, x.Currency });                     // tetikleme taraması (BackgroundService)
                e.HasIndex(x => new { x.Status, x.VariantId, x.Currency, x.TargetPrice });      // varyant bazlı eşik karşılaştırması
            });
        }
    }
}