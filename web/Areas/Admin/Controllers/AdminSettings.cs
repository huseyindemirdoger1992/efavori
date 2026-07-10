using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    public class AdminSettings : Controller
    {
        [Area("Admin")]
        [Route("{culture}/Admin/[controller]/[action]")]
        public IActionResult AllBackgroundServicesFrequencyRateIndex()
        {
            return View();
        }
    }
}
