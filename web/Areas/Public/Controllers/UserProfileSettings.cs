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
        public IActionResult BasicInfo()
        {
            return View();
        }

        public IActionResult Social()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Customize()
        {
            return View();
        }

        public IActionResult Security()
        {
            return View();
        }
    }
}
