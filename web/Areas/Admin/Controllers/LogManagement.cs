using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    public class LogManagement : Controller
    {
        [Area("Admin")]
        [Route("{culture}/Admin/[controller]/[action]")]
        public IActionResult List()
        {
            return View();
        }
    }
}
