using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    public class Shopping : Controller
    {
        public IActionResult Cart() => View();

        public IActionResult OrderHistory() => View();

        public IActionResult Favorites() => View();

        public IActionResult FavoriteHistory() => View();

        public IActionResult Coupons() => View();

        public IActionResult Reviews() => View();

        public IActionResult SellerMessages() => View();
    }
}
