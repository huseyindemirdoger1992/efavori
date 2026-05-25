using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    public class AccountSettings : Controller
    {
        public IActionResult Addresses() => View();

        public IActionResult PaymentMethods() => View();

        public IActionResult NotificationPreferences() => View();
    }
}
