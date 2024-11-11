using Microsoft.Extensions.Configuration;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
namespace Final.net.Areas.Admin.CategoryService;
public class CategoryService 
{
    private readonly IConfiguration _configuration;

    public CategoryService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> UploadImageToCloudinary(IFormFile CategoryImage)
    {
        try
        {
             var cloudName = _configuration["CloudinarySettings:CloudName"];
            var apiKey = _configuration["CloudinarySettings:ApiKey"];
            var apiSecret = _configuration["CloudinarySettings:ApiSecret"];

            var cloudinaryAccount = new Account(cloudName, apiKey, apiSecret);
            var cloudinary = new Cloudinary(cloudinaryAccount);
            
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(CategoryImage.FileName, CategoryImage.OpenReadStream())
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadResult.Url.ToString();
            }
            else
            {
                throw new Exception("Không thể upload ảnh lên Cloudinary.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Lỗi khi upload ảnh: " + ex.Message);
        }
    }
}
