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
        private readonly UserInfos _userInfos; // 1. Servisi ekle

        public AccountActivationMailStatu(UserInfos userInfos, EmailSender _emailSender)
        {
            _userInfos = userInfos;
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
            // 1. Guard Clauses: Hızlı kontrol ve erken çıkış
            if (Btn_isProcessing_01 || EmailSentStatus || use == null)
            {
                if (use == null) await ShowNotification("danger", "Hata", "Kullanıcı bulunamadı.", null);
                return;
            }

            try
            {
                // 2. State Başlangıcı
                Btn_isProcessing_01 = true;
                EmailSentStatus = true;
                RemainingSeconds = 180;

                // 3. Aktivasyon Kodu Üretimi (Modern Random)
                use.AccountActivationMailCode = Random.Shared.Next(100000, 999999);
                // 4. Veritabanı İşlemi
                await using var db = await DbFactory.CreateDbContextAsync(_cts.Token);
                db.Users.Update(use);
                await db.SaveChangesAsync(_cts.Token);

                // TODO: Mail gönderme servisini burada await edin

                var attachments = new List<Attachment>();
                //attachments.Add(new Attachment("C:\\dosyalar\\rapor.pdf"));
                //attachments.Add(new Attachment("C:\\dosyalar\\resim.jpg"));

                var details = _userInfos.GetCurrentUserDetails();
                string safeIp = System.Net.WebUtility.HtmlEncode(details.IpAddress);
                string safeDevice = System.Net.WebUtility.HtmlEncode(details.UserAgent);
                string displayDate = DateTime.Now.ToString("dd MMMM yyyy HH:mm");

                if (use.Language == "tr")
                {
                    // E-posta aktivasyon kodu ve güvenlik bilgilerini içeren mail içeriği
                    string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
        
        <div style='padding: 30px 30px 20px 30px; text-align: center;'>
            <img src='https://1drv.ms/i/c/ce20faaddbdfba2e/IQRDszi9BOX5R5T7a1eLE650ASZlWwgS5x-PiZHVWp1UzD4?width=180&height=180' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
        </div>

        <div style='padding: 0 40px 30px 40px;'>
            <h2 style='color: #1a237e; font-size: 20px; font-weight: 600; margin-bottom: 10px; text-align: center;'>E-posta Doğrulama</h2>
            <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>Hesap doğrulama işleminizi tamamlamak için aşağıdaki kodu kullanın.</p>
            
            <div style='background-color: #f0f4ff; border: 1px solid #d1d9e6; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
                <span style='display: block; font-size: 11px; color: #5c6bc0; text-transform: uppercase; font-weight: bold; margin-bottom: 8px;'>Doğrulama Kodunuz</span>
                <span style='font-family: ""Courier New"", Courier, monospace; font-size: 36px; font-weight: bold; color: #1a237e; letter-spacing: 6px;'>{use.AccountActivationMailCode}</span>
            </div>

            <div style='border-top: 1px solid #eee; padding-top: 20px;'>
                <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>İşlem Bilgileri</p>
                <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                    <tr>
                        <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Tarih</td>
                        <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>IP Adresi</td>
                        <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>Cihaz</td>
                        <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                    </tr>
                </table>
            </div>

            <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
                Bu kodun süresi 3 dakika geçerlidir. Eğer bu işlemi siz yapmadıysanız lütfen hesabınızı güvenceye alın.
            </p>
        </div>

        <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
            <p style='font-size: 10px; color: #bbb; margin: 0;'>
                Bu mail otomatik olarak gönderilmiştir. Lütfen yanıtlamayınız.
                <br>İşlem Kayıt No: {details.TraceId}
            </p>
        </div>
    </div>";

                    await emailSender.SendEmailAsync("security@efavori.com", use.ContactInformation.Email, $"{use.AccountActivationMailCode} - Hesap Doğrulama Kodunuz", emailBody, attachments);
                }
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
                // Sayfa kapandı veya işlem iptal edildi, sessizce çık
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
        // Kullanıcının input alanına girdiği değer için bir değişken olduğunu varsayıyorum: 
        // [Parameter] public int UserEnteredCode { get; set; }

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