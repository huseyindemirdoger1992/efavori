using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("")]
    [Route("{TaskFrameworkId:guid}")]
    [Route("{Value?}/{TaskFrameworkId?}")]
    [Route("{culture}/Public/[controller]/[action]")]
    [Route("{culture}/Public/[controller]/[action]/{Value?}")]
    [Route("{culture}/Public/[controller]/[action]/{Value}/{TaskFrameworkId}")]
    [Route("{culture}/Public/[controller]/[action]/{TaskId:guid}")]
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
