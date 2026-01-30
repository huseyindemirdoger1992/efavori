using api;
using data;
using Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using razor._Shared;

namespace razor
{
    public partial class Example : ComponentBase, IAsyncDisposable
    {
        [Parameter] public Users? use { get; set; }

        #region Services 
        [Inject] protected IDbContextFactory<_ApplicationConnectionDb> DbFactory { get; init; } = default!;
        [Inject] protected NavigationManager Navigation { get; init; } = default!;
        [Inject] protected IJSRuntime JS { get; init; } = default!;
        [Inject] protected TakeLogs Logger { get; init; } = default!;
        // Api Entegrasyonu
        [Inject] protected EmailSender ApiEmailSender { get; init; } = default!;
        // [Inject] protected api.Media ApiMedia { get; init; } = default!;
        [Inject] protected TakeLogs ApiTakeLogs { get; init; } = default!;
        // [Inject] protected TextualFunctions ApiTextualFunctions { get; init; } = default!;
        [Inject] protected UserInfos ApiUserInfos { get; init; } = default!;
        #endregion

        #region State Management
        private Notification? notificationRef;
        private readonly CancellationTokenSource _cts = new();
        private readonly SemaphoreSlim _dbLock = new(1, 1);
        private bool _disposed;
        #endregion

        #region Lifecycle
        protected override async Task OnInitializedAsync()
        {
            await LoadData();
            _ = StartAutoRefreshLoop(TimeSpan.FromSeconds(10));
        }

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
        private readonly Dictionary<string, bool> _processingStates = new();
        protected bool IsButtonProcessing(string key) => _processingStates.GetValueOrDefault(key, false);

        // 1. Merkezi Yönetim: Tüm ortak mantık burada toplanır.
        private async Task<bool> RunWithStateAsync(string key, Func<Task> action, bool useDbLock = false)
        {
            if (IsButtonProcessing(key)) return false;

            if (useDbLock) await _dbLock.WaitAsync(_cts.Token);

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
                if (useDbLock) await ShowNotification("error", "Hata", "İşlem başarısız oldu.", null);

                return false;
            }
            finally
            {
                _processingStates[key] = false;
                if (useDbLock) _dbLock.Release();
                StateHasChanged();
            }
        }

        // 2. Kullanım: Daha kısa ve temiz metotlar.
        protected async Task HandleButtonClick(string buttonKey, Func<Task> action)
        {
            await RunWithStateAsync(buttonKey, async () => {
                await action();
                await Task.Delay(3000, _cts.Token);
            });
        }

        private async Task<bool> ExecuteWithLock(string key, Func<Task> action)
        {
            return await RunWithStateAsync(key, action, useDbLock: true);
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
