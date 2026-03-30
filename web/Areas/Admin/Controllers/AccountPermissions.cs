using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    public class AccountPermissions : Controller
    {
        [Area("Admin")]
        [Route("{culture}/Admin/[controller]/[action]")]
        public IActionResult Permissions()
        {
            return View();
        }
    }
}
