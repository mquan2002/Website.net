using Microsoft.AspNetCore.Mvc;
using Final.net.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class StoresController : Controller
{
    private readonly PizzaStoreContext _context;

    public StoresController(PizzaStoreContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var stores = _context.Stores.ToList(); // Lấy danh sách cửa hàng từ database
        return View(stores);  // Trả về view hiển thị danh sách cửa hàng
    }

    // API để lấy cửa hàng với tọa độ từ cơ sở dữ liệu
    public async Task<IActionResult> GetStoresWithCoordinates()
    {
        var stores = _context.Stores.ToList(); // Lấy danh sách cửa hàng từ database
        var storesWithCoordinates = new List<object>();

        foreach (var store in stores)
        {
            var coordinates = await GetCoordinatesFromAddress(store.Address);
            storesWithCoordinates.Add(new
            {
                store.Name,
                store.Address,
                store.Description,
                Latitude = coordinates.Item1,
                Longitude = coordinates.Item2
            });
        }

        return Json(storesWithCoordinates);
    }

    // Hàm gọi Google Geocoding API để lấy tọa độ từ địa chỉ
    private async Task<Tuple<double, double>> GetCoordinatesFromAddress(string address)
    {
        var apiKey = "AIzaSyANikJbHGEqE8QbOEtOBOvAXOoUjMy8rIA"; // Thay thế bằng API key của bạn
        var geocodingUrl = $"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key={apiKey}";

        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.GetStringAsync(geocodingUrl);
            var jsonResponse = JObject.Parse(response);

            if (jsonResponse["status"].ToString() == "OK")
            {
                var lat = (double)jsonResponse["results"][0]["geometry"]["location"]["lat"];
                var lng = (double)jsonResponse["results"][0]["geometry"]["location"]["lng"];
                return new Tuple<double, double>(lat, lng);
            }

            // Nếu không tìm thấy tọa độ, trả về tọa độ mặc định (0,0)
            return new Tuple<double, double>(0, 0);
        }
    }
}
