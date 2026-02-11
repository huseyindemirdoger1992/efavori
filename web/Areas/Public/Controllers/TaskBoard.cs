using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("{culture}/Public/[controller]/[action]")]
    [Route("{culture}/Public/[controller]/[action]/{Value?}")]
    public class TaskBoard : Controller
    {
        public IActionResult CentralSystemTaskBoard(string Value)
        {
            ViewBag.Value = Value;
            return View();
        }
    }
}
