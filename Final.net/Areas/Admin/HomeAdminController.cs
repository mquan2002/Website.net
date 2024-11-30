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

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "User")
            {
                return Unauthorized("Bạn không có quyền truy cập trang này.");
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
