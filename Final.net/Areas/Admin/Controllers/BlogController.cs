using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final.net.Models;
using Final.net.Areas.Admin.BlogService;

namespace Final.net.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/[controller]")]
    public class BlogController : Controller
    {
        private readonly PizzaStoreContext _context;

        public BlogController(PizzaStoreContext context)
        {
            _context = context;
        }

        // GET: Blog
        [HttpGet("")]
        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 5;
            var totalBlogs = await _context.Blogs.CountAsync();
            var totalPages = (int)Math.Ceiling(totalBlogs / (double)pageSize);

            var blogs = await _context.Blogs
                .OrderBy(b => b.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            return View(blogs);
        }

        // GET: Blog/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View(new Blogs());
        }

        // POST: Blog/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blogs blog)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    blog.UpdatedAt = DateTime.Now;
                    _context.Add(blog);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error: {ex.Message}");
                }
            }

            return View(blog);
        }


        // GET: Blog/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Blogs blog)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingBlog = await _context.Blogs.FindAsync(id);
                    if (existingBlog == null)
                    {
                        return NotFound();
                    }

                    existingBlog.Title = blog.Title;
                    existingBlog.LinkURL = blog.LinkURL;
                    existingBlog.ImageURL = blog.ImageURL;
                    existingBlog.Active = blog.Active;  // Active giá trị tự động gán từ checkbox
                    existingBlog.UpdatedAt = DateTime.Now;

                    _context.Update(existingBlog);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Blogs.Any(e => e.Id == blog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(blog);
        }

        // GET: Blog/Details/5
        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }




        // GET: Blog/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Blog/Delete/5
        [HttpPost("Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }
    }
}