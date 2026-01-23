using data;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace razor._Shared.tr.Modals.Account
{
    public partial class Login : ComponentBase
    {
        [Inject] private IHttpClientFactory HttpClientFactory { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

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
            loginMessage = null;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                loginMessage = "E-posta ve şifre gereklidir.";
                await ShowNotification("error", "Hata", loginMessage, null);
                return;
            }

            var client = HttpClientFactory.CreateClient();
            client.BaseAddress = new Uri(Navigation.BaseUri);

            // Kültürü URI'dan alıyoruz
            var uri = new Uri(Navigation.Uri);
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var culture = segments.Length > 0 ? segments[0] : "en";

            // Doğru endpoint: /tr/Account/login
            var endpoint = $"/{culture}/Account/login";


            // bu alan sürekli 404 dönüyor. nedeni ne olabilir?
            var response = await client.PostAsJsonAsync(endpoint, new { Email = email, Password = password });

            

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResult>();
                loginMessage = result?.message ?? "Giriş başarılı!";
                await ShowNotification("success", "Başarılı", loginMessage, null);

                // Oturum açıldıktan sonra sayfayı yenile
                Navigation.NavigateTo($"/{culture}/Customer/Home/Index", forceLoad: true);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                loginMessage = !string.IsNullOrWhiteSpace(error) ? error : "E-posta veya şifre hatalı ya da hesap aktif değil.";
                await ShowNotification("error", "Hata", loginMessage, null);
            }
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