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

            var attachments = new List<Attachment>();
            //attachments.Add(new Attachment("C:\\dosyalar\\rapor.pdf"));
            //attachments.Add(new Attachment("C:\\dosyalar\\resim.jpg"));

            var details = _userInfos.GetCurrentUserDetails();
            string safeIp = System.Net.WebUtility.HtmlEncode(details.IpAddress);
            string safeDevice = System.Net.WebUtility.HtmlEncode(details.UserAgent);
            string displayDate = DateTime.Now.ToString("dd MMMM yyyy HH:mm");

            if (culture == "tr")
            {
                string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
        
        <div style='padding: 30px 30px 20px 30px; text-align: center;'>
            <img src='https://1drv.ms/i/c/ce20faaddbdfba2e/IQRDszi9BOX5R5T7a1eLE650ASZlWwgS5x-PiZHVWp1UzD4?width=180&height=180' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
        </div>

        <div style='padding: 0 40px 30px 40px;'>
            <h2 style='color: #1a237e; font-size: 20px; font-weight: 600; margin-bottom: 20px; border-bottom: 2px solid #f0f0f0; padding-bottom: 10px; text-align: center;'>Güvenlik Bildirimi: Yeni Giriş</h2>
            
            <p style='font-size: 15px;'>Sayın Kullanıcımız,</p>
            <p style='font-size: 14px; color: #555;'>Hesabınıza yönelik gerçekleştirilen yeni bir oturum açma işlemi tespit edilmiştir. Güvenliğiniz için işlemin ayrıntıları aşağıdadır:</p>
            
            <table style='width: 100%; border-collapse: collapse; margin: 25px 0; background-color: #f8f9fa; border-radius: 6px;'>
                <tr>
                    <td style='padding: 12px 15px; font-weight: 600; color: #666; font-size: 13px; border-bottom: 1px solid #ffffff;'>Tarih / Saat</td>
                    <td style='padding: 12px 15px; font-size: 13px; border-bottom: 1px solid #ffffff;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 12px 15px; font-weight: 600; color: #666; font-size: 13px; border-bottom: 1px solid #ffffff;'>IP Adresi</td>
                    <td style='padding: 12px 15px; font-size: 13px; font-family: monospace; border-bottom: 1px solid #ffffff;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 12px 15px; font-weight: 600; color: #666; font-size: 13px;'>Cihaz Bilgisi</td>
                    <td style='padding: 12px 15px; font-size: 12px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>

            <p style='font-size: 13px; color: #777; text-align: center;'>
                Bu erişim tarafınızca sağlandıysa bu bildirimi dikkate almayabilirsiniz.
            </p>

            <div style='background-color: #fff4f4; border-left: 4px solid #d32f2f; border-right: 4px solid #d32f2f; padding: 20px; margin-top: 25px; text-align: center;'>
                <p style='margin: 0 0 15px 0; font-size: 13px; color: #b71c1c; line-height: 1.5;'>
                    <strong>Eğer bu işlem size ait değilse;</strong> hesabınızın güvenliğini sağlamak adına lütfen vakit kaybetmeden şifrenizi güncelleyiniz ve destek birimimizle iletişime geçiniz.
                </p>
                <hr style='border: 0; border-top: 1px solid rgba(211, 47, 47, 0.2); margin: 15px 0;' />
                <a href='https://efavori.com' style='display: inline-block; font-size: 14px; color: #d32f2f; text-decoration: none; font-weight: bold;'>
                    efavori internet sitesi
                </a>
            </div>
        </div>

        <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
            <p style='font-size: 11px; color: #999; margin: 0;'>
                Bu e-posta, hesabınızın güvenliğini sağlamak amacıyla sistem tarafından otomatik olarak gönderilmiştir.
            </p>
            <p style='font-size: 10px; color: #bbb; margin-top: 8px; letter-spacing: 0.5px;'>
                İşlem Kayıt No (Trace ID): {details.TraceId}
            </p>
        </div>
    </div>";
                await emailSender.SendEmailAsync(model.Email, "Güvenlik Uyarısı: Yeni Giriş Yapıldı", emailBody, attachments);
            }
            if (culture == "en")
            {
                string emailBody = $@"
<div style='font-family: ""Inter"", -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e8e8e8; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 12px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 25px 30px; text-align: center;'>
        <img src='https://1drv.ms/i/c/ce20faaddbdfba2e/IQRDszi9BOX5R5T7a1eLE650ASZlWwgS5x-PiZHVWp1UzD4?width=180&height=180' alt='Security Logo' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #1a237e; font-size: 22px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Security Alert: New Sign-in</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>We detected a new sign-in to your account.</p>
        
        <p style='font-size: 15px; font-weight: 600;'>Hello,</p>
        <p style='font-size: 14px; color: #555;'>For your protection, we're sending this email to ensure it was you. Here are the details of the sign-in activity:</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #fcfcfc; border: 1px solid #f0f0f0; border-radius: 8px;'>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #f0f0f0;'>Date / Time</td>
                <td style='padding: 14px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
            </tr>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #f0f0f0;'>IP Address</td>
                <td style='padding: 14px 15px; font-size: 13px; font-family: ""Courier New"", Courier, monospace; color: #333; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
            </tr>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px;'>Device</td>
                <td style='padding: 14px 15px; font-size: 13px; color: #333;'>{safeDevice}</td>
            </tr>
        </table>

        <p style='font-size: 13px; color: #888; text-align: center; font-style: italic;'>
            If this was you, you can safely ignore this message.
        </p>

        <div style='background-color: #fff9f9; border: 1px solid #ffdbdb; border-radius: 8px; padding: 20px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 15px 0; font-size: 14px; color: #c62828; font-weight: 500;'>
                <strong>Wasn't you?</strong>
            </p>
            <p style='margin: 0 0 15px 0; font-size: 13px; color: #444; line-height: 1.5;'>
                Please change your password immediately and contact our support team to secure your account.
            </p>
            <a href='https://efavori.com' style='display: inline-block; padding: 10px 20px; background-color: #d32f2f; color: #ffffff; text-decoration: none; font-weight: 600; font-size: 13px; border-radius: 5px;'>
                Secure My Account
            </a>
        </div>
    </div>

    <div style='background-color: #f9fafb; padding: 25px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 11px; color: #999; margin: 0;'>
            This is an automated security notification. Please do not reply to this email.
        </p>
        <p style='font-size: 10px; color: #bbb; margin-top: 10px; letter-spacing: 0.5px; text-transform: uppercase;'>
            Trace ID: {details.TraceId}
        </p>
    </div>
</div>";
                await emailSender.SendEmailAsync(model.Email, "Security Alert: New Sign-in Detected", emailBody, attachments);
            }
            if (culture == "az")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 10px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 2px 10px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://1drv.ms/i/c/ce20faaddbdfba2e/IQRDszi9BOX5R5T7a1eLE650ASZlWwgS5x-PiZHVWp1UzD4?width=180&height=180' alt='Security' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #1a237e; font-size: 20px; font-weight: 700; margin-bottom: 15px; border-bottom: 2px solid #f5f5f5; padding-bottom: 15px; text-align: center;'>Təhlükəsizlik Bildirişi: Yeni Giriş</h2>
        
        <p style='font-size: 15px; font-weight: 600;'>Hörmətli İstifadəçi,</p>
        <p style='font-size: 14px; color: #555;'>Hesabınıza yeni bir giriş qeydə alınmışdır. Təhlükəsizliyiniz üçün daxilolma məlumatlarını nəzərdən keçirin:</p>
        
        <table style='width: 100%; border-collapse: collapse; margin: 25px 0; background-color: #fcfcfc; border: 1px solid #f0f0f0; border-radius: 8px;'>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #666; font-size: 13px; border-bottom: 1px solid #f0f0f0;'>Tarix və Saat</td>
                <td style='padding: 12px 15px; font-size: 13px; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
            </tr>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #666; font-size: 13px; border-bottom: 1px solid #f0f0f0;'>IP Ünvanı</td>
                <td style='padding: 12px 15px; font-size: 13px; font-family: monospace; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
            </tr>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #666; font-size: 13px;'>Cihaz Məlumatı</td>
                <td style='padding: 12px 15px; font-size: 12px; color: #444;'>{safeDevice}</td>
            </tr>
        </table>

        <p style='font-size: 13px; color: #888; text-align: center;'>
            Əgər bu giriş sizin tərəfinizdən edilibsə, bu bildirişə məhəl qoymaya bilərsiniz.
        </p>

        <div style='background-color: #fffafa; border: 1px solid #ffeded; border-radius: 8px; padding: 20px; margin-top: 25px; text-align: center;'>
            <p style='margin: 0 0 12px 0; font-size: 14px; color: #b71c1c; font-weight: bold;'>
                Bu əməliyyatı siz etməmisinizsə:
            </p>
            <p style='margin: 0 0 18px 0; font-size: 13px; color: #444;'>
                Hesabınızın təhlükəsizliyini təmin etmək üçün dərhal şifrənizi yeniləyin və dəstək komandamızla əlaqə saxlayın.
            </p>
            <a href='https://efavori.com' style='display: inline-block; background-color: #d32f2f; color: #ffffff; padding: 10px 20px; text-decoration: none; font-size: 13px; font-weight: bold; border-radius: 4px;'>
                Hesabı qoru
            </a>
        </div>
    </div>

    <div style='background-color: #f8f9fa; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 11px; color: #999; margin: 0;'>
            Bu e-poçt təhlükəsizlik məqsədilə sistem tərəfindən avtomatik göndərilib.
        </p>
        <p style='font-size: 10px; color: #bbb; margin-top: 8px;'>
            Əməliyyat ID (Trace ID): {details.TraceId}
        </p>
    </div>
