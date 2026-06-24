using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    public class MainCssJs : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
