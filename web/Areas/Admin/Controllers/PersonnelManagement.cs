using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("{culture}/Admin/[controller]/[action]")]
    public class PersonnelManagement : Controller
    {
        public IActionResult AllUsers()
        {
            return View();
        }
    }
}
