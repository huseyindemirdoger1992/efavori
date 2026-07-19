using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    public class Categories : Controller
    {
        public IActionResult CategoryManagementProducts()
        {
            return View();
        }
    }
}
