using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Route("{culture}/Customer/[controller]/[action]")]
    public class Home : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
