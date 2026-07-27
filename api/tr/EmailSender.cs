using data; // EmailHistory sınıfının bulunduğu namespace
using data._Products;
using data._Users;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; // Dosya isimlerini birleştirmek için
using System.Net;
using System.Net.Mail;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace api.tr
{
    public class EmailSender
    {
        [Inject] protected IDbContextFactory<_ApplicationConnectionDb> DbFactory { get; init; } = default!;
        protected readonly CancellationTokenSource _cts = new();

        private string? SmtpServer = null;
        private int SmtpPort = 0;
        private string? EmailAddress = null;
        private string? EmailPassword = null;

        private readonly TakeLogs _logger;
        private readonly _ApplicationConnectionDb _context;
        private readonly UserInfos _userInfos;

        public EmailSender(TakeLogs logger, _ApplicationConnectionDb context, UserInfos userInfos)
        {
            _logger = logger;
            _context = context;
            _userInfos = userInfos;
        }
        // Email gönderme aksiyonu
        public async Task SendEmailAsync(string SenderEmailAddress, string recipient, string subject, string body, List<Attachment>? attachments = null)
        {
            using var message = new MailMessage();
            switch (SenderEmailAddress.ToLower())
            {
                case "security@efavori.com":
                    SmtpServer = "srvm16.trwww.com";
                    SmtpPort = 587;
                    EmailAddress = "security@efavori.com";
                    EmailPassword = "SXAk4obokyOePCJ48VsR";
                    message.From = new MailAddress(EmailAddress, EmailAddress);
                    break;

                case "tasks@efavori.com":
                    SmtpServer = "srvm16.trwww.com";
                    SmtpPort = 587;
                    EmailAddress = "tasks@efavori.com";
                    EmailPassword = "BrrXG9xF9DZGdWFAzpn6";
                    message.From = new MailAddress(EmailAddress, EmailAddress);
                    break;

                default:
                    SmtpServer = "srvm16.trwww.com";
                    SmtpPort = 587;
                    EmailAddress = "security@efavori.com";
                    EmailPassword = "SXAk4obokyOePCJ48VsR";
                    message.From = new MailAddress(EmailAddress, EmailAddress);
                    break;
            }
            // Kayıt için kullanılacak ek dosya isimleri
            string attachmentNames = attachments != null
                ? string.Join(", ", attachments.Select(a => a.Name))
                : string.Empty;

            try
            {
                message.To.Add(recipient);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                if (attachments != null && attachments.Count > 0)
                {
                    foreach (var attachment in attachments)
                    {
                        message.Attachments.Add(attachment);
                    }
                }

                using var smtp = new SmtpClient(SmtpServer, SmtpPort)
                {
                    Credentials = new NetworkCredential(EmailAddress, EmailPassword),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };

                await smtp.SendMailAsync(message);
                var details = _userInfos.GetCurrentUserDetails();

                // --- KAYIT İŞLEMİ BAŞLIYOR ---
                var history = new EmailHistory
                {

                    FromWhom = EmailAddress,
                    ToWho = recipient,
                    Subject = subject,
                    Body = body,
                    Attachments = attachmentNames,
                    TraceId = details.TraceId.ToString(),
                    SentDate = DateTime.UtcNow
                };

                _context.EmailHistory.Add(history);
                await _context.SaveChangesAsync();
                // --- KAYIT İŞLEMİ BİTTİ ---
            }
            catch (Exception ex)
            {
                try
                {
                    await _logger.TakeIt(
                        userId: null,
                        PageNameSpaceTitle: "namespace api",
                        action: "SendEmailAsync",
                        exception: ex.Message,
                        stackTrace: ex.StackTrace
                    );
                }
                catch { }
            }
        }

        // security@efavori.com | Yeni kullanıcı hesabı oluşturulduğunda
        public async Task SendNewRegisterInfoEmailAsync(string Culture, string Email)
        {
            var attachments = new List<Attachment>();
            //attachments.Add(new Attachment("C:\\dosyalar\\rapor.pdf"));
            //attachments.Add(new Attachment("C:\\dosyalar\\resim.jpg"));

            var details = _userInfos.GetCurrentUserDetails();
            string safeIp = System.Net.WebUtility.HtmlEncode(details.IpAddress);
            string safeDevice = System.Net.WebUtility.HtmlEncode(details.UserAgent);
            string displayDate = DateTime.Now.ToString("dd MMMM yyyy HH:mm");

            if (Culture == "tr")
            {
                string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
        
        <div style='padding: 40px 30px 20px 30px; text-align: center; background: linear-gradient(to bottom, #f8f9ff, #ffffff);'>
            <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 130px; height: auto; display: inline-block;' />
        </div>

        <div style='padding: 0 40px 30px 40px;'>
            <h2 style='color: #1a237e; font-size: 24px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Aramıza Hoş Geldiniz!</h2>
            <p style='font-size: 15px; color: #555; text-align: center;'>Hesabınız başarıyla oluşturuldu. efavori dünyasına ilk adımınızı attığınız için heyecanlıyız.</p>
            
            <div style='text-align: center; margin: 30px 0;'>
                <a href='https://efavori.com/en/Customer/Home/Index' style='background-color: #1a237e; color: #ffffff; padding: 12px 30px; text-decoration: none; border-radius: 6px; font-weight: 600; font-size: 14px; display: inline-block;'>
                    Hemen Başlayın
                </a>
            </div>

            <div style='background-color: #f8f9fa; border: 1px solid #eee; border-radius: 8px; padding: 20px; margin-top: 20px;'>
                <p style='margin-top: 0; margin-bottom: 15px; font-size: 13px; font-weight: 700; color: #1a237e; border-bottom: 1px solid #e0e0e0; padding-bottom: 8px;'>Kayıt İşlem Detayları</p>
                <table style='width: 100%; border-collapse: collapse;'>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777;'>IP Adresi</td>
                        <td style='padding: 6px 0; font-size: 12px; color: #333; text-align: right; font-family: monospace;'>{safeIp}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777;'>Tarih / Saat</td>
                        <td style='padding: 6px 0; font-size: 12px; color: #333; text-align: right;'>{displayDate}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777; vertical-align: top;'>Cihaz Bilgisi</td>
                        <td style='padding: 6px 0; font-size: 11px; color: #333; text-align: right; line-height: 1.4;'>{safeDevice}</td>
                    </tr>
                </table>
            </div>

            <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
                Bu kayıt işlemi size ait değilse, lütfen güvenlik ekibimizle iletişime geçiniz.
            </p>
        </div>

        <div style='background-color: #f4f4f4; padding: 25px 40px; text-align: center; border-top: 1px solid #eee;'>
            <p style='font-size: 11px; color: #999; margin: 0;'>
                Bu bir sistem mesajıdır, lütfen yanıtlamayınız.
            </p>
            <p style='font-size: 10px; color: #bbb; margin-top: 8px;'>
                İşlem Kayıt No: {details.TraceId} | © 2026 efavori
            </p>
        </div>
    </div>";
                await SendEmailAsync("security@efavori.com", Email, "efavori'ye Hoş Geldiniz!", emailBody, attachments);
            }
            if (Culture == "en")
            {
                string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
        
        <div style='padding: 40px 30px 20px 30px; text-align: center; background: linear-gradient(to bottom, #f8f9ff, #ffffff);'>
            <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 130px; height: auto; display: inline-block;' />
        </div>

        <div style='padding: 0 40px 30px 40px;'>
            <h2 style='color: #1a237e; font-size: 24px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Welcome to Our Community!</h2>
            <p style='font-size: 15px; color: #555; text-align: center;'>Your account has been successfully created. We are excited to have you take your first step into the world of efavori.</p>
            
            <div style='text-align: center; margin: 30px 0;'>
                <a href='https://efavori.com/en/Customer/Home/Index' style='background-color: #1a237e; color: #ffffff; padding: 12px 30px; text-decoration: none; border-radius: 6px; font-weight: 600; font-size: 14px; display: inline-block;'>
                    Get Started Now
                </a>
            </div>

            <div style='background-color: #f8f9fa; border: 1px solid #eee; border-radius: 8px; padding: 20px; margin-top: 20px;'>
                <p style='margin-top: 0; margin-bottom: 15px; font-size: 13px; font-weight: 700; color: #1a237e; border-bottom: 1px solid #e0e0e0; padding-bottom: 8px;'>Registration Details</p>
                <table style='width: 100%; border-collapse: collapse;'>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777;'>IP Address</td>
                        <td style='padding: 6px 0; font-size: 12px; color: #333; text-align: right; font-family: monospace;'>{safeIp}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777;'>Date / Time</td>
                        <td style='padding: 6px 0; font-size: 12px; color: #333; text-align: right;'>{displayDate}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777; vertical-align: top;'>Device Info</td>
                        <td style='padding: 6px 0; font-size: 11px; color: #333; text-align: right; line-height: 1.4;'>{safeDevice}</td>
                    </tr>
                </table>
            </div>

            <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
                If this registration does not belong to you, please contact our security team.
            </p>
        </div>

        <div style='background-color: #f4f4f4; padding: 25px 40px; text-align: center; border-top: 1px solid #eee;'>
            <p style='font-size: 11px; color: #999; margin: 0;'>
                This is a system message, please do not reply.
            </p>
            <p style='font-size: 10px; color: #bbb; margin-top: 8px;'>
                Trace ID: {details.TraceId} | © 2026 efavori
            </p>
        </div>
    </div>";

                await SendEmailAsync("security@efavori.com", Email, "Welcome to efavori!", emailBody, attachments);

            }
            if (Culture == "az")
            {
                string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
        
        <div style='padding: 40px 30px 20px 30px; text-align: center; background: linear-gradient(to bottom, #f8f9ff, #ffffff);'>
            <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 130px; height: auto; display: inline-block;' />
        </div>

        <div style='padding: 0 40px 30px 40px;'>
            <h2 style='color: #1a237e; font-size: 24px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Aramıza Xoş Gəldiniz!</h2>
            <p style='font-size: 15px; color: #555; text-align: center;'>Hesabınız uğurla yaradıldı. efavori dünyasına ilk addımınızı atdığınız üçün həyəcanlıyıq.</p>
            
            <div style='text-align: center; margin: 30px 0;'>
                <a href='https://efavori.com/en/Customer/Home/Index' style='background-color: #1a237e; color: #ffffff; padding: 12px 30px; text-decoration: none; border-radius: 6px; font-weight: 600; font-size: 14px; display: inline-block;'>
                    İndi Başlayın
                </a>
            </div>

            <div style='background-color: #f8f9fa; border: 1px solid #eee; border-radius: 8px; padding: 20px; margin-top: 20px;'>
                <p style='margin-top: 0; margin-bottom: 15px; font-size: 13px; font-weight: 700; color: #1a237e; border-bottom: 1px solid #e0e0e0; padding-bottom: 8px;'>Qeydiyyat Məlumatları</p>
                <table style='width: 100%; border-collapse: collapse;'>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777;'>IP Ünvanı</td>
                        <td style='padding: 6px 0; font-size: 12px; color: #333; text-align: right; font-family: monospace;'>{safeIp}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777;'>Tarix / Saat</td>
                        <td style='padding: 6px 0; font-size: 12px; color: #333; text-align: right;'>{displayDate}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777; vertical-align: top;'>Cihaz Məlumatı</td>
                        <td style='padding: 6px 0; font-size: 11px; color: #333; text-align: right; line-height: 1.4;'>{safeDevice}</td>
                    </tr>
                </table>
            </div>

            <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
                Bu qeydiyyat prosesi sizə aid deyilsə, xahiş edirik təhlükəsizlik komandamızla əlaqə saxlayın.
            </p>
        </div>

        <div style='background-color: #f4f4f4; padding: 25px 40px; text-align: center; border-top: 1px solid #eee;'>
            <p style='font-size: 11px; color: #999; margin: 0;'>
                Bu bir sistem mesajıdır, xahiş edirik cavablandırmayın.
            </p>
            <p style='font-size: 10px; color: #bbb; margin-top: 8px;'>
                Trace ID: {details.TraceId} | © 2026 efavori
            </p>
        </div>
    </div>";

                await SendEmailAsync("security@efavori.com", Email, "efavori-yə Xoş Gəldiniz!", emailBody, attachments);
            }
            if (Culture == "de")
            {
                string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
        
        <div style='padding: 40px 30px 20px 30px; text-align: center; background: linear-gradient(to bottom, #f8f9ff, #ffffff);'>
            <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 130px; height: auto; display: inline-block;' />
        </div>

        <div style='padding: 0 40px 30px 40px;'>
            <h2 style='color: #1a237e; font-size: 24px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Willkommen bei uns!</h2>
            <p style='font-size: 15px; color: #555; text-align: center;'>Ihr Konto wurde erfolgreich erstellt. Wir freuen uns, dass Sie den ersten Schritt in die Welt von efavori gemacht haben.</p>
            
            <div style='text-align: center; margin: 30px 0;'>
                <a href='https://efavori.com/en/Customer/Home/Index' style='background-color: #1a237e; color: #ffffff; padding: 12px 30px; text-decoration: none; border-radius: 6px; font-weight: 600; font-size: 14px; display: inline-block;'>
                    Jetzt starten
                </a>
            </div>

            <div style='background-color: #f8f9fa; border: 1px solid #eee; border-radius: 8px; padding: 20px; margin-top: 20px;'>
                <p style='margin-top: 0; margin-bottom: 15px; font-size: 13px; font-weight: 700; color: #1a237e; border-bottom: 1px solid #e0e0e0; padding-bottom: 8px;'>Details zur Registrierung</p>
                <table style='width: 100%; border-collapse: collapse;'>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777;'>IP-Adresse</td>
                        <td style='padding: 6px 0; font-size: 12px; color: #333; text-align: right; font-family: monospace;'>{safeIp}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777;'>Datum / Uhrzeit</td>
                        <td style='padding: 6px 0; font-size: 12px; color: #333; text-align: right;'>{displayDate}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777; vertical-align: top;'>Geräteinformationen</td>
                        <td style='padding: 6px 0; font-size: 11px; color: #333; text-align: right; line-height: 1.4;'>{safeDevice}</td>
                    </tr>
                </table>
            </div>

            <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
                Wenn diese Registrierung nicht von Ihnen stammt, kontaktieren Sie bitte unser Sicherheitsteam.
            </p>
        </div>

        <div style='background-color: #f4f4f4; padding: 25px 40px; text-align: center; border-top: 1px solid #eee;'>
            <p style='font-size: 11px; color: #999; margin: 0;'>
                Dies ist eine systemgenerierte Nachricht, bitte antworten Sie nicht darauf.
            </p>
            <p style='font-size: 10px; color: #bbb; margin-top: 8px;'>
                Trace ID: {details.TraceId} | © 2026 efavori
            </p>
        </div>
    </div>";

                await SendEmailAsync("security@efavori.com", Email, "Willkommen bei efavori!", emailBody, attachments);
            }
            if (Culture == "es")
            {
                string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
        
        <div style='padding: 40px 30px 20px 30px; text-align: center; background: linear-gradient(to bottom, #f8f9ff, #ffffff);'>
            <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 130px; height: auto; display: inline-block;' />
        </div>

        <div style='padding: 0 40px 30px 40px;'>
            <h2 style='color: #1a237e; font-size: 24px; font-weight: 700; margin-bottom: 10px; text-align: center;'>¡Bienvenido a nuestra comunidad!</h2>
            <p style='font-size: 15px; color: #555; text-align: center;'>Tu cuenta ha sido creada con éxito. Estamos emocionados de que des tu primer paso en el mundo de efavori.</p>
            
            <div style='text-align: center; margin: 30px 0;'>
                <a href='https://efavori.com/en/Customer/Home/Index' style='background-color: #1a237e; color: #ffffff; padding: 12px 30px; text-decoration: none; border-radius: 6px; font-weight: 600; font-size: 14px; display: inline-block;'>
                    Comenzar ahora
                </a>
            </div>

            <div style='background-color: #f8f9fa; border: 1px solid #eee; border-radius: 8px; padding: 20px; margin-top: 20px;'>
                <p style='margin-top: 0; margin-bottom: 15px; font-size: 13px; font-weight: 700; color: #1a237e; border-bottom: 1px solid #e0e0e0; padding-bottom: 8px;'>Detalles del registro</p>
                <table style='width: 100%; border-collapse: collapse;'>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777;'>Dirección IP</td>
                        <td style='padding: 6px 0; font-size: 12px; color: #333; text-align: right; font-family: monospace;'>{safeIp}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777;'>Fecha / Hora</td>
                        <td style='padding: 6px 0; font-size: 12px; color: #333; text-align: right;'>{displayDate}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777; vertical-align: top;'>Información del dispositivo</td>
                        <td style='padding: 6px 0; font-size: 11px; color: #333; text-align: right; line-height: 1.4;'>{safeDevice}</td>
                    </tr>
                </table>
            </div>

            <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
                Si este registro no le pertenece, por favor póngase en contacto con nuestro equipo de seguridad.
            </p>
        </div>

        <div style='background-color: #f4f4f4; padding: 25px 40px; text-align: center; border-top: 1px solid #eee;'>
            <p style='font-size: 11px; color: #999; margin: 0;'>
                Este es un mensaje del sistema, por favor no responda.
            </p>
            <p style='font-size: 10px; color: #bbb; margin-top: 8px;'>
                Trace ID: {details.TraceId} | © 2026 efavori
            </p>
        </div>
    </div>";

                await SendEmailAsync("security@efavori.com", Email, "¡Bienvenido a efavori!", emailBody, attachments);
            }
            if (Culture == "fr")
            {
                string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
        
        <div style='padding: 40px 30px 20px 30px; text-align: center; background: linear-gradient(to bottom, #f8f9ff, #ffffff);'>
            <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 130px; height: auto; display: inline-block;' />
        </div>

        <div style='padding: 0 40px 30px 40px;'>
            <h2 style='color: #1a237e; font-size: 24px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Bienvenue parmi nous !</h2>
            <p style='font-size: 15px; color: #555; text-align: center;'>Votre compte a été créé avec succès. Nous sommes ravis que vous fassiez vos premiers pas dans l'univers d'efavori.</p>
            
            <div style='text-align: center; margin: 30px 0;'>
                <a href='https://efavori.com/en/Customer/Home/Index' style='background-color: #1a237e; color: #ffffff; padding: 12px 30px; text-decoration: none; border-radius: 6px; font-weight: 600; font-size: 14px; display: inline-block;'>
                    Commencer dès maintenant
                </a>
            </div>

            <div style='background-color: #f8f9fa; border: 1px solid #eee; border-radius: 8px; padding: 20px; margin-top: 20px;'>
                <p style='margin-top: 0; margin-bottom: 15px; font-size: 13px; font-weight: 700; color: #1a237e; border-bottom: 1px solid #e0e0e0; padding-bottom: 8px;'>Détails de l'inscription</p>
                <table style='width: 100%; border-collapse: collapse;'>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777;'>Adresse IP</td>
                        <td style='padding: 6px 0; font-size: 12px; color: #333; text-align: right; font-family: monospace;'>{safeIp}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777;'>Date / Heure</td>
                        <td style='padding: 6px 0; font-size: 12px; color: #333; text-align: right;'>{displayDate}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777; vertical-align: top;'>Infos sur l'appareil</td>
                        <td style='padding: 6px 0; font-size: 11px; color: #333; text-align: right; line-height: 1.4;'>{safeDevice}</td>
                    </tr>
                </table>
            </div>

            <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
                Si cette inscription ne provient pas de vous, veuillez contacter notre équipe de sécurité.
            </p>
        </div>

        <div style='background-color: #f4f4f4; padding: 25px 40px; text-align: center; border-top: 1px solid #eee;'>
            <p style='font-size: 11px; color: #999; margin: 0;'>
                Ceci est un message système, merci de ne pas y répondre.
            </p>
            <p style='font-size: 10px; color: #bbb; margin-top: 8px;'>
                Trace ID: {details.TraceId} | © 2026 efavori
            </p>
        </div>
    </div>";

                await SendEmailAsync("security@efavori.com", Email, "Bienvenue chez efavori !", emailBody, attachments);
            }
            if (Culture == "hi")
            {
                string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
        
        <div style='padding: 40px 30px 20px 30px; text-align: center; background: linear-gradient(to bottom, #f8f9ff, #ffffff);'>
            <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 130px; height: auto; display: inline-block;' />
        </div>

        <div style='padding: 0 40px 30px 40px;'>
            <h2 style='color: #1a237e; font-size: 24px; font-weight: 700; margin-bottom: 10px; text-align: center;'>हमारे साथ आपका स्वागत है!</h2>
            <p style='font-size: 15px; color: #555; text-align: center;'>आपका खाता सफलतापूर्वक बना लिया गया है। हमें खुशी है कि आपने efavori की दुनिया में अपना पहला कदम रखा है।</p>
            
            <div style='text-align: center; margin: 30px 0;'>
                <a href='https://efavori.com/en/Customer/Home/Index' style='background-color: #1a237e; color: #ffffff; padding: 12px 30px; text-decoration: none; border-radius: 6px; font-weight: 600; font-size: 14px; display: inline-block;'>
                    अभी शुरू करें
                </a>
            </div>

            <div style='background-color: #f8f9fa; border: 1px solid #eee; border-radius: 8px; padding: 20px; margin-top: 20px;'>
                <p style='margin-top: 0; margin-bottom: 15px; font-size: 13px; font-weight: 700; color: #1a237e; border-bottom: 1px solid #e0e0e0; padding-bottom: 8px;'>पंजीकरण विवरण</p>
                <table style='width: 100%; border-collapse: collapse;'>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777;'>IP पता</td>
                        <td style='padding: 6px 0; font-size: 12px; color: #333; text-align: right; font-family: monospace;'>{safeIp}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777;'>दिनांक / समय</td>
                        <td style='padding: 6px 0; font-size: 12px; color: #333; text-align: right;'>{displayDate}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777; vertical-align: top;'>डिवाइस की जानकारी</td>
                        <td style='padding: 6px 0; font-size: 11px; color: #333; text-align: right; line-height: 1.4;'>{safeDevice}</td>
                    </tr>
                </table>
            </div>

            <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
                यदि यह पंजीकरण आपने नहीं किया है, तो कृपया हमारी सुरक्षा टीम से संपर्क करें।
            </p>
        </div>

        <div style='background-color: #f4f4f4; padding: 25px 40px; text-align: center; border-top: 1px solid #eee;'>
            <p style='font-size: 11px; color: #999; margin: 0;'>
                यह एक सिस्टम जनित संदेश है, कृपया इसका उत्तर न दें।
            </p>
            <p style='font-size: 10px; color: #bbb; margin-top: 8px;'>
                Trace ID: {details.TraceId} | © 2026 efavori
            </p>
        </div>
    </div>";

                await SendEmailAsync("security@efavori.com", Email, "efavori में आपका स्वागत है!", emailBody, attachments);
            }
            if (Culture == "pt")
            {
                string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
        
        <div style='padding: 40px 30px 20px 30px; text-align: center; background: linear-gradient(to bottom, #f8f9ff, #ffffff);'>
            <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 130px; height: auto; display: inline-block;' />
        </div>

        <div style='padding: 0 40px 30px 40px;'>
            <h2 style='color: #1a237e; font-size: 24px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Bem-vindo à nossa comunidade!</h2>
            <p style='font-size: 15px; color: #555; text-align: center;'>A sua conta foi criada com sucesso. Estamos entusiasmados por vê-lo dar o seu primeiro passo no mundo da efavori.</p>
            
            <div style='text-align: center; margin: 30px 0;'>
                <a href='https://efavori.com/en/Customer/Home/Index' style='background-color: #1a237e; color: #ffffff; padding: 12px 30px; text-decoration: none; border-radius: 6px; font-weight: 600; font-size: 14px; display: inline-block;'>
                    Comece Agora
                </a>
            </div>

            <div style='background-color: #f8f9fa; border: 1px solid #eee; border-radius: 8px; padding: 20px; margin-top: 20px;'>
                <p style='margin-top: 0; margin-bottom: 15px; font-size: 13px; font-weight: 700; color: #1a237e; border-bottom: 1px solid #e0e0e0; padding-bottom: 8px;'>Detalhes do Registro</p>
                <table style='width: 100%; border-collapse: collapse;'>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777;'>Endereço IP</td>
                        <td style='padding: 6px 0; font-size: 12px; color: #333; text-align: right; font-family: monospace;'>{safeIp}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777;'>Data / Hora</td>
                        <td style='padding: 6px 0; font-size: 12px; color: #333; text-align: right;'>{displayDate}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777; vertical-align: top;'>Informações do Dispositivo</td>
                        <td style='padding: 6px 0; font-size: 11px; color: #333; text-align: right; line-height: 1.4;'>{safeDevice}</td>
                    </tr>
                </table>
            </div>

            <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
                Se este registro não foi feito por você, entre em contato com nossa equipe de segurança.
            </p>
        </div>

        <div style='background-color: #f4f4f4; padding: 25px 40px; text-align: center; border-top: 1px solid #eee;'>
            <p style='font-size: 11px; color: #999; margin: 0;'>
                Esta é uma mensagem do sistema, por favor não responda.
            </p>
            <p style='font-size: 10px; color: #bbb; margin-top: 8px;'>
                Trace ID: {details.TraceId} | © 2026 efavori
            </p>
        </div>
    </div>";

                await SendEmailAsync("security@efavori.com", Email, "Bem-vindo à efavori!", emailBody, attachments);
            }
            if (Culture == "ru")
            {
                string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
        
        <div style='padding: 40px 30px 20px 30px; text-align: center; background: linear-gradient(to bottom, #f8f9ff, #ffffff);'>
            <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 130px; height: auto; display: inline-block;' />
        </div>

        <div style='padding: 0 40px 30px 40px;'>
            <h2 style='color: #1a237e; font-size: 24px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Добро пожаловать к нам!</h2>
            <p style='font-size: 15px; color: #555; text-align: center;'>Ваш аккаунт был успешно создан. Мы рады, что вы сделали свой первый шаг в мир efavori.</p>
            
            <div style='text-align: center; margin: 30px 0;'>
                <a href='https://efavori.com/en/Customer/Home/Index' style='background-color: #1a237e; color: #ffffff; padding: 12px 30px; text-decoration: none; border-radius: 6px; font-weight: 600; font-size: 14px; display: inline-block;'>
                    Начать сейчас
                </a>
            </div>

            <div style='background-color: #f8f9fa; border: 1px solid #eee; border-radius: 8px; padding: 20px; margin-top: 20px;'>
                <p style='margin-top: 0; margin-bottom: 15px; font-size: 13px; font-weight: 700; color: #1a237e; border-bottom: 1px solid #e0e0e0; padding-bottom: 8px;'>Детали регистрации</p>
                <table style='width: 100%; border-collapse: collapse;'>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777;'>IP-адрес</td>
                        <td style='padding: 6px 0; font-size: 12px; color: #333; text-align: right; font-family: monospace;'>{safeIp}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777;'>Дата / Время</td>
                        <td style='padding: 6px 0; font-size: 12px; color: #333; text-align: right;'>{displayDate}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777; vertical-align: top;'>Информация об устройстве</td>
                        <td style='padding: 6px 0; font-size: 11px; color: #333; text-align: right; line-height: 1.4;'>{safeDevice}</td>
                    </tr>
                </table>
            </div>

            <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
                Если вы не совершали эту регистрацию, пожалуйста, свяжитесь с нашей службой безопасности.
            </p>
        </div>

        <div style='background-color: #f4f4f4; padding: 25px 40px; text-align: center; border-top: 1px solid #eee;'>
            <p style='font-size: 11px; color: #999; margin: 0;'>
                Это системное сообщение, пожалуйста, не отвечайте на него.
            </p>
            <p style='font-size: 10px; color: #bbb; margin-top: 8px;'>
                Trace ID: {details.TraceId} | © 2026 efavori
            </p>
        </div>
    </div>";

                await SendEmailAsync("security@efavori.com", Email, "Добро пожаловать в efavori!", emailBody, attachments);
            }
            if (Culture == "zh")
            {
                string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
        
        <div style='padding: 40px 30px 20px 30px; text-align: center; background: linear-gradient(to bottom, #f8f9ff, #ffffff);'>
            <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 130px; height: auto; display: inline-block;' />
        </div>

        <div style='padding: 0 40px 30px 40px;'>
            <h2 style='color: #1a237e; font-size: 24px; font-weight: 700; margin-bottom: 10px; text-align: center;'>欢迎加入我们！</h2>
            <p style='font-size: 15px; color: #555; text-align: center;'>您的账户已成功创建。我们很高兴您迈出了进入 efavori 世界的第一步。</p>
            
            <div style='text-align: center; margin: 30px 0;'>
                <a href='https://efavori.com/en/Customer/Home/Index' style='background-color: #1a237e; color: #ffffff; padding: 12px 30px; text-decoration: none; border-radius: 6px; font-weight: 600; font-size: 14px; display: inline-block;'>
                    立即开始
                </a>
            </div>

            <div style='background-color: #f8f9fa; border: 1px solid #eee; border-radius: 8px; padding: 20px; margin-top: 20px;'>
                <p style='margin-top: 0; margin-bottom: 15px; font-size: 13px; font-weight: 700; color: #1a237e; border-bottom: 1px solid #e0e0e0; padding-bottom: 8px;'>注册详情</p>
                <table style='width: 100%; border-collapse: collapse;'>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777;'>IP 地址</td>
                        <td style='padding: 6px 0; font-size: 12px; color: #333; text-align: right; font-family: monospace;'>{safeIp}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777;'>日期 / 时间</td>
                        <td style='padding: 6px 0; font-size: 12px; color: #333; text-align: right;'>{displayDate}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px 0; font-size: 12px; color: #777; vertical-align: top;'>设备信息</td>
                        <td style='padding: 6px 0; font-size: 11px; color: #333; text-align: right; line-height: 1.4;'>{safeDevice}</td>
                    </tr>
                </table>
            </div>

            <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
                如果此注册操作非您本人所为，请联系我们的安全团队。
            </p>
        </div>

        <div style='background-color: #f4f4f4; padding: 25px 40px; text-align: center; border-top: 1px solid #eee;'>
            <p style='font-size: 11px; color: #999; margin: 0;'>
                这是一条系统消息，请勿回复。
            </p>
            <p style='font-size: 10px; color: #bbb; margin-top: 8px;'>
                Trace ID: {details.TraceId} | © 2026 efavori
            </p>
        </div>
    </div>";

                await SendEmailAsync("security@efavori.com", Email, "欢迎来到 efavori！", emailBody, attachments);
            }
        }

        //security@efavori.com | Doğru şekilde giriş yapıldığında
        public async Task SendLoginInfoEmailAsync(string Culture, string Email)
        {
            var attachments = new List<Attachment>();
            var details = _userInfos.GetCurrentUserDetails();
            string safeIp = System.Net.WebUtility.HtmlEncode(details.IpAddress);
            string safeDevice = System.Net.WebUtility.HtmlEncode(details.UserAgent);
            string displayDate = DateTime.Now.ToString("dd MMMM yyyy HH:mm");

            if (Culture == "tr")
            {
                string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
        
        <div style='padding: 30px 30px 20px 30px; text-align: center;'>
            <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
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
                await SendEmailAsync("security@efavori.com", Email, "Güvenlik Uyarısı: Yeni Giriş Yapıldı", emailBody, attachments);
            }
            if (Culture == "en")
            {
                string emailBody = $@"
<div style='font-family: ""Inter"", -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e8e8e8; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 12px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 25px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Security Logo' style='max-width: 100px; height: auto; display: inline-block;' />
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
                await SendEmailAsync("security@efavori.com", Email, "Security Alert: New Sign-in Detected", emailBody, attachments);
            }
            if (Culture == "az")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 10px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 2px 10px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Security' style='max-width: 100px; height: auto; display: inline-block;' />
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

                await SendEmailAsync("security@efavori.com", Email, "Təhlükəsizlik Xəbərdarlığı: Yeni Giriş", emailBody, attachments);

            }
            if (Culture == "de")
            {
                string emailBody = $@"
<div style='font-family: ""Helvetica Neue"", Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #dddddd; padding: 0; border-radius: 10px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 5px 15px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Sicherheit' style='max-width: 100px; height: auto; display: inline-block;' />
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

                await SendEmailAsync("security@efavori.com", Email, "Sicherheitswarnung: Neue Anmeldung erkannt", emailBody, attachments);

            }
            if (Culture == "es")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.03);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Seguridad' style='max-width: 100px; height: auto; display: inline-block;' />
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

                await SendEmailAsync("security@efavori.com", Email, "Alerta de seguridad: Nuevo inicio de sesión detectado", emailBody, attachments);

            }
            if (Culture == "fr")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Sécurité' style='max-width: 100px; height: auto; display: inline-block;' />
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
                await SendEmailAsync("security@efavori.com", Email, "Alerte de sécurité : Nouvelle connexion détectée", emailBody, attachments);

            }
            if (Culture == "hi")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Security' style='max-width: 100px; height: auto; display: inline-block;' />
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
                await SendEmailAsync("security@efavori.com", Email, "सुरक्षा अलर्ट: नया लॉगिन मिला", emailBody, attachments);

            }
            if (Culture == "pt")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.03);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Segurança' style='max-width: 100px; height: auto; display: inline-block;' />
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

                await SendEmailAsync("security@efavori.com", Email, "Alerta de segurança: Novo acesso detectado", emailBody, attachments);

            }
            if (Culture == "ru")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.03);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Безопасность' style='max-width: 100px; height: auto; display: inline-block;' />
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

                await SendEmailAsync("security@efavori.com", Email, "Оповещение о безопасности: Обнаружен новый вход", emailBody, attachments);

            }
            if (Culture == "zh")
            {
                string emailBody = $@"
<div style='font-family: ""PingFang SC"", ""Microsoft YaHei"", ""Helvetica Neue"", Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.03);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='安全通知' style='max-width: 100px; height: auto; display: inline-block;' />
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

                await SendEmailAsync("security@efavori.com", Email, "安全警报：检测到新登录", emailBody, attachments);

            }

        }

        //security@efavori.com | Yanlış girilen şifre bildirimi
        public async Task SendLoginErrorInfoEmailAsync(string Culture, string Email)
        {
            var attachments = new List<Attachment>();

            var details = _userInfos.GetCurrentUserDetails();
            string safeIp = System.Net.WebUtility.HtmlEncode(details.IpAddress);
            string safeDevice = System.Net.WebUtility.HtmlEncode(details.UserAgent);
            string displayDate = DateTime.Now.ToString("dd MMMM yyyy HH:mm");

            if (Culture == "tr")
            {
                string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
        
        <div style='padding: 30px 30px 20px 30px; text-align: center;'>
            <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
        </div>

        <div style='padding: 0 40px 30px 40px;'>
            <h2 style='color: #d32f2f; font-size: 20px; font-weight: 600; margin-bottom: 20px; border-bottom: 2px solid #f0f0f0; padding-bottom: 10px; text-align: center;'>Dikkat: Başarısız Giriş Denemesi</h2>
            
            <p style='font-size: 15px;'>Sayın Kullanıcımız,</p>
            <p style='font-size: 14px; color: #555;'>Hesabınıza yönelik başarısız bir oturum açma denemesi tespit edilmiştir. Bu, hesabınızın güvenliğini korumak için önemlidir. Denemenin ayrıntıları aşağıdadır:</p>
            
            <table style='width: 100%; border-collapse: collapse; margin: 25px 0; background-color: #fff5f5; border-radius: 6px;'>
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

            <p style='font-size: 13px; color: #777;'>
                <strong>Hata Sebebi:</strong> Girilen şifre hatalıdır. Lütfen doğru şifrenizi kullanarak tekrar deneyin.
            </p>

            <div style='background-color: #fff4f4; border-left: 4px solid #d32f2f; border-right: 4px solid #d32f2f; padding: 20px; margin-top: 25px;'>
                <p style='margin: 0 0 15px 0; font-size: 13px; color: #b71c1c; line-height: 1.5;'>
                    <strong>Şifrenizi mi unuttuğunuz?</strong> Hesabınıza erişim sağlamak için lütfen şifre sıfırlama seçeneğini kullanınız.
                </p>
                <hr style='border: 0; border-top: 1px solid rgba(211, 47, 47, 0.2); margin: 15px 0;' />
                <p style='margin: 0; font-size: 13px; color: #777;'>
                    Bu deneme sizin tarafınızdan yapılmadıysa, hesabınızı korumak adına lütfen şifrenizi güncelleyiniz.
                </p>
            </div>

            <div style='background-color: #f9f9f9; padding: 15px; margin-top: 20px; border-radius: 6px; text-align: center;'>
                <p style='margin: 0; font-size: 12px; color: #666;'>
                    🔒 Güvenli tutun: Güçlü ve karmaşık şifreler kullanın, şifrenizi kimseyle paylaşmayın.
                </p>
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
                await SendEmailAsync("security@efavori.com", Email, "Başarısız Giriş Denemesi", emailBody, attachments);
            }
            if (Culture == "en")
            {
                string emailBody = $@"
<div style='font-family: ""Inter"", -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e8e8e8; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 12px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 25px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Security Logo' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #d32f2f; font-size: 22px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Failed Login Attempt</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>We detected an unsuccessful login attempt on your account.</p>
        
        <p style='font-size: 15px; font-weight: 600;'>Hello,</p>
        <p style='font-size: 14px; color: #555;'>For your protection, here are the details of the failed login attempt:</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #fff5f5; border: 1px solid #ffe0e0; border-radius: 8px;'>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #ffe0e0;'>Date / Time</td>
                <td style='padding: 14px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #ffe0e0;'>{displayDate}</td>
            </tr>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #ffe0e0;'>IP Address</td>
                <td style='padding: 14px 15px; font-size: 13px; font-family: ""Courier New"", Courier, monospace; color: #333; border-bottom: 1px solid #ffe0e0;'>{safeIp}</td>
            </tr>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px;'>Device</td>
                <td style='padding: 14px 15px; font-size: 13px; color: #333;'>{safeDevice}</td>
            </tr>
        </table>

        <p style='font-size: 13px; color: #d32f2f; text-align: center; font-weight: 500;'>
            <strong>Reason:</strong> The password you entered is incorrect.
        </p>

        <div style='background-color: #fff9f9; border: 1px solid #ffdbdb; border-radius: 8px; padding: 20px; margin-top: 25px;'>
            <p style='margin: 0 0 15px 0; font-size: 14px; color: #c62828; font-weight: 500;'>
                <strong>Forgot your password?</strong>
            </p>
            <p style='margin: 0 0 15px 0; font-size: 13px; color: #555; line-height: 1.5;'>
                Use the password reset option to regain access to your account. If you continue to experience issues, please contact our support team.
            </p>
            <p style='margin: 0; font-size: 12px; color: #777;'>
                ⚠️ If this was not you, please secure your account by updating your password immediately.
            </p>
        </div>

        <div style='background-color: #f9f9f9; padding: 15px; margin-top: 20px; border-radius: 6px; text-align: center;'>
            <p style='margin: 0; font-size: 12px; color: #666;'>
                🔒 Stay safe: Use strong passwords and never share them with anyone.
            </p>
        </div>
    </div>

    <div style='background-color: #f4f4f4; padding: 25px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 11px; color: #999; margin: 0;'>
            This is an automated security notification. Please do not reply to this email.
        </p>
        <p style='font-size: 10px; color: #bbb; margin-top: 10px; letter-spacing: 0.5px; text-transform: uppercase;'>
            Trace ID: {details.TraceId}
        </p>
    </div>
</div>";
                await SendEmailAsync("security@efavori.com", Email, "Failed Login Attempt Detected", emailBody, attachments);
            }
            if (Culture == "az")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 10px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 2px 10px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Security' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #d32f2f; font-size: 20px; font-weight: 700; margin-bottom: 15px; border-bottom: 2px solid #f5f5f5; padding-bottom: 15px; text-align: center;'>Diqqət: Uğursuz Giriş Cəsdi</h2>
        
        <p style='font-size: 15px; font-weight: 600;'>Hörmətli İstifadəçi,</p>
        <p style='font-size: 14px; color: #555;'>Hesabınıza uğursuz bir giriş cəsdi qeydə alınmışdır. Həmin cəsdin məlumatlarını aşağıda görmək olar:</p>
        
        <table style='width: 100%; border-collapse: collapse; margin: 25px 0; background-color: #fff5f5; border: 1px solid #ffe0e0; border-radius: 8px;'>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #666; font-size: 13px; border-bottom: 1px solid #ffe0e0;'>Tarix və Saat</td>
                <td style='padding: 12px 15px; font-size: 13px; border-bottom: 1px solid #ffe0e0;'>{displayDate}</td>
            </tr>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #666; font-size: 13px; border-bottom: 1px solid #ffe0e0;'>IP Ünvanı</td>
                <td style='padding: 12px 15px; font-size: 13px; font-family: monospace; border-bottom: 1px solid #ffe0e0;'>{safeIp}</td>
            </tr>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #666; font-size: 13px;'>Cihaz Məlumatı</td>
                <td style='padding: 12px 15px; font-size: 12px; color: #444;'>{safeDevice}</td>
            </tr>
        </table>

        <p style='font-size: 13px; color: #d32f2f; text-align: center; font-weight: 600;'>
            <strong>Səbəb:</strong> Daxil etilən şifrə yanlışdır.
        </p>

        <div style='background-color: #fff9f9; border: 1px solid #ffdbdb; border-radius: 8px; padding: 20px; margin-top: 25px;'>
            <p style='margin: 0 0 12px 0; font-size: 14px; color: #c62828; font-weight: bold;'>
                Şifrənizi unutmuşsunuz?
            </p>
            <p style='margin: 0 0 15px 0; font-size: 13px; color: #555;'>
                Hesabınıza daxil olmaq üçün şifrenizi sıfırlama seçəniyi istifadə edin.
            </p>
            <p style='margin: 0; font-size: 12px; color: #777;'>
                ⚠️ Bu siz deyilsinizsə, lütfen hesabınızı qorumaq üçün şifrenizi yeniləyin.
            </p>
        </div>

        <div style='background-color: #f9f9f9; padding: 15px; margin-top: 20px; border-radius: 6px; text-align: center;'>
            <p style='margin: 0; font-size: 12px; color: #666;'>
                🔒 Təhlükəsiz qalın: Güclü şifrələr istifadə edin.
            </p>
        </div>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 11px; color: #999; margin: 0;'>
            Bu e-poçt təhlükəsizlik məqsədilə sistem tərəfindən avtomatik göndərilib.
        </p>
        <p style='font-size: 10px; color: #bbb; margin-top: 8px;'>
            Əməliyyat ID (Trace ID): {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", Email, "Uğursuz Giriş Cəsdi", emailBody, attachments);

            }
            if (Culture == "de")
            {
                string emailBody = $@"
<div style='font-family: ""Helvetica Neue"", Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #dddddd; padding: 0; border-radius: 10px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 5px 15px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Sicherheit' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #d32f2f; font-size: 20px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Fehlgeschlagener Anmeldeversuch</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>Ein fehlgeschlagener Anmeldeversuch wurde erkannt.</p>
        
        <p style='font-size: 15px;'>Guten Tag,</p>
        <p style='font-size: 14px; color: #555;'>Zu Ihrer Sicherheit informieren wir Sie über einen fehlgeschlagenen Anmeldeversuch. Hier sind die Details:</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #fff5f5; border: 1px solid #ffe0e0; border-radius: 8px;'>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #ffe0e0;'>Datum / Uhrzeit</td>
                <td style='padding: 12px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #ffe0e0;'>{displayDate}</td>
            </tr>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #ffe0e0;'>IP-Adresse</td>
                <td style='padding: 12px 15px; font-size: 13px; font-family: monospace; color: #333; border-bottom: 1px solid #ffe0e0;'>{safeIp}</td>
            </tr>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px;'>Gerät</td>
                <td style='padding: 12px 15px; font-size: 13px; color: #333;'>{safeDevice}</td>
            </tr>
        </table>

        <p style='font-size: 13px; color: #d32f2f; text-align: center; font-weight: 600;'>
            <strong>Grund:</strong> Das eingegebene Passwort ist falsch.
        </p>

        <div style='background-color: #fff9f9; border: 1px solid #ffdbdb; border-radius: 8px; padding: 20px; margin-top: 25px;'>
            <p style='margin: 0 0 12px 0; font-size: 14px; color: #c53030; font-weight: bold;'>
                Passwort vergessen?
            </p>
            <p style='margin: 0 0 15px 0; font-size: 13px; color: #555;'>
                Verwenden Sie die Passwort-Zurücksetzen-Option, um auf Ihr Konto zuzugreifen.
            </p>
            <p style='margin: 0; font-size: 12px; color: #777;'>
                ⚠️ Falls dies nicht Sie waren, aktualisieren Sie sofort Ihr Passwort.
            </p>
        </div>

        <div style='background-color: #f9f9f9; padding: 15px; margin-top: 20px; border-radius: 6px; text-align: center;'>
            <p style='margin: 0; font-size: 12px; color: #666;'>
                🔒 Bleiben Sie sicher: Verwenden Sie starke Passwörter.
            </p>
        </div>
    </div>

    <div style='background-color: #f4f4f4; padding: 25px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 11px; color: #999; margin: 0;'>
            Dies ist eine automatisch generierte Sicherheitsbenachrichtigung. Bitte antworten Sie nicht auf diese E-Mail.
        </p>
        <p style='font-size: 10px; color: #bbb; margin-top: 10px; letter-spacing: 0.5px;'>
            Trace-ID: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", Email, "Fehlgeschlagener Anmeldeversuch erkannt", emailBody, attachments);

            }
            if (Culture == "es")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.03);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Seguridad' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #d32f2f; font-size: 22px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Intento de Inicio de Sesión Fallido</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>Se ha detectado un intento fallido de inicio de sesión.</p>
        
        <p style='font-size: 15px;'>Estimado usuario/a,</p>
        <p style='font-size: 14px; color: #555;'>Para su protección, le informamos sobre un intento fallido de acceso a su cuenta:</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #fff5f5; border: 1px solid #ffe0e0; border-radius: 8px;'>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #ffe0e0;'>Fecha / Hora</td>
                <td style='padding: 14px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #ffe0e0;'>{displayDate}</td>
            </tr>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #ffe0e0;'>Dirección IP</td>
                <td style='padding: 14px 15px; font-size: 13px; font-family: monospace; color: #333; border-bottom: 1px solid #ffe0e0;'>{safeIp}</td>
            </tr>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px;'>Dispositivo</td>
                <td style='padding: 14px 15px; font-size: 13px; color: #333;'>{safeDevice}</td>
            </tr>
        </table>

        <p style='font-size: 13px; color: #d32f2f; text-align: center; font-weight: 600;'>
            <strong>Motivo:</strong> La contraseña ingresada es incorrecta.
        </p>

        <div style='background-color: #fff8f8; border: 1px solid #ffebeb; border-radius: 8px; padding: 25px; margin-top: 25px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 15px; color: #d32f2f; font-weight: bold;'>
                ¿Olvidó su contraseña?
            </p>
            <p style='margin: 0 0 15px 0; font-size: 13px; color: #555;'>
                Utilice la opción de restablecimiento de contraseña para recuperar el acceso.
            </p>
            <p style='margin: 0; font-size: 12px; color: #777;'>
                ⚠️ Si no fue usted, actualice su contraseña de inmediato.
            </p>
        </div>

        <div style='background-color: #f9f9f9; padding: 15px; margin-top: 20px; border-radius: 6px; text-align: center;'>
            <p style='margin: 0; font-size: 12px; color: #666;'>
                🔒 Manténgase seguro: Use contraseñas fuertes.
            </p>
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

                await SendEmailAsync("security@efavori.com", Email, "Intento de inicio de sesión fallido detectado", emailBody, attachments);

            }
            if (Culture == "fr")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Sécurité' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #d32f2f; font-size: 20px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Tentative de Connexion Échouée</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>Une tentative échouée de connexion a été détectée.</p>
        
        <p style='font-size: 15px;'>Bonjour,</p>
        <p style='font-size: 14px; color: #555;'>Pour votre sécurité, nous vous informons d'une tentative échouée de connexion :</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #fff5f5; border: 1px solid #ffe0e0; border-radius: 8px;'>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #ffe0e0;'>Date et heure</td>
                <td style='padding: 12px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #ffe0e0;'>{displayDate}</td>
            </tr>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #ffe0e0;'>Adresse IP</td>
                <td style='padding: 12px 15px; font-size: 13px; font-family: monospace; color: #333; border-bottom: 1px solid #ffe0e0;'>{safeIp}</td>
            </tr>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px;'>Appareil</td>
                <td style='padding: 12px 15px; font-size: 13px; color: #333;'>{safeDevice}</td>
            </tr>
        </table>

        <p style='font-size: 13px; color: #d32f2f; text-align: center; font-weight: 600;'>
            <strong>Motif :</strong> Le mot de passe saisi est incorrect.
        </p>

        <div style='background-color: #fff9f9; border: 1px solid #ffebeb; border-radius: 10px; padding: 25px; margin-top: 25px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 15px; color: #c62828; font-weight: bold;'>
                Mot de passe oublié ?
            </p>
            <p style='margin: 0 0 15px 0; font-size: 13px; color: #555;'>
                Utilisez l'option de réinitialisation du mot de passe pour accéder à votre compte.
            </p>
            <p style='margin: 0; font-size: 12px; color: #777;'>
                ⚠️ Si ce n'était pas vous, mettez à jour votre mot de passe immédiatement.
            </p>
        </div>

        <div style='background-color: #f9f9f9; padding: 15px; margin-top: 20px; border-radius: 6px; text-align: center;'>
            <p style='margin: 0; font-size: 12px; color: #666;'>
                🔒 Restez en sécurité : Utilisez des mots de passe forts.
            </p>
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
                await SendEmailAsync("security@efavori.com", Email, "Tentative de connexion échouée détectée", emailBody, attachments);

            }
            if (Culture == "hi")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Security' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #d32f2f; font-size: 22px; font-weight: 700; margin-bottom: 10px; text-align: center;'>विफल लॉगिन प्रयास</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>आपके खाते में एक विफल लॉगिन प्रयास देखा गया है।</p>
        
        <p style='font-size: 15px;'>नमस्ते,</p>
        <p style='font-size: 14px; color: #555;'>आपकी सुरक्षा के लिए, यहाँ विफल लॉगिन प्रयास के विवरण दिए गए हैं:</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #fff5f5; border: 1px solid #ffe0e0; border-radius: 8px;'>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #ffe0e0;'>दिनांक / समय</td>
                <td style='padding: 12px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #ffe0e0;'>{displayDate}</td>
            </tr>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #ffe0e0;'>आईपी पता (IP Address)</td>
                <td style='padding: 12px 15px; font-size: 13px; font-family: monospace; color: #333; border-bottom: 1px solid #ffe0e0;'>{safeIp}</td>
            </tr>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px;'>डिवाइस (Device)</td>
                <td style='padding: 12px 15px; font-size: 13px; color: #333;'>{safeDevice}</td>
            </tr>
        </table>

        <p style='font-size: 13px; color: #d32f2f; text-align: center; font-weight: 600;'>
            <strong>कारण:</strong> दर्ज किया गया पासवर्ड गलत है।
        </p>

        <div style='background-color: #fff8f8; border: 1px solid #ffebeb; border-radius: 10px; padding: 25px; margin-top: 25px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 16px; color: #d32f2f; font-weight: bold;'>
                क्या पासवर्ड भूल गए हैं?
            </p>
            <p style='margin: 0 0 15px 0; font-size: 14px; color: #555;'>
                अपना खाता पुनः प्राप्त करने के लिए पासवर्ड रीसेट विकल्प का उपयोग करें।
            </p>
            <p style='margin: 0; font-size: 12px; color: #777;'>
                ⚠️ यदि यह आप नहीं थे, तो तुरंत अपना पासवर्ड बदलें।
            </p>
        </div>

        <div style='background-color: #f9f9f9; padding: 15px; margin-top: 20px; border-radius: 6px; text-align: center;'>
            <p style='margin: 0; font-size: 12px; color: #666;'>
                🔒 सुरक्षित रहें: मजबूत पासवर्ड का उपयोग करें।
            </p>
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
                await SendEmailAsync("security@efavori.com", Email, "विफल लॉगिन प्रयास मिला", emailBody, attachments);

            }
            if (Culture == "pt")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.03);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Segurança' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #d32f2f; font-size: 22px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Tentativa de Acesso Falhada</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>Detectamos uma tentativa falhada de acesso na sua conta.</p>
        
        <p style='font-size: 15px;'>Olá,</p>
        <p style='font-size: 14px; color: #555;'>Para sua proteção, aqui estão os detalhes da tentativa falhada:</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #fff5f5; border: 1px solid #ffe0e0; border-radius: 8px;'>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #ffe0e0;'>Data / Hora</td>
                <td style='padding: 14px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #ffe0e0;'>{displayDate}</td>
            </tr>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #ffe0e0;'>Endereço IP</td>
                <td style='padding: 14px 15px; font-size: 13px; font-family: monospace; color: #333; border-bottom: 1px solid #ffe0e0;'>{safeIp}</td>
            </tr>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px;'>Dispositivo</td>
                <td style='padding: 14px 15px; font-size: 13px; color: #333;'>{safeDevice}</td>
            </tr>
        </table>

        <p style='font-size: 13px; color: #d32f2f; text-align: center; font-weight: 600;'>
            <strong>Motivo:</strong> A senha inserida é incorreta.
        </p>

        <div style='background-color: #fff8f8; border: 1px solid #ffebeb; border-radius: 10px; padding: 25px; margin-top: 25px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 15px; color: #d32f2f; font-weight: bold;'>
                Esqueceu sua senha?
            </p>
            <p style='margin: 0 0 15px 0; font-size: 13px; color: #555;'>
                Use a opção de redefinição de senha para recuperar o acesso.
            </p>
            <p style='margin: 0; font-size: 12px; color: #777;'>
                ⚠️ Se não foi você, atualize sua senha imediatamente.
            </p>
        </div>

        <div style='background-color: #f9f9f9; padding: 15px; margin-top: 20px; border-radius: 6px; text-align: center;'>
            <p style='margin: 0; font-size: 12px; color: #666;'>
                🔒 Mantenha-se seguro: Use senhas fortes.
            </p>
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

                await SendEmailAsync("security@efavori.com", Email, "Tentativa de acesso falhada detectada", emailBody, attachments);

            }
            if (Culture == "ru")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.03);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Безопасность' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #d32f2f; font-size: 20px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Неудачная попытка входа</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>Обнаружена неудачная попытка входа в ваш аккаунт.</p>
        
        <p style='font-size: 15px;'>Здравствуйте,</p>
        <p style='font-size: 14px; color: #555;'>В целях безопасности мы уведомляем вас о неудачной попытке входа:</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #fff5f5; border: 1px solid #ffe0e0; border-radius: 8px;'>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #ffe0e0;'>Дата / Время</td>
                <td style='padding: 12px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #ffe0e0;'>{displayDate}</td>
            </tr>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #ffe0e0;'>IP-адрес</td>
                <td style='padding: 12px 15px; font-size: 13px; font-family: monospace; color: #333; border-bottom: 1px solid #ffe0e0;'>{safeIp}</td>
            </tr>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px;'>Устройство</td>
                <td style='padding: 12px 15px; font-size: 13px; color: #333;'>{safeDevice}</td>
            </tr>
        </table>

        <p style='font-size: 13px; color: #d32f2f; text-align: center; font-weight: 600;'>
            <strong>Причина:</strong> Введённый пароль неверен.
        </p>

        <div style='background-color: #fff8f8; border: 1px solid #ffebeb; border-radius: 10px; padding: 25px; margin-top: 25px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 15px; color: #d32f2f; font-weight: bold;'>
                Забыли пароль?
            </p>
            <p style='margin: 0 0 15px 0; font-size: 13px; color: #555;'>
                Используйте функцию восстановления пароля для доступа к вашему аккаунту.
            </p>
            <p style='margin: 0; font-size: 12px; color: #777;'>
                ⚠️ Если это были не вы, немедленно измените пароль.
            </p>
        </div>

        <div style='background-color: #f9f9f9; padding: 15px; margin-top: 20px; border-radius: 6px; text-align: center;'>
            <p style='margin: 0; font-size: 12px; color: #666;'>
                🔒 Оставайтесь в безопасности: Используйте надёжные пароли.
            </p>
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

                await SendEmailAsync("security@efavori.com", Email, "Обнаружена неудачная попытка входа", emailBody, attachments);

            }
            if (Culture == "zh")
            {
                string emailBody = $@"
<div style='font-family: ""PingFang SC"", ""Microsoft YaHei"", ""Helvetica Neue"", Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.03);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='安全通知' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #d32f2f; font-size: 22px; font-weight: 700; margin-bottom: 10px; text-align: center;'>登录失败警报</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>检测到您账户的一次失败登录尝试。</p>
        
        <p style='font-size: 15px;'>尊敬的用户，您好：</p>
        <p style='font-size: 14px; color: #555;'>为了保护您的账户安全，以下是失败登录尝试的具体信息：</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #fff5f5; border: 1px solid #ffe0e0; border-radius: 8px;'>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #ffe0e0;'>日期 / 时间</td>
                <td style='padding: 14px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #ffe0e0;'>{displayDate}</td>
            </tr>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #ffe0e0;'>IP 地址</td>
                <td style='padding: 14px 15px; font-size: 13px; font-family: monospace; color: #333; border-bottom: 1px solid #ffe0e0;'>{safeIp}</td>
            </tr>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px;'>设备信息</td>
                <td style='padding: 14px 15px; font-size: 13px; color: #333;'>{safeDevice}</td>
            </tr>
        </table>

        <p style='font-size: 13px; color: #d32f2f; text-align: center; font-weight: 600;'>
            <strong>原因：</strong>输入的密码不正确。
        </p>

        <div style='background-color: #fff8f8; border: 1px solid #ffebeb; border-radius: 10px; padding: 25px; margin-top: 25px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 15px; color: #d32f2f; font-weight: bold;'>
                忘记了密码？
            </p>
            <p style='margin: 0 0 15px 0; font-size: 13px; color: #555;'>
                请使用密码重置功能来恢复您的账户访问权限。
            </p>
            <p style='margin: 0; font-size: 12px; color: #777;'>
                ⚠️ 如果不是您操作的，请立即修改您的密码。
            </p>
        </div>

        <div style='background-color: #f9f9f9; padding: 15px; margin-top: 20px; border-radius: 6px; text-align: center;'>
            <p style='margin: 0; font-size: 12px; color: #666;'>
                🔒 保持安全：使用强密码。
            </p>
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

                await SendEmailAsync("security@efavori.com", Email, "登录失败警报", emailBody, attachments);

            }

        }

        //security@efavori.com | Email doğrulama kodu
        public async Task SendAccountActivationCodeInfoEmailAsync(string Culture, string Email)
        {
            Users use = await _context.Users.FirstOrDefaultAsync(x => x.ContactInformation.Email == Email, _cts.Token);
            use.AccountActivationMailCode = Random.Shared.Next(100000, 999999);
            _context.Users.Update(use);
            await _context.SaveChangesAsync(_cts.Token);

            var attachments = new List<Attachment>();
            //attachments.Add(new Attachment("C:\\dosyalar\\rapor.pdf"));
            //attachments.Add(new Attachment("C:\\dosyalar\\resim.jpg"));

            var details = _userInfos.GetCurrentUserDetails();
            string safeIp = System.Net.WebUtility.HtmlEncode(details.IpAddress);
            string safeDevice = System.Net.WebUtility.HtmlEncode(details.UserAgent);
            string displayDate = DateTime.Now.ToString("dd MMMM yyyy HH:mm");

            if (Culture == "tr")
            {
                // E-posta aktivasyon kodu ve güvenlik bilgilerini içeren mail içeriği
                string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
        
        <div style='padding: 30px 30px 20px 30px; text-align: center;'>
            <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
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

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, $"{use.AccountActivationMailCode} - Hesap Doğrulama Kodunuz", emailBody, attachments);
            }
            if (Culture == "en")
            {
                // Email body containing email activation code and security information
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #1a237e; font-size: 20px; font-weight: 600; margin-bottom: 10px; text-align: center;'>Email Verification</h2>
        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>Please use the code below to complete your account verification process.</p>
        
        <div style='background-color: #f0f4ff; border: 1px solid #d1d9e6; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <span style='display: block; font-size: 11px; color: #5c6bc0; text-transform: uppercase; font-weight: bold; margin-bottom: 8px;'>Your Verification Code</span>
            <span style='font-family: ""Courier New"", Courier, monospace; font-size: 36px; font-weight: bold; color: #1a237e; letter-spacing: 6px;'>{use.AccountActivationMailCode}</span>
        </div>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>Transaction Details</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Date</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>IP Address</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>Device</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            This code is valid for 3 minutes. If you did not perform this action, please secure your account.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            This is an automated email. Please do not reply.
            <br>Trace ID: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, $"{use.AccountActivationMailCode} - Your Account Verification Code", emailBody, attachments);
            }
            if (Culture == "az")
            {
                // E-poçt aktivasiya kodu və təhlükəsizlik məlumatlarını ehtiva edən mail məzmunu
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #1a237e; font-size: 20px; font-weight: 600; margin-bottom: 10px; text-align: center;'>E-poçt Təsdiqləmə</h2>
        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>Hesab təsdiqləmə prosesini tamamlamaq üçün aşağıdakı kodu istifadə edin.</p>
        
        <div style='background-color: #f0f4ff; border: 1px solid #d1d9e6; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <span style='display: block; font-size: 11px; color: #5c6bc0; text-transform: uppercase; font-weight: bold; margin-bottom: 8px;'>Təsdiqləmə Kodunuz</span>
            <span style='font-family: ""Courier New"", Courier, monospace; font-size: 36px; font-weight: bold; color: #1a237e; letter-spacing: 6px;'>{use.AccountActivationMailCode}</span>
        </div>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>Əməliyyat Məlumatları</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Tarix</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>IP Ünvanı</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>Cihaz</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            Bu kodun etibarlılıq müddəti 3 dəqiqədir. Əgər bu əməliyyatı siz etməmisinizsə, lütfən hesabınızın təhlükəsizliyini təmin edin.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            Bu e-poçt avtomatik olaraq göndərilib. Lütfən cavab yazmayın.
            <br>Əməliyyat nömrəsi: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, $"{use.AccountActivationMailCode} - Hesab Təsdiqləmə Kodunuz", emailBody, attachments);
            }
            if (Culture == "de")
            {
                // E-Mail-Inhalt mit Aktivierungscode und Sicherheitsinformationen
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #1a237e; font-size: 20px; font-weight: 600; margin-bottom: 10px; text-align: center;'>E-Mail Bestätigung</h2>
        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>Bitte verwenden Sie den folgenden Code, um Ihre Kontoverifizierung abzuschließen.</p>
        
        <div style='background-color: #f0f4ff; border: 1px solid #d1d9e6; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <span style='display: block; font-size: 11px; color: #5c6bc0; text-transform: uppercase; font-weight: bold; margin-bottom: 8px;'>Ihr Bestätigungscode</span>
            <span style='font-family: ""Courier New"", Courier, monospace; font-size: 36px; font-weight: bold; color: #1a237e; letter-spacing: 6px;'>{use.AccountActivationMailCode}</span>
        </div>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>Transaktionsdetails</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Datum</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>IP-Adresse</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>Gerät</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            Dieser Code ist 3 Minuten lang gültig. Wenn Sie diese Anfrage nicht gestellt haben, sichern Sie bitte Ihr Konto.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            Dies ist eine automatisch generierte E-Mail. Bitte antworten Sie nicht darauf.
            <br>Trace-ID: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, $"{use.AccountActivationMailCode} - Ihr Bestätigungscode", emailBody, attachments);
            }
            if (Culture == "es")
            {
                // Contenido del correo que incluye el código de activación y la información de seguridad
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #1a237e; font-size: 20px; font-weight: 600; margin-bottom: 10px; text-align: center;'>Verificación de Correo</h2>
        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>Utilice el siguiente código para completar el proceso de verificación de su cuenta.</p>
        
        <div style='background-color: #f0f4ff; border: 1px solid #d1d9e6; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <span style='display: block; font-size: 11px; color: #5c6bc0; text-transform: uppercase; font-weight: bold; margin-bottom: 8px;'>Su Código de Verificación</span>
            <span style='font-family: ""Courier New"", Courier, monospace; font-size: 36px; font-weight: bold; color: #1a237e; letter-spacing: 6px;'>{use.AccountActivationMailCode}</span>
        </div>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>Información de la Operación</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Fecha</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Dirección IP</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>Dispositivo</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            Este código es válido por 3 minutos. Si no ha realizado esta acción, por favor asegure su cuenta.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            Este correo ha sido enviado automáticamente. Por favor, no responda.
            <br>ID de Seguimiento: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, $"{use.AccountActivationMailCode} - Su código de verificación de cuenta", emailBody, attachments);
            }
            if (Culture == "fr")
            {
                // Contenu de l'e-mail contenant le code d'activation et les informations de sécurité
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #1a237e; font-size: 20px; font-weight: 600; margin-bottom: 10px; text-align: center;'>Vérification de l'e-mail</h2>
        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>Veuillez utiliser le code ci-dessous pour terminer la vérification de votre compte.</p>
        
        <div style='background-color: #f0f4ff; border: 1px solid #d1d9e6; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <span style='display: block; font-size: 11px; color: #5c6bc0; text-transform: uppercase; font-weight: bold; margin-bottom: 8px;'>Votre Code de Vérification</span>
            <span style='font-family: ""Courier New"", Courier, monospace; font-size: 36px; font-weight: bold; color: #1a237e; letter-spacing: 6px;'>{use.AccountActivationMailCode}</span>
        </div>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>Informations sur la Transaction</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Date</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Adresse IP</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>Appareil</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            Ce code est valable pendant 3 minutes. Si vous n'êtes pas à l'origine de cette demande, veuillez sécuriser votre compte.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            Ceci est un message automatique. Veuillez ne pas y répondre.
            <br>ID de suivi : {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, $"{use.AccountActivationMailCode} - Votre code de vérification de compte", emailBody, attachments);
            }
            if (Culture == "hi")
            {
                // सक्रियण कोड और सुरक्षा जानकारी वाला ईमेल विषय वस्तु
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #1a237e; font-size: 20px; font-weight: 600; margin-bottom: 10px; text-align: center;'>ईमेल सत्यापन</h2>
        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>अपनी खाता सत्यापन प्रक्रिया को पूरा करने के लिए नीचे दिए गए कोड का उपयोग करें।</p>
        
        <div style='background-color: #f0f4ff; border: 1px solid #d1d9e6; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <span style='display: block; font-size: 11px; color: #5c6bc0; text-transform: uppercase; font-weight: bold; margin-bottom: 8px;'>आपका सत्यापन कोड</span>
            <span style='font-family: ""Courier New"", Courier, monospace; font-size: 36px; font-weight: bold; color: #1a237e; letter-spacing: 6px;'>{use.AccountActivationMailCode}</span>
        </div>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>लेनदेन की जानकारी</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>दिनांक</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>आईपी पता</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>डिवाइस</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            यह कोड 3 मिनट के लिए वैध है। यदि आपने यह क्रिया नहीं की है, तो कृपया अपना खाता सुरक्षित करें।
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            यह ईमेल स्वचालित रूप से भेजा गया है। कृपया इसका उत्तर न दें।
            <br>ट्रेस आईडी: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, $"{use.AccountActivationMailCode} - आपका खाता सत्यापन कोड", emailBody, attachments);
            }
            if (Culture == "pt")
            {
                // Conteúdo do e-mail contendo o código de ativação e informações de segurança
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #1a237e; font-size: 20px; font-weight: 600; margin-bottom: 10px; text-align: center;'>Verificação de E-mail</h2>
        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>Utilize o código abaixo para concluir o processo de verificação da sua conta.</p>
        
        <div style='background-color: #f0f4ff; border: 1px solid #d1d9e6; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <span style='display: block; font-size: 11px; color: #5c6bc0; text-transform: uppercase; font-weight: bold; margin-bottom: 8px;'>Seu Código de Verificação</span>
            <span style='font-family: ""Courier New"", Courier, monospace; font-size: 36px; font-weight: bold; color: #1a237e; letter-spacing: 6px;'>{use.AccountActivationMailCode}</span>
        </div>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>Informações da Operação</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Data</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Endereço IP</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>Dispositivo</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            Este código é válido por 3 minutos. Se você não solicitou esta operação, por favor, proteja sua conta.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            Este e-mail foi enviado automaticamente. Por favor, não responda.
            <br>ID da Transação: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, $"{use.AccountActivationMailCode} - Seu Código de Verificação de Conta", emailBody, attachments);
            }
            if (Culture == "ru")
            {
                // Содержимое письма с кодом активации и информацией о безопасности
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #1a237e; font-size: 20px; font-weight: 600; margin-bottom: 10px; text-align: center;'>Подтверждение e-mail</h2>
        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>Используйте приведенный ниже код для завершения верификации вашего аккаунта.</p>
        
        <div style='background-color: #f0f4ff; border: 1px solid #d1d9e6; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <span style='display: block; font-size: 11px; color: #5c6bc0; text-transform: uppercase; font-weight: bold; margin-bottom: 8px;'>Ваш код подтверждения</span>
            <span style='font-family: ""Courier New"", Courier, monospace; font-size: 36px; font-weight: bold; color: #1a237e; letter-spacing: 6px;'>{use.AccountActivationMailCode}</span>
        </div>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>Информация о транзакции</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Дата</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>IP-адрес</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>Устройство</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            Этот код действителен в течение 3 минут. Если вы не совершали этого действия, пожалуйста, обезопасьте свой аккаунт.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            Это письмо отправлено автоматически. Пожалуйста, не отвечайте на него.
            <br>ID транзакции: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, $"{use.AccountActivationMailCode} - Ваш код подтверждения аккаунта", emailBody, attachments);
            }
            if (Culture == "zh")
            {
                // 包含电子邮件激活码和安全信息的邮件内容
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #1a237e; font-size: 20px; font-weight: 600; margin-bottom: 10px; text-align: center;'>电子邮件验证</h2>
        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>请使用以下代码完成您的账户验证流程。</p>
        
        <div style='background-color: #f0f4ff; border: 1px solid #d1d9e6; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <span style='display: block; font-size: 11px; color: #5c6bc0; text-transform: uppercase; font-weight: bold; margin-bottom: 8px;'>您的验证码</span>
            <span style='font-family: ""Courier New"", Courier, monospace; font-size: 36px; font-weight: bold; color: #1a237e; letter-spacing: 6px;'>{use.AccountActivationMailCode}</span>
        </div>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>交易信息</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>日期</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>IP 地址</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>设备</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            此代码在 3 分钟内有效。如果这不是您的操作，请尽快采取措施保护您的账户。
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            这是一封自动发送的电子邮件，请勿回复。
            <br>追踪 ID: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, $"{use.AccountActivationMailCode} - 您的账户验证码", emailBody, attachments);
            }
        }

        //security@efavori.com | Email doğrulandı
        public async Task SendAccountEmailAddressVerifiedNotificationAsync(string Culture, string Email)
        {
            Users use = await _context.Users.FirstOrDefaultAsync(x => x.ContactInformation.Email == Email, _cts.Token);

            var attachments = new List<Attachment>();

            var details = _userInfos.GetCurrentUserDetails();
            string safeIp = System.Net.WebUtility.HtmlEncode(details.IpAddress);
            string safeDevice = System.Net.WebUtility.HtmlEncode(details.UserAgent);
            string displayDate = DateTime.Now.ToString("dd MMMM yyyy HH:mm");

            if (Culture == "tr")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <div style='background-color: #e8f5e9; border: 2px solid #4caf50; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <div style='font-size: 48px; color: #4caf50; margin-bottom: 10px;'>✓</div>
            <h2 style='color: #2e7d32; font-size: 20px; font-weight: 600; margin-bottom: 10px;'>E-posta Adresiniz Doğrulandı</h2>
            <p style='font-size: 14px; color: #555; margin: 0;'>Hesabınız başarıyla aktif edilmiştir.</p>
        </div>

        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>
            Sayın <strong>{use.ContactInformation.Email}</strong>,<br>
            E-posta adresiniz başarıyla doğrulanmıştır ve hesabınız artık tamamen aktiftir.
        </p>

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
            Eğer bu işlemi siz yapmadıysanız lütfen derhal hesabınızı güvenceye alın ve destek ekibimizle iletişime geçin.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            Bu mail otomatik olarak gönderilmiştir. Lütfen yanıtlamayınız.
            <br>İşlem Kayıt No: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, "✓ E-posta Adresiniz Doğrulandı", emailBody, attachments);
            }
            if (Culture == "en")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <div style='background-color: #e8f5e9; border: 2px solid #4caf50; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <div style='font-size: 48px; color: #4caf50; margin-bottom: 10px;'>✓</div>
            <h2 style='color: #2e7d32; font-size: 20px; font-weight: 600; margin-bottom: 10px;'>Your Email Address Has Been Verified</h2>
            <p style='font-size: 14px; color: #555; margin: 0;'>Your account has been successfully activated.</p>
        </div>

        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>
            Dear <strong>{use.ContactInformation.Email}</strong>,<br>
            Your email address has been successfully verified and your account is now fully active.
        </p>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>Transaction Details</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Date</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>IP Address</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>Device</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            If you did not perform this action, please secure your account immediately and contact our support team.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            This is an automated email. Please do not reply.
            <br>Trace ID: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, "✓ Your Email Address Has Been Verified", emailBody, attachments);
            }
            if (Culture == "az")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <div style='background-color: #e8f5e9; border: 2px solid #4caf50; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <div style='font-size: 48px; color: #4caf50; margin-bottom: 10px;'>✓</div>
            <h2 style='color: #2e7d32; font-size: 20px; font-weight: 600; margin-bottom: 10px;'>E-poçt Ünvanınız Təsdiqləndi</h2>
            <p style='font-size: 14px; color: #555; margin: 0;'>Hesabınız uğurla aktivləşdirilmişdir.</p>
        </div>

        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>
            Hörmətli <strong>{use.ContactInformation.Email}</strong>,<br>
            E-poçt ünvanınız uğurla təsdiqlənmişdir və hesabınız indi tamamilə aktivdir.
        </p>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>Əməliyyat Məlumatları</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Tarix</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>IP Ünvanı</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>Cihaz</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            Əgər bu əməliyyatı siz etməmisinizsə, lütfən dərhal hesabınızın təhlükəsizliyini təmin edin və dəstək komandamızla əlaqə saxlayın.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            Bu e-poçt avtomatik olaraq göndərilib. Lütfən cavab yazmayın.
            <br>Əməliyyat nömrəsi: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, "✓ E-poçt Ünvanınız Təsdiqləndi", emailBody, attachments);
            }
            if (Culture == "de")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <div style='background-color: #e8f5e9; border: 2px solid #4caf50; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <div style='font-size: 48px; color: #4caf50; margin-bottom: 10px;'>✓</div>
            <h2 style='color: #2e7d32; font-size: 20px; font-weight: 600; margin-bottom: 10px;'>Ihre E-Mail-Adresse Wurde Bestätigt</h2>
            <p style='font-size: 14px; color: #555; margin: 0;'>Ihr Konto wurde erfolgreich aktiviert.</p>
        </div>

        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>
            Sehr geehrte/r <strong>{use.ContactInformation.Email}</strong>,<br>
            Ihre E-Mail-Adresse wurde erfolgreich bestätigt und Ihr Konto ist jetzt vollständig aktiv.
        </p>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>Transaktionsdetails</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Datum</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>IP-Adresse</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>Gerät</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            Wenn Sie diese Anfrage nicht gestellt haben, sichern Sie bitte sofort Ihr Konto und kontaktieren Sie unser Support-Team.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            Dies ist eine automatisch generierte E-Mail. Bitte antworten Sie nicht darauf.
            <br>Trace-ID: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, "✓ Ihre E-Mail-Adresse Wurde Bestätigt", emailBody, attachments);
            }
            if (Culture == "es")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <div style='background-color: #e8f5e9; border: 2px solid #4caf50; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <div style='font-size: 48px; color: #4caf50; margin-bottom: 10px;'>✓</div>
            <h2 style='color: #2e7d32; font-size: 20px; font-weight: 600; margin-bottom: 10px;'>Su Dirección de Correo Ha Sido Verificada</h2>
            <p style='font-size: 14px; color: #555; margin: 0;'>Su cuenta ha sido activada exitosamente.</p>
        </div>

        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>
            Estimado/a <strong>{use.ContactInformation.Email}</strong>,<br>
            Su dirección de correo electrónico ha sido verificada exitosamente y su cuenta ahora está completamente activa.
        </p>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>Información de la Operación</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Fecha</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Dirección IP</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>Dispositivo</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            Si no ha realizado esta acción, por favor asegure su cuenta inmediatamente y contacte a nuestro equipo de soporte.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            Este correo ha sido enviado automáticamente. Por favor, no responda.
            <br>ID de Seguimiento: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, "✓ Su Dirección de Correo Ha Sido Verificada", emailBody, attachments);
            }
            if (Culture == "fr")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <div style='background-color: #e8f5e9; border: 2px solid #4caf50; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <div style='font-size: 48px; color: #4caf50; margin-bottom: 10px;'>✓</div>
            <h2 style='color: #2e7d32; font-size: 20px; font-weight: 600; margin-bottom: 10px;'>Votre Adresse E-mail a Été Vérifiée</h2>
            <p style='font-size: 14px; color: #555; margin: 0;'>Votre compte a été activé avec succès.</p>
        </div>

        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>
            Cher/Chère <strong>{use.ContactInformation.Email}</strong>,<br>
            Votre adresse e-mail a été vérifiée avec succès et votre compte est maintenant entièrement actif.
        </p>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>Informations sur la Transaction</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Date</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Adresse IP</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>Appareil</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            Si vous n'êtes pas à l'origine de cette demande, veuillez sécuriser immédiatement votre compte et contacter notre équipe de support.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            Ceci est un message automatique. Veuillez ne pas y répondre.
            <br>ID de suivi : {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, "✓ Votre Adresse E-mail a Été Vérifiée", emailBody, attachments);
            }
            if (Culture == "hi")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <div style='background-color: #e8f5e9; border: 2px solid #4caf50; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <div style='font-size: 48px; color: #4caf50; margin-bottom: 10px;'>✓</div>
            <h2 style='color: #2e7d32; font-size: 20px; font-weight: 600; margin-bottom: 10px;'>आपका ईमेल पता सत्यापित हो गया है</h2>
            <p style='font-size: 14px; color: #555; margin: 0;'>आपका खाता सफलतापूर्वक सक्रिय हो गया है।</p>
        </div>

        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>
            प्रिय <strong>{use.ContactInformation.Email}</strong>,<br>
            आपका ईमेल पता सफलतापूर्वक सत्यापित हो गया है और आपका खाता अब पूरी तरह से सक्रिय है।
        </p>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>लेनदेन की जानकारी</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>दिनांक</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>आईपी पता</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>डिवाइस</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            यदि आपने यह क्रिया नहीं की है, तो कृपया तुरंत अपना खाता सुरक्षित करें और हमारी सहायता टीम से संपर्क करें।
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            यह ईमेल स्वचालित रूप से भेजा गया है। कृपया इसका उत्तर न दें।
            <br>ट्रेस आईडी: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, "✓ आपका ईमेल पता सत्यापित हो गया है", emailBody, attachments);
            }
            if (Culture == "pt")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <div style='background-color: #e8f5e9; border: 2px solid #4caf50; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <div style='font-size: 48px; color: #4caf50; margin-bottom: 10px;'>✓</div>
            <h2 style='color: #2e7d32; font-size: 20px; font-weight: 600; margin-bottom: 10px;'>Seu Endereço de E-mail Foi Verificado</h2>
            <p style='font-size: 14px; color: #555; margin: 0;'>Sua conta foi ativada com sucesso.</p>
        </div>

        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>
            Prezado(a) <strong>{use.ContactInformation.Email}</strong>,<br>
            Seu endereço de e-mail foi verificado com sucesso e sua conta agora está totalmente ativa.
        </p>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>Informações da Operação</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Data</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Endereço IP</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>Dispositivo</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            Se você não solicitou esta operação, por favor, proteja sua conta imediatamente e entre em contato com nossa equipe de suporte.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            Este e-mail foi enviado automaticamente. Por favor, não responda.
            <br>ID da Transação: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, "✓ Seu Endereço de E-mail Foi Verificado", emailBody, attachments);
            }
            if (Culture == "ru")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <div style='background-color: #e8f5e9; border: 2px solid #4caf50; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <div style='font-size: 48px; color: #4caf50; margin-bottom: 10px;'>✓</div>
            <h2 style='color: #2e7d32; font-size: 20px; font-weight: 600; margin-bottom: 10px;'>Ваш Адрес Электронной Почты Подтвержден</h2>
            <p style='font-size: 14px; color: #555; margin: 0;'>Ваш аккаунт успешно активирован.</p>
        </div>

        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>
            Уважаемый(ая) <strong>{use.ContactInformation.Email}</strong>,<br>
            Ваш адрес электронной почты успешно подтвержден и ваш аккаунт теперь полностью активен.
        </p>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>Информация о транзакции</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Дата</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>IP-адрес</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>Устройство</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            Если вы не совершали этого действия, пожалуйста, немедленно обезопасьте свой аккаунт и свяжитесь с нашей службой поддержки.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            Это письмо отправлено автоматически. Пожалуйста, не отвечайте на него.
            <br>ID транзакции: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, "✓ Ваш Адрес Электронной Почты Подтвержден", emailBody, attachments);
            }
            if (Culture == "zh")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <div style='background-color: #e8f5e9; border: 2px solid #4caf50; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <div style='font-size: 48px; color: #4caf50; margin-bottom: 10px;'>✓</div>
            <h2 style='color: #2e7d32; font-size: 20px; font-weight: 600; margin-bottom: 10px;'>您的电子邮件地址已验证</h2>
            <p style='font-size: 14px; color: #555; margin: 0;'>您的账户已成功激活。</p>
        </div>

        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>
            尊敬的 <strong>{use.ContactInformation.Email}</strong>,<br>
            您的电子邮件地址已成功验证，您的账户现已完全激活。
        </p>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>交易信息</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>日期</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>IP 地址</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>设备</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            如果这不是您的操作，请立即采取措施保护您的账户并联系我们的支持团队。
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            这是一封自动发送的电子邮件，请勿回复。
            <br>追踪 ID: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, "✓ 您的电子邮件地址已验证", emailBody, attachments);
            }
        }

        //security@efavori.com | Çıkış işlemi
        public async Task SendLogOutInfoEmailAsync(string Culture, string Email)
        {
            var attachments = new List<Attachment>();

            var details = _userInfos.GetCurrentUserDetails();
            string safeIp = System.Net.WebUtility.HtmlEncode(details.IpAddress);
            string safeDevice = System.Net.WebUtility.HtmlEncode(details.UserAgent);
            string displayDate = DateTime.Now.ToString("dd MMMM yyyy HH:mm");

            if (Culture == "tr")
            {
                string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
        
        <div style='padding: 30px 30px 20px 30px; text-align: center;'>
            <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
        </div>

        <div style='padding: 0 40px 30px 40px;'>
            <h2 style='color: #1a237e; font-size: 20px; font-weight: 600; margin-bottom: 20px; border-bottom: 2px solid #f0f0f0; padding-bottom: 10px; text-align: center;'>Oturum Kapatma Bildirimi</h2>
            
            <p style='font-size: 15px;'>Sayın Kullanıcımız,</p>
            <p style='font-size: 14px; color: #555;'>Hesabınızdan başarıyla çıkış yapılmıştır. İşlemin ayrıntıları aşağıdadır:</p>
            
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
                Hesabınız güvenli bir şekilde kapatılmıştır.
            </p>

            <div style='background-color: #f0f4ff; border-left: 4px solid #1a237e; border-right: 4px solid #1a237e; padding: 20px; margin-top: 25px; text-align: center;'>
                <p style='margin: 0 0 15px 0; font-size: 13px; color: #1a237e; line-height: 1.5;'>
                    <strong>Güvenlik İpucu:</strong> Genel bilgisayarlardan çıkış yaptıktan sonra tarayıcıyı tamamen kapatmayı unutmayın.
                </p>
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
                await SendEmailAsync("security@efavori.com", Email, "Oturum Kapatma Bildirimi", emailBody, attachments);
            }
            if (Culture == "en")
            {
                string emailBody = $@"
<div style='font-family: ""Inter"", -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e8e8e8; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 12px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 25px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Security Logo' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #1a237e; font-size: 22px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Sign-out Confirmation</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>You have successfully signed out of your account.</p>
        
        <p style='font-size: 15px; font-weight: 600;'>Hello,</p>
        <p style='font-size: 14px; color: #555;'>Here are the details of your sign-out activity:</p>
        
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
            Your account is now secured and signed out from all active sessions.
        </p>

        <div style='background-color: #f0f4ff; border: 1px solid #c5d9f9; border-radius: 8px; padding: 20px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 14px; color: #1a237e; font-weight: 600;'>
                Security Tip
            </p>
            <p style='margin: 0; font-size: 13px; color: #444; line-height: 1.5;'>
                When using shared computers, always remember to sign out and close your browser completely.
            </p>
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
                await SendEmailAsync("security@efavori.com", Email, "Sign-out Confirmation", emailBody, attachments);
            }
            if (Culture == "az")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 10px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 2px 10px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Security' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #1a237e; font-size: 20px; font-weight: 700; margin-bottom: 15px; border-bottom: 2px solid #f5f5f5; padding-bottom: 15px; text-align: center;'>Çıkış Bildirişi</h2>
        
        <p style='font-size: 15px; font-weight: 600;'>Hörmətli İstifadəçi,</p>
        <p style='font-size: 14px; color: #555;'>Hesabınızdan uğurla çıxış etmisiniz. Çıxış məlumatları aşağıda verilmişdir:</p>
        
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
            Hesabınız təhlükəsiz şəkildə kapatılmışdır.
        </p>

        <div style='background-color: #f0f4ff; border: 1px solid #c5d9f9; border-radius: 8px; padding: 20px; margin-top: 25px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 14px; color: #1a237e; font-weight: bold;'>
                Təhlükəsizlik İpucu
            </p>
            <p style='margin: 0; font-size: 13px; color: #444;'>
                Ortaq kompüterlərdən istifadə edərkən həmişə çıxış etməyi və brauzeri tamamilə bağlamağı unutmayın.
            </p>
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

                await SendEmailAsync("security@efavori.com", Email, "Çıxış Bildirişi", emailBody, attachments);

            }
            if (Culture == "de")
            {
                string emailBody = $@"
<div style='font-family: ""Helvetica Neue"", Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #dddddd; padding: 0; border-radius: 10px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 5px 15px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Sicherheit' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #0d47a1; font-size: 20px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Abmeldung bestätigt</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>Sie haben sich erfolgreich von Ihrem Konto abgemeldet.</p>
        
        <p style='font-size: 15px;'>Guten Tag,</p>
        <p style='font-size: 14px; color: #555;'>Hier sind die Details Ihrer Abmeldung:</p>
        
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
            Ihr Konto ist nun sicher abgemeldet.
        </p>

        <div style='background-color: #f0f4ff; border: 1px solid #c5d9f9; border-radius: 8px; padding: 20px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 14px; color: #0d47a1; font-weight: bold;'>
                Sicherheitstipp
            </p>
            <p style='margin: 0; font-size: 13px; color: #4a5568;'>
                Denken Sie daran, bei der Verwendung öffentlicher Computer den Browser vollständig zu schließen.
            </p>
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

                await SendEmailAsync("security@efavori.com", Email, "Abmeldung bestätigt", emailBody, attachments);

            }
            if (Culture == "es")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.03);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Seguridad' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #1a237e; font-size: 22px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Confirmación de Cierre de Sesión</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>Ha cerrado sesión exitosamente en su cuenta.</p>
        
        <p style='font-size: 15px;'>Estimado usuario/a,</p>
        <p style='font-size: 14px; color: #555;'>Aquí están los detalles de su cierre de sesión:</p>
        
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
            Su cuenta está ahora segura y ha cerrado sesión.
        </p>

        <div style='background-color: #f0f4ff; border: 1px solid #c5d9f9; border-radius: 8px; padding: 20px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 14px; color: #1a237e; font-weight: bold;'>
                Consejo de Seguridad
            </p>
            <p style='margin: 0; font-size: 13px; color: #555;'>
                Al usar computadoras compartidas, recuerde siempre cerrar sesión y cerrar el navegador completamente.
            </p>
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

                await SendEmailAsync("security@efavori.com", Email, "Confirmación de cierre de sesión", emailBody, attachments);

            }
            if (Culture == "fr")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Sécurité' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #1a237e; font-size: 20px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Confirmation de déconnexion</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>Vous avez été déconnecté de votre compte avec succès.</p>
        
        <p style='font-size: 15px;'>Bonjour,</p>
        <p style='font-size: 14px; color: #555;'>Voici les détails de votre déconnexion :</p>
        
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
            Votre compte est maintenant sécurisé et déconnecté.
        </p>

        <div style='background-color: #f0f4ff; border: 1px solid #c5d9f9; border-radius: 10px; padding: 20px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 14px; color: #1a237e; font-weight: bold;'>
                Conseil de sécurité
            </p>
            <p style='margin: 0; font-size: 13px; color: #555;'>
                Lorsque vous utilisez des ordinateurs partagés, n'oubliez pas de vous déconnecter et de fermer complètement votre navigateur.
            </p>
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
                await SendEmailAsync("security@efavori.com", Email, "Confirmation de déconnexion", emailBody, attachments);

            }
            if (Culture == "hi")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Security' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #1a237e; font-size: 22px; font-weight: 700; margin-bottom: 10px; text-align: center;'>लॉगआउट की पुष्टि</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>आप अपने खाते से सफलतापूर्वक लॉगआउट हो गए हैं।</p>
        
        <p style='font-size: 15px;'>नमस्ते,</p>
        <p style='font-size: 14px; color: #555;'>आपके लॉगआउट के विवरण यहां दिए गए हैं:</p>
        
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
            आपका खाता अब सुरक्षित है और लॉगआउट हो गया है।
        </p>

        <div style='background-color: #f0f4ff; border: 1px solid #c5d9f9; border-radius: 10px; padding: 20px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 16px; color: #1a237e; font-weight: bold;'>
                सुरक्षा सुझाव
            </p>
            <p style='margin: 0; font-size: 14px; color: #555;'>
                साझा कंप्यूटर का उपयोग करते समय, हमेशा लॉगआउट करना और ब्राउज़र को पूरी तरह बंद करना याद रखें।
            </p>
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
                await SendEmailAsync("security@efavori.com", Email, "लॉगआउट की पुष्टि", emailBody, attachments);

            }
            if (Culture == "pt")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.03);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Segurança' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #1a237e; font-size: 22px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Confirmação de Logout</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>Você saiu de sua conta com sucesso.</p>
        
        <p style='font-size: 15px;'>Olá,</p>
        <p style='font-size: 14px; color: #555;'>Aqui estão os detalhes do seu logout:</p>
        
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
            Sua conta está agora segura e desconectada.
        </p>

        <div style='background-color: #f0f4ff; border: 1px solid #c5d9f9; border-radius: 10px; padding: 20px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 15px; color: #1a237e; font-weight: bold;'>
                Dica de Segurança
            </p>
            <p style='margin: 0; font-size: 13px; color: #555;'>
                Ao usar computadores compartilhados, lembre-se sempre de fazer logout e fechar o navegador completamente.
            </p>
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

                await SendEmailAsync("security@efavori.com", Email, "Confirmação de logout", emailBody, attachments);

            }
            if (Culture == "ru")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.03);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Безопасность' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #1a237e; font-size: 20px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Подтверждение выхода</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>Вы успешно вышли из своего аккаунта.</p>
        
        <p style='font-size: 15px;'>Здравствуйте,</p>
        <p style='font-size: 14px; color: #555;'>Вот детали вашего выхода:</p>
        
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
            Ваш аккаунт теперь безопасен и вы вышли из него.
        </p>

        <div style='background-color: #f0f4ff; border: 1px solid #c5d9f9; border-radius: 10px; padding: 20px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 15px; color: #1a237e; font-weight: bold;'>
                Совет по безопасности
            </p>
            <p style='margin: 0; font-size: 13px; color: #555;'>
                При использовании общих компьютеров не забывайте выходить и полностью закрывать браузер.
            </p>
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

                await SendEmailAsync("security@efavori.com", Email, "Подтверждение выхода", emailBody, attachments);

            }
            if (Culture == "zh")
            {
                string emailBody = $@"
<div style='font-family: ""PingFang SC"", ""Microsoft YaHei"", ""Helvetica Neue"", Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.03);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='安全通知' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #1a237e; font-size: 22px; font-weight: 700; margin-bottom: 10px; text-align: center;'>登出确认</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>您已成功退出账户。</p>
        
        <p style='font-size: 15px;'>尊敬的用户，您好：</p>
        <p style='font-size: 14px; color: #555;'>以下是您的登出信息：</p>
        
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
            您的账户现已安全登出。
        </p>

        <div style='background-color: #f0f4ff; border: 1px solid #c5d9f9; border-radius: 10px; padding: 20px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 15px; color: #1a237e; font-weight: bold;'>
                安全提示
            </p>
            <p style='margin: 0; font-size: 13px; color: #555;'>
                在使用公共电脑时，请记得登出账户并完全关闭浏览器。
            </p>
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

                await SendEmailAsync("security@efavori.com", Email, "登出确认", emailBody, attachments);

            }

        }

        // security@efavori.com | Şifre sıfırlama kodu  
        public async Task SendAccountAccountPasswordResetMailCodeInformationEmailAsync(string Culture, string Email)
        {
            Users use = await _context.Users.FirstOrDefaultAsync(x => x.ContactInformation.Email == Email, _cts.Token);
            use.AccountPasswordResetMailCode = Random.Shared.Next(100000, 999999);
            _context.Users.Update(use);
            await _context.SaveChangesAsync(_cts.Token);

            var attachments = new List<Attachment>();

            var details = _userInfos.GetCurrentUserDetails();
            string safeIp = System.Net.WebUtility.HtmlEncode(details.IpAddress);
            string safeDevice = System.Net.WebUtility.HtmlEncode(details.UserAgent);
            string displayDate = DateTime.Now.ToString("dd MMMM yyyy HH:mm");

            if (Culture == "tr")
            {
                // Şifre sıfırlama kodu ve güvenlik bilgilerini içeren mail içeriği
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #c62828; font-size: 20px; font-weight: 600; margin-bottom: 10px; text-align: center;'>Şifre Sıfırlama</h2>
        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>Şifrenizi sıfırlamak için aşağıdaki kodu kullanın.</p>
        
        <div style='background-color: #ffebee; border: 1px solid #ef9a9a; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <span style='display: block; font-size: 11px; color: #d32f2f; text-transform: uppercase; font-weight: bold; margin-bottom: 8px;'>Şifre Sıfırlama Kodunuz</span>
            <span style='font-family: ""Courier New"", Courier, monospace; font-size: 36px; font-weight: bold; color: #c62828; letter-spacing: 6px;'>{use.AccountPasswordResetMailCode}</span>
        </div>

        <div style='background-color: #fff3e0; border-left: 4px solid #ff9800; padding: 12px 16px; margin-bottom: 20px; border-radius: 4px;'>
            <p style='font-size: 12px; color: #e65100; margin: 0; font-weight: 600;'>⚠️ Güvenlik Uyarısı</p>
            <p style='font-size: 11px; color: #bf360c; margin: 5px 0 0 0;'>Bu işlemi siz yapmadıysanız, lütfen derhal şifrenizi değiştirin ve hesabınızı güvenceye alın.</p>
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
            Bu kodun süresi 3 dakika geçerlidir. Kod yalnızca bir kez kullanılabilir.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            Bu mail otomatik olarak gönderilmiştir. Lütfen yanıtlamayınız.
            <br>İşlem Kayıt No: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, $"{use.AccountPasswordResetMailCode} - Şifre Sıfırlama Kodunuz", emailBody, attachments);
            }
            if (Culture == "en")
            {
                // Password reset code and security information email content
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #c62828; font-size: 20px; font-weight: 600; margin-bottom: 10px; text-align: center;'>Password Reset</h2>
        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>Use the code below to reset your password.</p>
        
        <div style='background-color: #ffebee; border: 1px solid #ef9a9a; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <span style='display: block; font-size: 11px; color: #d32f2f; text-transform: uppercase; font-weight: bold; margin-bottom: 8px;'>Your Password Reset Code</span>
            <span style='font-family: ""Courier New"", Courier, monospace; font-size: 36px; font-weight: bold; color: #c62828; letter-spacing: 6px;'>{use.AccountPasswordResetMailCode}</span>
        </div>

        <div style='background-color: #fff3e0; border-left: 4px solid #ff9800; padding: 12px 16px; margin-bottom: 20px; border-radius: 4px;'>
            <p style='font-size: 12px; color: #e65100; margin: 0; font-weight: 600;'>⚠️ Security Warning</p>
            <p style='font-size: 11px; color: #bf360c; margin: 5px 0 0 0;'>If you did not request this, please change your password immediately and secure your account.</p>
        </div>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>Transaction Details</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Date</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>IP Address</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>Device</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            This code is valid for 3 minutes. The code can only be used once.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            This is an automated email. Please do not reply.
            <br>Trace ID: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, $"{use.AccountPasswordResetMailCode} - Your Password Reset Code", emailBody, attachments);
            }
            if (Culture == "az")
            {
                // Şifrə sıfırlama kodu və təhlükəsizlik məlumatlarını ehtiva edən mail məzmunu
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #c62828; font-size: 20px; font-weight: 600; margin-bottom: 10px; text-align: center;'>Şifrə Sıfırlama</h2>
        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>Şifrənizi sıfırlamaq üçün aşağıdakı kodu istifadə edin.</p>
        
        <div style='background-color: #ffebee; border: 1px solid #ef9a9a; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <span style='display: block; font-size: 11px; color: #d32f2f; text-transform: uppercase; font-weight: bold; margin-bottom: 8px;'>Şifrə Sıfırlama Kodunuz</span>
            <span style='font-family: ""Courier New"", Courier, monospace; font-size: 36px; font-weight: bold; color: #c62828; letter-spacing: 6px;'>{use.AccountPasswordResetMailCode}</span>
        </div>

        <div style='background-color: #fff3e0; border-left: 4px solid #ff9800; padding: 12px 16px; margin-bottom: 20px; border-radius: 4px;'>
            <p style='font-size: 12px; color: #e65100; margin: 0; font-weight: 600;'>⚠️ Təhlükəsizlik Xəbərdarlığı</p>
            <p style='font-size: 11px; color: #bf360c; margin: 5px 0 0 0;'>Bu əməliyyatı siz etməmisinizsə, lütfən dərhal şifrənizi dəyişdirin və hesabınızın təhlükəsizliyini təmin edin.</p>
        </div>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>Əməliyyat Məlumatları</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Tarix</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>IP Ünvanı</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>Cihaz</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            Bu kodun etibarlılıq müddəti 3 dəqiqədir. Kod yalnız bir dəfə istifadə edilə bilər.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            Bu e-poçt avtomatik olaraq göndərilib. Lütfən cavab yazmayın.
            <br>Əməliyyat nömrəsi: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, $"{use.AccountPasswordResetMailCode} - Şifrə Sıfırlama Kodunuz", emailBody, attachments);
            }
            if (Culture == "de")
            {
                // E-Mail-Inhalt mit Passwort-Zurücksetzungscode und Sicherheitsinformationen
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #c62828; font-size: 20px; font-weight: 600; margin-bottom: 10px; text-align: center;'>Passwort Zurücksetzen</h2>
        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>Verwenden Sie den folgenden Code, um Ihr Passwort zurückzusetzen.</p>
        
        <div style='background-color: #ffebee; border: 1px solid #ef9a9a; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <span style='display: block; font-size: 11px; color: #d32f2f; text-transform: uppercase; font-weight: bold; margin-bottom: 8px;'>Ihr Code zum Zurücksetzen</span>
            <span style='font-family: ""Courier New"", Courier, monospace; font-size: 36px; font-weight: bold; color: #c62828; letter-spacing: 6px;'>{use.AccountPasswordResetMailCode}</span>
        </div>

        <div style='background-color: #fff3e0; border-left: 4px solid #ff9800; padding: 12px 16px; margin-bottom: 20px; border-radius: 4px;'>
            <p style='font-size: 12px; color: #e65100; margin: 0; font-weight: 600;'>⚠️ Sicherheitswarnung</p>
            <p style='font-size: 11px; color: #bf360c; margin: 5px 0 0 0;'>Wenn Sie diese Anfrage nicht gestellt haben, ändern Sie bitte sofort Ihr Passwort und sichern Sie Ihr Konto.</p>
        </div>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>Transaktionsdetails</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Datum</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>IP-Adresse</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>Gerät</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            Dieser Code ist 3 Minuten lang gültig. Der Code kann nur einmal verwendet werden.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            Dies ist eine automatisch generierte E-Mail. Bitte antworten Sie nicht darauf.
            <br>Trace-ID: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, $"{use.AccountPasswordResetMailCode} - Ihr Code zum Passwort zurücksetzen", emailBody, attachments);
            }
            if (Culture == "es")
            {
                // Contenido del correo con código de restablecimiento de contraseña e información de seguridad
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #c62828; font-size: 20px; font-weight: 600; margin-bottom: 10px; text-align: center;'>Restablecer Contraseña</h2>
        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>Utilice el siguiente código para restablecer su contraseña.</p>
        
        <div style='background-color: #ffebee; border: 1px solid #ef9a9a; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <span style='display: block; font-size: 11px; color: #d32f2f; text-transform: uppercase; font-weight: bold; margin-bottom: 8px;'>Su Código de Restablecimiento</span>
            <span style='font-family: ""Courier New"", Courier, monospace; font-size: 36px; font-weight: bold; color: #c62828; letter-spacing: 6px;'>{use.AccountPasswordResetMailCode}</span>
        </div>

        <div style='background-color: #fff3e0; border-left: 4px solid #ff9800; padding: 12px 16px; margin-bottom: 20px; border-radius: 4px;'>
            <p style='font-size: 12px; color: #e65100; margin: 0; font-weight: 600;'>⚠️ Advertencia de Seguridad</p>
            <p style='font-size: 11px; color: #bf360c; margin: 5px 0 0 0;'>Si no solicitó esto, cambie su contraseña inmediatamente y asegure su cuenta.</p>
        </div>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>Información de la Operación</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Fecha</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Dirección IP</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>Dispositivo</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            Este código es válido por 3 minutos. El código solo se puede usar una vez.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            Este correo ha sido enviado automáticamente. Por favor, no responda.
            <br>ID de Seguimiento: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, $"{use.AccountPasswordResetMailCode} - Su código de restablecimiento de contraseña", emailBody, attachments);
            }
            if (Culture == "fr")
            {
                // Contenu de l'e-mail avec code de réinitialisation de mot de passe et informations de sécurité
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #c62828; font-size: 20px; font-weight: 600; margin-bottom: 10px; text-align: center;'>Réinitialisation du Mot de Passe</h2>
        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>Utilisez le code ci-dessous pour réinitialiser votre mot de passe.</p>
        
        <div style='background-color: #ffebee; border: 1px solid #ef9a9a; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <span style='display: block; font-size: 11px; color: #d32f2f; text-transform: uppercase; font-weight: bold; margin-bottom: 8px;'>Votre Code de Réinitialisation</span>
            <span style='font-family: ""Courier New"", Courier, monospace; font-size: 36px; font-weight: bold; color: #c62828; letter-spacing: 6px;'>{use.AccountPasswordResetMailCode}</span>
        </div>

        <div style='background-color: #fff3e0; border-left: 4px solid #ff9800; padding: 12px 16px; margin-bottom: 20px; border-radius: 4px;'>
            <p style='font-size: 12px; color: #e65100; margin: 0; font-weight: 600;'>⚠️ Avertissement de Sécurité</p>
            <p style='font-size: 11px; color: #bf360c; margin: 5px 0 0 0;'>Si vous n'êtes pas à l'origine de cette demande, changez immédiatement votre mot de passe et sécurisez votre compte.</p>
        </div>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>Informations sur la Transaction</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Date</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Adresse IP</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>Appareil</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            Ce code est valable pendant 3 minutes. Le code ne peut être utilisé qu'une seule fois.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            Ceci est un message automatique. Veuillez ne pas y répondre.
            <br>ID de suivi : {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, $"{use.AccountPasswordResetMailCode} - Votre code de réinitialisation de mot de passe", emailBody, attachments);
            }
            if (Culture == "hi")
            {
                // पासवर्ड रीसेट कोड और सुरक्षा जानकारी वाला ईमेल विषय वस्तु
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #c62828; font-size: 20px; font-weight: 600; margin-bottom: 10px; text-align: center;'>पासवर्ड रीसेट</h2>
        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>अपना पासवर्ड रीसेट करने के लिए नीचे दिए गए कोड का उपयोग करें।</p>
        
        <div style='background-color: #ffebee; border: 1px solid #ef9a9a; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <span style='display: block; font-size: 11px; color: #d32f2f; text-transform: uppercase; font-weight: bold; margin-bottom: 8px;'>आपका रीसेट कोड</span>
            <span style='font-family: ""Courier New"", Courier, monospace; font-size: 36px; font-weight: bold; color: #c62828; letter-spacing: 6px;'>{use.AccountPasswordResetMailCode}</span>
        </div>

        <div style='background-color: #fff3e0; border-left: 4px solid #ff9800; padding: 12px 16px; margin-bottom: 20px; border-radius: 4px;'>
            <p style='font-size: 12px; color: #e65100; margin: 0; font-weight: 600;'>⚠️ सुरक्षा चेतावनी</p>
            <p style='font-size: 11px; color: #bf360c; margin: 5px 0 0 0;'>यदि आपने यह अनुरोध नहीं किया है, तो कृपया तुरंत अपना पासवर्ड बदलें और अपने खाते को सुरक्षित करें।</p>
        </div>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>लेनदेन की जानकारी</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>दिनांक</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>आईपी पता</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>डिवाइस</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            यह कोड 3 मिनट के लिए वैध है। कोड का उपयोग केवल एक बार किया जा सकता है।
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            यह ईमेल स्वचालित रूप से भेजा गया है। कृपया इसका उत्तर न दें।
            <br>ट्रेस आईडी: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, $"{use.AccountPasswordResetMailCode} - आपका पासवर्ड रीसेट कोड", emailBody, attachments);
            }
            if (Culture == "pt")
            {
                // Conteúdo do e-mail com código de redefinição de senha e informações de segurança
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #c62828; font-size: 20px; font-weight: 600; margin-bottom: 10px; text-align: center;'>Redefinição de Senha</h2>
        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>Utilize o código abaixo para redefinir sua senha.</p>
        
        <div style='background-color: #ffebee; border: 1px solid #ef9a9a; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <span style='display: block; font-size: 11px; color: #d32f2f; text-transform: uppercase; font-weight: bold; margin-bottom: 8px;'>Seu Código de Redefinição</span>
            <span style='font-family: ""Courier New"", Courier, monospace; font-size: 36px; font-weight: bold; color: #c62828; letter-spacing: 6px;'>{use.AccountPasswordResetMailCode}</span>
        </div>

        <div style='background-color: #fff3e0; border-left: 4px solid #ff9800; padding: 12px 16px; margin-bottom: 20px; border-radius: 4px;'>
            <p style='font-size: 12px; color: #e65100; margin: 0; font-weight: 600;'>⚠️ Aviso de Segurança</p>
            <p style='font-size: 11px; color: #bf360c; margin: 5px 0 0 0;'>Se você não solicitou esta operação, altere sua senha imediatamente e proteja sua conta.</p>
        </div>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>Informações da Operação</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Data</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Endereço IP</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>Dispositivo</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            Este código é válido por 3 minutos. O código só pode ser usado uma vez.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            Este e-mail foi enviado automaticamente. Por favor, não responda.
            <br>ID da Transação: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, $"{use.AccountPasswordResetMailCode} - Seu Código de Redefinição de Senha", emailBody, attachments);
            }
            if (Culture == "ru")
            {
                // Содержимое письма с кодом сброса пароля и информацией о безопасности
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #c62828; font-size: 20px; font-weight: 600; margin-bottom: 10px; text-align: center;'>Сброс Пароля</h2>
        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>Используйте приведенный ниже код для сброса вашего пароля.</p>
        
        <div style='background-color: #ffebee; border: 1px solid #ef9a9a; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <span style='display: block; font-size: 11px; color: #d32f2f; text-transform: uppercase; font-weight: bold; margin-bottom: 8px;'>Ваш код сброса пароля</span>
            <span style='font-family: ""Courier New"", Courier, monospace; font-size: 36px; font-weight: bold; color: #c62828; letter-spacing: 6px;'>{use.AccountPasswordResetMailCode}</span>
        </div>

        <div style='background-color: #fff3e0; border-left: 4px solid #ff9800; padding: 12px 16px; margin-bottom: 20px; border-radius: 4px;'>
            <p style='font-size: 12px; color: #e65100; margin: 0; font-weight: 600;'>⚠️ Предупреждение о Безопасности</p>
            <p style='font-size: 11px; color: #bf360c; margin: 5px 0 0 0;'>Если вы не запрашивали это, немедленно измените свой пароль и обезопасьте свой аккаунт.</p>
        </div>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>Информация о транзакции</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>Дата</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>IP-адрес</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>Устройство</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            Этот код действителен в течение 3 минут. Код можно использовать только один раз.
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            Это письмо отправлено автоматически. Пожалуйста, не отвечайте на него.
            <br>ID транзакции: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, $"{use.AccountPasswordResetMailCode} - Ваш код сброса пароля", emailBody, attachments);
            }
            if (Culture == "zh")
            {
                // 包含密码重置代码和安全信息的邮件内容
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #c62828; font-size: 20px; font-weight: 600; margin-bottom: 10px; text-align: center;'>密码重置</h2>
        <p style='font-size: 14px; color: #555; text-align: center; margin-bottom: 25px;'>请使用以下代码重置您的密码。</p>
        
        <div style='background-color: #ffebee; border: 1px solid #ef9a9a; border-radius: 8px; padding: 20px; text-align: center; margin-bottom: 30px;'>
            <span style='display: block; font-size: 11px; color: #d32f2f; text-transform: uppercase; font-weight: bold; margin-bottom: 8px;'>您的重置代码</span>
            <span style='font-family: ""Courier New"", Courier, monospace; font-size: 36px; font-weight: bold; color: #c62828; letter-spacing: 6px;'>{use.AccountPasswordResetMailCode}</span>
        </div>

        <div style='background-color: #fff3e0; border-left: 4px solid #ff9800; padding: 12px 16px; margin-bottom: 20px; border-radius: 4px;'>
            <p style='font-size: 12px; color: #e65100; margin: 0; font-weight: 600;'>⚠️ 安全警告</p>
            <p style='font-size: 11px; color: #bf360c; margin: 5px 0 0 0;'>如果这不是您的操作，请立即更改密码并保护您的账户。</p>
        </div>

        <div style='border-top: 1px solid #eee; padding-top: 20px;'>
            <p style='font-size: 12px; font-weight: bold; color: #888; margin-bottom: 10px; text-transform: uppercase;'>交易信息</p>
            <table style='width: 100%; border-collapse: collapse; background-color: #fcfcfc; border-radius: 6px;'>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>日期</td>
                    <td style='padding: 8px 12px; font-size: 12px; color: #444; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px; border-bottom: 1px solid #f0f0f0;'>IP 地址</td>
                    <td style='padding: 8px 12px; font-size: 12px; font-family: monospace; color: #444; border-bottom: 1px solid #f0f0f0;'>{safeIp}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 12px; font-weight: 600; color: #666; font-size: 12px;'>设备</td>
                    <td style='padding: 8px 12px; font-size: 11px; color: #444;'>{safeDevice}</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 12px; color: #999; text-align: center; margin-top: 25px;'>
            此代码在 3 分钟内有效。代码只能使用一次。
        </p>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 10px; color: #bbb; margin: 0;'>
            这是一封自动发送的电子邮件，请勿回复。
            <br>追踪 ID: {details.TraceId}
        </p>
    </div>
</div>";

                await SendEmailAsync("security@efavori.com", use.ContactInformation.Email, $"{use.AccountPasswordResetMailCode} - 您的密码重置代码", emailBody, attachments);
            }
        }

        public async Task SendTaskStatuNewTasksMailEmail(string Culture, string Email,string Boss, string Categories, string TaskTitle)
        {
            var attachments = new List<Attachment>();
            var details = _userInfos.GetCurrentUserDetails();
            string displayDate = DateTime.Now.ToString("dd MMMM yyyy HH:mm");

            if (Culture == "tr")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #1a237e; font-size: 20px; font-weight: 600; margin-bottom: 20px; border-bottom: 2px solid #f0f0f0; padding-bottom: 10px; text-align: center;'><strong>$'{TaskTitle}'</strong> Görev Bildirimi</h2>
        
        <p style='font-size: 15px;'>Sayın Kullanıcımız,</p>
        <p style='font-size: 14px; color: #555;'>Size <strong>$'{Boss}'</strong> tarafından yeni bir görev atanmıştır. Kategori olarak <strong>$'{Categories}'</strong> belirlenen <strong>$'{TaskTitle}'</strong> başlıklı görev detaylarını kontrol etmek için lütfen sisteme giriş yapınız.</p>
        
        <table style='width: 100%; border-collapse: collapse; margin: 25px 0; background-color: #f8f9fa; border-radius: 6px;'>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #666; font-size: 13px; border-bottom: 1px solid #ffffff;'>Bildirim Tarihi</td>
                <td style='padding: 12px 15px; font-size: 13px; border-bottom: 1px solid #ffffff;'>{displayDate}</td>
            </tr>
        </table>

        <div style='background-color: #e8f5e9; border-left: 4px solid #4caf50; border-right: 4px solid #4caf50; padding: 20px; margin-top: 25px; text-align: center;'>
            <p style='margin: 0 0 15px 0; font-size: 13px; color: #2e7d32; line-height: 1.5;'>
                <strong>Görev detaylarını görüntülemek için sisteme giriş yapınız.</strong>
            </p>
            <hr style='border: 0; border-top: 1px solid rgba(76, 175, 80, 0.2); margin: 15px 0;' />
            <a href='https://efavori.com' style='display: inline-block; font-size: 14px; color: #2e7d32; text-decoration: none; font-weight: bold;'>
                efavori internet sitesi
            </a>
        </div>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 11px; color: #999; margin: 0;'>
            Bu e-posta, görev yönetim sistemi tarafından otomatik olarak gönderilmiştir.
        </p>
        <p style='font-size: 10px; color: #bbb; margin-top: 8px; letter-spacing: 0.5px;'>
            İşlem Kayıt No (Trace ID): {details.TraceId}
        </p>
    </div>
</div>";
                await SendEmailAsync("tasks@efavori.com", Email, "Yeni Görev Atandı", emailBody, attachments);
            }

            if (Culture == "en")
            {
                string emailBody = $@"
<div style='font-family: ""Inter"", -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e8e8e8; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 12px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 25px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Task Logo' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #1a237e; font-size: 22px; font-weight: 700; margin-bottom: 10px; text-align: center;'>New Task Assignment</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>You have been assigned a new task.</p>
        
        <p style='font-size: 15px; font-weight: 600;'>Hello,</p>
        <p style='font-size: 14px; color: #555;'>A new task has been assigned to you. Please log in to the system to view the task details.</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #fcfcfc; border: 1px solid #f0f0f0; border-radius: 8px;'>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #f0f0f0;'>Notification Date</td>
                <td style='padding: 14px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
            </tr>
        </table>

        <div style='background-color: #f1f8f4; border: 1px solid #c8e6c9; border-radius: 8px; padding: 20px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 15px 0; font-size: 14px; color: #2e7d32; font-weight: 500;'>
                <strong>Access the system to view your new task.</strong>
            </p>
            <a href='https://efavori.com' style='display: inline-block; padding: 10px 20px; background-color: #4caf50; color: #ffffff; text-decoration: none; font-weight: 600; font-size: 13px; border-radius: 5px;'>
                View Task
            </a>
        </div>
    </div>

    <div style='background-color: #f9fafb; padding: 25px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 11px; color: #999; margin: 0;'>
            This is an automated task notification. Please do not reply to this email.
        </p>
        <p style='font-size: 10px; color: #bbb; margin-top: 10px; letter-spacing: 0.5px; text-transform: uppercase;'>
            Trace ID: {details.TraceId}
        </p>
    </div>
</div>";
                await SendEmailAsync("tasks@efavori.com", Email, "New Task Assigned", emailBody, attachments);
            }

            if (Culture == "az")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 10px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 2px 10px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Task' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #1a237e; font-size: 20px; font-weight: 700; margin-bottom: 15px; border-bottom: 2px solid #f5f5f5; padding-bottom: 15px; text-align: center;'>Yeni Tapşırıq Bildirişi</h2>
        
        <p style='font-size: 15px; font-weight: 600;'>Hörmətli İstifadəçi,</p>
        <p style='font-size: 14px; color: #555;'>Sizə yeni bir tapşırıq təyin edilmişdir. Tapşırıq detallarını görmək üçün sistemə daxil olun.</p>
        
        <table style='width: 100%; border-collapse: collapse; margin: 25px 0; background-color: #fcfcfc; border: 1px solid #f0f0f0; border-radius: 8px;'>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #666; font-size: 13px; border-bottom: 1px solid #f0f0f0;'>Bildiriş Tarixi</td>
                <td style='padding: 12px 15px; font-size: 13px; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
            </tr>
        </table>

        <div style='background-color: #f1f8f4; border: 1px solid #c8e6c9; border-radius: 8px; padding: 20px; margin-top: 25px; text-align: center;'>
            <p style='margin: 0 0 12px 0; font-size: 14px; color: #2e7d32; font-weight: bold;'>
                Yeni tapşırığı görmək üçün sistemə daxil olun.
            </p>
            <a href='https://efavori.com' style='display: inline-block; background-color: #4caf50; color: #ffffff; padding: 10px 20px; text-decoration: none; font-size: 13px; font-weight: bold; border-radius: 4px;'>
                Tapşırığa bax
            </a>
        </div>
    </div>

    <div style='background-color: #f8f9fa; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 11px; color: #999; margin: 0;'>
            Bu e-poçt tapşırıq idarəetmə sistemi tərəfindən avtomatik göndərilib.
        </p>
        <p style='font-size: 10px; color: #bbb; margin-top: 8px;'>
            Əməliyyat ID (Trace ID): {details.TraceId}
        </p>
    </div>
