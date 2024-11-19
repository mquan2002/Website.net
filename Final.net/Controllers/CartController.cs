using Microsoft.AspNetCore.Mvc;
using Final.net.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Linq;
using Final.net.Services;

namespace Final.net.Controllers
{
    public class CartController : BaseController
    {
        private const string CartSessionKey = "CartSession";
        private readonly PizzaStoreContext _context;

        public CartController(CartService cartService, PizzaStoreContext context) : base(cartService)
        {
            _context = context;
        }

        // Phương thức để hiển thị giỏ hàng
        public IActionResult Index()
        {
            var cart = GetCartItems();
            ViewBag.CartItemCount = cart.Sum(item => item.Quantity);
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

        public IActionResult ShowEditCartItemModal(int productId, int sizeId, int crustId)
        {
            var cart = GetCartItems();

            // Lấy sản phẩm từ giỏ hàng
            var cartItem = cart.FirstOrDefault(item =>
                item.ProductId == productId &&
                item.SizeId == sizeId &&
                item.CrustId == crustId);

            if (cartItem == null)
            {
                return NotFound("Không tìm thấy sản phẩm trong giỏ hàng.");
            }

            // Lấy danh sách Size và Crust từ database
            ViewBag.Sizes = _context.Sizes.ToList();
            ViewBag.Crusts = _context.Crusts.ToList();
            ViewBag.ProductName = cartItem.ProductName;

            return PartialView("_EditCartModal", cartItem);
        }


        // Phương thức thêm sản phẩm vào giỏ hàng với Size và Crust
        [HttpPost]
        public IActionResult AddToCart(int productId, string productName, double price, string imageUrl, int sizeId, int crustId, int quantity = 1)
        {
            var cart = GetCartItems();

            // Kiểm tra Size và Crust từ database
            var size = _context.Sizes.FirstOrDefault(s => s.SizeId == sizeId);
            var crust = _context.Crusts.FirstOrDefault(c => c.CrustId == crustId);

            if (size == null || crust == null)
            {
                return BadRequest("Size hoặc Crust không hợp lệ.");
            }

            // Kiểm tra sản phẩm đã có trong giỏ hàng hay chưa
            var cartItem = cart.FirstOrDefault(item =>
                item.ProductId == productId &&
                item.SizeId == sizeId &&
                item.CrustId == crustId &&
                item.ProductName == productName);

            if (cartItem != null)
            {
                // Nếu sản phẩm đã tồn tại, tăng số lượng
                cartItem.Quantity += quantity;
            }
            else
            {
                // Thêm sản phẩm mới vào giỏ hàng
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = productName,
                    BasePrice = price, // Lưu giá gốc
                    Quantity = quantity,
                    ImageUrl = imageUrl,
                    SizeId = sizeId,
                    CrustId = crustId,
                    Size = size,
                    Crust = crust
                });
            }

            SaveCartSession(cart);
            ViewBag.CartItemCount = cart.Sum(item => item.Quantity);

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
                ViewBag.CartItemCount = cart.Sum(item => item.Quantity);
            }

            return Json(new { success = true, message = "Sản phẩm đã được xóa khỏi giỏ hàng!" });
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

        [HttpGet]
        public JsonResult GetCartItemCount()
        {
            var cart = GetCartItems();
            return Json(new { count = cart.Sum(item => item.Quantity) });
        }


        [HttpPost]
        public IActionResult EditCart(int productId, int sizeId, int crustId, int quantity)
        {
            var cart = GetCartItems();

            var cartItem = cart.FirstOrDefault(item => item.ProductId == productId);
            if (cartItem != null)
            {
                // Lấy thông tin Size và Crust từ database
                var size = _context.Sizes.FirstOrDefault(s => s.SizeId == sizeId);
                var crust = _context.Crusts.FirstOrDefault(c => c.CrustId == crustId);

                if (size == null || crust == null)
                {
                    return Json(new { success = false, message = "Size hoặc Crust không hợp lệ." });
                }

                // Cập nhật thông tin sản phẩm
                cartItem.SizeId = sizeId;
                cartItem.CrustId = crustId;
                cartItem.Size = size;
                cartItem.Crust = crust;
                cartItem.Quantity = quantity;
            }

            SaveCartSession(cart);
            ViewBag.CartItemCount = cart.Sum(item => item.Quantity);

            return Json(new { success = true, message = "Sản phẩm đã được cập nhật!" });
        }







        [HttpPost]
        public IActionResult UpdateCartItem(int productId, int sizeId, int crustId, int newSizeId, int newCrustId, int quantity)
        {
            var cart = GetCartItems();

            var cartItem = cart.FirstOrDefault(item =>
                item.ProductId == productId &&
                item.SizeId == sizeId &&
                item.CrustId == crustId);

            if (cartItem != null)
            {
                // Lấy thông tin size và crust mới từ database
                var newSize = _context.Sizes.FirstOrDefault(s => s.SizeId == newSizeId);
                var newCrust = _context.Crusts.FirstOrDefault(c => c.CrustId == newCrustId);

                if (newSize == null || newCrust == null)
                {
                    return BadRequest("Size hoặc Crust không hợp lệ.");
                }

                // Cập nhật thông tin sản phẩm
                cartItem.SizeId = newSizeId;
                cartItem.CrustId = newCrustId;
                cartItem.Size = newSize;
                cartItem.Crust = newCrust;
                cartItem.Quantity = quantity;
            }

            SaveCartSession(cart);
            
            return Json(new { success = true, message = "Sản phẩm đã được cập nhật!" });
        }



        public IActionResult RefreshCartIcon()
        {
            var cart = GetCartItems();
            ViewBag.CartItemCount = cart.Sum(item => item.Quantity);
            return PartialView("_CartIcon");
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
