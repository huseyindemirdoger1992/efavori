using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    public class Home : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
