using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public
{
    [Area("Public")]
    [Route("")]
    [Route("{TaskFrameworkId:guid}")]
    [Route("{Value?}/{TaskFrameworkId?}")]
    [Route("{culture}/Public/[controller]/[action]")]
    [Route("{culture}/Public/[controller]/[action]/{Value?}")]
    [Route("{culture}/Public/[controller]/[action]/{Value}/{TaskFrameworkId}")]
    [Route("{culture}/Public/[controller]/[action]/{TaskId:guid}")]
    public class Search : Controller
    {
        public IActionResult Wanted(string SearchWantedText)
        {
            ViewBag.SearchWantedText = SearchWantedText;
            return View();
        }
    }
}
