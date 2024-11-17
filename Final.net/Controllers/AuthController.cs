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