</div>";

                await emailSender.SendEmailAsync(model.Email, "Təhlükəsizlik Xəbərdarlığı: Yeni Giriş", emailBody, attachments);
            }
            if (culture == "de")
            {
                string emailBody = $@"
<div style='font-family: ""Helvetica Neue"", Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #dddddd; padding: 0; border-radius: 10px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 5px 15px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://1drv.ms/i/c/ce20faaddbdfba2e/IQRDszi9BOX5R5T7a1eLE650ASZlWwgS5x-PiZHVWp1UzD4?width=180&height=180' alt='Sicherheit' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #0d47a1; font-size: 20px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Sicherheitswarnung: Neue Anmeldung</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>Ein neuer Login-Vorgang wurde in Ihrem Konto registriert.</p>
        
        <p style='font-size: 15px;'>Guten Tag,</p>
        <p style='font-size: 14px; color: #555;'>Zu Ihrer Sicherheit informieren wir Sie über eine neue Anmeldung bei Ihrem Konto. Hier sind die Details:</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #fafafa; border: 1px solid #eeeeee; border-radius: 8px;'>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #eeeeee;'>Datum / Uhrzeit</td>
                <td style='padding: 12px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #eeeeee;'>{displayDate}</td>
            </tr>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #eeeeee;'>IP-Adresse</td>
                <td style='padding: 12px 15px; font-size: 13px; font-family: monospace; color: #333; border-bottom: 1px solid #eeeeee;'>{safeIp}</td>
            </tr>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px;'>Gerät</td>
                <td style='padding: 12px 15px; font-size: 13px; color: #333;'>{safeDevice}</td>
            </tr>
        </table>

        <p style='font-size: 12px; color: #999; text-align: center;'>
            Wenn Sie dies waren, können Sie diese Nachricht ignorieren.
        </p>

        <div style='background-color: #fff5f5; border: 1px solid #feb2b2; border-radius: 8px; padding: 20px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 14px; color: #c53030; font-weight: bold;'>
                Waren Sie das nicht?
            </p>
            <p style='margin: 0 0 20px 0; font-size: 13px; color: #4a5568;'>
                Bitte ändern Sie sofort Ihr Passwort und kontaktieren Sie unseren Support, um Ihr Konto zu schützen.
            </p>
            <a href='https://efavori.com' style='display: inline-block; padding: 12px 24px; background-color: #c53030; color: #ffffff; text-decoration: none; font-weight: bold; font-size: 13px; border-radius: 6px;'>
                Konto sichern
            </a>
        </div>
    </div>

    <div style='background-color: #f7f9fc; padding: 25px 40px; text-align: center; border-top: 1px solid #eeeeee;'>
        <p style='font-size: 11px; color: #a0aec0; margin: 0;'>
            Dies ist eine automatisch generierte Sicherheitsbenachrichtigung. Bitte antworten Sie nicht auf diese E-Mail.
        </p>
        <p style='font-size: 10px; color: #cbd5e0; margin-top: 10px; letter-spacing: 0.5px;'>
            Trace-ID: {details.TraceId}
        </p>
    </div>
</div>";

                await emailSender.SendEmailAsync(model.Email, "Sicherheitswarnung: Neue Anmeldung erkannt", emailBody, attachments);
            }
            if (culture == "es")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.03);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://1drv.ms/i/c/ce20faaddbdfba2e/IQRDszi9BOX5R5T7a1eLE650ASZlWwgS5x-PiZHVWp1UzD4?width=180&height=180' alt='Seguridad' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #1a237e; font-size: 22px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Alerta de Seguridad: Nuevo Inicio de Sesión</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>Hemos detectado una nueva actividad de acceso en su cuenta.</p>
        
        <p style='font-size: 15px;'>Estimado usuario/a,</p>
        <p style='font-size: 14px; color: #555;'>Para su protección, le informamos sobre los detalles del reciente inicio de sesión:</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #f9f9f9; border: 1px solid #eeeeee; border-radius: 8px;'>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #eeeeee;'>Fecha / Hora</td>
                <td style='padding: 14px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #eeeeee;'>{displayDate}</td>
            </tr>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #eeeeee;'>Dirección IP</td>
                <td style='padding: 14px 15px; font-size: 13px; font-family: monospace; color: #333; border-bottom: 1px solid #eeeeee;'>{safeIp}</td>
            </tr>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px;'>Dispositivo</td>
                <td style='padding: 14px 15px; font-size: 13px; color: #333;'>{safeDevice}</td>
            </tr>
        </table>

        <p style='font-size: 12px; color: #999; text-align: center;'>
            Si ha sido usted, puede ignorar este mensaje con total seguridad.
        </p>

        <div style='background-color: #fff8f8; border: 1px solid #ffebeb; border-radius: 8px; padding: 25px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 15px; color: #d32f2f; font-weight: bold;'>
                ¿No reconoce esta actividad?
            </p>
            <p style='margin: 0 0 20px 0; font-size: 13px; color: #555;'>
                Proteja su cuenta de inmediato cambiando su contraseña y contactando con nuestro equipo de soporte.
            </p>
            <a href='https://efavori.com' style='display: inline-block; padding: 12px 25px; background-color: #d32f2f; color: #ffffff; text-decoration: none; font-weight: bold; font-size: 13px; border-radius: 6px;'>
                Asegurar mi cuenta
            </a>
        </div>
    </div>

    <div style='background-color: #f4f5f7; padding: 25px 40px; text-align: center; border-top: 1px solid #eeeeee;'>
        <p style='font-size: 11px; color: #9aa4af; margin: 0;'>
            Esta es una notificación automática de seguridad. Por favor, no responda a este correo.
        </p>
        <p style='font-size: 10px; color: #bcccdc; margin-top: 10px; letter-spacing: 0.5px;'>
            ID de Seguimiento: {details.TraceId}
        </p>
    </div>
