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
            using (_ApplicationConnectionDb db = new _ApplicationConnectionDb())
            {
                // Join işleminde hem Product hem de Seo verisini anonim bir tipte birleştiriyoruz
                var pageData = db.ProductSeo
                    .Where(seo => seo.Slug == id)
                    .Join(
                        db.Products,
                        seo => seo.ProductId,
                        p => p.Id,
                        (seo, p) => new { Product = p, SeoInfo = seo }
                    )
                    .FirstOrDefault();

                // SEO için kritik: Ürün bulunamazsa 404 sayfasına yönlendirin (Soft-404 hatasını önler)
                if (pageData == null)
                {
                    return NotFound();
                }

                // HTML <head> etiketlerinin beklediği parametreleri ViewData'ya aktarıyoruz
                // (Veritabanınızdaki kolon isimlerinin Title, Description, Keywords olduğunu varsayıyorum)
                ViewData["Title"] = pageData.Product.Name;
                ViewData["Description"] = pageData.Product.ShortDescription;
                ViewData["Keywords"] = pageData.Product.Tags ?? pageData.Product.AiOriginalTags;

                // View tarafında ürün detaylarını göstermek için ViewBag'i dolduruyoruz
                ViewBag.ProductProfile = pageData.Product;

                return View();
            }
        }
    }
}
