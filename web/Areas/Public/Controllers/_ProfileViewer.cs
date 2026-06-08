using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("/Public/[controller]/[action]/{id?}")]
    public class _ProfileViewer : Controller
    {
        public IActionResult UserProfile(Guid id)
        {
            ViewBag.UserProfile = id;
            return View();
        }
    }
}
