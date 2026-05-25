using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    public class Connections : Controller
    {
        public IActionResult FollowedStores() => View();

        public IActionResult RecentlyViewed() => View();

        public IActionResult Friends() => View();

        public IActionResult Colleagues() => View();

        public IActionResult BlockedUsers() => View();
    }
}