</div>";

                await emailSender.SendEmailAsync(model.Email, "Alerta de seguridad: Nuevo inicio de sesión detectado", emailBody, attachments);
            }
            if (culture == "fr")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://1drv.ms/i/c/ce20faaddbdfba2e/IQRDszi9BOX5R5T7a1eLE650ASZlWwgS5x-PiZHVWp1UzD4?width=180&height=180' alt='Sécurité' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #1a237e; font-size: 20px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Alerte de sécurité : Nouvelle connexion</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>Une nouvelle activité de connexion a été détectée sur votre compte.</p>
        
        <p style='font-size: 15px;'>Bonjour,</p>
        <p style='font-size: 14px; color: #555;'>Pour votre sécurité, nous vous informons des détails de cette connexion :</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #fcfcfc; border: 1px solid #f0f0f0; border-radius: 8px;'>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #f0f0f0;'>Date et heure</td>
                <td style='padding: 12px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
            </tr>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #f0f0f0;'>Adresse IP</td>
                <td style='padding: 12px 15px; font-size: 13px; font-family: monospace; color: #333; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
            </tr>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px;'>Appareil</td>
                <td style='padding: 12px 15px; font-size: 13px; color: #333;'>{safeDevice}</td>
            </tr>
        </table>

        <p style='font-size: 12px; color: #999; text-align: center;'>
            S'il s'agit de vous, vous pouvez ignorer cet e-mail en toute sécurité.
        </p>

        <div style='background-color: #fff9f9; border: 1px solid #ffebeb; border-radius: 10px; padding: 25px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 15px; color: #c62828; font-weight: bold;'>
                Ce n'était pas vous ?
            </p>
            <p style='margin: 0 0 20px 0; font-size: 13px; color: #555;'>
                Veuillez changer votre mot de passe immédiatement et contacter notre support pour sécuriser votre compte.
            </p>
            <a href='https://efavori.com' style='display: inline-block; padding: 12px 25px; background-color: #d32f2f; color: #ffffff; text-decoration: none; font-weight: bold; font-size: 13px; border-radius: 6px;'>
                Sécuriser mon compte
            </a>
        </div>
    </div>

    <div style='background-color: #f8f9fa; padding: 25px 40px; text-align: center; border-top: 1px solid #eeeeee;'>
        <p style='font-size: 11px; color: #9aa4af; margin: 0;'>
            Ceci est une notification automatique. Merci de ne pas répondre à cet e-mail.
        </p>
        <p style='font-size: 10px; color: #bcccdc; margin-top: 10px; letter-spacing: 0.5px;'>
            ID de suivi : {details.TraceId}
        </p>
    </div>
