using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    public class ChatMessage : Controller
    {
        [Area("Public")]
        [Route("{culture}/Public/[controller]/[action]")]
        public IActionResult LiveChatMessage()
        {
            return View();
        }
    }
}
