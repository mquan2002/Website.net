﻿using Microsoft.AspNetCore.Mvc;
using Final.net.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Final.net.Services;

namespace Final.net.Controllers
{
    public class CartController : BaseController
    {
        private readonly EmailService _emailService;
        public CartController(PizzaStoreContext context, EmailService emailService) : base(context)
        {
            _emailService = emailService;
        }

        // Hiển thị giỏ hàng
        public IActionResult Index()
        {
            var cart = GetCartItems();
            ViewBag.CartItemCount = cart.Sum(item => item.Quantity);
            double totalPrice = cart.Sum(item => item.TotalPrice);
            ViewBag.TotalPriceOrder = totalPrice;
            return View(cart);
        }

        public IActionResult ApplyDiscount(string voucherCode, double totalPriceOrder)
        {
            if (!string.IsNullOrEmpty(voucherCode))
            {
                // Voucher cho hoá đơn từ 500.000
                Voucher? validVoucher = _context.Vouchers
                    .FirstOrDefault(x => x.VoucherCode.Trim() == voucherCode.Trim() && totalPriceOrder >= x.MinPrice && x.MinPrice >= 500000 && x.IsActive == true);
                if (validVoucher is not null)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Thành công",
                        data = new
                        {
                            NewTotalOrder = totalPriceOrder - validVoucher.DiscountPrice,
                            validVoucher = validVoucher
                        }
                    });
                }
                return Json(new { success = false, message = "Voucher này không phù hợp" });
            }
            return Json(new { success = false, message = "Không nhận được dữ liệu" });
        }

        public IActionResult GetListVoucher()
        {
            return Json(_context.Vouchers.ToList());
        }

        // Lấy danh sách sản phẩm trong giỏ hàng của user
        private List<CartItem> GetCartItems()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new List<CartItem>();
            }

            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");



            return _context.CartItems
                .Include(c => c.Size)
                .Include(c => c.Crust)
                .Where(c => c.UserId == userId)
                .ToList();
        }

        // Hiển thị modal thêm vào giỏ hàng
        public IActionResult ShowAddToCartModal(int productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Crusts = _context.Crusts.ToList();
            ViewBag.Sizes = _context.Sizes.ToList();
            ViewBag.ProductId = productId;
            ViewBag.ProductName = product.ProductName;
            ViewBag.ProductPrice = product.Price;
            ViewBag.ImageUrl = product.ImageUrl;

            return PartialView("_AddToCartModal");
        }
        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public IActionResult AddToCart(int productId, string productName, double price, string imageUrl, int sizeId, int crustId, int quantity = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("Bạn cần đăng nhập để thêm sản phẩm vào giỏ hàng.");
            }

            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");

            var size = _context.Sizes.FirstOrDefault(s => s.SizeId == sizeId);
            var crust = _context.Crusts.FirstOrDefault(c => c.CrustId == crustId);

            if (size == null || crust == null)
            {
                return BadRequest("Size hoặc Crust không hợp lệ.");
            }

            var cartItem = _context.CartItems.FirstOrDefault(item =>
                item.ProductId == productId &&
                item.SizeId == sizeId &&
                item.CrustId == crustId &&
                item.UserId == userId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    ProductId = productId,
                    ProductName = productName,
                    BasePrice = price,
                    Quantity = quantity,
                    ImageUrl = imageUrl,
                    SizeId = sizeId,
                    CrustId = crustId,
                    Size = size,
                    Crust = crust,
                    UserId = userId
                };

                _context.CartItems.Add(cartItem);
            }

            _context.SaveChanges();
            return Json(new { success = true, message = "Sản phẩm đã được thêm vào giỏ hàng!" });
        }

        // Sửa thông tin sản phẩm trong giỏ hàng
        [HttpPost]
        public IActionResult EditCart(int productId, int sizeId, int crustId, int quantity)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");

            var cart = GetCartItems();

            var cartItem = cart.FirstOrDefault(item => item.ProductId == productId);
            if (cartItem != null)
            {

                // Cập nhật số lượng
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

                _context.SaveChanges();
                return Json(new { success = true, message = "Cập nhật sản phẩm thành công!" });
            }

            return NotFound("Không tìm thấy sản phẩm trong giỏ hàng.");
        }
        public IActionResult ShowEditCartItemModal(int productId, int sizeId, int crustId)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");

            // Lấy sản phẩm từ giỏ hàng theo UserId
            var cartItem = _context.CartItems.FirstOrDefault(item =>
                item.ProductId == productId &&
                item.SizeId == sizeId &&
                item.CrustId == crustId &&
                item.UserId == userId);

            if (cartItem == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng." });
            }

            // Lấy danh sách Size và Crust từ database
            ViewBag.Sizes = _context.Sizes.ToList();
            ViewBag.Crusts = _context.Crusts.ToList();
            ViewBag.ProductName = cartItem.ProductName;

            return PartialView("_EditCartModal", cartItem);
        }

        // Xóa sản phẩm khỏi giỏ hàng
        [HttpPost]
        public IActionResult RemoveFromCart(int productId, int sizeId, int crustId)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");

            var cartItem = _context.CartItems.FirstOrDefault(item =>
                item.ProductId == productId &&
                item.SizeId == sizeId &&
                item.CrustId == crustId &&
                item.UserId == userId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }

            return Json(new { success = true, message = "Sản phẩm đã được xóa khỏi giỏ hàng!" });
        }
        [HttpPost]
        public IActionResult PaymentPOST(double? totalPayment)
        {
            if (totalPayment == null || (totalPayment.HasValue && double.IsNaN(totalPayment.Value)))
            {
                TempData["totalPayment"] = null;
            }
            else
            {
                TempData["totalPayment"] = totalPayment.ToString();
            }
            var url = Url.Action("Payment", "Cart");
            return Json(new {url = url});
        }

        [HttpGet]
        public IActionResult Payment()
        {
            // Lấy userId từ session
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);



            if (user == null)
            {
                return NotFound("User not found");
            }
            var paymentMethods = _context.Payments.ToList();

            // Truyền dữ liệu phương thức thanh toán vào View
            ViewBag.PaymentMethods = paymentMethods;
            // Lấy các sản phẩm trong giỏ hàng
            var cart = GetCartItems();
            double totalPrice = 0;
            if (TempData["totalPayment"] == null)
            {
                foreach (var item in cart)
                {
                    double itemPrice = item.BasePrice;

                // Kiểm tra và tính lại giá theo kích thước
                if (item.Size != null) // Kiểm tra nếu có size
                {
                    if (item.Size.SizeName == "Cỡ 7 inch")
                    {
                        // Giả sử không có thay đổi giá cho kích thước 7 inch
                        itemPrice += 0;
                    }
                    else if (item.Size.SizeName == "Cỡ 9 inch")
                    {
                        itemPrice += 80000; // Cộng thêm 80,000 cho kích thước 9 inch
                    }
                    else if (item.Size.SizeName == "Cỡ 12 inch")
                    {
                        itemPrice += 150000; // Cộng thêm 150,000 cho kích thước 12 inch
                    }
                }

                    // Tính tổng giá sản phẩm sau khi thay đổi
                    totalPrice += itemPrice * item.Quantity;
                }
            }
            else
                totalPrice = Convert.ToDouble(TempData["totalPayment"]);

            // Lưu tổng giá vào ViewBag để sử dụng trong View
            ViewBag.TotalPrice = totalPrice;

            return View(user);
        }
        // Thanh toán giỏ hàng
        [HttpPost]
        public IActionResult ConfirmPayment(User updatedUser, string notes, string paymentMethod)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Kiểm tra và gán giá trị mặc định cho 'notes' nếu cần
            notes = string.IsNullOrWhiteSpace(notes) ? string.Empty : notes;

            // Update user information
            user.Address = string.IsNullOrWhiteSpace(updatedUser.Address) ? user.Address : updatedUser.Address;
            user.Phone = string.IsNullOrWhiteSpace(updatedUser.Phone) ? user.Phone : updatedUser.Phone;
            user.Notes = string.IsNullOrWhiteSpace(updatedUser.Notes) ? user.Notes : updatedUser.Notes;
            _context.SaveChanges();
            //var cart = _context.CartItems.Where(c => c.UserId == userId).ToList();
            var cart = _context.CartItems
                .Include(c => c.Size)
                .Include(c => c.Crust)
                .Where(c => c.UserId == userId)
                .ToList();


            if (!cart.Any())
            {
                return BadRequest("Giỏ hàng trống.");
            }

            double totalPrice = cart.Sum(item => (item.BasePrice   + (item.Size.SizeCost ?? 0)) * item.Quantity);
            // var newDelivery = new Delivery
            // {
                  
            //     DeliveryStatus = "Pending",             // Trạng thái giao hàng

            // };

            var payment = _context.Payments.FirstOrDefault(p => p.Method == paymentMethod);
            if (payment == null)
            {
                return BadRequest("Phương thức thanh toán không hợp lệ.");
            }

            var newOrder = new Order
            {
                Address = string.IsNullOrWhiteSpace(updatedUser.Address) ? user.Address : updatedUser.Address, // Cung cấp giá trị cho Address
                SDT = string.IsNullOrWhiteSpace(updatedUser.Phone) ? user.Phone : updatedUser.Phone, // Cung cấp giá trị cho Phone
                TotalAmount = totalPrice,
                OrderDate = DateTime.Now,
                UserId = userId,
                DeliveryId = 1,
                PaymentId = payment.PaymentId,
                // PaymentStatus = newDelivery.DeliveryStatus,
                // PaymentMethod = payment.Method,
                Notes = notes,
            };

            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            foreach (var item in cart)
            {
                var orderItem = new OrderItem
                {
                    OrderId = newOrder.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.BasePrice,
                    Price = (item.BasePrice   + (item.Size.SizeCost ?? 0))* item.Quantity,
                    SizeId = item.SizeId,
                    CrustId = item.CrustId
                };

                _context.OrderItem.Add(orderItem);
            }
            _context.SaveChanges();
            // Xóa giỏ hàng sau khi thanh toán thành công
            _context.CartItems.RemoveRange(cart);
            _context.SaveChanges();

            // Tạo nội dung email
            string productDetails = string.Join("", cart.Select(item =>
         $"<tr>" +
         $"<td>{item.ProductName}</td>" +
         $"<td>{item.Size?.SizeName ?? "N/A"}</td>" +
         $"<td>{item.Crust?.CrustName ?? "N/A"}</td>" +
         $"<td>{item.Quantity}</td>" +
         $"<td>{(item.BasePrice + (item.Size?.SizeName == "Cỡ 9 inch" ? 80000 : item.Size?.SizeName == "Cỡ 12 inch" ? 150000 : 0)) * item.Quantity:N0} VND</td>" +
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

        // Đếm số lượng sản phẩm trong giỏ hàng
        [HttpGet]
        public JsonResult GetCartItemCount()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { count = 0 });
            }

            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");

            var count = _context.CartItems
                .Where(c => c.UserId == userId)
                .Sum(c => c.Quantity);

            return Json(new { count });
        }
    }
}
