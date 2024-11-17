using Final.net.Models;
using Final.net.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Final.net.Controllers
{
    public class MenuController : BaseController
    {
        private readonly PizzaStoreContext _context;
        public MenuController(CartService cartService, PizzaStoreContext context) : base(cartService)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var categories = _context.Categories.Include(c => c.Products).ToList();
            return View(categories);
        }
    }
}