</div>";
                await SendEmailAsync("tasks@efavori.com", Email, "Yeni Tapşırıq Təyin Edildi", emailBody, attachments);
            }

            if (Culture == "de")
            {
                string emailBody = $@"
<div style='font-family: ""Helvetica Neue"", Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #dddddd; padding: 0; border-radius: 10px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 5px 15px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Aufgabe' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #0d47a1; font-size: 20px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Neue Aufgabenzuweisung</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>Ihnen wurde eine neue Aufgabe zugewiesen.</p>
        
        <p style='font-size: 15px;'>Guten Tag,</p>
        <p style='font-size: 14px; color: #555;'>Ihnen wurde eine neue Aufgabe zugewiesen. Bitte melden Sie sich im System an, um die Aufgabendetails anzuzeigen.</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #fafafa; border: 1px solid #eeeeee; border-radius: 8px;'>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #eeeeee;'>Benachrichtigungsdatum</td>
                <td style='padding: 12px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #eeeeee;'>{displayDate}</td>
            </tr>
        </table>

        <div style='background-color: #f1f8f4; border: 1px solid #c8e6c9; border-radius: 8px; padding: 20px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 14px; color: #2e7d32; font-weight: bold;'>
                Melden Sie sich an, um Ihre neue Aufgabe anzuzeigen.
            </p>
            <a href='https://efavori.com' style='display: inline-block; padding: 12px 24px; background-color: #4caf50; color: #ffffff; text-decoration: none; font-weight: bold; font-size: 13px; border-radius: 6px;'>
                Aufgabe anzeigen
            </a>
        </div>
    </div>

    <div style='background-color: #f7f9fc; padding: 25px 40px; text-align: center; border-top: 1px solid #eeeeee;'>
        <p style='font-size: 11px; color: #a0aec0; margin: 0;'>
            Dies ist eine automatisch generierte Aufgabenbenachrichtigung. Bitte antworten Sie nicht auf diese E-Mail.
        </p>
        <p style='font-size: 10px; color: #cbd5e0; margin-top: 10px; letter-spacing: 0.5px;'>
            Trace-ID: {details.TraceId}
        </p>
    </div>
</div>";
                await SendEmailAsync("tasks@efavori.com", Email, "Neue Aufgabe zugewiesen", emailBody, attachments);
            }

            if (Culture == "es")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.03);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Tarea' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #1a237e; font-size: 22px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Nueva Asignación de Tarea</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>Se le ha asignado una nueva tarea.</p>
        
        <p style='font-size: 15px;'>Estimado usuario/a,</p>
        <p style='font-size: 14px; color: #555;'>Se le ha asignado una nueva tarea. Por favor, inicie sesión en el sistema para ver los detalles de la tarea.</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #f9f9f9; border: 1px solid #eeeeee; border-radius: 8px;'>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #eeeeee;'>Fecha de Notificación</td>
                <td style='padding: 14px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #eeeeee;'>{displayDate}</td>
            </tr>
        </table>

        <div style='background-color: #f1f8f4; border: 1px solid #c8e6c9; border-radius: 8px; padding: 25px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 15px; color: #2e7d32; font-weight: bold;'>
                Acceda al sistema para ver su nueva tarea.
            </p>
            <a href='https://efavori.com' style='display: inline-block; padding: 12px 25px; background-color: #4caf50; color: #ffffff; text-decoration: none; font-weight: bold; font-size: 13px; border-radius: 6px;'>
                Ver tarea
            </a>
        </div>
    </div>

    <div style='background-color: #f4f5f7; padding: 25px 40px; text-align: center; border-top: 1px solid #eeeeee;'>
        <p style='font-size: 11px; color: #9aa4af; margin: 0;'>
            Esta es una notificación automática de tareas. Por favor, no responda a este correo.
        </p>
        <p style='font-size: 10px; color: #bcccdc; margin-top: 10px; letter-spacing: 0.5px;'>
            ID de Seguimiento: {details.TraceId}
        </p>
    </div>
