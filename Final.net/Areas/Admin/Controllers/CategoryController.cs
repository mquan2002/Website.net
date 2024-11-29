using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Final.net.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Final.net.Areas.Admin.CategoryService;
namespace Final.net.Areas_Admin_Controllers
{
    [Area("Admin")]
    [Route("admin/[controller]")]
    public class CategoryController : Controller
    {
        private readonly PizzaStoreContext _context;
        private readonly CategoryService _categoryService;

        public CategoryController(PizzaStoreContext context, CategoryService categoryService)
        {
            _context = context;
            _categoryService = categoryService;
        }

        // GET: Category
        [HttpGet("")]
        public async Task<IActionResult> Index(int page = 1, int searchType = 0, string searchValue = "")
        {
            const int pageSize = 2;
            IQueryable<Category> query = _context.Categories;

            if (searchType == 1 && !int.TryParse(searchValue, out int categoryId22222221))
            {
                ViewData["Error"] = "Id danh mục sản phẩm phải là 1 số"; 
            }
            if (!string.IsNullOrEmpty(searchValue))
            {
                if (searchType == 1) 
                {
                    if (int.TryParse(searchValue, out int categoryId))
                    {
                        query = query.Where(c => c.CategoryId == categoryId);
                    }
                }
                else if (searchType == 2) // Tìm kiếm theo tên
                {
                    query = query.Where(c => c.CategoryName.Contains(searchValue));
                }
            }
            var totalCategories = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCategories / (double)pageSize);

            var categories = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["SearchType"] = searchType;
            ViewData["SearchValue"] = searchValue;
            ViewData["TotalCategory"] = totalCategories;

            return View(categories);
        }


        // GET: Category/Details/5
        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Category/Create
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category, IFormFile CategoryImage)
        {
            if (await _context.Categories.AnyAsync(c => c.CategoryName.ToLower() == category.CategoryName.ToLower()))
            {
                ModelState.AddModelError("CategoryName", "Thể loại này đã tồn tại.");
                return View(category);
            }

            if (CategoryImage != null && CategoryImage.Length > 0)
            {
                try
                {
                    var imageUrl = await _categoryService.UploadImageToCloudinary(CategoryImage);
                    category.CategoryImage = imageUrl;
                    ModelState.Remove("CategoryImage");
                    if (ModelState.IsValid)
                    {
                        _context.Add(category);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }

                    else
                    {
                        ModelState.AddModelError("CategoryImage", "Không thể upload ảnh lên Cloudinary.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("CategoryImage", "Lỗi khi upload ảnh: " + ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("CategoryImage", "Vui lòng chọn một ảnh.");
            }
            return View(category);
        }


        // GET: Category/Edit/5
        // Có thể dùng [HttpGet("Edit")]
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Category/Edit/5
        // Có thể dùng [HttpPost("Edit")]
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category, IFormFile? CategoryImage)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (await _context.Categories.AnyAsync(c => c.CategoryName.ToLower() == category.CategoryName.ToLower() && c.CategoryId != id))
            {
                ModelState.AddModelError("CategoryName", "Thể loại này đã tồn tại.");
                var existingCategory = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.CategoryId == id);
                if (existingCategory != null)
                {
                    category.CategoryImage = existingCategory.CategoryImage;
                }
                return View(category);
            }

            var existingCategoryForEdit = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.CategoryId == id);
            if (existingCategoryForEdit == null)
            {
                return NotFound();
            }

            if (CategoryImage != null && CategoryImage.Length > 0)
            {
                var imageUrl = await _categoryService.UploadImageToCloudinary(CategoryImage);
                category.CategoryImage = imageUrl;
            }
            else
            {
                category.CategoryImage = existingCategoryForEdit.CategoryImage;
                ModelState.Remove("CategoryImage");
            }

            try
            {
                category.UpdatedAt = DateTime.Now;
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.CategoryId))
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

        // GET: Category/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost("Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoryInUse = await _context.Products.AnyAsync(p => p.CategoryId == id);

            if (categoryInUse)
            {
                // Thêm thông báo lỗi vào ModelState
                ModelState.AddModelError("", "Không thể xóa thể loại này vì nó đang được sử dụng trong các sản phẩm.");

                // Truyền lại thể loại vào view để hiển thị
                var category = await _context.Categories.FindAsync(id);
                return View(category);
            }

            var categoryToDelete = await _context.Categories.FindAsync(id);
            if (categoryToDelete != null)
            {
                _context.Categories.Remove(categoryToDelete);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}



