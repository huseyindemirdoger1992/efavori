using api;
using data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace razor._Shared
{
    public partial class ThisPageIsBeingPrepared : ComponentBase, IDisposable
    {
        // Profesyonel Enjeksiyonlar
        [Inject] private IDbContextFactory<_ApplicationConnectionDb> DbFactory { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] private TakeLogs Logger { get; set; } = default!; // Logger artık DI üzerinden geliyor

        private List<Country> Country_ = new();
        private bool Btn_isProcessing_01 = false;
        private Notification? notificationRef;

        // 10 saniyelik döngü için kontrolcü
        private CancellationTokenSource _periodicCts = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadData();

            // 10 saniyede bir çalışacak metodu başlatıyoruz (Fire and forget)
            _ = StartPeriodicTaskAsync();
        }

        private async Task LoadData()
        {
            try
            {
                using var db = await DbFactory.CreateDbContextAsync();
                // Performans için AsNoTracking
                Country_ = await db.Country.AsNoTracking().ToListAsync(); 
            }
            catch (Exception ex)
            {
                await LogError("LoadData", "Country fetch", ex);
            }
        }

        // --- İSTEDİĞİN 10 SANİYELİK METOD ---
        private async Task StartPeriodicTaskAsync()
        {
            while (!_periodicCts.Token.IsCancellationRequested)
            {
                try
                {
                    // Buraya her 10 saniyede bir yapılmasını istediğin işi yaz
                    await MyPeriodicAction();

                    // 10 saniye bekle (sayfa kapanırsa bekleme iptal edilir)
                    await Task.Delay(10000, _periodicCts.Token);
                }
                // Sayfa kapandı, döngüden çık
                catch (TaskCanceledException) { break; }
                catch (Exception ex)
                {
                    await LogError("PeriodicTask", "Routine work", ex);
                }
            }
        }

        private async Task MyPeriodicAction()
        {
            // Örnek: Verileri tazele veya bir durumu kontrol et
            Console.WriteLine($"Sistem saati: {DateTime.Now} - Arka plan işi çalıştı.");
            await Task.CompletedTask;
        }

        private async Task Action()
        {
            // Çift tıklamayı engelle
            if (Btn_isProcessing_01) return;

            try
            {
                Btn_isProcessing_01 = true;

                // Aksiyon kodların buraya...

                await ShowNotification("success", "Başarılı", "İşlem tamamlandı.", null);
                await Task.Delay(2000);
            }
            catch (Exception ex)
            {
                await LogError("Action", "Main Action", ex);
            }
            finally
            {
                // Hata olsa da olmasa da butonu serbest bırak
                Btn_isProcessing_01 = false;
            }
        }

        // Merkezi Loglama Yardımı
        private async Task LogError(string title, string action, Exception ex)
        {
            try
            {
                await Logger.TakeIt(null, title, action, ex.Message, ex.StackTrace);
            }
            catch { /* Loglama bile çökerse sistemi kilitleme */ }
        }

        private async Task ShowNotification(string type, string title, string text, string? image)
        {
            var imageUrl = string.IsNullOrWhiteSpace(image) ? "https://picsum.photos/120?" : image;
            if (notificationRef != null)
                await notificationRef.Launch(type, title, text, imageUrl);
        }

        // Sayfadan çıkıldığında zamanlayıcıyı durdur (Bellek yönetimi)
        public void Dispose()
        {
            _periodicCts.Cancel();
            _periodicCts.Dispose();
        }
    }
}