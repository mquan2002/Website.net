using System.Security.Claims;
using Final.net.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpGet("admin/login")]
        public IActionResult SignInWithAdmin()
        {
            return View();
        }


        // Đăng nhập
        [HttpPost]
        public async Task<IActionResult> SignIn(string username, string password)
        {
            // Tìm user theo email
            var user = await _context.Users
                        .Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.Username == username && u.IsDeleted == false && u.RoleId == 2);


            if (user == null)
            {
                ModelState.AddModelError("Username", "Username không tồn tại.");
                return View();
            }

            Console.WriteLine(password);


            // Kiểm tra mật khẩu
            var result = _passwordHasher.VerifyHashedPassword(null, user.Password, password);
            if (result == PasswordVerificationResult.Failed)
            {
                Console.WriteLine(result);

                ModelState.AddModelError("Password", "Mật khẩu không đúng.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Đăng nhập và lưu thông tin vào cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Đăng nhập thành công
            // Lưu thông tin đăng nhập (ví dụ: sử dụng session)
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("Address", user.Address);
            HttpContext.Session.SetString("Phone", user.Phone);


            return RedirectToAction("Index", "Home");
        }

        // Đăng ký
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([Bind("Username,Password,Email,Address,Phone")] User newUser, string passwordConfirm)
        {

            ViewData["PasswordConfirm"] = passwordConfirm;

            if (newUser.Password != passwordConfirm)
            {
                ModelState.AddModelError("PasswordConfirm", "Mật khẩu xác nhận không trùng khớp.");
                return View(newUser);
            }


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


            newUser.RoleId = 2;

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Chuyển hướng đến trang đăng nhập
            return RedirectToAction("SignIn");

        }




        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear(); // Clear session data
            await HttpContext.SignOutAsync(); // Sign out user
            return RedirectToAction("Index", "Home"); // Redirect to home page
        }



        // // Đăng nhập
        [HttpPost("admin/login")]
        public async Task<IActionResult> SignInWithAdmin(string username, string password)
        {
            // Tìm user theo email
            var user = await _context.Users
                        .Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.Username == username && u.IsDeleted == false && (u.RoleId == 1 || u.RoleId == 3));


            if (user == null)
            {
                ModelState.AddModelError("Username", "Username không tồn tại.");
                return View();
            }

            Console.WriteLine(password);


            // Kiểm tra mật khẩu
            var result = _passwordHasher.VerifyHashedPassword(null, user.Password, password);
            if (result == PasswordVerificationResult.Failed)
            {
                Console.WriteLine(result);

                ModelState.AddModelError("Password", "Mật khẩu không đúng.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Đăng nhập và lưu thông tin vào cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Đăng nhập thành công
            // Lưu thông tin đăng nhập (ví dụ: sử dụng session)
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("Address", user.Address);
            HttpContext.Session.SetString("Phone", user.Phone);
            HttpContext.Session.SetString("RoleId", user.RoleId.ToString());
            return RedirectToAction("Index", "admin");
        }


    }
}
