using Final.net.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Final.net.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/[controller]")]
    public class OrderController : Controller
    {
        private readonly PizzaStoreContext _context;

        public OrderController(PizzaStoreContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index(int page = 1, int searchType = 0, string searchValue = "")
        {
            const int pageSize = 5;

            IQueryable<Order> query = _context.Orders;

            if (searchType == 1 && !int.TryParse(searchValue, out int product222))
            {
                ViewData["Error"] = "Id sản phẩm phải là 1 số";
            }
            if (!string.IsNullOrEmpty(searchValue))
            {
                if (searchType == 1)
                {
                    if (int.TryParse(searchValue, out int OrderId))
                    {
                        query = query.Where(p => p.OrderId == OrderId);
                    }
                }
                else if (searchType == 2) // Tìm kiếm theo tên
                {
                    query = query.Where(p => p.SDT.Contains(searchValue));
                }
            }

            var totalOrders = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalOrders / (double)pageSize);
            // Lấy danh sách Order cùng thông tin Payment
            var orders = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(o => o.Payment) // Bao gồm bảng Payment
                .Include(o => o.Delivery) // Bao gồm bảng Payment
                .Include(o => o.User)
                .ToListAsync(); // Trả về danh sách Orders thực sự

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["SearchType"] = searchType;
            ViewData["SearchValue"] = searchValue;
            ViewData["TotalOrder"] = totalOrders;
            ViewData["SearchTypeName"] = searchType == 1 ? "Id" : "số điện thoại";
            return View(orders); // Trả về View với danh sách Order

        }

        // [HttpGet("Detail/{id}")]
        // // GET: Product/Details/5
        // public async Task<IActionResult> Details(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }

        //     var order = await _context.Orders
        //         .Include(o => o.Payment)
        //         .Include(o => o.Delivery)
        //         .FirstOrDefaultAsync(m => m.OrderId == id);
        //     if (order == null)
        //     {
        //         return NotFound();
        //     }

        //     return View(order);
        // }

        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy thông tin đơn hàng theo id
            var order = await _context.Orders
                .Include(o => o.Payment)
                .Include(o => o.Delivery)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            var orderItems = await _context.OrderItem
                .Include(oi => oi.Product) // Bao gồm thông tin sản phẩm
                .Include(oi => oi.Size) // Bao gồm thông tin Size
                .Include(oi => oi.Crust) // Bao gồm thông tin Crust
                .Where(oi => oi.OrderId == id)
                .Select(oi => new
                {
                    oi.OrderItemId,
                    oi.Product.ProductName,
                    oi.Product.ImageUrl,
                    oi.Size.SizeName,
                    oi.Crust.CrustName,
                    oi.Quantity,
                    oi.UnitPrice,
                    oi.Price
                })
                .ToListAsync();

            // Truyền dữ liệu vào View qua ViewBag
            ViewBag.Order = order;
            ViewBag.OrderItems = orderItems;

            return View();
        }

        [HttpGet("Edit/{id}")]
        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "Method", order.PaymentId);
            ViewData["DeliveryId"] = new SelectList(_context.Deliveries, "DeliveryId", "DeliveryStatus", order.DeliveryId);
            return View(order);
        }


        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            var existingOrder = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(p => p.OrderId == id);
            if (existingOrder == null)
            {
                return NotFound();
            }

            try
            {
                Console.WriteLine($"Notes: {order.Notes}");
                _context.Update(order);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(order.OrderId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
            return View(order);
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }

        [HttpGet("Delete/{id}")]
        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Payment)
                .Include(o => o.Delivery)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Product/Delete/5
        [HttpPost("Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
