using System.Net.Mail;
using System.Security.Claims;
using api;
using data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace web.Account.Controllers
{
    // Rota tanımını netleştirdik, [controller] kullanımı esneklik sağlar
    [Route("{culture}/Account/[action]")]
    public class AccountController : Controller
    {
        private readonly _ApplicationConnectionDb _db;
        private readonly UserInfos _userInfos; // 1. UserInfos servisi
        private readonly EmailSender emailSender; // 2. EmailSender servisi

        // 3. Constructor'a servisleri ekleyin
        public AccountController(_ApplicationConnectionDb db, UserInfos userInfos, EmailSender _emailSender)
        {
            _db = db;
            _userInfos = userInfos;
            emailSender = _emailSender;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        // JSON (Fetch/Axios) ile çağrılacağı için
        public async Task<IActionResult> Login([FromForm] LoginRequest model, string culture = "en")
        {
            var userDetails = _userInfos.GetCurrentUserDetails();

            try
            {
                // 1. Temel Kontroller
                if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                {
                    await LogFailedLoginAttempt(null, userDetails, "Boş email veya şifre");
                    return BadRequest(new { message = "E-posta ve şifre zorunludur." });
                }

                // 2. Kullanıcı Sorgulama (AsNoTracking performansı artırır)
                var user = await _db.Users
                    .AsNoTracking()
                    .Include(u => u.ContactInformation)
                    .FirstOrDefaultAsync(u => u.ContactInformation.Email == model.Email);

                // 3. Doğrulama (Not: Şifreleme/Hashing kullanmanı şiddetle öneririm)
                if (user == null)
                {
                    await LogFailedLoginAttempt(null, userDetails, "E-posta bulunamadı");
                    return Unauthorized(new { message = "E-posta veya şifre hatalı." });
                }

                if (user.Password != model.Password)
                {
                    await LogFailedLoginAttempt(user.Id, userDetails, "Şifre hatalı");
                    return Unauthorized(new { message = "E-posta veya şifre hatalı." });
                }

                if (user.IsActive != true)
                {
                    await LogFailedLoginAttempt(user.Id, userDetails, "Hesap aktif değil");
                    return Unauthorized(new { message = "Hesabınız aktif değil." });
                }

                // 4. Başarılı giriş denemesini kaydet
                await LogSuccessfulLoginAttempt(user.Id, userDetails);

                // 5. Claim Oluşturma
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.ContactInformation.Email ?? ""),
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Role, user.UsersType ?? "Customer"),
                    new Claim("LastLogin", DateTime.UtcNow.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 6. Auth Özellikleri
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // "Beni Hatırla" aktif
                    ExpiresUtc = DateTimeOffset.UtcNow.AddYears(1),
                    AllowRefresh = true,
                    IssuedUtc = DateTimeOffset.UtcNow
                };

                // 7. Giriş İşlemi
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                culture = (user.Language != null) ? user.Language : "en";

                // 8. Giriş bilgileri e-postası gönder
                await emailSender.SendLoginInfoEmailAsync(culture, model.Email);

                // 9. Başarılı Yanıt ve Yönlendirme URL'i dönüyoruz
                return Redirect($"/{culture}/Customer/Home/Index");
            }
            catch (Exception ex)
            {
                // Hata loglaması
                await LogException(ex, userDetails, "Login");
                return StatusCode(500, new { message = "Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz." });
            }
        }

        [HttpGet, HttpPost] // Her iki yöntemi de desteklesin
        public async Task<IActionResult> Logout(string culture = "en")
        {
            var userDetails = _userInfos.GetCurrentUserDetails();

            try
            {
                // Oturum kapatmadan önce kullanıcı bilgisini al
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Çıkış denemesini kaydet
                if (!string.IsNullOrWhiteSpace(userId) && Guid.TryParse(userId, out var userGuid))
                {
                    await LogLogoutAttempt(userGuid, userDetails);
                }

                // Oturumu kapat
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // Logout e-postası gönder
                if (!string.IsNullOrWhiteSpace(userEmail))
                {
                    await emailSender.SendLogOutInfoEmailAsync(culture, userEmail);
                }

                return Redirect($"/{culture}/Customer/Home/Index"); // Ana sayfaya dön
            }
            catch (Exception ex)
            {
                // Hata loglaması
                await LogException(ex, userDetails, "Logout");
                return StatusCode(500, new { message = "Çıkış sırasında bir hata oluştu." });
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
                // Log kaydı başarısız olsa bile uygulamayı kırmayın
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

        private async Task LogLogoutAttempt(Guid userId, UserDetail userDetails)
        {
            try
            {
                var loginTry = new LoginTry
                {
                    Id = Guid.NewGuid(),
                    UsersId = userId,
                    AttemptDate = DateTime.UtcNow,
                    IsSuccessful = true, // Çıkış başarılı
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
                System.Diagnostics.Debug.WriteLine($"Logout kayıt hatası: {ex.Message}");
            }
        }

        #endregion

        #region Hata Logları (Logs)

        private async Task LogException(Exception ex, UserDetail userDetails, string action)
        {
            try
            {
                var log = new Logs
                {
                    Id = Guid.NewGuid(),
                    UserId = GetCurrentUserId(),
                    PageNameSpaceTitle = "Account.Controller",
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
        /// User-Agent'ından platform bilgisini çıkarır (Web, Mobile, vb.)
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
        /// User-Agent'ından tarayıcı bilgisini çıkarır
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

        /// <summary>
        /// Mevcut kullanıcının ID'sini veya null'ı döndürür
        /// </summary>
        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
                return userId;

            return null;
        }

        #endregion
    }

    // Login isteği için yardımcı sınıf
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
