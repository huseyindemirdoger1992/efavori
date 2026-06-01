using data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    [Route("{culture}/Admin/[controller]/[action]/{id?}")]
    public class Store : Controller
    {
        public IActionResult Add()
        {
            return View();
        }
        public IActionResult List()
        {
            return View();
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            ViewBag.StoreId = id;
            using (var db = new _ApplicationConnectionDb())
            {
                var store = await db.Stores
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == id);
                ViewBag.StoreData = store;
            }

            return View();
        }
    }
}
