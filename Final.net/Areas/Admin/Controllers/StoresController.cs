using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final.net.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Final.net.Controllers
{
    public class StoresController : Controller
    {
        private readonly PizzaStoreContext _context;

        public StoresController(PizzaStoreContext context)
        {
            _context = context;
        }


        // Hiển thị trang bản đồ
        [HttpGet]
        public async Task<IActionResult> Index(string address = null)
        {
            var stores = await _context.Stores.ToListAsync();

            // Chuyển đổi tọa độ từ số nguyên sang số thực
            var storesWithCoordinates = stores.Select(store => new
            {
                store.Id,
                store.Name,
                store.Address,
                store.Description,
                latitude = store.Latitude / 1000000.0,  // Chuyển đổi tọa độ vĩ độ
                longitude = store.Longitude / 1000000.0  // Chuyển đổi tọa độ kinh độ
            }).ToList();


            return View();
        }
    }
}