</div>";

                await emailSender.SendEmailAsync(model.Email, "Alerte de sécurité : Nouvelle connexion détectée", emailBody, attachments);
            }
            if (culture == "hi")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://1drv.ms/i/c/ce20faaddbdfba2e/IQRDszi9BOX5R5T7a1eLE650ASZlWwgS5x-PiZHVWp1UzD4?width=180&height=180' alt='Security' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #1a237e; font-size: 22px; font-weight: 700; margin-bottom: 10px; text-align: center;'>सुरक्षा अलर्ट: नया लॉगिन</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>आपके खाते में एक नया लॉगिन देखा गया है।</p>
        
        <p style='font-size: 15px;'>नमस्ते,</p>
        <p style='font-size: 14px; color: #555;'>आपकी सुरक्षा के लिए, हम आपको इस लॉगिन के विवरण भेज रहे हैं:</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #f9f9f9; border: 1px solid #eeeeee; border-radius: 8px;'>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #eeeeee;'>दिनांक / समय</td>
                <td style='padding: 12px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #eeeeee;'>{displayDate}</td>
            </tr>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #eeeeee;'>आईपी पता (IP Address)</td>
                <td style='padding: 12px 15px; font-size: 13px; font-family: monospace; color: #333; border-bottom: 1px solid #eeeeee;'>{safeIp}</td>
            </tr>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px;'>डिवाइस (Device)</td>
                <td style='padding: 12px 15px; font-size: 13px; color: #333;'>{safeDevice}</td>
            </tr>
        </table>

        <p style='font-size: 12px; color: #999; text-align: center;'>
            यदि यह आप ही थे, तो आप इस संदेश को नज़रअंदाज़ कर सकते हैं।
        </p>

        <div style='background-color: #fff8f8; border: 1px solid #ffebeb; border-radius: 10px; padding: 25px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 16px; color: #d32f2f; font-weight: bold;'>
                क्या यह आप नहीं थे?
            </p>
            <p style='margin: 0 0 20px 0; font-size: 14px; color: #555;'>
                कृपया तुरंत अपना पासवर्ड बदलें और अपने खाते को सुरक्षित करने के लिए हमारी सहायता टीम से संपर्क करें।
            </p>
            <a href='https://efavori.com' style='display: inline-block; padding: 12px 25px; background-color: #d32f2f; color: #ffffff; text-decoration: none; font-weight: bold; font-size: 14px; border-radius: 6px;'>
                मेरा खाता सुरक्षित करें
            </a>
        </div>
    </div>

    <div style='background-color: #f4f5f7; padding: 25px 40px; text-align: center; border-top: 1px solid #eeeeee;'>
        <p style='font-size: 11px; color: #9aa4af; margin: 0;'>
            यह एक स्वचालित सुरक्षा सूचना है। कृपया इस ईमेल का उत्तर न दें।
        </p>
        <p style='font-size: 10px; color: #bcccdc; margin-top: 10px; letter-spacing: 0.5px;'>
            ट्रेस आईडी (Trace ID): {details.TraceId}
        </p>
    </div>
