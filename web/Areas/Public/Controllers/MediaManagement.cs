using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    public class MediaManagement : Controller
    {
        [Area("Public")]
        [Route("{culture}/Public/[controller]/[action]")]
        public IActionResult MediaGallery()
        {
            return View();
        }
    }
}
