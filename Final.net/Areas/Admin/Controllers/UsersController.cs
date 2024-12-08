using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Final.net.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Final.net.Areas.Admin.Views.HomeAdmin
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly PizzaStoreContext _context;
        private readonly PasswordHasher<object> _passwordHasher;

        public UsersController(PizzaStoreContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<object>();
        }

        // GET: Users
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string searchQuery = "")
        {

            // var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // // Check if the user has admin role
            // var currentUserRole = HttpContext.Session.GetString("RoleId");
            // // if(currentUserRole)
            // if (userRole != "Admin")
            // {
            //     return Unauthorized("Bạn không có quyền truy cập trang này.");
            // }
            var currentUserRole = HttpContext.Session.GetString("RoleId");
            if (currentUserRole != "1" && currentUserRole != "3")
            {
                return NotFound();
            }
            if (currentUserRole == "3")
            {
                return Unauthorized("Bạn không có quyền sử dụng chức năng này.");
            }
            const int pageSize = 5;  // Number of users per page

            // Query the users, optionally filter by search query
            var query = _context.Users
                                .Where(u => u.IsDeleted == false);

            // Apply search filters if any
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                // query = query.Where(u => u.Username.Contains(searchQuery) || u.Email.Contains(searchQuery) || u.Phone.Contains(searchQuery));
                query = query.Where(u => u.Username.Contains(searchQuery));
            }

            // Calculate the total number of users (with search filters applied)
            var totalUsers = await query.CountAsync();

            // Calculate the total number of pages
            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            // Get the users for the current page (with search filters applied)
            var users = await query
                                .Include(u => u.Role)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

            // Pass pagination and search query to the view
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["SearchQuery"] = searchQuery;

            return View(users);
        }




        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var currentUserRole = HttpContext.Session.GetString("RoleId");
            if (currentUserRole != "1" && currentUserRole != "3")
            {
                return NotFound();
            }
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            var currentUserRole = HttpContext.Session.GetString("RoleId");
            if (currentUserRole != "1" && currentUserRole != "3")
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Password,Email,Address,Phone,RoleId")] User user)
        {
            var currentUserRole = HttpContext.Session.GetString("RoleId");
            if (currentUserRole != "1" && currentUserRole != "3")
            {
                return NotFound();
            }
            // Check if the username already exists
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                ModelState.AddModelError("Username", "Username đã tồn tại");
                ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
                return View(user);
            }

            user.Password = _passwordHasher.HashPassword(null, user.Password);

            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var currentUserRole = HttpContext.Session.GetString("RoleId");
            if (currentUserRole != "1" && currentUserRole != "3")
            {
                return NotFound();
            }
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", user.RoleId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Username,Password,RoleId,Id")] User user)
        {
            var currentUserRole = HttpContext.Session.GetString("RoleId");
            if (currentUserRole != "1" && currentUserRole != "3")
            {
                return NotFound();
            }
            if (id != user.Id)
            {
                return NotFound();
            }

            // Kiểm tra xem Username đã tồn tại hay chưa (trừ user hiện tại)
            var usernameExisting = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == user.Username && u.Id != id);

            if (usernameExisting != null)
            {
                // Username đã tồn tại
                ModelState.AddModelError("Username", "Username đã tồn tại.");
                ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
                return View(user);
            }

            try
            {
                // Lấy bản ghi user hiện tại từ database
                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                {
                    return NotFound();
                }

                // Cập nhật chỉ các trường thay đổi
                if (existingUser.Username != user.Username)
                {
                    existingUser.Username = user.Username;
                }


                if (existingUser.RoleId != user.RoleId)
                {
                    existingUser.RoleId = user.RoleId;
                }

                if (user.IsDeleted != existingUser.IsDeleted)
                {
                    existingUser.IsDeleted = user.IsDeleted;
                }

                if (existingUser.Password != user.Password)
                {
                    // Hash và cập nhật mật khẩu nếu mật khẩu mới được cung cấp
                    if (!string.IsNullOrEmpty(user.Password))
                    {
                        var hashedPassword = _passwordHasher.HashPassword(null, user.Password);
                        if (existingUser.Password != hashedPassword)
                        {
                            existingUser.Password = hashedPassword;
                        }
                    }
                }

                // Cập nhật trường UpdatedDate
                existingUser.UpdatedDate = DateTime.Now;

                // Chỉ cập nhật các trường thay đổi
                _context.Entry(existingUser).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }



        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var currentUserRole = HttpContext.Session.GetString("RoleId");
            if (currentUserRole != "1" && currentUserRole != "3")
            {
                return NotFound();
            }
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUserRole = HttpContext.Session.GetString("RoleId");
            Console.WriteLine(currentUserRole);
            if (currentUserRole != "1" && currentUserRole != "3")
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsDeleted = true;
                // Set the IsDeleted flag to true instead of deleting the user entity

                _context.Users.Update(user);
                // _context.Users.Delete(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }

}