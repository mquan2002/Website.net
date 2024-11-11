using Microsoft.AspNetCore.Mvc;

namespace Final.net.Controllers
{
    public class SignController : Controller
    {
        public IActionResult SignIn()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }
    }
}
