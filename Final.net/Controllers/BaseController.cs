using Final.net.Services;
using Microsoft.AspNetCore.Mvc;

namespace Final.net.Controllers
{
    public class BaseController : Controller
    {
        protected readonly CartService _cartService;

        public BaseController(CartService cartService)
        {
            _cartService = cartService;
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            // Lấy danh sách sản phẩm trong giỏ hàng
            var cart = _cartService.GetCartItems();

            // Cập nhật tổng số lượng sản phẩm
            ViewBag.CartItemCount = cart?.Sum(item => item.Quantity) ?? 0;

            base.OnActionExecuting(context);
        }
    }
}
