using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    public class DraftPage : Controller
    {
        public IActionResult ThisPageIsBeingPrepared()
        {
            return View();
        }
        public IActionResult ThisPageIsBeingPreparedProfile()
        {
            return View();
        }
        public IActionResult ThisPageIsBeingPreparedProfileMedia()
        {
            return View();
        }
        public IActionResult ArtificialIntelligence()
        {
            return View();
        }
    }
}
