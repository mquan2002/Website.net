using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final.net.Models;
using Newtonsoft.Json;

namespace Final.net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoresApiController : ControllerBase
    {
        private readonly PizzaStoreContext _context;

        public StoresApiController(PizzaStoreContext context)
        {
            _context = context;
        }

        // API GET để lấy tất cả cửa hàng
        [HttpGet]
        public async Task<IActionResult> GetStores()
        {
            var stores = await _context.Stores.ToListAsync();

            if (stores == null || stores.Count == 0)
            {
                return NotFound();  // Trả về lỗi nếu không có cửa hàng
            }

            // Log dữ liệu (trong môi trường phát triển)
            Console.WriteLine("Stores Data: " + JsonConvert.SerializeObject(stores));

            return Ok(stores);
        }


        // API GET để lấy cửa hàng theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStore(int id)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }
            return Ok(store);
        }
    }
}
