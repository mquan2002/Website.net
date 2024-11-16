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
        public async Task<IActionResult> Index()
        {
            var pizzaStoreContext = _context.Users.Include(u => u.Role);
            return View(await pizzaStoreContext.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
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
            if (id != user.Id)
            {
                return NotFound();
            }

            // Check if the username already exists for another user (excluding current user)
            var usernameExisting = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username && u.Id != id);

            if (usernameExisting != null)
            {
                // Username already exists
                ModelState.AddModelError("Username", "Username đã tồn tại.");
                ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
                return View(user);
            }

            try
            {
                // Fetch the existing user record to preserve non-edited fields
                var existingUser = await _context.Users.FindAsync(id);

                if (existingUser == null)
                {
                    return NotFound();
                }

                // Map updated properties from user DTO to the existing user entity
                existingUser.Username = user.Username;
                existingUser.RoleId = user.RoleId;
                existingUser.IsDeleted = user.IsDeleted;
                existingUser.UpdatedDate = DateTime.Now; // Set UpdatedDate to the current date/time

                // If a new password is provided, hash and update it
                if (!string.IsNullOrEmpty(user.Password))
                {
                    existingUser.Password = _passwordHasher.HashPassword(null, user.Password);
                }

                // Update the user entity in the database
                _context.Update(existingUser);
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
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
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