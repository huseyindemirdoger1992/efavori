using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("{culture}/Public/[controller]/[action]")]
    [Route("{culture}/Public/[controller]/[action]/{id?}")]
    public class _ProfileViewer : Controller
    {
        public IActionResult UserProfile(Guid id)
        {
            ViewBag.UserProfile = id;
            return View();
        }
    }
}
