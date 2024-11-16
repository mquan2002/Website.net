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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.IsDeleted == false);
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

        // Đăng ký
        [HttpPost]
        public async Task<IActionResult> SignUp([Bind("Username,Password,Email,Address,Phone")] User newUser)
        {

            //if (!ModelState.IsValid)
            //{
            //    return View(newUser); // Return the view with validation errors
            //}

            // Kiểm tra xem email đã tồn tại chưa
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == newUser.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Username đã tồn tại.");
                return View(newUser);
            }
            if (!string.IsNullOrEmpty(newUser.Password))
            {
                newUser.Password = _passwordHasher.HashPassword(null, newUser.Password);
            }
            else
            {
                // Xử lý nếu mật khẩu bị null hoặc rỗng (nếu cần)
                ModelState.AddModelError("Password", "Mật khẩu không được để trống.");
                return View(newUser);
            }


            newUser.Password = _passwordHasher.HashPassword(null, newUser.Password);
            newUser.RoleId = 2;

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Chuyển hướng đến trang đăng nhập
            return RedirectToAction("SignIn");

        }
           


    }
}
