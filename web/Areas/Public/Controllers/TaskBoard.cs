using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("{culture}/Public/[controller]/[action]")]
    public class TaskBoard : Controller
    {
        public IActionResult PlanningTaskBoard()
        {
            return View();
        }
        public IActionResult InProcessTaskBoard()
        {
            return View();
        }
        public IActionResult CompletedTaskBoard()
        {
            return View();
        }
    }
}
