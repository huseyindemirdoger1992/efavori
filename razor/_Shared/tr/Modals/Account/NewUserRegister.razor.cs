using api;
using api.tr;
using data;
using data._Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Net;
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

        private string _maxDate = DateTime.Now.AddYears(-18).ToString("yyyy-MM-dd");

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

        private bool Btn_isProcessing_01 = false;

        public async Task UserSave()
        {
            if (Btn_isProcessing_01) return;
            Btn_isProcessing_01 = true;


            // E-posta Kontrolü
            if (string.IsNullOrWhiteSpace(_UserEmail))
            {
                await ShowNotification("warning", "E-posta Gerekli", "Lütfen devam etmek için e-posta adresinizi yazın.", null);
                await Task.Delay(4000);
                Btn_isProcessing_01 = false;
                return;
            }

            if (!_UserEmail.Contains("@") || !_UserEmail.Contains("."))
            {
                await ShowNotification("danger", "Geçersiz Format", "Girdiğiniz e-posta adresi hatalı görünüyor. Lütfen kontrol edin.", null);
                await Task.Delay(4000);
                Btn_isProcessing_01 = false;
                return;
            }

            // Telefon Kontrolü
            if (string.IsNullOrWhiteSpace(_UserPhoneNumber))
            {
                await ShowNotification("warning", "Telefon Gerekli", "Telefon numaranızı girmelisiniz.", null);
                await Task.Delay(4000);
                Btn_isProcessing_01 = false;
                return;
            }

            // Kişisel Bilgiler
            if (string.IsNullOrWhiteSpace(_user.FirstName))
            {
                await ShowNotification("warning", "Ad Alanı Boş", "Lütfen adınızı belirtiniz.", null);
                await Task.Delay(4000);
                Btn_isProcessing_01 = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(_user.LastName))
            {
                await ShowNotification("warning", "Soyad Alanı Boş", "Lütfen soyadınızı belirtiniz.", null);
                await Task.Delay(4000);
                Btn_isProcessing_01 = false;
                return;
            }

            if (_user.DateOfBirth == null)
            {
                await ShowNotification("warning", "Tarih Seçilmedi", "Doğum tarihinizi takvimden seçmeniz gerekiyor.", null);
                await Task.Delay(4000);
                Btn_isProcessing_01 = false;
                return;
            }

            // Tercihler
            if (string.IsNullOrWhiteSpace(_user.Language))
            {
                await ShowNotification("info", "Dil Seçimi", "Lütfen tercih ettiğiniz iletişim dilini seçin.", null);
                await Task.Delay(4000);
                Btn_isProcessing_01 = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(_user.Currency))
            {
                await ShowNotification("info", "Para Birimi", "İşlemleriniz için bir para birimi belirlemelisiniz.", null);
                await Task.Delay(4000);
                Btn_isProcessing_01 = false;
                return;
            }

            // Hesap Detayları
            if (string.IsNullOrWhiteSpace(_user.UsersType))
            {
                await ShowNotification("info", "Kullanıcı Tipi", "Lütfen hesap türünüzü (Müşteri/Satıcı vb.) seçin.", null);
                await Task.Delay(4000);
                Btn_isProcessing_01 = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(_user.Password))
            {
                await ShowNotification("danger", "Şifre Belirlenmedi", "Güvenliğiniz için bir kullanıcı şifresi oluşturmalısınız.", null);
                await Task.Delay(4000);
                Btn_isProcessing_01 = false;
                return;
            }
            if (_user.DateOfBirth > DateTime.Now.AddYears(-18))
            {
                await ShowNotification("danger", "Yaş kısıtlaması", "Yaşınız 18'den küçük olduğu için kayıt yapamazsınız.", null);
                await Task.Delay(4000);
                Btn_isProcessing_01 = false;
                return;
            }

            var validationResult = PasswordValidator.ValidatePassword(_user.Password);

            if (!validationResult.IsValid)
            {
                await ShowNotification("danger", "Şifre Hataası", validationResult.Message, null);
                await Task.Delay(4000);
                Btn_isProcessing_01 = false;
                return;
            }

            // Şifre güç seviyesi
            int strength = PasswordValidator.CalculatePasswordStrength(_user.Password);

            // Çok zayıf şifre varsa uyar (opsiyonel)
            if (strength < 60)
            {
                await ShowNotification("warning", "Dikkat", $"Şifreniz zayıf gözüküyor.", null);
                await Task.Delay(4000);
                Btn_isProcessing_01 = false;
                return;
            }
            else
            {
                // --- 2. VERİ HAZIRLAMA VE ÖN KONTROLLER ---

                TextualFunctions tf = new TextualFunctions();
                var normalizedPhone = tf.NormalizePhoneNumberEditor(_UserPhoneNumber);

                // Sponsor ve kullanıcı e-postası aynı mı?
                if (!string.IsNullOrWhiteSpace(_Sponsored) && _UserEmail.Trim().ToLower() == _Sponsored.Trim().ToLower() && _user.UsersType == "Vendor")
                {
                    await ShowNotification("danger", "Kayıt Hatası", "Kullanıcı e-posta adresi ile sponsor e-posta adresi aynı olamaz.", null);
                    await Task.Delay(4000);
                    Btn_isProcessing_01 = false;
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

                        await ShowNotification("danger", "Mükerrer Kayıt", $"Bu {duplicateType} zaten sistemde kayıtlı.", null);
                        await Task.Delay(4000);
                        Btn_isProcessing_01 = false;
                        return;
                    }

                    // --- 4. KAYIT İŞLEMİ ---

                    _user.ContactInformation = new ContactInformation
                    {
                        CountryPhoneCode = _SelectedPhoneCode,
                        PhoneNumber = normalizedPhone,
                        Email = _UserEmail.Trim().ToLower()
                    };

                    _user.HeaderMenuType = _user.UsersType;
                    _user.IsActive = true;
                    _user.RegistrationDate = DateTime.UtcNow;
                    _user.AccountActivationMailDeadline = DateTime.UtcNow.AddDays(3);
                    _user.AccountActivationMailStatu = false;

                    _user.Password = GlobalSecurityProvider.Encrypt(_user.Password);

                    var attachments = new List<Attachment>();
                    var details = UserInfos.GetCurrentUserDetails();
                    string safeIp = System.Net.WebUtility.HtmlEncode(details.IpAddress);
                    string safeDevice = System.Net.WebUtility.HtmlEncode(details.UserAgent);
                    string displayDate = DateTime.UtcNow.ToString("dd MMMM yyyy HH:mm");

                    #region Kayıt Emaili Gönderir
                    await emailSender.SendNewRegisterInfoEmailAsync(_user.Language, _user.ContactInformation.Email);
                    #endregion

                    // Bu alan içerisindeki tüm değerler "true" olarak atanmalı: _user.IsPrivateOrPublic
                    _user.IsPrivateOrPublic = new IsPrivateOrPublic
                    {
                        // 1. TEMEL GÖRÜNÜRLÜK AYARLARI
                        IsProfilePublic = true,
                        IsAllowPostsView = true,
                        IsShowLastSeen = true,
                        IsShowOnlineStatus = true,
                        IsSearchEngineIndexingAllowed = true,

                        // 2. İLETİŞİM VE BAĞLANTI AYARLARI
                        IsAllowFriendRequest = true,
                        IsAllowDirectMessages = true,
                        IsAllowVoiceCalls = true,
                        IsAllowVideoCalls = true,
                        IsReadReceiptsEnabled = true,

                        // 3. KİŞİSEL BİLGİ GİZLİLİĞİ
                        IsLocationVisible = true,
                        IsShowFollowerCount = true,
                        IsShowFollowingList = true,
                        IsShowFollowerList = true,

                        // 4. ETKİLEŞİM VE İÇERİK YÖNETİMİ
                        IsAllowComments = true,
                        IsAllowTagging = true,
                        IsAllowStorySharing = true,
                        IsAllowPostDownloading = true,

                        // 5. BİLDİRİM VE GÜVENLİK AYARLARI
                        IsEmailNotificationEnabled = true,
                        IsPushNotificationEnabled = true,
                        IsSmsNotificationEnabled = true,
                        IsTwoFactorAuthEnabled = true,

                        // 6. VERİ POLİTİKASI VE REKLAM
                        IsPersonalizedAdsEnabled = true,
                        IsDataCollectionAllowed = true,
                        IsAiTrainingAllowed = true,
                    };

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
                        await Task.Delay(4000);
                        Btn_isProcessing_01 = false;
                        await dbLog.SaveChangesAsync();
                    }

                    // Kullanıcıya hata bildirimi göster
                    await ShowNotification("danger", "Sistem Hatası", "Kayıt sırasında teknik bir hata oluştu. Lütfen tekrar deneyiniz.", null);
                }
                finally
                {
                    await Task.Delay(4000);
                    Btn_isProcessing_01 = false;
                }
            }
        }
    }
}