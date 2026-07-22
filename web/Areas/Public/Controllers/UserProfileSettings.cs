using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("{culture}/Public/[controller]/[action]")]
    public class UserProfileSettings : Controller
    {

        public IActionResult BasicInfo() => View();
        public IActionResult Social() => View();
        public IActionResult Privacy() => View();
        public IActionResult Customize() => View();
        public IActionResult Security() => View();


        public IActionResult UserAddressMethod() => View();

        public IActionResult UserPaymentMethod() => View();

    }
}
