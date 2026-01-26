using data;
using api;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace razor._Shared.tr.Modals.Account
{
    public partial class Login : ComponentBase
    {
        [Inject] private IHttpClientFactory HttpClientFactory { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private Notification? notificationRef;
        private async Task ShowNotification(string type, string title, string text, string? image)
        {
            var imageUrl = string.IsNullOrWhiteSpace(image) ? "https://picsum.photos/120?" : image;
            if (notificationRef != null)
                await notificationRef.Launch(type, title, text, imageUrl);
        }

        private string? email = "huseyindemirdoger1992@gmail.com";
        private string? password = "9090";
        private string? loginMessage;

        private async Task LoginUserAsync()
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await ShowNotification("error", "Hata", "E-posta ve şifre gereklidir.", null);
                return;
            }

            // Backend'in kültürü anlaması için
            var uri = new Uri(Navigation.Uri);
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var culture = segments.Length > 0 ? segments[0] : "en";

            await ShowNotification("success", "Başarılı", "Giriş yapılıyor...", null);

            var emailSender = new EmailSender();
            await emailSender.SendEmailAsync(
                email,
                "Test Maili",
                "<b>Merhaba</b><br/>Bu mail SMTP üzerinden gönderildi."
            );


            // HttpClient ile değil, JS aracılığıyla formu POST ediyoruz. 
            // Bu sayede tarayıcı Set-Cookie yanıtını doğrudan işler.
            var endpoint = $"/{culture}/Account/Login";
            await JSRuntime.InvokeVoidAsync("submitLoginForm", endpoint, email, password);
        }
        private class LoginResult
        {
            public string message { get; set; }
            public UserInfo user { get; set; }
        }

        private class UserInfo
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
        }
    }
}