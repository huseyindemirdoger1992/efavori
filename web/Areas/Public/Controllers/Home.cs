using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("{culture}/Public/[controller]/[action]")]
    public class Home : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "The Global Marketplace for Everything You Need";
            ViewData["Description"] = "Shop millions of products from thousands of sellers on efavori.com. Discover the best deals on electronics, fashion, home goods, and more with secure global shipping.";
            ViewData["Keywords"] = "efavori, online shopping, global marketplace, multi-vendor platform, best deals, e-commerce, buy online";

            return View();
        }
        public IActionResult GetGitHubCommits()
        {
            return View();
        }
    }
}
