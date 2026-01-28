using api;
using data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace razor._Shared.tr.Header
{
    /// <summary>
    /// Veri odaklı sayfalar için standartlaştırılmış soyut temel sınıf.
    /// Profesyonel bir yapı; hata toleransı, kaynak yönetimi ve thread-safety üzerine kurulur.
    /// </summary>
    public partial class AccountActivationMailStatu : ComponentBase, IDisposable
    {
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
            // _ = StartAutoRefreshLoop(TimeSpan.FromSeconds(10));
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
        private bool Btn_isProcessing_02 = false;

        // E-posta gönderim durumu
        private bool EmailSentStatus = false;
        private int? EmailActivationCode = null;

        private int RemainingSeconds { get; set; } = 180;

        // Gelen kullanıcı parametresi
        [Parameter] public Users? use { get; set; }



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
                    PageNameSpaceTitle: "public abstract partial class ThisPageIsBeingPrepared : ComponentBase, IDisposable",
                    action: $"LoadData",
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
            if (EmailSentStatus) return;

            try
            {
                Btn_isProcessing_01 = true;
                EmailSentStatus = true;
                RemainingSeconds = 180; // Süreyi sıfırla

                Random rnd = new();
                EmailActivationCode = rnd.Next(100000, 999999);

                if (use != null && EmailActivationCode.HasValue)
                {
                    use.AccountActivationMailCode = EmailActivationCode.Value;
                }

                // E-posta gönderim işlemi burada tetikleniyorsa yap...
                await ShowNotification("success", "Mail Gönderildi", $"{use.ContactInformation.Email}", null);
            }
            catch (Exception ex)
            {
                await _logger.TakeIt(
                    userId: null,
                    PageNameSpaceTitle: "AccountActivationMailStatu",
                    action: "Action",
                    exception: ex.Message,
                    stackTrace: ex.StackTrace
                );
            }
            finally
            {
                // 180 saniyeyi birer birer düşürelim
                while (RemainingSeconds > 0)
                {
                    await Task.Delay(1000); // 1 saniye bekle
                    RemainingSeconds--;
                    StateHasChanged(); // UI'ı her saniye güncelle
                }

                Btn_isProcessing_01 = false;
                EmailSentStatus = false;
                await LoadData();
            }
        }
        protected async Task EmailControl()
        {
            if (Btn_isProcessing_02) return;
            try
            {
                Btn_isProcessing_02 = true;

                Random rnd = new();
                EmailActivationCode = rnd.Next(100000, 999999);

                if (use != null && EmailActivationCode.HasValue)
                {
                    use.AccountActivationMailCode = EmailActivationCode.Value;
                    use.AccountActivationMailStatu = true;
                    await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);
                    db.Users.Update(use);
                    await db.SaveChangesAsync(_cts.Token);
                }
                await ShowNotification("success", "Tebrikler", $"Email adresiniz doğrulandı.", null);
            }
            catch (Exception ex)
            {
                await _logger.TakeIt(
                    userId: null,
                    PageNameSpaceTitle: "AccountActivationMailStatu",
                    action: "Action",
                    exception: ex.Message,
                    stackTrace: ex.StackTrace
                );
            }
            finally
            {
                Btn_isProcessing_02 = false;
                await Task.Delay(3000);
                Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
            }
        }
        #endregion

    }
}