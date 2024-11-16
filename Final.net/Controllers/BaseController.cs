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
            var cart = _cartService.GetCartItems();
            ViewBag.CartItemCount = cart.Count;
            base.OnActionExecuting(context);
        }
    }
}