</div>";

                await emailSender.SendEmailAsync(model.Email, "सुरक्षा अलर्ट: नया लॉगिन मिला", emailBody, attachments);
            }
            if (culture == "pt")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.03);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://1drv.ms/i/c/ce20faaddbdfba2e/IQRDszi9BOX5R5T7a1eLE650ASZlWwgS5x-PiZHVWp1UzD4?width=180&height=180' alt='Segurança' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #1a237e; font-size: 22px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Alerta de Segurança: Novo Acesso</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>Detectamos um novo início de sessão na sua conta.</p>
        
        <p style='font-size: 15px;'>Olá,</p>
        <p style='font-size: 14px; color: #555;'>Para sua proteção, aqui estão os detalhes do acesso recente:</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #f9f9f9; border: 1px solid #eeeeee; border-radius: 8px;'>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #eeeeee;'>Data / Hora</td>
                <td style='padding: 14px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #eeeeee;'>{displayDate}</td>
            </tr>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #eeeeee;'>Endereço IP</td>
                <td style='padding: 14px 15px; font-size: 13px; font-family: monospace; color: #333; border-bottom: 1px solid #eeeeee;'>{safeIp}</td>
            </tr>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px;'>Dispositivo</td>
                <td style='padding: 14px 15px; font-size: 13px; color: #333;'>{safeDevice}</td>
            </tr>
        </table>

        <p style='font-size: 12px; color: #999; text-align: center;'>
            Se foi você, pode ignorar esta mensagem com segurança.
        </p>

        <div style='background-color: #fff8f8; border: 1px solid #ffebeb; border-radius: 10px; padding: 25px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 15px; color: #d32f2f; font-weight: bold;'>
                Não reconhece este acesso?
            </p>
            <p style='margin: 0 0 20px 0; font-size: 13px; color: #555;'>
                Proteja sua conta imediatamente alterando sua senha e entrando em contato com nossa equipe de suporte.
            </p>
            <a href='https://efavori.com' style='display: inline-block; padding: 12px 25px; background-color: #d32f2f; color: #ffffff; text-decoration: none; font-weight: bold; font-size: 13px; border-radius: 6px;'>
                Segurar minha conta
            </a>
        </div>
    </div>

    <div style='background-color: #f4f5f7; padding: 25px 40px; text-align: center; border-top: 1px solid #eeeeee;'>
        <p style='font-size: 11px; color: #9aa4af; margin: 0;'>
            Esta é uma notificação automática de segurança. Por favor, não responda a este e-mail.
        </p>
        <p style='font-size: 10px; color: #bcccdc; margin-top: 10px; letter-spacing: 0.5px;'>
            ID de Rastreamento: {details.TraceId}
        </p>
    </div>
