using Final.net.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Final.net.Controllers
{
    public class MenuController : BaseController
    {
        public MenuController(PizzaStoreContext context) : base(context)
        {
        }

        public IActionResult Index()
        {
            try
            {
                var categories = _context.Categories
                .Where(c => c.DeletedAt == null)
                .Include(c => c.Products) 
                .ToList();
                foreach (var category in categories)
                {
                    category.Products = category.Products
                        .Where(p => p.CategoryId > 0 && p.DeletedAt == null)
                        .ToList();
                }

                return View(categories);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tải dữ liệu menu: {ex.Message}");
                return StatusCode(500, "Lỗi khi tải dữ liệu menu.");
            }
        }


        [HttpGet]
        public IActionResult SearchLive(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query không hợp lệ.");
            }

            try
            {
                // Tìm kiếm sản phẩm theo tên
                var results = _context.Products
                    .Where(p => p.ProductName.ToLower().Contains(query.ToLower()))
                    .Select(p => new
                    {
                        p.ProductId,
                        p.ProductName,
                        p.ImageUrl,
                        p.Price
                    })
                    .ToList();

                return Json(results);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần (tùy thuộc vào logger bạn đang dùng)
                Console.WriteLine($"Lỗi khi tìm kiếm sản phẩm: {ex.Message}");
                return StatusCode(500, "Lỗi khi tìm kiếm sản phẩm.");
            }
        }
    }
}
