using data;
using data._Product;
using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("/Public/[controller]/[action]/{id?}")]
    public class _Viewer : Controller
    {
        public IActionResult UserProfile(Guid id)
        {
            ViewBag.UserProfile = id;
            return View();
        }
        public IActionResult ProductProfile(string id)
        {
            _ApplicationConnectionDb db = new _ApplicationConnectionDb();

            // Slug üzerinden SEO tablosunu ve Products tablosunu birleştirip ürünü çekiyoruz
            ViewBag.ProductProfile = db.ProductSeo
                .Where(seo => seo.Slug == id)
                .Join(
                    db.Products,
                    seo => seo.ProductId,
                    p => p.Id,
                    (seo, p) => p
                )
                .FirstOrDefault();

            return View();
        }
    }
}
