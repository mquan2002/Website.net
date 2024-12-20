using Final.net.Models;
using Final.net.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Final.net.Controllers
{
    public class CheckOutController : BaseController
    {
        private readonly PizzaStoreContext _context;

        public CheckOutController(PizzaStoreContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
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
                .Include(o => o.OrderItems)
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
                return Json(new { success = false, message = "Hãy nhập ID đơn hàng hoặc Số điện thoại" });
            }

            try
            {
                var isPhoneNumber = orderId.All(char.IsDigit) && orderId.Length >= 10;

                var order = await _context.Orders
                .Include(o => o.Payment)
                .Include(o => o.Delivery)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(o => o.Product)
                .FirstOrDefaultAsync(o =>
                isPhoneNumber
                    ? o.SDT == orderId  // Tìm theo số điện thoại
                    : o.OrderId.ToString() == orderId);

                if (order != null)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Đơn hàng tìm thấy.",
                        data = new
                        {
                            order.OrderId,
                            order.Payment.Method,
                            order.Address,
                            order.Delivery.DeliveryStatus,
                            order.User.Username,
                            order.OrderItems.First().Product.ImageUrl,
                            order.OrderDate,
                            order.TotalAmount
                        }
                    });
                }
                else
                {
                    return Json(new { success = false, message = $"Order {orderId} not found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã có lỗi khi chạy, hãy thử lại", error = ex.Message });
            }

        }

    }
}