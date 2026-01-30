using api;
using data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using razor._Shared;

namespace razor
{
    public partial class Example : ComponentBase, IDisposable
    {
        [Parameter] public Users? use { get; set; }

        #region Services 
        [Inject] protected IDbContextFactory<_ApplicationConnectionDb> DbFactory { get; init; } = default!;
        [Inject] protected NavigationManager Navigation { get; init; } = default!;
        [Inject] protected IJSRuntime JS { get; init; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        #endregion

        #region State Management
        private Notification? notificationRef;
        protected readonly CancellationTokenSource _cts = new();
        private readonly TakeLogs _logger;
        private readonly UserInfos _userInfos;
        private readonly EmailSender _emailSender;
        public Example(TakeLogs logger, UserInfos userInfos, EmailSender emailSender)
        {
            _logger = logger;
            _userInfos = userInfos;
            _emailSender = emailSender;
        }
        #endregion

        #region Lifecycle
        protected override async Task OnInitializedAsync()
        {
            await LoadData();
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
            _cts.Cancel();
            _cts.Dispose();
        }
        #endregion

        //----------------------------------------------

        #region Data Operations

        private bool Btn_isProcessing_01 = false;


        // Data Collection
        protected List<Country>? Country_;
        protected List<Users>? Users_;
        protected Users? _Users = new();

        protected virtual async Task LoadData()
        {
            try
            {
                await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);

                Country_ = await db.Country.ToListAsync(_cts.Token);

                Users_ = await db.Users
                    .Where(
                    x =>
                    x.FirstName == "Deneme"
                    &&
                    x.LastName == "Test"
                    )
                    .ToListAsync(_cts.Token);

                _Users = await db.Users
                    .FirstOrDefaultAsync(
                    x =>
                    x.FirstName == "Deneme" &&
                    x.LastName == "Test"
                    , _cts.Token);

            }
            catch (OperationCanceledException) { /* Sessizce sonlandır */ }
            catch (Exception ex)
            {
                try
                {
                    await _logger.TakeIt(
                        userId: null,
                        PageNameSpaceTitle: "public abstract partial class ThisPageIsBeingPrepared : ComponentBase, IDisposable",
                        action: $"LoadData",
                        exception: ex.Message,
                        stackTrace: ex.StackTrace
                    );
                }
                catch { }

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

                // db.Users.Add(use);
                // await db.SaveChangesAsync(_cts.Token);

                // db.Users.Update(use);
                // await db.SaveChangesAsync(_cts.Token);

                await ShowNotification("success", "Başarılı", "Kullanıcı kaydedildi.", null);

                await LoadData();
            }
            catch (Exception ex)
            {
                try
                {
                    await _logger.TakeIt(
                        userId: null,
                        PageNameSpaceTitle: "public abstract partial class ThisPageIsBeingPrepared : ComponentBase, IDisposable",
                        action: $"Action",
                        exception: ex.Message,
                        stackTrace: ex.StackTrace
                    );
                }
                catch { }
            }
            finally
            {
                await Task.Delay(4000);
                Btn_isProcessing_01 = false;
                await LoadData();
            }
        }
        #endregion

    }
}