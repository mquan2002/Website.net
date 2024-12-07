using Microsoft.AspNetCore.Mvc;
using Final.net.Models;
using Newtonsoft.Json;
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

        // Kiểm tra và xử lý các giá trị Latitude và Longitude nếu cần
        foreach (var store in stores)
        {
            // Kiểm tra giá trị Latitude và Longitude có NULL không trước khi sử dụng
            if (store.Latitude == null || store.Longitude == null)
            {
                // Gán tọa độ mặc định nếu không có tọa độ
                store.Latitude = store.Latitude ?? 0; // Nếu Latitude là null thì gán mặc định là 0
                store.Longitude = store.Longitude ?? 0; // Nếu Longitude là null thì gán mặc định là 0
            }
        }

        return View(stores);  // Trả về view hiển thị danh sách cửa hàng
    }




    public async Task<IActionResult> UpdateAllStoreCoordinates()
    {
        var stores = _context.Stores.ToList(); // Lấy danh sách tất cả các cửa hàng

        foreach (var store in stores)
        {
            if (store.Latitude == null || store.Longitude == null)
            {
                var coordinates = await GetCoordinatesFromAddress(store.Address); // Gọi API Geocoding

                // Kiểm tra và gán tọa độ nếu có
                if (coordinates.Latitude.HasValue && coordinates.Longitude.HasValue)
                {
                    store.Latitude = coordinates.Latitude.Value;
                    store.Longitude = coordinates.Longitude.Value;
                    _context.Stores.Update(store);
                }
                else
                {
                    store.Latitude = store.Latitude ?? 0; // Gán tọa độ mặc định nếu không có tọa độ
                    store.Longitude = store.Longitude ?? 0;
                    _context.Stores.Update(store);
                }
            }
        }

        await _context.SaveChangesAsync(); // Lưu các thay đổi vào cơ sở dữ liệu
        return Ok("Đã cập nhật tọa độ cho các cửa hàng.");
    }


    private async Task<(double? Latitude, double? Longitude)> GetCoordinatesFromAddress(string address)
    {
        var encodedAddress = Uri.EscapeDataString(address);
        var url = $"https://nominatim.openstreetmap.org/search?q={encodedAddress}&format=json";

        using (var httpClient = new HttpClient())
        {
            try
            {
                var response = await httpClient.GetStringAsync(url);

                // Kiểm tra xem dữ liệu nhận được có hợp lệ không
                if (string.IsNullOrWhiteSpace(response))
                {
                    return (null, null); // Trả về null nếu dữ liệu không hợp lệ
                }

                var result = JsonConvert.DeserializeObject<List<GeocodingResult>>(response);

                if (result != null && result.Count > 0)
                {
                    var firstResult = result.First();
                    return (Convert.ToDouble(firstResult.Lat), Convert.ToDouble(firstResult.Lon));
                }
                else
                {
                    // Trả về null nếu không có kết quả
                    return (null, null);
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần thiết
                return (null, null);
            }
        }
    }





}

// Class để ánh xạ kết quả từ API Geocoding
public class GeocodingResult
{
    public string Lat { get; set; }
    public string Lon { get; set; }
}
