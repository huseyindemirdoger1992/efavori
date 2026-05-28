using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("{culture}/Public/[controller]/[action]")]
    public class Home : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetGitHubCommits()
        {
            return View();
        }
    }
}
