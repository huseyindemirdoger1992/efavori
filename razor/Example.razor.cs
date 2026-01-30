using api;
using data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using razor._Shared;

namespace razor
{
    public partial class Example : ComponentBase, IAsyncDisposable
    {
        #region 
        [Parameter] public Users? use { get; set; }

        #region Services 
        [Inject] protected IDbContextFactory<_ApplicationConnectionDb> DbFactory { get; init; } = default!;
        [Inject] protected NavigationManager Navigation { get; init; } = default!;
        [Inject] protected IJSRuntime JS { get; init; } = default!;
        [Inject] protected TakeLogs Logger { get; init; } = default!;
        [Inject] protected EmailSender EmailSender { get; init; } = default!;
        #endregion

        #region State Management
        private Notification? notificationRef;
        private readonly CancellationTokenSource _cts = new();
        private readonly SemaphoreSlim _dbLock = new(1, 1);
        private readonly Dictionary<string, bool> _processingStates = new();
        private bool _disposed;
        #endregion

        #region Lifecycle
        protected override async Task OnInitializedAsync()
        {
            await LoadData();
            _ = StartAutoRefreshLoop(TimeSpan.FromSeconds(10));
        }

        // Repetitive Tasks
        private async Task StartAutoRefreshLoop(TimeSpan interval)
        {
            using var timer = new PeriodicTimer(interval);
            try
            {
                while (await timer.WaitForNextTickAsync(_cts.Token))
                {
                    await LoadData();
                    await InvokeAsync(StateHasChanged);
                }
            }
            catch (OperationCanceledException) { }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;

            await _cts.CancelAsync();
            _cts.Dispose();
            _dbLock.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Button Management
        /// <summary>
        /// Tıklanan butonun işlem durumunu kontrol eder
        /// </summary>
        protected bool IsButtonProcessing(string buttonKey)
        {
            return _processingStates.GetValueOrDefault(buttonKey, false);
        }

        /// <summary>
        /// Butona tıklandığında çalışacak belirli süre butonu devre dışı bırakır
        /// </summary>
        protected async Task HandleButtonClick(string buttonKey, Func<Task> action)
        {
            // Eğer buton zaten işlem yapıyorsa, tekrar tıklanmasını engelle
            if (IsButtonProcessing(buttonKey))
                return;

            try
            {
                // Butonu işlem durumuna al
                _processingStates[buttonKey] = true;
                StateHasChanged();

                // Asıl işlemi gerçekleştir
                await action();

                // İşlem bittikten sonra 4 saniye bekle
                await Task.Delay(3000, _cts.Token);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                await LogError(buttonKey, ex);
            }
            finally
            {
                // Butonu tekrar aktif hale getir
                _processingStates[buttonKey] = false;
                StateHasChanged();
            }
        }
        #endregion


        #region  Helpers
        private async Task LogError(string action, Exception ex)
        {
            try
            {
                await Logger.TakeIt(
                    userId: use?.Id,
                    PageNameSpaceTitle: GetType().Name,
                    action: action,
                    exception: ex.Message,
                    stackTrace: ex.StackTrace
                );
            }
            catch { }
        }
        private async Task ShowNotification(string type, string title, string text, string? image)
        {
            if (notificationRef is null) return;
            var imageUrl = string.IsNullOrWhiteSpace(image) ? "https://picsum.photos/120?" : image;
            await notificationRef.Launch(type, title, text, imageUrl);
        }
        private async Task<bool> ExecuteWithLock(string key, Func<Task> action)
        {
            if (_processingStates.GetValueOrDefault(key)) return false;

            await _dbLock.WaitAsync(_cts.Token);
            try
            {
                _processingStates[key] = true;
                StateHasChanged();
                await action();
                return true;
            }
            catch (OperationCanceledException) { return false; }
            catch (Exception ex)
            {
                await LogError(key, ex);
                await ShowNotification("error", "Hata", "İşlem başarısız oldu.", null);
                return false;
            }
            finally
            {
                _processingStates[key] = false;
                _dbLock.Release();
                StateHasChanged();
            }
        }
        #endregion

        // ------------------------------------------------------- Crud Actions -------------------------------------------------------

        #region Data Operations

        protected List<Country>? Country_;
        protected List<Users>? Users_;
        protected Users? _Users;

        protected virtual async Task LoadData()
        {
            try
            {
                await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);

                Country_ = await db.Country
                    .AsNoTracking()
                    .ToListAsync(_cts.Token);

                Users_ = await db.Users
                    .AsNoTracking()
                    .Where(x => x.FirstName == "Deneme" && x.LastName == "Test")
                    .ToListAsync(_cts.Token);

                _Users = await db.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.FirstName == "Deneme" && x.LastName == "Test", _cts.Token);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                await LogError(nameof(LoadData), ex);
            }
        }

        protected async Task Action()
        {
            await ExecuteWithLock(nameof(Action), async () =>
            {
                await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);

                // db.Users.Add(use);
                // await db.SaveChangesAsync(_cts.Token);

                // db.Users.Update(use);
                // await db.SaveChangesAsync(_cts.Token);

                await ShowNotification("success", "Başarılı", "İşlem tamamlandı.", null);
                await Task.Delay(500, _cts.Token);
                await LoadData();
            });
        }

        #endregion
   
    }
}