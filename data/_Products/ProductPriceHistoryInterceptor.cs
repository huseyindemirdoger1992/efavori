using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using data._Products;

namespace data.Interceptors
{
    /// <summary>
    /// FİYAT GEÇMİŞİ INTERCEPTOR'I.
    /// ProductPrices üzerindeki her Add/Update işlemini yakalar ve ProductPriceHistory'e
    /// append-only satır yazar. Yazma işlemi, tetikleyen SaveChanges ile AYNI transaction'da
    /// gerçekleşir — fiyat değişti ama geçmiş yazılmadı durumu OLUŞAMAZ.
    ///
    /// KAYIT (Program.cs / DI):
    ///   builder.Services.AddSingleton&lt;ProductPriceHistoryInterceptor&gt;();
    ///   builder.Services.AddDbContextFactory&lt;YourDbContext&gt;((sp, opt) =&gt;
    ///       opt.UseSqlServer(cs)
    ///          .AddInterceptors(sp.GetRequiredService&lt;ProductPriceHistoryInterceptor&gt;()));
    /// Interceptor stateless'tır; Singleton kaydı kısa ömürlü context deseniyle uyumludur.
    ///
    /// ALAN EŞLEMESİ (ProductPrices → ProductPriceHistory):
    ///   ProductVariantId   → VariantId
    ///   DiscountStartDate  → DiscountStartUtc
    ///   DiscountEndDate    → DiscountEndUtc
    /// ProductPrices.ProductVariantId nullable olduğundan ProductPriceHistory.VariantId de
    /// 'Guid?' olmalıdır (ürün seviyesinde, varyantsız fiyat satırları da geçmişe yazılır).
    ///
    /// SINIRLARI (README §4'te ayrıntılı):
    /// - ExecuteUpdate/ExecuteDelete ve ham SQL, ChangeTracker'a uğramadığı için YAKALANMAZ.
    ///   Toplu fiyat güncellemeleri bu yollarla yapılacaksa geçmiş satırları elle yazılmalıdır.
    /// - "Kim değiştirdi" bilgisi ProductPrices.UpdatedByUserId'den okunur; interceptor
    ///   HttpContext/oturuma bağımlı DEĞİLDİR (Blazor Server'da scope karmaşası doğurmaz).
    /// - "Neden değişti" bilgisi için servis katmanı, kaydetmeden önce PriceChangeContext'e
    ///   kaynak bilgisini yazar; yazmazsa Manual varsayılır.
    /// </summary>
    public sealed class ProductPriceHistoryInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is not null)
                WriteHistoryRows(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData.Context is not null)
                WriteHistoryRows(eventData.Context);

            return base.SavingChanges(eventData, result);
        }

        /// <summary>
        /// Değişen ProductPrices satırlarını tarar ve gerekli geçmiş kayıtlarını ekler.
        /// Yeni eklenen ProductPriceHistory satırları da ChangeTracker'a girdiğinden
        /// aynı SaveChanges çağrısıyla, tek transaction içinde yazılır.
        /// </summary>
        private static void WriteHistoryRows(DbContext context)
        {
            // ToList(): döngü içinde Add yapacağımız için koleksiyonun kopyası alınır.
            var entries = context.ChangeTracker
                .Entries<ProductPrices>()
                .Where(x => x.State is EntityState.Added or EntityState.Modified)
                .ToList();

            if (entries.Count == 0) return;

            var context_ = PriceChangeContext.Current;   // servis katmanının bildirdiği kaynak (opsiyonel)
            var nowUtc = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                var price = entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    context.Add(new ProductPriceHistory
                    {
                        ProductId = price.ProductId,
                        VariantId = price.ProductVariantId,
                        ProductPriceId = price.Id,
                        Currency = price.Currency,
                        NewListPrice = price.ListPrice,
                        NewSalePrice = price.SalePrice,
                        NewDiscountedPrice = price.DiscountedPrice,
                        NewDiscountStartUtc = price.DiscountStartDate,
                        NewDiscountEndUtc = price.DiscountEndDate,
                        NewIsActive = price.IsActive,
                        IsAutoConverted = price.IsAutoConverted,
                        ChangeSource = context_?.Source ?? PriceChangeSource.InitialCreate,
                        UsedExchangeRate = context_?.UsedExchangeRate,
                        MoneyExchangeRateId = context_?.MoneyExchangeRateId,
                        ImportRowId = context_?.ImportRowId,
                        CampaignId = context_?.CampaignId,
                        Note = context_?.Note,
                        CreatedAtUtc = nowUtc,
                        CreatedByUserId = price.CreatedByUserId
                    });
                    continue;
                }

                // --- Modified: yalnızca İZLENEN alanlardan biri gerçekten değiştiyse satır yaz ---
                if (!HasTrackedChange(entry)) continue;

                context.Add(new ProductPriceHistory
                {
                    ProductId = price.ProductId,
                    VariantId = price.ProductVariantId,
                    ProductPriceId = price.Id,
                    Currency = price.Currency,

                    OldListPrice = Original<decimal?>(entry, nameof(ProductPrices.ListPrice)),
                    NewListPrice = price.ListPrice,
                    OldSalePrice = Original<decimal?>(entry, nameof(ProductPrices.SalePrice)),
                    NewSalePrice = price.SalePrice,
                    OldDiscountedPrice = Original<decimal?>(entry, nameof(ProductPrices.DiscountedPrice)),
                    NewDiscountedPrice = price.DiscountedPrice,
                    OldDiscountStartUtc = Original<DateTime?>(entry, nameof(ProductPrices.DiscountStartDate)),
                    NewDiscountStartUtc = price.DiscountStartDate,
                    OldDiscountEndUtc = Original<DateTime?>(entry, nameof(ProductPrices.DiscountEndDate)),
                    NewDiscountEndUtc = price.DiscountEndDate,
                    OldIsActive = Original<bool?>(entry, nameof(ProductPrices.IsActive)),
                    NewIsActive = price.IsActive,

                    IsAutoConverted = price.IsAutoConverted,
                    ChangeSource = context_?.Source ?? ResolveDefaultSource(entry),
                    UsedExchangeRate = context_?.UsedExchangeRate,
                    MoneyExchangeRateId = context_?.MoneyExchangeRateId,
                    ImportRowId = context_?.ImportRowId,
                    CampaignId = context_?.CampaignId,
                    Note = context_?.Note,
                    CreatedAtUtc = nowUtc,
                    CreatedByUserId = price.UpdatedByUserId ?? price.CreatedByUserId
                });
            }
        }

        /// <summary>İzlenen alanlardan herhangi biri değişti mi? (Yalnızca UpdatedAtUtc dokunuşları geçmiş üretmez.)</summary>
        private static bool HasTrackedChange(EntityEntry<ProductPrices> entry)
        {
            string[] tracked =
            {
                nameof(ProductPrices.ListPrice),
                nameof(ProductPrices.SalePrice),
                nameof(ProductPrices.DiscountedPrice),
                nameof(ProductPrices.DiscountStartDate),
                nameof(ProductPrices.DiscountEndDate),
                nameof(ProductPrices.IsActive)
            };

            foreach (var name in tracked)
            {
                var p = entry.Property(name);
                if (p.IsModified && !Equals(p.OriginalValue, p.CurrentValue))
                    return true;
            }
            return false;
        }

        /// <summary>Servis katmanı kaynak bildirmemişse: yalnızca IsActive değiştiyse ActivationChange, aksi halde Manual.</summary>
        private static PriceChangeSource ResolveDefaultSource(EntityEntry<ProductPrices> entry)
        {
            bool priceChanged =
                entry.Property(nameof(ProductPrices.ListPrice)).IsModified ||
                entry.Property(nameof(ProductPrices.SalePrice)).IsModified ||
                entry.Property(nameof(ProductPrices.DiscountedPrice)).IsModified;

            return priceChanged ? PriceChangeSource.Manual : PriceChangeSource.ActivationChange;
        }

        private static T? Original<T>(EntityEntry<ProductPrices> entry, string propertyName)
        {
            var value = entry.Property(propertyName).OriginalValue;
            return value is null ? default : (T)value;
        }
    }

    /// <summary>
    /// FİYAT DEĞİŞİM BAĞLAMI. Interceptor "neyin değiştiğini" ChangeTracker'dan okur ama
    /// "NEDEN değiştiğini" bilemez. Servis katmanı, SaveChanges çağrısını bu bağlam içine
    /// alarak kaynağı bildirir:
    ///
    ///   using (PriceChangeContext.Begin(PriceChangeSource.AutoConversion, usedExchangeRate: 34.12m, moneyExchangeRateId: id))
    ///   {
    ///       price.SalePrice = converted;
    ///       await db.SaveChangesAsync();
    ///   }
    ///
    /// AsyncLocal kullanılır: Blazor Server'da eşzamanlı devam eden akışlar birbirinin
    /// bağlamını görmez; kapsam dışına çıkıldığında otomatik temizlenir.
    /// </summary>
    public sealed class PriceChangeContext : IDisposable
    {
        private static readonly AsyncLocal<PriceChangeContext?> _current = new();

        /// <summary>Geçerli akıştaki bağlam (yoksa null → interceptor varsayılan kaynağa düşer).</summary>
        public static PriceChangeContext? Current => _current.Value;

        /// <summary>Değişim kaynağı.</summary>
        public PriceChangeSource Source { get; private init; }

        /// <summary>Kur dönüşümünde kullanılan kur.</summary>
        public decimal? UsedExchangeRate { get; private init; }

        /// <summary>Kur satırı izi (MoneyExchangeRate.Id).</summary>
        public Guid? MoneyExchangeRateId { get; private init; }

        /// <summary>İçe aktarım satırı izi.</summary>
        public Guid? ImportRowId { get; private init; }

        /// <summary>Kampanya izi.</summary>
        public Guid? CampaignId { get; private init; }

        /// <summary>Serbest açıklama.</summary>
        public string? Note { get; private init; }

        private readonly PriceChangeContext? _previous;

        private PriceChangeContext(PriceChangeContext? previous) => _previous = previous;

        /// <summary>Yeni bir fiyat değişim bağlamı açar. using ile kullanılmalıdır.</summary>
        public static PriceChangeContext Begin(
            PriceChangeSource source,
            decimal? usedExchangeRate = null,
            Guid? moneyExchangeRateId = null,
            Guid? importRowId = null,
            Guid? campaignId = null,
            string? note = null)
        {
            var ctx = new PriceChangeContext(_current.Value)
            {
                Source = source,
                UsedExchangeRate = usedExchangeRate,
                MoneyExchangeRateId = moneyExchangeRateId,
                ImportRowId = importRowId,
                CampaignId = campaignId,
                Note = note
            };
            _current.Value = ctx;
            return ctx;
        }

        /// <summary>Bağlamı kapatır ve bir önceki bağlamı geri yükler (iç içe kullanım güvenlidir).</summary>
        public void Dispose() => _current.Value = _previous;
    }
}