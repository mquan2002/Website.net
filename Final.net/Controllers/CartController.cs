using Microsoft.AspNetCore.Mvc;
using Final.net.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Linq;

namespace Final.net.Controllers
{
    public class CartController : Controller
    {
        private const string CartSessionKey = "CartSession";
        private readonly PizzaStoreContext _context;

        public CartController(PizzaStoreContext context)
        {
            _context = context;
        }

        // Phương thức để hiển thị giỏ hàng
        public IActionResult Index()
        {
            var cart = GetCartItems();
            return View(cart);
        }

        // Phương thức để lấy danh sách Crusts từ database
        private List<Crust> GetCrusts()
        {
            return _context.Crusts.ToList();
        }

        // Phương thức để lấy danh sách Sizes từ database
        private List<Size> GetSizes()
        {
            return _context.Sizes.ToList();
        }

        // Phương thức để hiển thị modal thêm vào giỏ hàng với lựa chọn Crust và Size
        public IActionResult ShowAddToCartModal(int productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                return NotFound();
            }

            var crusts = GetCrusts();
            var sizes = GetSizes();

            ViewBag.Crusts = crusts;
            ViewBag.Sizes = sizes;
            ViewBag.ProductId = productId;
            ViewBag.ProductName = product.ProductName;
            ViewBag.ProductPrice = product.Price;
            ViewBag.ImageUrl = product.ImageUrl;

            return PartialView("_AddToCartModal");
        }


        // Phương thức thêm sản phẩm vào giỏ hàng với Size và Crust
        [HttpPost]
        public IActionResult AddToCart(int productId, string productName, double price, string imageUrl, int sizeId, int crustId, int quantity)
        {
            var cart = GetCartItems();

            // Kiểm tra Size và Crust từ database
            var size = _context.Sizes.FirstOrDefault(s => s.SizeId == sizeId);
            var crust = _context.Crusts.FirstOrDefault(c => c.CrustId == crustId);

            if (size == null || crust == null)
            {
                return BadRequest("Size hoặc Crust không hợp lệ.");
            }

            // Tính tổng giá sản phẩm với kích cỡ (Size)
            double finalPrice = price + (size.SizeCost ?? 0);

            // Kiểm tra xem sản phẩm đã có trong giỏ với cùng Size và Crust chưa
            var cartItem = cart.FirstOrDefault(item => item.ProductId == productId && item.SizeId == sizeId && item.CrustId == crustId);
            if (cartItem != null)
            {
                // Nếu sản phẩm đã tồn tại trong giỏ, tăng số lượng
                cartItem.Quantity += quantity;
            }
            else
            {
                // Nếu sản phẩm chưa có trong giỏ, thêm sản phẩm mới với Size và Crust
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = productName,
                    Price = finalPrice, // Sử dụng giá đã tính bao gồm SizeCost
                    Quantity = quantity,
                    ImageUrl = imageUrl,
                    SizeId = sizeId,
                    CrustId = crustId,
                    Size = size,
                    Crust = crust
                });
            }

            SaveCartSession(cart);
            return Json(new { success = true, message = "Sản phẩm đã được thêm vào giỏ hàng!" });
        }

        // Phương thức xóa sản phẩm khỏi giỏ hàng
        [HttpPost]
        public IActionResult RemoveFromCart(int productId, int sizeId, int crustId)
        {
            var cart = GetCartItems();
            var cartItem = cart.FirstOrDefault(item => item.ProductId == productId && item.SizeId == sizeId && item.CrustId == crustId);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
                SaveCartSession(cart);
            }

            return RedirectToAction("Index");
        }

        // Phương thức lấy danh sách sản phẩm trong giỏ hàng từ Session
        private List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(session))
            {
                return new List<CartItem>();
            }

            try
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(session);
            }
            catch
            {
                return new List<CartItem>();
            }
        }

        // Phương thức lưu giỏ hàng vào Session
        private void SaveCartSession(List<CartItem> cart)
        {
            try
            {
                HttpContext.Session.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
            }
            catch
            {
                // Xử lý khi session không thể lưu
            }
        }
    }
}
