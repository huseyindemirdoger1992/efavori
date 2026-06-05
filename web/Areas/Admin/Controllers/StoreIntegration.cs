using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    [Route("{culture}/Admin/[controller]/[action]/{id?}")]
    public class StoreIntegration : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
