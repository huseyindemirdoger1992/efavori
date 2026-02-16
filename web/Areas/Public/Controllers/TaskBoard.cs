using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("{culture}/Public/[controller]/[action]")]
    [Route("{culture}/Public/[controller]/[action]/{Value?}")]
    [Route("{culture}/Public/[controller]/[action]/{Value}/{TaskFrameworkId}")]
    public class TaskBoard : Controller
    {
        public IActionResult CentralSystemTaskBoard(string Value, Guid TaskFrameworkId)
        {
            ViewBag.Value = Value;
            ViewBag.TaskFrameworkId = TaskFrameworkId;
            return View();
        }
    }
}
