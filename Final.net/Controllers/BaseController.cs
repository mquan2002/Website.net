using Final.net.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Final.net.Controllers
{
    public class BaseController : Controller
    {
        protected readonly PizzaStoreContext _context;

        public BaseController(PizzaStoreContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            if (User.Identity.IsAuthenticated)
            {
                // Lấy UserId từ thông tin đăng nhập
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    // Lấy tổng số lượng sản phẩm trong giỏ hàng của user
                    var cartItemCount = _context.CartItems
                                                .Where(ci => ci.UserId == userId)
                                                .Sum(ci => ci.Quantity);

                    ViewBag.CartItemCount = cartItemCount;
                }
                else
                {
                    // Trường hợp UserId không tồn tại hoặc không hợp lệ
                    ViewBag.CartItemCount = 0;
                }
            }
            else
            {
                // Nếu user chưa đăng nhập, giỏ hàng sẽ rỗng
                ViewBag.CartItemCount = 0;
            }

            base.OnActionExecuting(context);
        }
    }
}
