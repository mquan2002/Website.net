using Microsoft.AspNetCore.Mvc;

namespace Final.net.Controllers
{
    public class MenuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
