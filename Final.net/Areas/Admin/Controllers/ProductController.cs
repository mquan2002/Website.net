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
        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 5;  

            var totalProducts = await _context.Products.CountAsync();

            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            var products = await _context.Products
                .Skip((page - 1) * pageSize)  
                .Take(pageSize) 
                .Include(p => p.Category)   
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            return View(products);
        }

        [HttpGet("Detail/{id}")]
        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Product/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile ImageUrl)
        {
            if (await _context.Products.AnyAsync(c => c.ProductName.ToLower() == product.ProductName.ToLower()))
            {
                ModelState.AddModelError("ProductName", "Sản phẩm này đã tồn tại.");
                ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
                return View(product); // Trả lại model với dữ liệu đã nhập
            }

            if (ImageUrl != null && ImageUrl.Length > 0)
            {
                try
                {
                    var imageUrl = await _productService.UploadImageToCloudinary(ImageUrl);
                    product.ImageUrl = imageUrl;
                    ModelState.Remove("ImageUrl");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ImageUrl", "Lỗi khi upload ảnh: " + ex.Message);
                    return View(product); // Trả lại model khi có lỗi upload ảnh
                }
            }
            else
            {
                ModelState.AddModelError("ImageUrl", "Vui lòng chọn một ảnh.");
                return View(product); // Trả lại model khi không chọn ảnh
            }

            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Trả lại view với thông báo lỗi nếu ModelState không hợp lệ
            return View(product);
        }



        [HttpGet("Edit/{id}")]
        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
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
            if (id != product.ProductId)
            {
                return NotFound();
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
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
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
