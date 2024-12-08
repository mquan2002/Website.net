using System.Security.Claims;
using Final.net.Models;
using Microsoft.AspNetCore.Mvc;

namespace Final.Areas.Admin
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    public class HomeAdminController : Controller
    {
        PizzaStoreContext db = new PizzaStoreContext();
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            // var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserRole = HttpContext.Session.GetString("RoleId");
            if (currentUserRole != "1" && currentUserRole != "3")
            {
                return NotFound();
            }

            return View();
        }
        [Route("products")]
        public IActionResult DanhMucSanPham(int? page)
        {
            var lstProducts = db.Products.ToList();
            return View(lstProducts);
        }

    }
}
