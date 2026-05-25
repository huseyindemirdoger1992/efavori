using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    public class Support : Controller
    {
        public IActionResult NewTicket() => View();

        public IActionResult MyTickets() => View();
    }
}