</div>";

                await emailSender.SendEmailAsync(model.Email, "Alerta de segurança: Novo acesso detectado", emailBody, attachments);
            }
            if (culture == "ru")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.03);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://1drv.ms/i/c/ce20faaddbdfba2e/IQRDszi9BOX5R5T7a1eLE650ASZlWwgS5x-PiZHVWp1UzD4?width=180&height=180' alt='Безопасность' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #1a237e; font-size: 20px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Оповещение о безопасности: Новый вход</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>В вашем аккаунте зафиксирован новый вход в систему.</p>
        
        <p style='font-size: 15px;'>Здравствуйте,</p>
        <p style='font-size: 14px; color: #555;'>В целях безопасности мы уведомляем вас о деталях недавнего сеанса:</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #f9f9f9; border: 1px solid #eeeeee; border-radius: 8px;'>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #eeeeee;'>Дата / Время</td>
                <td style='padding: 12px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #eeeeee;'>{displayDate}</td>
            </tr>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #eeeeee;'>IP-адрес</td>
                <td style='padding: 12px 15px; font-size: 13px; font-family: monospace; color: #333; border-bottom: 1px solid #eeeeee;'>{safeIp}</td>
            </tr>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px;'>Устройство</td>
                <td style='padding: 12px 15px; font-size: 13px; color: #333;'>{safeDevice}</td>
            </tr>
        </table>

        <p style='font-size: 12px; color: #999; text-align: center;'>
            Если это были вы, просто проигнорируйте это сообщение.
        </p>

        <div style='background-color: #fff8f8; border: 1px solid #ffebeb; border-radius: 10px; padding: 25px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 15px; color: #d32f2f; font-weight: bold;'>
                Это были не вы?
            </p>
            <p style='margin: 0 0 20px 0; font-size: 13px; color: #555;'>
                Пожалуйста, немедленно смените пароль и свяжитесь с нашей службой поддержки для защиты вашего аккаунта.
            </p>
            <a href='https://efavori.com' style='display: inline-block; padding: 12px 25px; background-color: #d32f2f; color: #ffffff; text-decoration: none; font-weight: bold; font-size: 13px; border-radius: 6px;'>
                Обезопасить мой аккаунт
            </a>
        </div>
    </div>

    <div style='background-color: #f4f5f7; padding: 25px 40px; text-align: center; border-top: 1px solid #eeeeee;'>
        <p style='font-size: 11px; color: #9aa4af; margin: 0;'>
            Это автоматическое уведомление системы безопасности. Пожалуйста, не отвечайте на это письмо.
        </p>
        <p style='font-size: 10px; color: #bcccdc; margin-top: 10px; letter-spacing: 0.5px;'>
            ID транзакции (Trace ID): {details.TraceId}
        </p>
    </div>