</div>";
                await SendEmailAsync("tasks@efavori.com", Email, "Nueva tarea asignada", emailBody, attachments);
            }

            if (Culture == "fr")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Tâche' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #1a237e; font-size: 20px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Nouvelle Attribution de Tâche</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>Une nouvelle tâche vous a été assignée.</p>
        
        <p style='font-size: 15px;'>Bonjour,</p>
        <p style='font-size: 14px; color: #555;'>Une nouvelle tâche vous a été assignée. Veuillez vous connecter au système pour consulter les détails de la tâche.</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #fcfcfc; border: 1px solid #f0f0f0; border-radius: 8px;'>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #f0f0f0;'>Date de notification</td>
                <td style='padding: 12px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #f0f0f0;'>{displayDate}</td>
            </tr>
        </table>

        <div style='background-color: #f1f8f4; border: 1px solid #c8e6c9; border-radius: 10px; padding: 25px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 15px; color: #2e7d32; font-weight: bold;'>
                Connectez-vous pour voir votre nouvelle tâche.
            </p>
            <a href='https://efavori.com' style='display: inline-block; padding: 12px 25px; background-color: #4caf50; color: #ffffff; text-decoration: none; font-weight: bold; font-size: 13px; border-radius: 6px;'>
                Voir la tâche
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
                await SendEmailAsync("tasks@efavori.com", Email, "Nouvelle tâche assignée", emailBody, attachments);
            }

            if (Culture == "hi")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.05);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Task' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #1a237e; font-size: 22px; font-weight: 700; margin-bottom: 10px; text-align: center;'>नया कार्य असाइनमेंट</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>आपको एक नया कार्य सौंपा गया है।</p>
        
        <p style='font-size: 15px;'>नमस्ते,</p>
        <p style='font-size: 14px; color: #555;'>आपको एक नया कार्य सौंपा गया है। कार्य विवरण देखने के लिए कृपया सिस्टम में लॉगिन करें।</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #f9f9f9; border: 1px solid #eeeeee; border-radius: 8px;'>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #eeeeee;'>सूचना दिनांक</td>
                <td style='padding: 12px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #eeeeee;'>{displayDate}</td>
            </tr>
        </table>

        <div style='background-color: #f1f8f4; border: 1px solid #c8e6c9; border-radius: 10px; padding: 25px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 16px; color: #2e7d32; font-weight: bold;'>
                अपना नया कार्य देखने के लिए लॉगिन करें।
            </p>
            <a href='https://efavori.com' style='display: inline-block; padding: 12px 25px; background-color: #4caf50; color: #ffffff; text-decoration: none; font-weight: bold; font-size: 14px; border-radius: 6px;'>
                कार्य देखें
            </a>
        </div>
    </div>

    <div style='background-color: #f4f5f7; padding: 25px 40px; text-align: center; border-top: 1px solid #eeeeee;'>
        <p style='font-size: 11px; color: #9aa4af; margin: 0;'>
            यह एक स्वचालित कार्य सूचना है। कृपया इस ईमेल का उत्तर न दें।
        </p>
        <p style='font-size: 10px; color: #bcccdc; margin-top: 10px; letter-spacing: 0.5px;'>
            ट्रेस आईडी (Trace ID): {details.TraceId}
        </p>
    </div>
