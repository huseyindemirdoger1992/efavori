using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    public class Warehouse : Controller
    {
        public IActionResult Add()
        {
            return View();
        }
        public IActionResult List()
        {
            return View();
        }
    }
}
