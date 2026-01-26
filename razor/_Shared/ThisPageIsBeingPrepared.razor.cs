using api;
using data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace razor._Shared
{
    /// <summary>
    /// Veri odaklı sayfalar için standartlaştırılmış soyut temel sınıf.
    /// Profesyonel bir yapı; hata toleransı, kaynak yönetimi ve thread-safety üzerine kurulur.
    /// </summary>
    public partial class ThisPageIsBeingPrepared : ComponentBase, IDisposable
    {

        [Parameter] public Users? use { get; set; }

        #region Services
        [Inject] protected IDbContextFactory<_ApplicationConnectionDb> DbFactory { get; init; } = default!;
        [Inject] protected NavigationManager Navigation { get; init; } = default!;
        [Inject] protected IJSRuntime JS { get; init; } = default!;
        [Inject] protected TakeLogs Logger { get; init; } = default!;
        #endregion

        #region State Management
        protected readonly CancellationTokenSource _cts = new();
        private readonly TakeLogs _logger; // Logging servisi


        private Notification? notificationRef;
        #endregion

        #region Lifecycle
        protected override async Task OnInitializedAsync()
        {
            // İlk yükleme: Kullanıcı ekranı görmeden verinin hazır olması istenir.
            await LoadData();

            // Arka plan işleyicisi: UI akışını engellemeden (fire-and-forget) başlatılır.
            _ = StartAutoRefreshLoop(TimeSpan.FromSeconds(10));
        }

        #region Orchestration
        private async Task StartAutoRefreshLoop(TimeSpan interval)
        {
            using var timer = new PeriodicTimer(interval);
            try
            {
                while (await timer.WaitForNextTickAsync(_cts.Token))
                {
                    await LoadData();
                }
            }
            catch (OperationCanceledException) { }
        }

        private async Task ShowNotification(string type, string title, string text, string? image)
        {
            if (notificationRef is null) return;

            var imageUrl = string.IsNullOrWhiteSpace(image) ? "https://picsum.photos/120?" : image;
            await notificationRef.Launch(type, title, text, imageUrl);
        }
        #endregion
        public virtual void Dispose()
        {
            // 60 yılın kuralı: Açtığın kapıyı kapat, başlattığın sinyali durdur.
            _cts.Cancel();
            _cts.Dispose();
        }
        #endregion

        //----------------------------------------------

        #region Data Operations

        // Aksiyon buton grubu için thread-safe kontrol
        private bool Btn_isProcessing_01 = false;


        protected IReadOnlyList<Country> Countries { get; private set; } = Array.Empty<Country>();
        protected Users _newUser = new();

        protected virtual async Task LoadData()
        {
            try
            {
                await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);

                // Read-only operasyonlarda AsNoTracking performansın altın kuralıdır.
                Countries = await db.Country.AsNoTracking().ToListAsync(_cts.Token);
            }
            catch (OperationCanceledException) { /* Sessizce sonlandır */ }
            catch (Exception ex)
            {
                await _logger.TakeIt(
                    userId: null,
                    title: "public abstract partial class ThisPageIsBeingPrepared : ComponentBase, IDisposable | LoadData",
                    action: $"Take Sponsor",
                    exception: ex.Message,
                    stackTrace: ex.StackTrace
                );
            }
            finally
            {
                StateHasChanged();
            }
        }

        protected async Task Action()
        {
            if (Btn_isProcessing_01) return;

            try
            {
                Btn_isProcessing_01 = true;
                await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);

                db.Users.Add(_newUser);
                await db.SaveChangesAsync(_cts.Token);

                _newUser = new();
                await ShowNotification("success", "Başarılı", "Kullanıcı kaydedildi.", null);

                await LoadData();
            }
            catch (Exception ex)
            {
                await _logger.TakeIt(
                    userId: null,
                    title: "public abstract partial class ThisPageIsBeingPrepared : ComponentBase, IDisposable",
                    action: $"HandleSaveUser",
                    exception: ex.Message,
                    stackTrace: ex.StackTrace
                );
            }
            finally
            {
                await Task.Delay(3500);
                Btn_isProcessing_01 = false;
                await LoadData();
            }
        }
        #endregion

    }
}