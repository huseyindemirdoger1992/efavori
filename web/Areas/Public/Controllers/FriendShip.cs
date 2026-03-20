using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("{culture}/Public/[controller]/[action]")]
    public class FriendShip : Controller
    {
        public IActionResult Requests()
        {
            return View();
        }
    }
}
