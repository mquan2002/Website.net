using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Final.net.Models;
using Final.net.Areas.Admin.ProductService;

namespace Final.net.Areas_Admin_Controllers
{
    [Area("Admin")]
    [Route("admin/[controller]")]
    public class ProductController : Controller
    {
        private readonly PizzaStoreContext _context;
        private readonly ProductService _productService;


        public ProductController(PizzaStoreContext context, ProductService productService)
        {
            _context = context;
            _productService = productService;
        }

        // GET: Product
        [HttpGet("")]
        public async Task<IActionResult> Index(int page = 1, int searchType = 0, string searchValue = "")
        {
            var currentUserRole = HttpContext.Session.GetString("RoleId");
            if (currentUserRole != "1" && currentUserRole != "3")
            {
                return NotFound();
            }
            const int pageSize = 5;
            IQueryable<Product> query = _context.Products;

            if (searchType == 1 && !int.TryParse(searchValue, out int product222dotnet))
            {
                ViewData["Error"] = "Id sản phẩm phải là 1 số";
            }
            if (!string.IsNullOrEmpty(searchValue))
            {
                if (searchType == 1)
                {
                    if (int.TryParse(searchValue, out int productId))
                    {
                        query = query.Where(p => p.ProductId == productId);
                    }
                }
                else if (searchType == 2) // Tìm kiếm theo tên
                {
                    query = query.Where(p => p.ProductName.Contains(searchValue));
                }
            }
            var totalProducts = await query.Where(p => p.DeletedAt == null).CountAsync();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
            var products = await query
                .Where(p => p.DeletedAt == null)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Category)
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["SearchType"] = searchType;
            ViewData["SearchValue"] = searchValue;
            ViewData["TotalProduct"] = totalProducts;
            ViewData["SearchTypeName"] = searchType == 1 ? "Id" : "tên";
            var roleId = HttpContext.Session.GetString("RoleId");
            ViewData["RoleId"] = currentUserRole;
            return View(products);
        }
        [HttpGet("Detail/{id}")]
        // GET: Product/Details/5
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

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpGet("Create")]
        // GET: Product/Create
        public IActionResult Create()
        {
            var currentUserRole = HttpContext.Session.GetString("RoleId");
            if (currentUserRole != "1" && currentUserRole != "3")
            {
                return NotFound();
            }
            if (currentUserRole == "3")
            {
                return Unauthorized("Bạn không có quyền sử dụng chức năng này.");
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.DeletedAt == null), "CategoryId", "CategoryName");
            return View();
        }

        // POST: Product/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile ImageUrl)
        {
            var currentUserRole = HttpContext.Session.GetString("RoleId");
            if (currentUserRole != "1" && currentUserRole != "3")
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(product);
            }
            // if (string.IsNullOrEmpty(product.ProductName))
            // {
            //     ModelState.AddModelError("ProductName", "Tên sản phẩm không được rỗng");
            //     return View(product);
            // }
            if (await _context.Products.AnyAsync(c => c.ProductName.ToLower() == product.ProductName.ToLower()))
            {
                ModelState.AddModelError("ProductName", "Sản phẩm này đã tồn tại.");
                ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
                return View(product);
            }
            if (ImageUrl != null && ImageUrl.Length > 0)
            {
                try
                {
                    var imageUrl = await _productService.UploadImageToCloudinary(ImageUrl);
                    product.ImageUrl = imageUrl;
                    ModelState.Remove("ImageUrl");
                    // if (ModelState.IsValid)
                    // {
                    Console.WriteLine("123");
                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                    // }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ImageUrl", "Lỗi khi upload ảnh: " + ex.Message);
                    return View(product);
                }
            }
            else
            {
                ModelState.AddModelError("ImageUrl", "Vui lòng chọn một ảnh.");
                return View(product);
            }



            return View(product);
        }



        [HttpGet("Edit/{id}")]
        // GET: Product/Edit/5
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

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }


        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? ImageUrl)
        {
            var currentUserRole = HttpContext.Session.GetString("RoleId");
            if (currentUserRole != "1" && currentUserRole != "3")
            {
                return NotFound();
            }
            if (id != product.ProductId)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            if (await _context.Products.AnyAsync(c => c.ProductName.ToLower() == product.ProductName.ToLower() && c.ProductId != id))
            {
                ModelState.AddModelError("ProductName", "Sản phẩm này đã tồn tại.");
                ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
                var retriveImage = await _context.Products.AsNoTracking().FirstOrDefaultAsync(c => c.ProductId == id);
                if (retriveImage != null)
                {
                    product.ImageUrl = retriveImage.ImageUrl;
                }
                return View(product);
            }

            var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);
            if (existingProduct == null)
            {
                return NotFound();
            }
            if (ImageUrl != null && ImageUrl.Length > 0)
            {
                var imageUrl = await _productService.UploadImageToCloudinary(ImageUrl);
                product.ImageUrl = imageUrl;
            }
            else
            {
                product.ImageUrl = existingProduct.ImageUrl;
                ModelState.Remove("ImageUrl");
            }

            try
            {
                product.UpdatedAt = DateTime.Now;
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.ProductId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
            return View(product);
        }


        [HttpGet("Delete/{id}")]
        // GET: Product/Delete/5
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

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        // POST: Product/Delete/5
        [HttpPost("Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUserRole = HttpContext.Session.GetString("RoleId");
            if (currentUserRole != "1" && currentUserRole != "3")
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {

                product.DeletedAt = DateTime.UtcNow;
                // _context.Products.Remove(product);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
