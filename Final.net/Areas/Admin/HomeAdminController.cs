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
