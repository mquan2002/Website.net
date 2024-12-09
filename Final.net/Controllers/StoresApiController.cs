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

        [HttpGet]
        public async Task<IActionResult> GetStores()
        {
            var stores = await _context.Stores.ToListAsync();

            // Chuyển đổi tọa độ từ số nguyên sang số thực
            var storesWithCorrectCoordinates = stores.Select(store => new
            {
                store.Id,
                store.Name,
                store.Address,
                store.Description,
                latitude = store.Latitude / 1000000.0,  // Chuyển đổi tọa độ vĩ độ
                longitude = store.Longitude / 1000000.0  // Chuyển đổi tọa độ kinh độ
            }).ToList();



            return Ok(storesWithCorrectCoordinates);
        }
    }
}
