using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    public class Store : Controller
    {
        public IActionResult Add()
        {
            return View();
        }
        public IActionResult List()
        {
            return View();
        }
        public IActionResult Chart()
        {
            return View();
        }
    }
}
