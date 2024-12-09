using Final.net.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Final.net.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(PizzaStoreContext context, ILogger<HomeController> logger) : base(context)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Kiểm tra người dùng đã đăng nhập chưa
            if (User.Identity.IsAuthenticated)
            {
                // Lấy Username từ Claims
                var username = User.Claims.FirstOrDefault(c => c.Type == "Name")?.Value;
                ViewBag.Username = username;
            }
            else
            {
                ViewBag.Username = null;
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
