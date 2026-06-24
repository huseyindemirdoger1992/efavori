using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    public class ArticlesCategories : Controller
    {
        public IActionResult ControllerArticlesCategoriesTr()
        {
            return View();
        }
    }
}
