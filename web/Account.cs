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
                    return Unauthorized(new { message = "E-posta veya şifre hatalı." });
                }

                if (user.Password != model.Password)
                {
                    return Unauthorized(new { message = "E-posta veya şifre hatalı." });
                }

                if (user.IsActive != true)
                {
                    return Unauthorized(new { message = "Hesabınız aktif değil." });
                }

                // 4. Claim Oluşturma
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.ContactInformation.Email ?? ""),
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Role, user.UsersType ?? "Customer"),
                    new Claim("LastLogin", DateTime.UtcNow.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 5. Auth Özellikleri
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // "Beni Hatırla" aktif
                    ExpiresUtc = DateTimeOffset.UtcNow.AddYears(1),
                    AllowRefresh = true,
                    IssuedUtc = DateTimeOffset.UtcNow
                };

                // 6. Giriş İşlemi
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                culture = (user.Language != null) ? user.Language : "en";

                // 7. Giriş bilgileri e-postası gönder
                await emailSender.SendLoginInfoEmailAsync(culture, model.Email);

                // 8. Başarılı Yanıt ve Yönlendirme URL'i dönüyoruz
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

        #region Hata Logları (Logs)

        private async Task LogException(Exception ex, UserDetail userDetails, string action)
        {
            try
            {
                var log = new Logs
                {
                    Id = Guid.NewGuid(),
                    UserId = GetCurrentUserId(),
                    PageNameSpaceTitle = "namespace web.Account.Controllers",
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