</div>";
                await SendEmailAsync("tasks@efavori.com", Email, "नया कार्य असाइन किया गया", emailBody, attachments);
            }

            if (Culture == "pt")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.03);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Tarefa' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #1a237e; font-size: 22px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Nova Atribuição de Tarefa</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>Uma nova tarefa foi atribuída a você.</p>
        
        <p style='font-size: 15px;'>Olá,</p>
        <p style='font-size: 14px; color: #555;'>Uma nova tarefa foi atribuída a você. Por favor, faça login no sistema para visualizar os detalhes da tarefa.</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #f9f9f9; border: 1px solid #eeeeee; border-radius: 8px;'>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #eeeeee;'>Data de Notificação</td>
                <td style='padding: 14px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #eeeeee;'>{displayDate}</td>
            </tr>
        </table>

        <div style='background-color: #f1f8f4; border: 1px solid #c8e6c9; border-radius: 10px; padding: 25px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 15px; color: #2e7d32; font-weight: bold;'>
                Acesse o sistema para ver sua nova tarefa.
            </p>
            <a href='https://efavori.com' style='display: inline-block; padding: 12px 25px; background-color: #4caf50; color: #ffffff; text-decoration: none; font-weight: bold; font-size: 13px; border-radius: 6px;'>
                Ver tarefa
            </a>
        </div>
    </div>

    <div style='background-color: #f4f5f7; padding: 25px 40px; text-align: center; border-top: 1px solid #eeeeee;'>
        <p style='font-size: 11px; color: #9aa4af; margin: 0;'>
            Esta é uma notificação automática de tarefas. Por favor, não responda a este e-mail.
        </p>
        <p style='font-size: 10px; color: #bcccdc; margin-top: 10px; letter-spacing: 0.5px;'>
            ID de Rastreamento: {details.TraceId}
        </p>
    </div>
