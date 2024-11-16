using System;
using System.Linq;
using System.Threading.Tasks;
using Final.net.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final.net.Controllers
{
    public class AuthController : Controller
    {
        private readonly PizzaStoreContext _context;
        private readonly PasswordHasher<object> _passwordHasher;

        public AuthController(PizzaStoreContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<object>();
        }

        // Đăng ký
        [HttpPost]
        public async Task<IActionResult> Register(string email, string password)
        {
            // Kiểm tra xem email đã tồn tại chưa
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
                return View();
            }

            // Tạo user mới
            var user = new User
            {
                Email = email,
                Password = _passwordHasher.HashPassword(null, password),
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                RoleId = 1 // Gán role mặc định, ví dụ: RoleId = 1 là người dùng bình thường
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Chuyển hướng đến trang đăng nhập
            return RedirectToAction("Login");
        }

       
        // Đăng xuất
        [HttpPost]
        public IActionResult Logout()
        {
            // Xóa session
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Login");
        }
    }
}
