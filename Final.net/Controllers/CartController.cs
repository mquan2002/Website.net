using Microsoft.AspNetCore.Mvc;
using Final.net.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Linq;
using Final.net.Services;
using Microsoft.EntityFrameworkCore;

namespace Final.net.Controllers
{
    public class CartController : BaseController
    {
        private const string CartSessionKey = "CartSession";
        private readonly PizzaStoreContext _context;
        private readonly EmailService _emailService;

        // Constructor with PizzaStoreContext and EmailService injected
        public CartController(CartService cartService, PizzaStoreContext context, EmailService emailService) : base(cartService)
        {
            _context = context;
            _emailService = emailService;
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
            var session = HttpContext.Session.GetString("CartSession");
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
        [HttpGet]
        public IActionResult Payment()
        {
            // Lấy userId từ session
            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var cart = GetCartItems();
            double totalPrice = cart.Sum(item => item.BasePrice * item.Quantity);
            ViewBag.TotalPrice = totalPrice;

            return View(user);
        }
        [HttpPost]
        public IActionResult ConfirmPayment(User updatedUser, string paymentMethod, string notes)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (string.IsNullOrEmpty(notes))
            {
                notes = ""; // or some default value
            }

            updatedUser.Notes = notes;

            // Check if the address is empty or null
            if (string.IsNullOrWhiteSpace(user.Address))
            {
                return BadRequest("Vui lòng cung cấp địa chỉ giao hàng.");
            }

            // Cập nhật thông tin người dùng
            user.Address = string.IsNullOrWhiteSpace(updatedUser.Address) ? user.Address : updatedUser.Address;
            user.Phone = string.IsNullOrWhiteSpace(updatedUser.Phone) ? user.Phone : updatedUser.Phone;
            _context.SaveChanges();

            // Lấy giỏ hàng từ Session
            var cart = GetCartItems();

            if (!cart.Any())
            {
                return BadRequest("Giỏ hàng trống.");
            }

            double totalPrice = cart.Sum(item => item.BasePrice * item.Quantity);

            // Get or create a delivery for the user
            var delivery = _context.Deliveries.FirstOrDefault(d => d.DeliveryId == userId && d.DeliveryStatus == "Pending");
            if (delivery == null)
            {
                // Create a new delivery record if not found
                delivery = new Delivery
                {
                   
                    DeliveryStatus = "Pending",
                    
                };
                _context.Deliveries.Add(delivery);
                _context.SaveChanges();
            }

            var payment = _context.Payments.FirstOrDefault(p => p.Method == paymentMethod);
            if (payment == null)
            {
                // Create a new payment record if not found
                payment = new Payment
                {
                    Method = paymentMethod,
                  
                };
                _context.Payments.Add(payment);
                _context.SaveChanges(); // Save changes to generate PaymentId
            }

            var newOrder = new Order
            {
                UserId = userId,
                TotalAmount = totalPrice,
                OrderDate = DateTime.Now,
                PaymentStatus = "Pending",
                PaymentMethod = paymentMethod,  // Sử dụng thông tin phương thức thanh toán từ form
                Notes = notes,
                Address = user.Address,
                SDT = user.Phone,
                DeliveryId = delivery.DeliveryId,  // Add the DeliveryId to the Order
                PaymentId = payment.PaymentId
            };

            _context.Orders.Add(newOrder);

            // Truy vấn lại các đối tượng từ cơ sở dữ liệu và thực hiện xóa
            var cartItemsToDelete = _context.CartItems
                .Where(c => cart.Select(i => i.CartItemId).Contains(c.CartItemId))
                .ToList();

            _context.CartItems.RemoveRange(cartItemsToDelete);
            _context.SaveChanges();

            // Tạo nội dung email
            string productDetails = string.Join("", cart.Select(item =>
                $"<tr>" +
                $"<td>{item.ProductName}</td>" +
                $"<td>{item.Size?.SizeName ?? "N/A"}</td>" +
                $"<td>{item.Crust?.CrustName ?? "N/A"}</td>" +
                $"<td>{item.Quantity}</td>" +
                $"<td>{item.TotalPrice:N0} VND</td>" +
                $"</tr>"));

            string emailBody = $@"
<h1>Cảm ơn bạn đã đặt hàng tại Pizza Store!</h1>
<p><strong>Tên khách hàng:</strong> {user.Username}</p>
<p><strong>Email:</strong> {user.Email}</p>
<p><strong>Địa chỉ giao hàng:</strong> {user.Address}</p>
<p><strong>Số điện thoại:</strong> {user.Phone}</p>
<p><strong>Tổng tiền:</strong> {totalPrice:N0} VND</p>
<br/>
<h2>Chi tiết đơn hàng:</h2>
<table border='1' cellpadding='5' cellspacing='0' style='border-collapse: collapse;'>
    <thead>
        <tr>
            <th>Sản phẩm</th>
            <th>Kích thước</th>
            <th>Loại</th>
            <th>Số lượng</th>
            <th>Thành tiền</th>
        </tr>
    </thead>
    <tbody>
        {productDetails}
    </tbody>
</table>
<br/>
<p>Đơn hàng của bạn đang được xử lý và sẽ sớm được giao.</p>
<p>Chúc bạn ngon miệng!</p>";

            // Gửi email
            _emailService.SendEmail(user.Email, "Xác nhận đơn hàng", emailBody);
            TempData["SuccessMessage"] = "Thanh toán thành công! Cảm ơn bạn đã mua hàng.";
            return RedirectToAction("PaymentSuccess");
        }

        [HttpGet]
        public IActionResult PaymentSuccess()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View();
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