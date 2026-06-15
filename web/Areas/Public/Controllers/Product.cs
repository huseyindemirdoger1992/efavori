using data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace web.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("{culture}/Public/[controller]/[action]")]
    [Route("{culture}/Public/[controller]/[action]/{id?}")]
    public class Product : Controller
    {
        public async Task<IActionResult> ProductViewer(Guid id)
        {
            using (var db = new _ApplicationConnectionDb())
            {
                var Product = await db.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == id);
                ViewBag.ProductData = Product;
            }
            return View();
        }
    }
}
