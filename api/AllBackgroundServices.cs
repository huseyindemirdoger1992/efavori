using data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace api
{
    /// <summary>
    /// Product tablosunda AiSeoKeywordsIsOk alanı true OLMAYAN ürünleri tarar ve
    /// HER 1 DAKİKADA 1 ürün için Gemini AI üzerinden SEO anahtar kelimeleri üretir.
    ///
    /// Hız politikası:
    /// - Tur başına EN FAZLA 1 API isteği (1 ürün) atılır.
    /// - Her tur arasında sabit 1 dakika beklenir (ürün bulunsa da bulunmasa da).
    /// - Sonuç: dakikada azami 1 istek => günde azami 1440 istek (1500 kotanın altında).
    ///
    /// Tasarım notları:
    /// - Bu servis SINGLETON'dur (AddHostedService). Bu yüzden SCOPED/TRANSIENT servisler
    ///   (GeminiAiService bir typed-HttpClient => transient) doğrudan enjekte edilmez;
    ///   her döngüde IServiceScopeFactory ile scope açılıp oradan çözülür (captive dependency önlenir).
    /// - IDbContextFactory zaten Singleton kaydı olduğundan doğrudan enjekte edilebilir.
    /// - Production'da DbContext global olarak NoTracking çalıştığından, güncelleme yapılacak
    ///   sorguda açıkça .AsTracking() kullanılır; aksi halde SaveChanges hiçbir şey kaydetmez.
    /// </summary>
    public class AllBackgroundServices : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IDbContextFactory<_ApplicationConnectionDb> _dbFactory;
        private readonly ILogger<AllBackgroundServices> _logger;

        // --- Ayarlanabilir sabitler ---
        // Turlar arası sabit bekleme. Dakikada 1 ürün => 1 dakika.
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

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
            _logger.LogInformation("AllBackgroundServices başlatıldı (dakikada 1 ürün).");

            // PeriodicTimer ile sabit aralık: işin süresi ne olursa olsun her turun
            // başlangıcı bir önceki turdan ~1 dakika sonra tetiklenir.
            using var timer = new PeriodicTimer(Interval);

            // İlk tur da bir periyot beklenerek başlatılır
            // (uygulama ve DB bağlantısı tam ayağa kalksın diye).
            while (await WaitForNextTickAsync(timer, durdurmaSinyali))
            {
                try
                {
                    // await ProcessSingleProductAsync(durdurmaSinyali);
                }
                catch (OperationCanceledException) when (durdurmaSinyali.IsCancellationRequested)
                {
                    // Uygulama kapanıyor; sessizce çık
                    break;
                }
                catch (Exception hata)
                {
                    // Turu etkileyen beklenmeyen hata: hem ILogger'a hem Logs tablosuna yaz
                    _logger.LogError(hata, "SEO anahtar kelime üretim turunda beklenmeyen hata.");
                    await SafeLogToDbAsync("ExecuteAsync", "SEO keyword generation tick", hata, durdurmaSinyali);
                }
            }

            _logger.LogInformation("AllBackgroundServices durduruldu.");
        }


        /// <summary>
        /// Logs tablosuna doğrudan hata kaydı yazar.
        /// Arka plan servisinde HttpContext bulunmadığından (TakeLogs/UserInfos HttpContext'e bağımlı),
        /// burada UserInfos'a uğramadan doğrudan DbContext üzerinden yazılır.
        /// </summary>
        private async Task SafeLogToDbAsync(string pageNameSpaceTitle, string action, Exception ex, CancellationToken token)
        {
            try
            {
                await using var db = await _dbFactory.CreateDbContextAsync(token);

                db.Logs.Add(new Logs
                {
                    UserId = null, // Sistem/arka plan işlemi: kullanıcı yok
                    PageNameSpaceTitle = $"[BACKGROUND] {pageNameSpaceTitle}",
                    Action = action,
                    Exception = ex.Message,
                    StackTrace = ex.StackTrace,
                    Date = DateTime.UtcNow
                });

                await db.SaveChangesAsync(token);
            }
            catch
            {
                // Loglama da başarısız olursa servisi kilitleme: sessizce geç (Shut Up)
            }
        }

        /// <summary>
        /// PeriodicTimer.WaitForNextTickAsync'i iptal sinyalinde exception fırlatmadan sarmalar.
        /// false dönerse: iptal istendi, döngüden çıkılmalı.
        /// </summary>
        private static async Task<bool> WaitForNextTickAsync(PeriodicTimer timer, CancellationToken token)
        {
            try
            {
                return await timer.WaitForNextTickAsync(token);
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }
    }
}