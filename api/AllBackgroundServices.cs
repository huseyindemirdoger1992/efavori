using data;
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
    // API'den dönecek JSON verisini karşılamak için oluşturulan kayıt (record)
    public record ExchangeRateApiResponse(string Base_code, Dictionary<string, decimal> Rates);

    public class AllBackgroundServices : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IDbContextFactory<_ApplicationConnectionDb> _dbFactory;
        private readonly ILogger<AllBackgroundServices> _logger;

        // HTTP istekleri için paylaşılan istemci
        private static readonly HttpClient _httpClient = new HttpClient();

        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(10);

        public AllBackgroundServices(
            IServiceScopeFactory scopeFactory,
            IDbContextFactory<_ApplicationConnectionDb> dbFactory,
            ILogger<AllBackgroundServices> logger)
        {
            _scopeFactory = scopeFactory;
            _dbFactory = dbFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken durdurmaSinyali)
        {
            _logger.LogInformation("AllBackgroundServices başlatıldı (10 Saniyede Bir Kur Değeri Kaydediliyor).");

            using var timer = new PeriodicTimer(Interval);

            try
            {
                // timer.WaitForNextTickAsync 10 saniyede bir true döner
                while (await timer.WaitForNextTickAsync(durdurmaSinyali))
                {
                    await FetchAndSaveExchangeRatesAsync(durdurmaSinyali);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Arka plan görevi durdurma sinyali aldı.");
            }

            _logger.LogInformation("AllBackgroundServices Durduruldu.");
        }

        private async Task FetchAndSaveExchangeRatesAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Ücretsiz ve API Key gerektirmeyen açık bir endpoint (Temel para birimi: USD)
                string apiUrl = "https://open.er-api.com/v6/latest/USD";

                var response = await _httpClient.GetFromJsonAsync<ExchangeRateApiResponse>(apiUrl, cancellationToken);

                if (response != null && response.Rates != null)
                {
                    using var context = await _dbFactory.CreateDbContextAsync(cancellationToken);

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

                    _logger.LogInformation("Güncel kurlar veritabanına kaydedildi. USD: 1 | EUR: {Eur} | AZN: {Azn} | TRY: {Try}",
                        newRate.Euro_Eur, newRate.Manat_Azn, newRate.Lira_Tl);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kur verileri çekilirken veya veritabanına kaydedilirken bir hata oluştu.");
            }
        }
    }
}