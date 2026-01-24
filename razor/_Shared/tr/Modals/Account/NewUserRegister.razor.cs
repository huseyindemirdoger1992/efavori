using api;
using data;
using data._Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace razor._Shared.tr.Modals.Account
{
    public partial class NewUserRegister : ComponentBase
    {
        [Inject] public IDbContextFactory<_ApplicationConnectionDb> DbFactory { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public UserInfos UserInfos { get; set; }

        private readonly TakeLogs _logger;


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
                        title: "CheckSponsorAsync",
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

                db.Users.Add(_user);
                await db.SaveChangesAsync();

                // Başarılı işlem bildirimi
                await ShowNotification("success", "İşlem Tamamlandı", "Kullanıcı kaydı başarıyla oluşturuldu.", null);



                var userDetail = UserInfos.GetCurrentUserDetails();
                var log = new Logs
                {
                    UserId = _user?.Id ?? null,
                    Title = "UserRegister",
                    Action = "NewUserCreated",
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

            }
            catch (Exception ex)
            {
                // Kullanıcı ve istek detaylarını al
                var userDetail = UserInfos.GetCurrentUserDetails();

                // Hata detaylarını logla
                var log = new Logs
                {
                    UserId = _user?.Id ?? null,
                    Title = "UserRegister",
                    Action = "NewUserCreated",
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