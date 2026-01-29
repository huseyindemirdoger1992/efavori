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
        private readonly UserInfos _userInfos; // 1. Servisi ekle
        private readonly EmailSender emailSender; // 1. EmailSender alanını ekleyin

        // 2. Constructor'a EmailSender parametresini ekleyin
        public AccountController(_ApplicationConnectionDb db, UserInfos userInfos, EmailSender _emailSender)
        {
            _db = db;
            _userInfos = userInfos;
            emailSender = _emailSender;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        // JSON (Fetch/Axios) ile çağrılacağı için
        // ... önceki kodlar değişmedi ...
        public async Task<IActionResult> Login([FromForm] LoginRequest model, string culture = "en")
        {
            // 1. Temel Kontroller
            if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest(new { message = "E-posta ve şifre zorunludur." });

            // 2. Kullanıcı Sorgulama (AsNoTracking performansı artırır)
            var user = await _db.Users
                .AsNoTracking()
                .Include(u => u.ContactInformation)
                .FirstOrDefaultAsync(u => u.ContactInformation.Email == model.Email);

            // 3. Doğrulama (Not: Şifreleme/Hashing kullanmanı şiddetle öneririm)
            if (user == null || user.Password != model.Password)
                return Unauthorized(new { message = "E-posta veya şifre hatalı." });

            if (user.IsActive != true)
                return Unauthorized(new { message = "Hesabınız aktif değil." });

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

            #region Mail gönderir
            await emailSender.SendLoginInfoEmailAsync(culture, model.Email);
            #endregion
            // 7. Başarılı Yanıt ve Yönlendirme URL'i dönüyoruz
            return Redirect($"/{culture}/Customer/Home/Index");
        }

        [HttpGet, HttpPost] // Her iki yöntemi de desteklesin
        public async Task<IActionResult> Logout(string culture = "en")
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // Oturumu kapat
            return Redirect($"/{culture}/Customer/Home/Index"); // Ana sayfaya dön
        }
    }

    // Login isteği için yardımcı sınıf
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}