using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("{culture}/Public/[controller]/[action]")]
    public class MainPrompter : Controller
    {
        public IActionResult MainPrompterPageCSHTML()
        {
            return View();
        }
    }
}
