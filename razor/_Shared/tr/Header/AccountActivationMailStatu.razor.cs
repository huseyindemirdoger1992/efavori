using api;
using data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Net.Mail;
using System.Text.RegularExpressions;

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

        private readonly EmailSender emailSender; // 1. EmailSender alanını ekleyin

        public AccountActivationMailStatu(EmailSender _emailSender)
        {
            emailSender = _emailSender;
        }

        private DateTime Deadline;

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

            if (use != null)
            {
                Deadline = Convert.ToDateTime(use.AccountActivationMailDeadline);
            }
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

        protected virtual async Task LoadData()
        {
            try
            {
            }
            finally
            {
                StateHasChanged();
            }
        }

        protected async Task Action()
        {
            if (Btn_isProcessing_01 || EmailSentStatus || use == null)
            {
                if (use == null) await ShowNotification("danger", "Hata", "Kullanıcı bulunamadı.", null);
                return;
            }

            try
            {
                Btn_isProcessing_01 = true;
                EmailSentStatus = true;
                RemainingSeconds = 180;

                await emailSender.SendAccountActivationCodeInfoEmailAsync(use.Language, use.ContactInformation.Email);

                await ShowNotification("success", "Mail Gönderildi", $"{use.ContactInformation.Email}", null);

                // 5. Timer Döngüsü (CancellationToken desteği ile)
                // Kullanıcı sayfadan çıkarsa Task.Delay hata fırlatır ve döngü durur (Bellek yönetimi)
                while (RemainingSeconds > 0)
                {
                    await Task.Delay(1000, _cts.Token);
                    RemainingSeconds--;
                    StateHasChanged();
                }
                await ShowNotification("success", "Bilgi", "Doğrulama kodu süresi doldu. Yeniden gönderebilirsiniz.", null);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                await _logger.TakeIt(
                    userId: use?.Id,
                    PageNameSpaceTitle: "AccountActivationMailStatu",
                    action: "Action",
                    exception: ex.Message,
                    stackTrace: ex.StackTrace
                );
                await ShowNotification("danger", "Hata", "Bir sorun oluştu.", null);
            }
            finally
            {
                // 6. Resetleme: Sadece döngü bittiğinde veya hata aldığında çalışır
                Btn_isProcessing_01 = false;
                EmailSentStatus = false;
                RemainingSeconds = 180;
                StateHasChanged();
            }
        }

        protected async Task EmailControl()
        {
            if (Btn_isProcessing_02) return;

            try
            {
                Btn_isProcessing_02 = true;

                // 1. Validasyon Kontrolleri
                if (use == null)
                {
                    await ShowNotification("danger", "Hata", "Kullanıcı bilgileri yüklenemedi.", null);
                    return;
                }

                if (use.AccountActivationMailCode != 0 && use.AccountActivationMailCode == EmailActivationCode)
                {
                    // Başarılı Durum
                    use.AccountActivationMailStatu = true;

                    await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);
                    db.Users.Update(use);
                    await db.SaveChangesAsync(_cts.Token);

                    await ShowNotification("success", "Tebrikler", "Email adresiniz başarıyla doğrulandı.", null);

                    // Başarılıysa yönlendir (Sayfayı yenilemek yerine ana sayfaya veya panele git)
                    await Task.Delay(4000);
                    Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
                }
                else
                {
                    // Hatalı Kod Durumu
                    await ShowNotification("danger", "Hata", "Girdiğiniz kod boş veya yanlış.", null);
                }
            }
            catch (Exception ex)
            {
                await _logger.TakeIt(
                    userId: use?.Id, // ID'yi loglamak hata çözmeyi kolaylaştırır
                    PageNameSpaceTitle: "AccountActivation",
                    action: "EmailControl",
                    exception: ex.Message,
                    stackTrace: ex.StackTrace
                );
                await ShowNotification("danger", "Sistem Hatası", "İşlem sırasında bir hata oluştu.", null);
            }
            finally
            {
                Btn_isProcessing_02 = false;
            }
        }
        #endregion

    }
}