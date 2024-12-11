using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Final.net.Models;
using Final.net.Areas.Admin.ProductService;

namespace Final.net.Areas_Admin_Controllers
{
    [Area("Admin")]
    [Route("admin/[controller]")]
    public class ShipperController : Controller
    {
        private readonly PizzaStoreContext _context;

        public ShipperController(PizzaStoreContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            // Fetch orders directly, including OrderItems and related Products
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems) // Include OrderItems
                    .ThenInclude(oi => oi.Product) // Include Product for each OrderItem
                .Where(o => o.DeliveryId != 4 && o.DeliveryId != 5) // Assuming status 1 indicates orders that need delivery
                .ToListAsync();

            return View(orders);
        }

        // GET: Shipper/_OrderDetails?orderId=1
        [HttpGet("_OrderDetails")]
        public async Task<IActionResult> OrderDetails(int orderId) // Chuyển từ 'id' thành 'orderId'
        {
            if (orderId <= 0)
            {
                Console.WriteLine($"Invalid OrderId: {orderId}");
                return BadRequest("Mã đơn hàng không hợp lệ.");
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Payment)
                .Include(o => o.Delivery)
                .Include(o => o.OrderItems)  // Include OrderItems to access Product data
                .ThenInclude(oi => oi.Product)  // Include the Product information for images
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                Console.WriteLine($"Order with OrderId {orderId} not found.");
                return NotFound();
            }

            return PartialView("_OrderDetails", order); // Return partial view with the order details
        }

        [HttpPost("UpdateDeliveryStatus")]
        public async Task<IActionResult> UpdateDeliveryStatus(int orderId, int deliveryId)
        {
            // Lấy đơn hàng theo OrderId
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
            }

            // Kiểm tra và cập nhật DeliveryId
            order.DeliveryId = deliveryId;

            // Lưu thay đổi vào cơ sở dữ liệu
            try
            {
                await _context.SaveChangesAsync(); // Lưu thay đổi
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Nếu có lỗi khi lưu thay đổi
                return Json(new { success = false, message = "Không thể cập nhật trạng thái đơn hàng. Lỗi: " + ex.Message });
            }
        }
    }
}