</div>";
                await SendEmailAsync("tasks@efavori.com", Email, "Nova tarefa atribuída", emailBody, attachments);
            }

            if (Culture == "ru")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.03);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Задача' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #1a237e; font-size: 20px; font-weight: 700; margin-bottom: 10px; text-align: center;'>Назначение новой задачи</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>Вам назначена новая задача.</p>
        
        <p style='font-size: 15px;'>Здравствуйте,</p>
        <p style='font-size: 14px; color: #555;'>Вам назначена новая задача. Пожалуйста, войдите в систему, чтобы просмотреть детали задачи.</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #f9f9f9; border: 1px solid #eeeeee; border-radius: 8px;'>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #eeeeee;'>Дата уведомления</td>
                <td style='padding: 12px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #eeeeee;'>{displayDate}</td>
            </tr>
        </table>

        <div style='background-color: #f1f8f4; border: 1px solid #c8e6c9; border-radius: 10px; padding: 25px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 15px; color: #2e7d32; font-weight: bold;'>
                Войдите в систему, чтобы увидеть вашу новую задачу.
            </p>
            <a href='https://efavori.com' style='display: inline-block; padding: 12px 25px; background-color: #4caf50; color: #ffffff; text-decoration: none; font-weight: bold; font-size: 13px; border-radius: 6px;'>
                Посмотреть задачу
            </a>
        </div>
    </div>

    <div style='background-color: #f4f5f7; padding: 25px 40px; text-align: center; border-top: 1px solid #eeeeee;'>
        <p style='font-size: 11px; color: #9aa4af; margin: 0;'>
            Это автоматическое уведомление о задачах. Пожалуйста, не отвечайте на это письмо.
        </p>
        <p style='font-size: 10px; color: #bcccdc; margin-top: 10px; letter-spacing: 0.5px;'>
            ID транзакции (Trace ID): {details.TraceId}
        </p>
    </div>
