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
        public IActionResult Profile()
        {
            return View();
        }
    }
}
