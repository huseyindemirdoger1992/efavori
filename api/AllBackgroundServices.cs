using data;
using data._Products;
using data._Systems;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace api
{
    public record ExchangeRateApiResponse(string Base_code, Dictionary<string, decimal> Rates);

    public class AllBackgroundServices : BackgroundService
    {
        private readonly IDbContextFactory<_ApplicationConnectionDb> _dbFactory;
        private readonly ILogger<AllBackgroundServices> _logger;

        private static readonly HttpClient _httpClient = new HttpClient();

        // ════════════════════════════════════════════════════════════
        //  AYARLAR — sıklık, izin ve limit değerleri artık VERİTABANINDAN
        //  (AllBackgroundServicesFrequencyRate, Id = 1) okunur.
        //  Ayarları AllBackgroundServicesTasks.razor sayfasından değiştirin.
        //
        //  Aşağıdaki sabitler yalnızca DB'de kayıt bulunamadığında veya
        //  değerler geçersiz olduğunda devreye giren GÜVENLİ VARSAYILANLARDIR.
        // ════════════════════════════════════════════════════════════

        private static readonly TimeSpan SettingsReadFallbackInterval = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan MinimumInterval = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan DefaultInterval = TimeSpan.FromSeconds(10);

        // ════════════════════════════════════════════════════════════

        public AllBackgroundServices(
            IDbContextFactory<_ApplicationConnectionDb> dbFactory,
            ILogger<AllBackgroundServices> logger)
        {
            _dbFactory = dbFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken durdurmaSinyali)
        {
            _logger.LogInformation("AllBackgroundServices başlatıldı (Döviz Kuru).");

            await RunLoopAsync("Döviz Kuru", FetchAndSaveExchangeRatesAsync, durdurmaSinyali);

            _logger.LogInformation("AllBackgroundServices Durduruldu.");
        }

        /// <summary>
        /// Verilen görevi, her tick'te VERİTABANINDAN taze okunan ayarlara göre
        /// (izin + aralık) çalıştırır. Ayarlar canlı okunduğu için servis
        /// yeniden başlatmadan değişiklikler uygulanır.
        /// </summary>
        private async Task RunLoopAsync(
            string taskName,
            Func<AllBackgroundServicesFrequencyRate, CancellationToken, Task> job,
            CancellationToken durdurmaSinyali)
        {
            try
            {
                while (!durdurmaSinyali.IsCancellationRequested)
                {
                    var settings = await LoadSettingsAsync(durdurmaSinyali);

                    var (isEnabled, interval) = ResolveTaskConfig(taskName, settings);

                    await Task.Delay(interval, durdurmaSinyali);

                    if (!isEnabled) continue;

                    await job(settings!, durdurmaSinyali);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("{Task} görevi durdurma sinyali aldı.", taskName);
            }
        }

        /// <summary>
        /// Görev adına göre ilgili izin + aralık değerini ayarlardan çözer.
        /// </summary>
        private (bool isEnabled, TimeSpan interval) ResolveTaskConfig(
            string taskName,
            AllBackgroundServicesFrequencyRate? settings)
        {
            if (settings == null)
                return (false, SettingsReadFallbackInterval);

            return taskName switch
            {
                "Döviz Kuru" => (
                    settings.IsCurrencyFetchEnabled,
                    NormalizeInterval(settings.CurrencyFetchIntervalInSeconds)),

                _ => (false, SettingsReadFallbackInterval)
            };
        }

        /// <summary>
        /// Saniye değerini güvenli TimeSpan'e çevirir; 0/negatif değerleri minimuma sabitler.
        /// </summary>
        private static TimeSpan NormalizeInterval(int seconds)
        {
            if (seconds <= 0) return DefaultInterval;
            var span = TimeSpan.FromSeconds(seconds);
            return span < MinimumInterval ? MinimumInterval : span;
        }

        /// <summary>
        /// Ayar tablosunun tek satırını (Id = 1) kısa ömürlü context ile okur.
        /// </summary>
        private async Task<AllBackgroundServicesFrequencyRate?> LoadSettingsAsync(CancellationToken cancellationToken)
        {
            try
            {
                await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

                return await db.Set<AllBackgroundServicesFrequencyRate>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == 1, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Arka plan servis ayarları okunamadı (Id = 1).");
                return null;
            }
        }

        // ──────────────────────────────────────────────
        //  DÖVİZ KURU
        // ──────────────────────────────────────────────
        private async Task FetchAndSaveExchangeRatesAsync(
            AllBackgroundServicesFrequencyRate settings,
            CancellationToken cancellationToken)
        {
            try
            {
                string apiUrl = "https://open.er-api.com/v6/latest/USD";

                var response = await _httpClient.GetFromJsonAsync<ExchangeRateApiResponse>(apiUrl, cancellationToken);

                if (response?.Rates != null)
                {
                    await using var context = await _dbFactory.CreateDbContextAsync(cancellationToken);

                    var newRate = new MoneyExchangeRate
                    {
                        Dolar_Usd = response.Rates.GetValueOrDefault("USD", 1m),
                        Euro_Eur = response.Rates.GetValueOrDefault("EUR", 0m),
                        Manat_Azn = response.Rates.GetValueOrDefault("AZN", 0m),
                        Lira_Tl = response.Rates.GetValueOrDefault("TRY", 0m),
                        CreatedAt = DateTime.Now
                    };

                    context.Set<MoneyExchangeRate>().Add(newRate);
                    await context.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation(
                        "Kurlar kaydedildi → EUR: {Eur} | AZN: {Azn} | TRY: {Try}",
                        newRate.Euro_Eur, newRate.Manat_Azn, newRate.Lira_Tl);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kur verileri çekilirken hata oluştu.");
            }
        }
    }
}