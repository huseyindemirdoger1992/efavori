using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    public class SystemEmailHistory : Controller
    {
        [Area("Admin")]
        [Route("{culture}/Admin/[controller]/[action]")]
        public IActionResult List()
        {
            return View();
        }
    }
}
