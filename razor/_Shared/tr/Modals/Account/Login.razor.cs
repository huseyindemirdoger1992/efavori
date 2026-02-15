using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Threading.Tasks;
using api;
using api.tr;
using data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace razor._Shared.tr.Modals.Account
{
    public partial class Login : ComponentBase
    {
        [Inject] private IHttpClientFactory HttpClientFactory { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private Notification? notificationRef;
        private readonly TakeLogs _logger;
        private readonly _ApplicationConnectionDb _db;
        private readonly UserInfos _userInfos; // 1. UserInfos servisi eklendi
        private readonly EmailSender emailSender; // 2. EmailSender servisi


        public Login(_ApplicationConnectionDb db, TakeLogs logger, UserInfos userInfos, EmailSender emailSender)
        {
            _db = db;
            _logger = logger;
            _userInfos = userInfos;
            this.emailSender = emailSender;
        }

        private bool Btn_isProcessing_01 = false;

        private async Task ShowNotification(string type, string title, string text, string? image)
        {
            var imageUrl = string.IsNullOrWhiteSpace(image) ? "https://picsum.photos/120?" : image;
            if (notificationRef != null)
                await notificationRef.Launch(type, title, text, imageUrl);
        }

        private string? email = "superadmin@efavori.com";
        private string? password = "Dragonfire0!";

        private async Task LoginUserAsync()
        {
            if (Btn_isProcessing_01) return;

            try
            {
                Btn_isProcessing_01 = true;

                // Kullanıcı bilgilerini al
                var userDetails = _userInfos.GetCurrentUserDetails();

                // 1. Temel Kontroller
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    await LogFailedLoginAttempt(null, userDetails, "Boş email veya şifre");
                    await ShowNotification("danger", "Hata", "E-posta ve şifre gereklidir.", null);
                    return;
                }

                // Backend'in kültürü anlaması için
                var uri = new Uri(Navigation.Uri);
                var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var culture = segments.Length > 0 ? segments[0] : "en";

                // 2. Kullanıcı Sorgulama
                var user = await _db.Users
                    .AsNoTracking()
                    .Include(u => u.ContactInformation)
                    .FirstOrDefaultAsync(u => u.ContactInformation.Email == email);

                // 3. Doğrulama
                if (user == null)
                {
                    await LogFailedLoginAttempt(null, userDetails, "E-posta bulunamadı");
                    await ShowNotification("danger", "Hata", "Girilen bilgilere ait kullanıcı bulunamadı.", null);
                    return;
                }
                string trypass = GlobalSecurityProvider.Decrypt(user.Password);
                if (trypass != password)
                {
                    await LogFailedLoginAttempt(user.Id, userDetails, "Şifre hatalı");
                    await ShowNotification("danger", "Hata", "E-posta ile girilen şifre eşleşmiyor.", null);
                    emailSender.SendLoginErrorInfoEmailAsync(user.Language, email);
                    return;
                }

                if (user.IsActive != true)
                {
                    await LogFailedLoginAttempt(user.Id, userDetails, "Hesap aktif değil");
                    await ShowNotification("danger", "Hata", "Hesabınız aktif değil.", null);
                    return;
                }

                // 4. Başarılı giriş denemesini kaydet
                await LogSuccessfulLoginAttempt(user.Id, userDetails);

                // 5. Başarılı giriş bildirimi göster
                await ShowNotification("success", "Başarılı", "Giriş yapılıyor...", null);

                // 6. Dil tercihi güncelle
                culture = user.Language ?? culture;

                // 7. Giriş formunu submit et
                var endpoint = $"/{culture}/Account/Login";
                await JSRuntime.InvokeVoidAsync("submitLoginForm", endpoint, email, password);
            }
            catch (Exception ex)
            {
                // Hata loglaması
                await LogException(ex, "LoginUserAsync");
                await ShowNotification("danger", "Hata", "Giriş sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.", null);
            }
            finally
            {
                await Task.Delay(4000);
                Btn_isProcessing_01 = false;
            }
        }

        #region Giriş/Çıkış Kayıtları (LoginTry)

        private async Task LogSuccessfulLoginAttempt(Guid userId, UserDetail userDetails)
        {
            try
            {
                var loginTry = new LoginTry
                {
                    Id = Guid.NewGuid(),
                    UsersId = userId,
                    AttemptDate = DateTime.UtcNow,
                    IsSuccessful = true,
                    IPAddress = userDetails.IpAddress,
                    UserAgent = userDetails.UserAgent,
                    RequestPath = userDetails.RequestPath,
                    Platform = ExtractPlatform(userDetails.UserAgent),
                    Browser = ExtractBrowser(userDetails.UserAgent)
                };

                _db.LoginTry.Add(loginTry);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoginTry kayıt hatası: {ex.Message}");
            }
        }

        private async Task LogFailedLoginAttempt(Guid? userId, UserDetail userDetails, string reason)
        {
            try
            {
                var loginTry = new LoginTry
                {
                    Id = Guid.NewGuid(),
                    UsersId = userId,
                    AttemptDate = DateTime.UtcNow,
                    IsSuccessful = false,
                    IPAddress = userDetails.IpAddress,
                    UserAgent = userDetails.UserAgent,
                    RequestPath = userDetails.RequestPath,
                    Platform = ExtractPlatform(userDetails.UserAgent),
                    Browser = ExtractBrowser(userDetails.UserAgent)
                };

                _db.LoginTry.Add(loginTry);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoginTry kayıt hatası: {ex.Message}");
            }
        }

        #endregion

        #region Hata Logları (Logs)

        private async Task LogException(Exception ex, string action)
        {
            try
            {
                var userDetails = _userInfos.GetCurrentUserDetails();

                var log = new Logs
                {
                    Id = Guid.NewGuid(),
                    UserId = null, // Henüz giriş yapılmadığı için userId null
                    PageNameSpaceTitle = "razor._Shared.tr.Modals.Account",
                    Action = action,
                    IpAddress = userDetails.IpAddress,
                    UserAgent = userDetails.UserAgent,
                    RequestPath = userDetails.RequestPath,
                    Languages = userDetails.Languages,
                    Exception = ex.Message,
                    StackTrace = ex.StackTrace,
                    Date = DateTime.UtcNow
                };

                _db.Logs.Add(log);
                await _db.SaveChangesAsync();
            }
            catch (Exception logEx)
            {
                System.Diagnostics.Debug.WriteLine($"Hata loglama başarısız: {logEx.Message}");
            }
        }

        #endregion

        #region Yardımcı Metodlar

        /// <summary>
        /// User-Agent'ından platform bilgisini çıkarır (iOS, Android, Windows, macOS, Linux, Web)
        /// </summary>
        private string ExtractPlatform(string userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
                return "Unknown";

            userAgent = userAgent.ToLower();

            if (userAgent.Contains("iphone") || userAgent.Contains("ipad") || userAgent.Contains("ipod"))
                return "iOS";
            if (userAgent.Contains("android"))
                return "Android";
            if (userAgent.Contains("windows"))
                return "Windows";
            if (userAgent.Contains("macintosh") || userAgent.Contains("mac os"))
                return "macOS";
            if (userAgent.Contains("linux"))
                return "Linux";

            return "Web";
        }

        /// <summary>
        /// User-Agent'ından tarayıcı bilgisini çıkarır (Chrome, Firefox, Safari, Edge, vb.)
        /// </summary>
        private string ExtractBrowser(string userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
                return "Unknown";

            userAgent = userAgent.ToLower();

            if (userAgent.Contains("edg/") || userAgent.Contains("edge/"))
                return "Edge";
            if (userAgent.Contains("chrome/"))
                return "Chrome";
            if (userAgent.Contains("firefox/"))
                return "Firefox";
            if (userAgent.Contains("safari/") && !userAgent.Contains("chrome/"))
                return "Safari";
            if (userAgent.Contains("opera/") || userAgent.Contains("opr/"))
                return "Opera";
            if (userAgent.Contains("trident/"))
                return "Internet Explorer";

            return "Unknown";
        }

        #endregion
    }
}