</div>";
                await SendEmailAsync("tasks@efavori.com", Email, "Назначена новая задача", emailBody, attachments);
            }

            if (Culture == "zh")
            {
                string emailBody = $@"
<div style='font-family: ""PingFang SC"", ""Microsoft YaHei"", ""Helvetica Neue"", Helvetica, Arial, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 20px auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 10px rgba(0,0,0,0.03);'>
    
    <div style='padding: 35px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='任务通知' style='max-width: 100px; height: auto; display: inline-block;' />
    </div>

    <div style='padding: 0 40px 35px 40px;'>
        <h2 style='color: #1a237e; font-size: 22px; font-weight: 700; margin-bottom: 10px; text-align: center;'>新任务分配通知</h2>
        <p style='font-size: 14px; color: #666; text-align: center; margin-bottom: 25px;'>您收到了一个新的任务分配。</p>
        
        <p style='font-size: 15px;'>尊敬的用户，您好：</p>
        <p style='font-size: 14px; color: #555;'>您收到了一个新的任务分配。请登录系统查看任务详情。</p>
        
        <table style='width: 100%; border-collapse: separate; border-spacing: 0; margin: 25px 0; background-color: #f9f9f9; border: 1px solid #eeeeee; border-radius: 8px;'>
            <tr>
                <td style='padding: 14px 15px; font-weight: 600; color: #777; font-size: 13px; border-bottom: 1px solid #eeeeee;'>通知日期</td>
                <td style='padding: 14px 15px; font-size: 13px; color: #333; border-bottom: 1px solid #eeeeee;'>{displayDate}</td>
            </tr>
        </table>

        <div style='background-color: #f1f8f4; border: 1px solid #c8e6c9; border-radius: 10px; padding: 25px; margin-top: 30px; text-align: center;'>
            <p style='margin: 0 0 10px 0; font-size: 15px; color: #2e7d32; font-weight: bold;'>
                请登录系统查看您的新任务。
            </p>
            <a href='https://efavori.com' style='display: inline-block; padding: 12px 25px; background-color: #4caf50; color: #ffffff; text-decoration: none; font-weight: bold; font-size: 14px; border-radius: 6px;'>
                查看任务
            </a>
        </div>
    </div>

    <div style='background-color: #f4f5f7; padding: 25px 40px; text-align: center; border-top: 1px solid #eeeeee;'>
        <p style='font-size: 11px; color: #9aa4af; margin: 0;'>
            这是系统生成的自动任务通知，请勿直接回复。
        </p>
        <p style='font-size: 10px; color: #bcccdc; margin-top: 10px; letter-spacing: 0.5px;'>
            追踪 ID (Trace ID): {details.TraceId}
        </p>
    </div>
</div>";
                await SendEmailAsync("tasks@efavori.com", Email, "新任务已分配", emailBody, attachments);
            }
        }
        public async Task SendTaskStatuToBeDoneMailEmail(string Culture, string Email, string TaskTitle, string Employee)
        {
            var attachments = new List<Attachment>();
            var details = _userInfos.GetCurrentUserDetails();
            string displayDate = DateTime.Now.ToString("dd MMMM yyyy HH:mm");

            if (Culture == "tr")
            {
                string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 8px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff;'>
    <div style='padding: 30px 30px 20px 30px; text-align: center;'>
        <img src='https://efavori.com/_files/main/logo/logo.png' alt='Logo' style='max-width: 120px; height: auto; display: inline-block;' />
    </div>
    <div style='padding: 0 40px 30px 40px;'>
        <h2 style='color: #1a237e; font-size: 20px; font-weight: 600; margin-bottom: 20px; border-bottom: 2px solid #f0f0f0; padding-bottom: 10px; text-align: center;'>Görev İşleme Alındı</h2>
        <p style='font-size: 15px;'>Sayın Kullanıcımız,</p>
        <p style='font-size: 14px; color: #555;'>Oluşturduğunuz <strong>$'{TaskTitle}'</strong> görevi, ilgili sorumlu tarafından <strong>işleme alınmıştır</strong>. İlgili personelin çalışma süreci başlatılmıştır.</p>
        <table style='width: 100%; border-collapse: collapse; margin: 25px 0; background-color: #f8f9fa; border-radius: 6px;'>
            <tr>
                <td style='padding: 12px 15px; font-weight: 600; color: #666; font-size: 13px; border-bottom: 1px solid #ffffff;'>İşlem Tarihi</td>
                <td style='padding: 12px 15px; font-size: 13px; border-bottom: 1px solid #ffffff;'>{displayDate}</td>
            </tr>
        </table>
        <div style='background-color: #e3f2fd; border-left: 4px solid #2196f3; padding: 20px; margin-top: 25px; text-align: center;'>
            <p style='margin: 0 0 15px 0; font-size: 13px; color: #0d47a1; line-height: 1.5;'>
                <strong>Güncel durumu takip etmek için sisteme giriş yapabilirsiniz.</strong>
            </p>
            <a href='https://efavori.com' style='display: inline-block; font-size: 14px; color: #0d47a1; text-decoration: none; font-weight: bold;'>efavori internet sitesi</a>
        </div>
    </div>
    <div style='background-color: #f4f4f4; padding: 20px 40px; text-align: center; border-top: 1px solid #eee;'>
        <p style='font-size: 11px; color: #999; margin: 0;'>Bu e-posta otomatik olarak gönderilmiştir. Lütfen yanıtlamayınız.</p>
        <p style='font-size: 10px; color: #bbb; margin-top: 8px;'>Trace ID: {details.TraceId}</p>
    </div>
</div>";
                await SendEmailAsync("tasks@efavori.com", Email, $"{TaskTitle} Görevi İşleme Alındı", emailBody, attachments);
            }
        }
        public async Task SendTaskStatuInProcessMailEmail(string Culture, string Email, string TaskTitle, string Employee)
        {

        }
        public async Task SendTaskStatuInEditingMailEmail(string Culture, string Email, string TaskTitle, string Employee)
        {

        }
        public async Task SendTaskStatuCompletedMailEmail(string Culture, string Email, string TaskTitle, string Employee)
        {

        }
    }
}