</div>";

                await emailSender.SendEmailAsync(model.Email, "Оповещение о безопасности: Обнаружен новый вход", emailBody, attachments);
            }
            if (culture == "zh")
            {
                string emailBody = $@"
<div style='font-family: ""PingFang SC"", ""Microsoft YaHei"", ""Helvetica Neue"", Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.03);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://1drv.ms/i/c/ce20faaddbdfba2e/IQRDszi9BOX5R5T7a1eLE650ASZlWwgS5x-PiZHVWp1UzD4?width=180&height=180' alt='安全通知' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #1a237e; font-size: 22px; font-weight: 700; margin-bottom: 10px; text-align: center;'>安全警报：新登录活动</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>您的账户检测到一次新的登录。</p>
        
        <p style='font-size: 15px;'>尊敬的用户，您好：</p>
        <p style='font-size: 14px; color: #555;'>为了保护您的账户安全，以下是本次登录的具体信息：</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #f9f9f9; border: 1px solid #eeeeee; border-radius: 8px;'>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #eeeeee;'>日期 / 时间</td>
                <td style='padding: 14px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #eeeeee;'>{displayDate}</td>
            </tr>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #eeeeee;'>IP 地址</td>
                <td style='padding: 14px 15px; font-size: 13px; font-family: monospace; color: #333; border-bottom: 1px solid #eeeeee;'>{safeIp}</td>
            </tr>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px;'>设备信息</td>
                <td style='padding: 14px 15px; font-size: 13px; color: #333;'>{safeDevice}</td>
            </tr>
        </table>

        <p style='font-size: 12px; color: #999; text-align: center;'>
            如果是您本人操作，可以忽略此邮件。
        </p>

        <div style='background-color: #fff8f8; border: 1px solid #ffebeb; border-radius: 10px; padding: 25px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 15px; color: #d32f2f; font-weight: bold;'>
                不是您本人操作？
            </p>
            <p style='margin: 0 0 20px 0; font-size: 13px; color: #555;'>
                请立即修改您的密码，并联系我们的客服团队以确保您的账户安全。
            </p>
            <a href='https://efavori.com' style='display: inline-block; padding: 12px 25px; background-color: #d32f2f; color: #ffffff; text-decoration: none; font-weight: bold; font-size: 14px; border-radius: 6px;'>
                保护我的账户
            </a>
        </div>
    </div>

    <div style='background-color: #f4f5f7; padding: 25px 40px; text-align: center; border-top: 1px solid #eeeeee;'>
        <p style='font-size: 11px; color: #9aa4af; margin: 0;'>
            这是系统生成的自动安全通知，请勿直接回复。
        </p>
        <p style='font-size: 10px; color: #bcccdc; margin-top: 10px; letter-spacing: 0.5px;'>
            追踪 ID (Trace ID): {details.TraceId}
        </p>
    </div>
</div>";

                await emailSender.SendEmailAsync(model.Email, "安全警报：检测到新登录", emailBody, attachments);
            }

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