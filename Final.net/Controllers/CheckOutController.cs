using Final.net.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Final.net.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly PizzaStoreContext _context;

        public CheckOutController(PizzaStoreContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index(int page = 1)
        {
            // const int pageSize = 5;

            // var totalOrders = await _context.Orders.CountAsync();

            // var totalPages = (int)Math.Ceiling(totalOrders / (double)pageSize);
            // // Lấy danh sách Order cùng thông tin Payment
            var orders = await _context.Orders
                // .Skip((page - 1) * pageSize)
                // .Take(pageSize)
                .Include(o => o.Payment) // Bao gồm bảng Payment
                .Include(o => o.Delivery) // Bao gồm bảng Payment
                .Include(o => o.User)
                .ToListAsync(); // Trả về danh sách Orders thực sự

            // ViewData["CurrentPage"] = page;
            // ViewData["TotalPages"] = totalPages;
            return View(orders); // Trả về View với danh sách Order

        }

        [HttpPost]
        public async Task<IActionResult> Search(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return Json(new { success = false, message = "Please enter a valid order ID." });
            }

            var order = await _context.Orders
                .Include(o => o.Payment)
                .Include(o => o.Delivery)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId.ToString() == orderId);

            if (order != null)
            {
                return Json(new
                {
                    success = true,
                    message = $"Order {order.OrderId}: Payment - {order.Payment.Method}, Delivery - {order.Delivery.DeliveryStatus}, Customer - {order.User.Username}",
                    data = new
                    {
                        order.OrderId,
                        order.Payment.Method,
                        order.Address,
                        order.Delivery.DeliveryStatus,
                        order.User.Username,
                    }
                });
            }
            else
            {
                return Json(new { success = false, message = $"Order {orderId} not found." });
            }
        }

    }
}