using Final.net.Models;
using Final.net.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Final.net.Controllers
{
    public class MenuController : BaseController
    {
        private readonly PizzaStoreContext _context;
        public MenuController(CartService cartService, PizzaStoreContext context) : base(cartService)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            try
            {
                var categories = _context.Categories.Include(c => c.Products).ToList();
                return View(categories);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần
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
                // Ghi log lỗi nếu cần
                return StatusCode(500, "Lỗi khi tìm kiếm sản phẩm.");
            }
        }
    }
}
