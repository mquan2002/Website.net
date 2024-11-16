using Final.net.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final.net.Controllers
{
    public class SignController : Controller
    {

        private readonly PizzaStoreContext _context;
        private readonly PasswordHasher<object> _passwordHasher;

        [ActivatorUtilitiesConstructor]
        public SignController(PizzaStoreContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<object>();
        }


        public IActionResult SignIn()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        // Đăng nhập
        [HttpPost]
        public async Task<IActionResult> SignIn(string username, string password)
        {
            // Tìm user theo email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                ModelState.AddModelError("Username", "Username không tồn tại.");
                return View();
            }

            // Kiểm tra mật khẩu
            var result = _passwordHasher.VerifyHashedPassword(null, user.Password, password);
            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("Password", "Mật khẩu không đúng.");
                return View();
            }

            // Đăng nhập thành công
            // Lưu thông tin đăng nhập (ví dụ: sử dụng session)
            HttpContext.Session.SetString("UserId", user.Id.ToString());

            return RedirectToAction("Index", "Home");
        }

    }
}
