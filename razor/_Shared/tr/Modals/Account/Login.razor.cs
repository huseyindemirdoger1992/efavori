using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Threading.Tasks;
using api;
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

        public Login(_ApplicationConnectionDb db, TakeLogs logger)
        {
            _db = db;
            _logger = logger;
        }

        private bool Btn_isProcessing_01 = false;

        private async Task ShowNotification(string type, string title, string text, string? image)
        {
            var imageUrl = string.IsNullOrWhiteSpace(image) ? "https://picsum.photos/120?" : image;
            if (notificationRef != null)
                await notificationRef.Launch(type, title, text, imageUrl);
        }

        private string? email = "huseyindemirdoger1992@gmail.com";
        private string? password = "9090";

        private async Task LoginUserAsync()
        {
            if (Btn_isProcessing_01) return;

            try
            {
                Btn_isProcessing_01 = true;

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    await ShowNotification("danger", "Hata", "E-posta ve şifre gereklidir.", null);
                    return;
                }

                // Backend'in kültürü anlaması için
                var uri = new Uri(Navigation.Uri);
                var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var culture = segments.Length > 0 ? segments[0] : "en";

                // HttpClient ile değil, JS aracılığıyla formu POST ediyoruz. 
                // Bu sayede tarayıcı Set-Cookie yanıtını doğrudan işler.
                var user = await _db.Users
                    .FirstOrDefaultAsync(u => u.ContactInformation.Email == email && u.Password == password);

                if (user != null)
                {
                    await ShowNotification("success", "Başarılı", "Giriş yapılıyor...", null);
                    culture = user.Language ?? culture;
                    var endpoint = $"/{culture}/Account/Login";
                    await JSRuntime.InvokeVoidAsync("submitLoginForm", endpoint, email, password);
                }
                else
                {
                    await ShowNotification("danger", "Hata", "Girilen bilgilere ait kullanıcı bulunamadı.", null);
                }


            }
            catch (Exception ex)
            {
                await _logger.TakeIt(
                    userId: null,
                    PageNameSpaceTitle: "namespace razor._Shared.tr.Modals.Account",
                    action: $"LoginUserAsync",
                    exception: ex.Message,
                    stackTrace: ex.StackTrace
                );
            }
            finally
            {
                await Task.Delay(4000);
                Btn_isProcessing_01 = false;
            }
        }
    }
}