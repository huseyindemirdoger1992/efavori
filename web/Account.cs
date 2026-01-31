using System.Net.Mail;
using System.Security.Claims;
using api.tr;
using data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace web.Account.Controllers
{
    [Route("{culture}/Account/[action]")]
    public class AccountController : Controller
    {
        private readonly _ApplicationConnectionDb _db;
        private readonly UserInfos _userInfos;
        private readonly EmailSender _emailSender;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            _ApplicationConnectionDb db,
            UserInfos userInfos,
            EmailSender emailSender,
            ILogger<AccountController> logger)
        {
            _db = db;
            _userInfos = userInfos;
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginRequest model, string culture = "en")
        {
            try
            {
                // 1. Temel Kontroller
                if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                {
                    _logger.LogWarning("Login attempt with missing email or password");
                    return BadRequest(new { message = "E-posta ve şifre zorunludur." });
                }

                // 2. Kullanıcı Sorgulama
                var user = await _db.Users
                    .AsNoTracking()
                    .Include(u => u.ContactInformation)
                    .FirstOrDefaultAsync(u => u.ContactInformation.Email == model.Email);

                // 3. Doğrulama
                if (user == null)
                {
                    _logger.LogWarning("Login attempt for non-existent email: {Email}", model.Email);
                    return Unauthorized(new { message = "E-posta veya şifre hatalı." });
                }

                if (user.Password != model.Password)
                {
                    _logger.LogWarning("Failed login attempt for email: {Email}", model.Email);
                    return Unauthorized(new { message = "E-posta veya şifre hatalı." });
                }

                if (user.IsActive != true)
                {
                    _logger.LogWarning("Login attempt for inactive user: {Email}", model.Email);
                    return Unauthorized(new { message = "Hesabınız aktif değil." });
                }

                // 4. Claim Oluşturma
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.ContactInformation.Email ?? ""),
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Role, user.UsersType ?? "Customer"),
                    new Claim("LastLogin", DateTime.UtcNow.ToString("o"))
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 5. Auth Özellikleri
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddYears(1),
                    AllowRefresh = true,
                    IssuedUtc = DateTimeOffset.UtcNow
                };

                // 6. Giriş İşlemi
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                culture = !string.IsNullOrEmpty(user.Language) ? user.Language : "en";

                _logger.LogInformation("User logged in successfully: {Email}", model.Email);

                // 7. E-posta gönderme işlemini arka planda yap (hata olsa bile login'i engellemesin)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailSender.SendLoginInfoEmailAsync(culture, model.Email);
                    }
                    catch (Exception emailEx)
                    {
                        _logger.LogError(emailEx, "Failed to send login email to {Email}", model.Email);
                    }
                });

                // 8. Başarılı Yanıt
                return Redirect($"/{culture}/Customer/Home/Index");
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error during login for email: {Email}", model?.Email ?? "unknown");
                return StatusCode(500, new { message = "Veritabanı hatası oluştu. Lütfen daha sonra tekrar deneyiniz." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for email: {Email}", model?.Email ?? "unknown");

                // Kullanıcı bilgilerini güvenli şekilde al
                UserDetail? userDetails = null;
                try
                {
                    userDetails = _userInfos.GetCurrentUserDetails();
                }
                catch (Exception userInfoEx)
                {
                    _logger.LogError(userInfoEx, "Failed to get user details for logging");
                }

                // Log kaydı yap
                if (userDetails != null)
                {
                    await LogException(ex, userDetails, "Login");
                }

                return StatusCode(500, new { message = "Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz." });
            }
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Logout(string culture = "en")
        {
            try
            {
                // Kullanıcı bilgisini al
                var userEmail = User?.FindFirst(ClaimTypes.Email)?.Value;
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                _logger.LogInformation("User logging out: {Email}", userEmail ?? "unknown");

                // Oturumu kapat
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // Logout e-postası gönder (arka planda)
                if (!string.IsNullOrWhiteSpace(userEmail))
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _emailSender.SendLogOutInfoEmailAsync(culture, userEmail);
                        }
                        catch (Exception emailEx)
                        {
                            _logger.LogError(emailEx, "Failed to send logout email to {Email}", userEmail);
                        }
                    });
                }

                return Redirect($"/{culture}/Customer/Home/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");

                // Hata olsa bile logout yapsın
                try
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }
                catch { }

                return Redirect($"/{culture}/Customer/Home/Index");
            }
        }

        #region Hata Logları

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
                _logger.LogError(logEx, "Failed to write exception log to database");
            }
        }

        #endregion

        #region Yardımcı Metodlar

        private Guid? GetCurrentUserId()
        {
            try
            {
                var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
                {
                    return userId;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get current user ID");
            }

            return null;
        }

        #endregion
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
