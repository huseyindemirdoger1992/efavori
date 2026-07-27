using api.tr;
using data;
using data._Products;
using data._Users;
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
    public partial class AccountActivationMailStatu : ComponentBase, IAsyncDisposable
    {
        #region Services
        [Inject] protected IDbContextFactory<_ApplicationConnectionDb> DbFactory { get; init; } = default!;
        [Inject] protected NavigationManager Navigation { get; init; } = default!;
        [Inject] protected IJSRuntime JS { get; init; } = default!;
        [Inject] protected TakeLogs Logger { get; init; } = default!;

        private readonly EmailSender emailSender;

        public AccountActivationMailStatu(EmailSender _emailSender)
        {
            emailSender = _emailSender;
        }

        private DateTime Deadline;

        #endregion

        #region State Management
        protected readonly CancellationTokenSource _cts = new();
        private PeriodicTimer? _refreshTimer; // Timer referansı

        private Notification? notificationRef;
        private string? Email = null;
        #endregion

        #region Lifecycle
        protected override async Task OnInitializedAsync()
        {
            try
            {
                // İlk yükleme: Kullanıcı ekranı görmeden verinin hazır olması istenir.
                await LoadData();

                // HATA FİKS #1: Null check yapıldıktan sonra Deadline set et
                if (use != null && !string.IsNullOrWhiteSpace(use.AccountActivationMailDeadline?.ToString()))
                {
                    Deadline = Convert.ToDateTime(use.AccountActivationMailDeadline);
                }
                else
                {
                    Deadline = DateTime.Now.AddMinutes(30); // Varsayılan deadline
                }

                // Arka plan işleyicisi: UI akışını engellemeden (fire-and-forget) başlatılır.
                // _ = StartAutoRefreshLoop(TimeSpan.FromSeconds(10));
            }
            catch (Exception ex)
            {
                await Logger.TakeIt(
                    userId: use?.Id,
                    PageNameSpaceTitle: "namespace razor._Shared.tr.Header",
                    action: "OnInitializedAsync",
                    exception: ex.Message,
                    stackTrace: ex.StackTrace
                );
                await ShowNotification("danger", "Başlangıç Hatası", "Sayfa yüklenirken bir hata oluştu.", null);
            }
        }

        #region Orchestration
        private async Task StartAutoRefreshLoop(TimeSpan interval)
        {
            using var timer = new PeriodicTimer(interval);
            _refreshTimer = timer; // Timer referansını kaydet

            try
            {
                while (await timer.WaitForNextTickAsync(_cts.Token))
                {
                    await LoadData();
                }
            }
            catch (OperationCanceledException)
            {
                // Expected - component disposed
            }
            catch (Exception ex)
            {
                await Logger.TakeIt(
                    userId: use?.Id,
                    PageNameSpaceTitle: "namespace razor._Shared.tr.Header",
                    action: "StartAutoRefreshLoop",
                    exception: ex.Message,
                    stackTrace: ex.StackTrace
                );
            }
        }

        private async Task ShowNotification(string type, string title, string text, string? image)
        {
            if (notificationRef is null) return;

            try
            {
                var imageUrl = string.IsNullOrWhiteSpace(image) ? "https://picsum.photos/120?" : image;
                await notificationRef.Launch(type, title, text, imageUrl);
            }
            catch (Exception ex)
            {
                await Logger.TakeIt(
                    userId: use?.Id,
                    PageNameSpaceTitle: "namespace razor._Shared.tr.Header",
                    action: "ShowNotification",
                    exception: ex.Message,
                    stackTrace: ex.StackTrace
                );
            }
        }
        #endregion

        // HATA FİKS #2: IDisposable yerine IAsyncDisposable kullan
        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            _cts.Cancel();
            _cts.Dispose();

            GC.SuppressFinalize(this);
        }

        #endregion

        //----------------------------------------------

        #region Data Operations

        // Aksiyon buton grubu için thread-safe kontrol
        private bool Btn_isProcessing_01 = false;
        private bool Btn_isProcessing_02 = false;

        // E-posta gönderim durumu
        private bool EmailSentStatus = false;

        // HATA FİKS #3: Nullable int yerine string kullan (input field genellikle string gelir)
        private string EmailActivationCode = string.Empty;

        private int RemainingSeconds { get; set; } = 180;

        // Gelen kullanıcı parametresi
        [Parameter] public Users? use { get; set; }

        protected virtual async Task LoadData()
        {
            try
            {
                // TODO: Veri yükleme işlemleri burada yapılacak
                StateHasChanged();
                Email = use.ContactInformation.Email;
            }
            catch (Exception ex)
            {
                await Logger.TakeIt(
                    userId: use?.Id,
                    PageNameSpaceTitle: "namespace razor._Shared.tr.Header",
                    action: "LoadData",
                    exception: ex.Message,
                    stackTrace: ex.StackTrace
                );
            }
        }

        protected async Task Action()
        {
            // HATA FİKS #4: Reentrance kontrolü ve null check sırasının düzeltilmesi
            if (Btn_isProcessing_01)
            {
                return; // Zaten işlem yapılıyor
            }

            if (use == null)
            {
                await ShowNotification("danger", "Hata", "Kullanıcı bulunamadı.", null);
                return;
            }

            try
            {
                Btn_isProcessing_01 = true;
                EmailSentStatus = true;
                RemainingSeconds = 180;

                // E-posta gönder
                await emailSender.SendAccountActivationCodeInfoEmailAsync(use.Language, Email);

                await ShowNotification("success", "Mail Gönderildi", $"{Email}", null);

                // HATA FİKS #5: Timer döngüsü içinde State tracking
                while (RemainingSeconds > 0)
                {
                    try
                    {
                        await Task.Delay(1000, _cts.Token);
                        RemainingSeconds--;
                        StateHasChanged();
                    }
                    catch (OperationCanceledException)
                    {
                        break; // Component disposed
                    }
                }

                // Süre dolduğunda notification göster
                if (RemainingSeconds <= 0)
                {
                    await ShowNotification("success", "Bilgi", "Doğrulama kodu süresi doldu. Yeniden gönderebilirsiniz.", null);
                }
            }
            catch (OperationCanceledException)
            {
                // Component disposed - bu exception'u handle et
                await ShowNotification("info", "İşlem İptal Edildi", "Sayfa kapatıldı.", null);
            }
            catch (Exception ex)
            {
                await Logger.TakeIt(
                    userId: use?.Id,
                    PageNameSpaceTitle: "namespace razor._Shared.tr.Header",
                    action: "Action",
                    exception: ex.Message,
                    stackTrace: ex.StackTrace
                );
                await ShowNotification("danger", "Hata", "E-posta gönderirken bir hata oluştu.", null);
            }
            finally
            {
                // HATA FİKS #6: Reset işlemini merkezi yönet
                Btn_isProcessing_01 = false;
                EmailSentStatus = false;
                RemainingSeconds = 180;
                StateHasChanged();
            }
        }

        protected async Task EmailControl()
        {
            if (Btn_isProcessing_02) return;
            if (use == null)
            {
                await ShowNotification("danger", "Hata", "Kullanıcı bilgileri yüklenemedi.", null);
                return;
            }

            try
            {
                Btn_isProcessing_02 = true;

                if (!int.TryParse(EmailActivationCode, out var enteredCode))
                {
                    await ShowNotification("danger", "Hata", "Lütfen geçerli bir kod girin.", null);
                    return;
                }

                // ⭐ Her işlem öncesi database'den güncel veriyi çek
                await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);
                var currentUser = await db.Users.FirstOrDefaultAsync(u => u.Id == use.Id, _cts.Token);

                if (currentUser == null)
                {
                    await ShowNotification("danger", "Hata", "Kullanıcı bulunamadı.", null);
                    return;
                }

                // ⭐ Güncel veride kontrol et
                if (currentUser.AccountActivationMailCode != 0 && currentUser.AccountActivationMailCode == enteredCode)
                {
                    currentUser.AccountActivationMailStatu = true;
                    db.Users.Update(currentUser);
                    await db.SaveChangesAsync(_cts.Token);

                    // ⭐ Local state'i de güncelle
                    use = currentUser;
                    await emailSender.SendAccountEmailAddressVerifiedNotificationAsync(use.Language, use.ContactInformation.Email);
                    await ShowNotification("success", "Tebrikler", "Email adresiniz başarıyla doğrulandı.", null);

                    await Task.Delay(2000, _cts.Token);
                    Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
                }
                else
                {
                    await ShowNotification("danger", "Hata",
                        currentUser.AccountActivationMailCode == 0
                            ? "Sistemde kod kaydı bulunamadı."
                            : "Girdiğiniz kod boş veya yanlış.",
                        null);
                }
            }
            catch (OperationCanceledException)
            {
                await ShowNotification("info", "İşlem İptal Edildi", "Sayfa kapatıldı.", null);
            }
            catch (DbUpdateException ex)
            {
                await Logger.TakeIt(userId: use?.Id, PageNameSpaceTitle: "namespace razor._Shared.tr.Header", action: "EmailControl - DbUpdate", exception: ex.Message, stackTrace: ex.StackTrace);
                await ShowNotification("danger", "Veritabanı Hatası", "Doğrulama kaydedilemedi. Lütfen tekrar deneyin.", null);
            }
            catch (Exception ex)
            {
                await Logger.TakeIt(userId: use?.Id, PageNameSpaceTitle: "namespace razor._Shared.tr.Header", action: "EmailControl", exception: ex.Message, stackTrace: ex.StackTrace);
                await ShowNotification("danger", "Sistem Hatası", "İşlem sırasında bir hata oluştu.", null);
            }
            finally
            {
                Btn_isProcessing_02 = false;
                StateHasChanged();
            }
        }       
        #endregion
    }
}