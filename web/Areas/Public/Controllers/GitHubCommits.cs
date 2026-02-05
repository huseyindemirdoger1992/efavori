using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("{culture}/Public/[controller]/[action]")]
    public class GitHubCommits : Controller
    {
        public IActionResult GetGitHubCommits()
        {
            return View();
        }
    }
}
