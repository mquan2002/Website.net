using Microsoft.AspNetCore.Mvc;

namespace Final.net.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
