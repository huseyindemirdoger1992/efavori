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
        private readonly IServiceScopeFactory _scopeFactory;

        private static readonly HttpClient _httpClient = new HttpClient();

        // ════════════════════════════════════════════════════════════
        //  GÖREV AYARLARI — Tüm sıklıkları sadece buradan değiştirin
        //  Örnek: TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(2)
        // ════════════════════════════════════════════════════════════

        /// <summary>Döviz kuru çekme aralığı</summary>
        private static readonly TimeSpan CurrencyInterval = TimeSpan.FromSeconds(30);

        /// <summary>Döviz kuru çekme aktif mi?</summary>
        private static readonly bool IsCurrencyEnabled = true;

        /// <summary>AI kategori özellik üretim aralığı</summary>
        private static readonly TimeSpan AiAttributeInterval = TimeSpan.FromSeconds(60);

        /// <summary>AI kategori özellik üretimi aktif mi?</summary>
        private static readonly bool IsAiAttributeEnabled = true;

        // ════════════════════════════════════════════════════════════

        public AllBackgroundServices(
            IDbContextFactory<_ApplicationConnectionDb> dbFactory,
            ILogger<AllBackgroundServices> logger,
            IServiceScopeFactory scopeFactory)
        {
            _dbFactory = dbFactory;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "AllBackgroundServices başlatıldı. Döviz: {CurrencyInterval}s | AI: {AiInterval}s",
                CurrencyInterval.TotalSeconds,
                AiAttributeInterval.TotalSeconds);

            var tasks = new List<Task>();

            if (IsCurrencyEnabled)
                tasks.Add(RunLoopAsync("Döviz Kuru", FetchAndSaveExchangeRatesAsync, CurrencyInterval, stoppingToken));
            else
                _logger.LogInformation("► Döviz Kuru devre dışı.");

            if (IsAiAttributeEnabled)
                tasks.Add(RunLoopAsync("AI Kategori Özellikleri", GenerateCategoryAttributesAsync, AiAttributeInterval, stoppingToken));
            else
                _logger.LogInformation("► AI Kategori Özellikleri devre dışı.");

            if (tasks.Count == 0)
            {
                _logger.LogWarning("Hiçbir görev aktif olmadığı için servis bekliyor...");
                await Task.Delay(Timeout.Infinite, stoppingToken);
                return;
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("AllBackgroundServices durdurma sinyali aldı.");
            }

            _logger.LogInformation("AllBackgroundServices durduruldu.");
        }

        /// <summary>
        /// Belirtilen aralıkta görevi çalıştırır.
        /// • Önceki çalışma bitmeden yeni çalışma başlamaz (atlanır).
        /// • Hatalar yakalanır; bir görevin hatası diğerini veya döngüyü durdurmaz.
        /// • Tamamen async çalışır, thread bloklamaz.
        /// </summary>
        private async Task RunLoopAsync(
            string taskName,
            Func<CancellationToken, Task> job,
            TimeSpan interval,
            CancellationToken cancellationToken)
        {
            using var timer = new PeriodicTimer(interval);
            using var semaphore = new SemaphoreSlim(1, 1);

            _logger.LogInformation(
                "{Task} döngüsü başladı (aralık: {Interval}s).",
                taskName,
                interval.TotalSeconds);

            try
            {
                while (await timer.WaitForNextTickAsync(cancellationToken))
                {
                    // Önceki çalışma hâlâ devam ediyor mu?
                    if (!await semaphore.WaitAsync(0, cancellationToken))
                    {
                        _logger.LogWarning(
                            "⏭ {Task} atlandı — önceki çalışma henüz bitmedi.",
                            taskName);
                        continue;
                    }

                    try
                    {
                        await job(cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        throw; // Durdurma sinyali yukarı taşınır
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "💥 {Task} görevinde hata oluştu.", taskName);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("{Task} döngüsü durduruldu.", taskName);
            }
        }

        // ──────────────────────────────────────────────
        //  AI KATEGORİ ÖZELLİKLERİ
        // ──────────────────────────────────────────────
        private async Task GenerateCategoryAttributesAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<AIAttributeManager>();

            await manager.ProcessNextPendingCategoryAsync(cancellationToken);
        }

        // ──────────────────────────────────────────────
        //  DÖVİZ KURU
        // ──────────────────────────────────────────────
        private async Task FetchAndSaveExchangeRatesAsync(CancellationToken cancellationToken)
        {
            const string apiUrl = "https://open.er-api.com/v6/latest/USD";

            var response = await _httpClient.GetFromJsonAsync<ExchangeRateApiResponse>(apiUrl, cancellationToken);

            if (response?.Rates == null)
            {
                _logger.LogWarning("Döviz API'den geçerli veri alınamadı.");
                return;
            }

            await using var context = await _dbFactory.CreateDbContextAsync(cancellationToken);

            var newRate = new MoneyExchangeRate
            {
                Dolar_Usd = response.Rates.GetValueOrDefault("USD", 1m),
                Euro_Eur = response.Rates.GetValueOrDefault("EUR", 0m),
                Manat_Azn = response.Rates.GetValueOrDefault("AZN", 0m),
                Lira_Tl = response.Rates.GetValueOrDefault("TRY", 0m),
                CreatedAt = DateTime.UtcNow
            };

            context.Set<MoneyExchangeRate>().Add(newRate);
            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "💱 Kurlar kaydedildi → EUR: {Eur} | AZN: {Azn} | TRY: {Try}",
                newRate.Euro_Eur,
                newRate.Manat_Azn,
                newRate.Lira_Tl);
        }
    }
}