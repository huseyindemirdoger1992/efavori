using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    [Route("{culture}/Admin/[controller]/[action]/{id?}")]
    public class Product : Controller
    {
        public IActionResult AddProduct()
        {
            return View();
        }
        public IActionResult BulkWordPressProductImport()
        {
            return View();
        } 
        public IActionResult ListProduct()
        {
            return View();
        } 
        public IActionResult ProductHistoryList()
        {
            return View();
        } 
    }
}
