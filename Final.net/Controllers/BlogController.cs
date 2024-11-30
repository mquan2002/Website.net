using Microsoft.AspNetCore.Mvc;
using Final.net.Models;
using System.Linq;

namespace Final.net.Controllers
{
    public class BlogController : Controller
    {
        private readonly PizzaStoreContext _context;

        public BlogController(PizzaStoreContext context)
        {
            _context = context;
        }

        // Phương thức Index để hiển thị danh sách bài viết
        public IActionResult Index()
        {
            var blogs = _context.Blogs.Where(b => b.Active).ToList(); // Lấy tất cả các bài viết đang hoạt động
            return View(blogs); // Trả về View với dữ liệu blog
        }

        // Phương thức Details để hiển thị chi tiết bài viết
        public IActionResult Details(int id)
        {
            var blog = _context.Blogs.FirstOrDefault(b => b.Id == id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }
    }
}
