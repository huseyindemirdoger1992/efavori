using api;
using data;
using data._Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace razor._Shared.tr.Modals.Account
{
    public partial class NewUserRegister : ComponentBase
    {
        [Inject] public IDbContextFactory<_ApplicationConnectionDb> DbFactory { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public UserInfos UserInfos { get; set; }
        [Inject] protected NavigationManager Navigation { get; init; } = default!;
        [Inject] protected IJSRuntime JS { get; init; } = default!;

        private readonly EmailSender emailSender;

        private readonly TakeLogs _logger;

        public NewUserRegister(EmailSender _emailSender)
        {
            emailSender = _emailSender;
        }

        private Users _user = new Users { };

        protected List<Country> _Countries = new();
        protected List<States> _States = new();
        protected List<Cities> _Cities = new();

        private Notification? notificationRef;
        private async Task ShowNotification(string type, string title, string text, string? image)
        {
            var imageUrl = string.IsNullOrWhiteSpace(image) ? "https://picsum.photos/120?" : image;
            if (notificationRef != null)
                await notificationRef.Launch(type, title, text, imageUrl);
        }

        private async Task CheckSponsorAsync()
        {
            var userDetail = UserInfos.GetCurrentUserDetails();

            try
            {
                _SponsorFullName = null;
                if (!string.IsNullOrWhiteSpace(_Sponsored))
                {
                    using var db = await DbFactory.CreateDbContextAsync();
                    var sponsor = await db.Users
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.ContactInformation != null && u.ContactInformation.Email == _Sponsored);

                    if (sponsor != null)
                    {
                        TextualFunctions tf = new TextualFunctions();
                        _SponsorFullName = $"{tf.FirstLastLetter(sponsor.FirstName)} {tf.FirstLastLetter(sponsor.LastName)}";
                        StateHasChanged();
                    }
                }
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                try
                {
                    await _logger.TakeIt(
                        userId: null,
                        PageNameSpaceTitle: "CheckSponsorAsync",
                        action: $"Take Sponsor",
                        exception: ex.Message,
                        stackTrace: ex.StackTrace
                    );
                }
                catch
                {
                }
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            await CheckSponsorAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            using var db = await DbFactory.CreateDbContextAsync();
            _Countries = await db.Country.AsNoTracking().OrderBy(c => c.name).ToListAsync();
        }

        protected async Task OnCountrySelected(ChangeEventArgs e)
        {
            int.TryParse(e.Value?.ToString(), out int countryId);
            _States.Clear();
            _Cities.Clear();

            if (countryId > 0)
            {
                using var db = await DbFactory.CreateDbContextAsync();
                _States = await db.States.AsNoTracking()
                    .Where(s => s.country_id == countryId)
                    .OrderBy(s => s.name).ToListAsync();
            }
            StateHasChanged();
        }

        protected async Task OnStateSelected(ChangeEventArgs e)
        {
            int.TryParse(e.Value?.ToString(), out int stateId);
            _Cities.Clear();

            if (stateId > 0)
            {
                using var db = await DbFactory.CreateDbContextAsync();
                _Cities = await db.Cities.AsNoTracking()
                    .Where(c => c.state_id == stateId)
                    .OrderBy(c => c.name).ToListAsync();
            }
            StateHasChanged();
        }

        private string? _Sponsored = string.Empty;
        private string? _SponsorFullName = null;
        private string? _SelectedPhoneCode = string.Empty;
        private string? _UserEmail = string.Empty;
        private string? _UserPhoneNumber = string.Empty;
        public async Task UserSave()
        {
            // --- 1. BOŞ ALAN KONTROLLERİ ---

            // E-posta Kontrolü
            if (string.IsNullOrWhiteSpace(_UserEmail))
            {
                await ShowNotification("warning", "E-posta Gerekli", "Lütfen devam etmek için e-posta adresinizi yazın.", null);
                return;
            }

            if (!_UserEmail.Contains("@") || !_UserEmail.Contains("."))
            {
                await ShowNotification("error", "Geçersiz Format", "Girdiğiniz e-posta adresi hatalı görünüyor. Lütfen kontrol edin.", null);
                return;
            }

            // Telefon Kontrolü
            if (string.IsNullOrWhiteSpace(_UserPhoneNumber))
            {
                await ShowNotification("warning", "Telefon Gerekli", "Telefon numaranızı girmelisiniz.", null);
                return;
            }

            // Kişisel Bilgiler
            if (string.IsNullOrWhiteSpace(_user.FirstName))
            {
                await ShowNotification("warning", "Ad Alanı Boş", "Lütfen adınızı belirtiniz.", null);
                return;
            }

            if (string.IsNullOrWhiteSpace(_user.LastName))
            {
                await ShowNotification("warning", "Soyad Alanı Boş", "Lütfen soyadınızı belirtiniz.", null);
                return;
            }

            if (_user.DateOfBirth == null)
            {
                await ShowNotification("warning", "Tarih Seçilmedi", "Doğum tarihinizi takvimden seçmeniz gerekiyor.", null);
                return;
            }

            // Tercihler
            if (string.IsNullOrWhiteSpace(_user.Language))
            {
                await ShowNotification("info", "Dil Seçimi", "Lütfen tercih ettiğiniz iletişim dilini seçin.", null);
                return;
            }

            if (string.IsNullOrWhiteSpace(_user.Currency))
            {
                await ShowNotification("info", "Para Birimi", "İşlemleriniz için bir para birimi belirlemelisiniz.", null);
                return;
            }

            // Hesap Detayları
            if (string.IsNullOrWhiteSpace(_user.UsersType))
            {
                await ShowNotification("info", "Kullanıcı Tipi", "Lütfen hesap türünüzü (Müşteri/Satıcı vb.) seçin.", null);
                return;
            }

            if (string.IsNullOrWhiteSpace(_user.Password))
            {
                await ShowNotification("error", "Şifre Belirlenmedi", "Güvenliğiniz için bir kullanıcı şifresi oluşturmalısınız.", null);
                return;
            }

            // --- 2. VERİ HAZIRLAMA VE ÖN KONTROLLER ---

            TextualFunctions tf = new TextualFunctions();
            var normalizedPhone = tf.NormalizePhoneNumberEditor(_UserPhoneNumber);

            // Sponsor ve kullanıcı e-postası aynı mı?
            if (!string.IsNullOrWhiteSpace(_Sponsored) && _UserEmail.Trim().ToLower() == _Sponsored.Trim().ToLower())
            {
                await ShowNotification("error", "Kayıt Hatası", "Kullanıcı e-posta adresi ile sponsor e-posta adresi aynı olamaz.", null);
                return;
            }

            try
            {
                using var db = await DbFactory.CreateDbContextAsync();

                // --- 3. VERİTABANI ÇAKIŞMA KONTROLÜ ---

                // Hem e-posta hem de telefon için benzersizlik kontrolü
                var existingUser = await db.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u =>
                        u.ContactInformation != null &&
                        (u.ContactInformation.Email.ToLower() == _UserEmail.ToLower() ||
                         u.ContactInformation.PhoneNumber == normalizedPhone));

                if (existingUser != null)
                {
                    // Hangi verinin çakıştığını kullanıcıya daha net belirtmek isterseniz:
                    string duplicateType = existingUser.ContactInformation.Email.ToLower() == _UserEmail.ToLower()
                        ? "E-posta adresi"
                        : "Telefon numarası";

                    await ShowNotification("error", "Mükerrer Kayıt", $"Bu {duplicateType} zaten sistemde kayıtlı.", null);
                    return;
                }

                // --- 4. KAYIT İŞLEMİ ---

                _user.ContactInformation = new ContactInformation
                {
                    CountryPhoneCode = _SelectedPhoneCode,
                    PhoneNumber = normalizedPhone,
                    Email = _UserEmail.Trim().ToLower()
                };

                _user.UserSponsorEmail = _Sponsored?.Trim().ToLower();
                _user.HeaderMenuType = _user.UsersType;
                _user.IsActive = true;
                _user.RegistrationDate = DateTime.UtcNow;
                _user.AccountActivationMailDeadline = DateTime.UtcNow.AddDays(3);
                _user.AccountActivationMailStatu = false;

                var attachments = new List<Attachment>();

                var details = UserInfos.GetCurrentUserDetails();
                string safeIp = System.Net.WebUtility.HtmlEncode(details.IpAddress);
                string safeDevice = System.Net.WebUtility.HtmlEncode(details.UserAgent);
                string displayDate = DateTime.UtcNow.ToString("dd MMMM yyyy HH:mm");

                #region Kayıt Emaili Gönderir
                if (_user.Language == "tr")
                {
                    string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
        
        <div style='padding: 40px 30px 20px 30px; text-align: center; background: linear-gradient(to bottom, #f8f9ff, #ffffff);'>
            <img src='https://1drv.ms/i/c/ce20faaddbdfba2e/IQRDszi9BOX5R5T7a1eLE650ASZlWwgS5x-PiZHVWp1UzD4?width=180&height=180' alt='Logo' style='max-width: 130px; height: auto; display: inline-block;' />
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
                Trace ID: {details.TraceId} | © 2026 efavori
            </p>
        </div>
    </div>";

                    await emailSender.SendEmailAsync(_UserEmail, "efavori'ye Hoş Geldiniz!", emailBody, attachments);
                }
                if (_user.Language == "en")
                {
                    string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
        
        <div style='padding: 40px 30px 20px 30px; text-align: center; background: linear-gradient(to bottom, #f8f9ff, #ffffff);'>
            <img src='https://1drv.ms/i/c/ce20faaddbdfba2e/IQRDszi9BOX5R5T7a1eLE650ASZlWwgS5x-PiZHVWp1UzD4?width=180&height=180' alt='Logo' style='max-width: 130px; height: auto; display: inline-block;' />
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

                    await emailSender.SendEmailAsync(_UserEmail, "Welcome to efavori!", emailBody, attachments);
                }
                if (_user.Language == "az")
                {
                    string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
        
        <div style='padding: 40px 30px 20px 30px; text-align: center; background: linear-gradient(to bottom, #f8f9ff, #ffffff);'>
            <img src='https://1drv.ms/i/c/ce20faaddbdfba2e/IQRDszi9BOX5R5T7a1eLE650ASZlWwgS5x-PiZHVWp1UzD4?width=180&height=180' alt='Logo' style='max-width: 130px; height: auto; display: inline-block;' />
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

                    await emailSender.SendEmailAsync(_UserEmail, "efavori-yə Xoş Gəldiniz!", emailBody, attachments);
                }
                if (_user.Language == "de")
                {
                    string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
        
        <div style='padding: 40px 30px 20px 30px; text-align: center; background: linear-gradient(to bottom, #f8f9ff, #ffffff);'>
            <img src='https://1drv.ms/i/c/ce20faaddbdfba2e/IQRDszi9BOX5R5T7a1eLE650ASZlWwgS5x-PiZHVWp1UzD4?width=180&height=180' alt='Logo' style='max-width: 130px; height: auto; display: inline-block;' />
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

                    await emailSender.SendEmailAsync(_UserEmail, "Willkommen bei efavori!", emailBody, attachments);
                }
                if (_user.Language == "es")
                {
                    string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
        
        <div style='padding: 40px 30px 20px 30px; text-align: center; background: linear-gradient(to bottom, #f8f9ff, #ffffff);'>
            <img src='https://1drv.ms/i/c/ce20faaddbdfba2e/IQRDszi9BOX5R5T7a1eLE650ASZlWwgS5x-PiZHVWp1UzD4?width=180&height=180' alt='Logo' style='max-width: 130px; height: auto; display: inline-block;' />
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

                    await emailSender.SendEmailAsync(_UserEmail, "¡Bienvenido a efavori!", emailBody, attachments);
                }
                if (_user.Language == "fr")
                {
                    string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
        
        <div style='padding: 40px 30px 20px 30px; text-align: center; background: linear-gradient(to bottom, #f8f9ff, #ffffff);'>
            <img src='https://1drv.ms/i/c/ce20faaddbdfba2e/IQRDszi9BOX5R5T7a1eLE650ASZlWwgS5x-PiZHVWp1UzD4?width=180&height=180' alt='Logo' style='max-width: 130px; height: auto; display: inline-block;' />
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

                    await emailSender.SendEmailAsync(_UserEmail, "Bienvenue chez efavori !", emailBody, attachments);
                }
                if (_user.Language == "hi")
                {
                    string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
        
        <div style='padding: 40px 30px 20px 30px; text-align: center; background: linear-gradient(to bottom, #f8f9ff, #ffffff);'>
            <img src='https://1drv.ms/i/c/ce20faaddbdfba2e/IQRDszi9BOX5R5T7a1eLE650ASZlWwgS5x-PiZHVWp1UzD4?width=180&height=180' alt='Logo' style='max-width: 130px; height: auto; display: inline-block;' />
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

                    await emailSender.SendEmailAsync(_UserEmail, "efavori में आपका स्वागत है!", emailBody, attachments);
                }
                if (_user.Language == "pt")
                {
                    string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
        
        <div style='padding: 40px 30px 20px 30px; text-align: center; background: linear-gradient(to bottom, #f8f9ff, #ffffff);'>
            <img src='https://1drv.ms/i/c/ce20faaddbdfba2e/IQRDszi9BOX5R5T7a1eLE650ASZlWwgS5x-PiZHVWp1UzD4?width=180&height=180' alt='Logo' style='max-width: 130px; height: auto; display: inline-block;' />
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

                    await emailSender.SendEmailAsync(_UserEmail, "Bem-vindo à efavori!", emailBody, attachments);
                }
                if (_user.Language == "ru")
                {
                    string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
        
        <div style='padding: 40px 30px 20px 30px; text-align: center; background: linear-gradient(to bottom, #f8f9ff, #ffffff);'>
            <img src='https://1drv.ms/i/c/ce20faaddbdfba2e/IQRDszi9BOX5R5T7a1eLE650ASZlWwgS5x-PiZHVWp1UzD4?width=180&height=180' alt='Logo' style='max-width: 130px; height: auto; display: inline-block;' />
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

                    await emailSender.SendEmailAsync(_UserEmail, "Добро пожаловать в efavori!", emailBody, attachments);
                }
                if (_user.Language == "zh")
                {
                    string emailBody = $@"
    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; border: 1px solid #e0e0e0; padding: 0; border-radius: 12px; max-width: 600px; margin: 0 auto; overflow: hidden; background-color: #ffffff; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
        
        <div style='padding: 40px 30px 20px 30px; text-align: center; background: linear-gradient(to bottom, #f8f9ff, #ffffff);'>
            <img src='https://1drv.ms/i/c/ce20faaddbdfba2e/IQRDszi9BOX5R5T7a1eLE650ASZlWwgS5x-PiZHVWp1UzD4?width=180&height=180' alt='Logo' style='max-width: 130px; height: auto; display: inline-block;' />
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

                    await emailSender.SendEmailAsync(_UserEmail, "欢迎来到 efavori！", emailBody, attachments);
                }
                #endregion

                db.Users.Add(_user);
                await db.SaveChangesAsync();

                // Başarılı işlem bildirimi
                await ShowNotification("success", "İşlem Tamamlandı", "Kullanıcı kaydı başarıyla oluşturuldu.", null);



                var userDetail = UserInfos.GetCurrentUserDetails();
                var log = new Logs
                {
                    UserId = _user?.Id ?? null,
                    PageNameSpaceTitle = "namespace razor._Shared.tr.Modals.Account",
                    Action = "UserSave",
                    IpAddress = userDetail.IpAddress,
                    UserAgent = userDetail.UserAgent,
                    RequestPath = userDetail.RequestPath,
                    Languages = userDetail.Languages,
                    Exception = null,
                    StackTrace = null,
                    Date = DateTime.UtcNow
                };
                db.Logs.Add(log);
                await db.SaveChangesAsync();

                await Task.Delay(3000);
                Navigation.NavigateTo(Navigation.Uri, forceLoad: true);

            }
            catch (Exception ex)
            {
                // Kullanıcı ve istek detaylarını al
                var userDetail = UserInfos.GetCurrentUserDetails();

                // Hata detaylarını logla
                var log = new Logs
                {
                    UserId = _user?.Id ?? null,
                    PageNameSpaceTitle = "namespace razor._Shared.tr.Modals.Account",
                    Action = "UserSave",
                    IpAddress = userDetail.IpAddress,
                    UserAgent = userDetail.UserAgent,
                    RequestPath = userDetail.RequestPath,
                    Languages = userDetail.Languages,
                    Exception = ex.Message,
                    StackTrace = ex.StackTrace,
                    Date = DateTime.UtcNow
                };

                // Yeni bir context ile log kaydını ekle (db context dispose edilmiş olabilir)
                using (var dbLog = await DbFactory.CreateDbContextAsync())
                {
                    dbLog.Logs.Add(log);
                    await dbLog.SaveChangesAsync();
                }

                // Kullanıcıya hata bildirimi göster
                await ShowNotification("error", "Sistem Hatası", "Kayıt sırasında teknik bir hata oluştu. Lütfen tekrar deneyiniz.", null);
            }
        }
    }
}