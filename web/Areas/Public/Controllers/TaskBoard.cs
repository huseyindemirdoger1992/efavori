using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    [Area("Public")]
    // [Route("")]
    // [Route("{TaskFrameworkId:guid}")]
    // [Route("{Value?}/{TaskFrameworkId?}")]
    [Route("{culture}/Public/[controller]/[action]")]
    [Route("{culture}/Public/[controller]/[action]/{Value?}")]
    [Route("{culture}/Public/[controller]/[action]/{Value}/{TaskFrameworkId}")]
    [Route("{culture}/Public/[controller]/[action]/{TaskId:guid}")]
    public class TaskBoard : Controller
    {
        public IActionResult CentralSystemTaskBoard(string? Value, Guid? TaskFrameworkId)
        {
            ViewBag.Value = Value;
            ViewBag.TaskFrameworkId = TaskFrameworkId;
            return View();
        }
        public IActionResult PrintTask(Guid? TaskId)
        {
            ViewBag.TaskId = TaskId;
            return View();
        }
    }
}
