using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final.net.Models; 


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

