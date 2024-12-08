using Microsoft.AspNetCore.Mvc;
using Final.net.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

public class StoresController : Controller
{
    private readonly PizzaStoreContext _context;

    public StoresController(PizzaStoreContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var stores = _context.Stores.ToList();
        return View(stores);
    }
}

