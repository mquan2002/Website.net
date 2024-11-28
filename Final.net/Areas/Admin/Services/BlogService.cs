using Microsoft.Extensions.Configuration;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Final.net.Areas.Admin.BlogService
{
    public class BlogService
    {
        private readonly IConfiguration _configuration;

        public BlogService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadImageToCloudinary(IFormFile blogImage)
        {
            try
            {
                // Load Cloudinary configuration from appsettings.json
                var cloudName = _configuration["CloudinarySettings:CloudName"];
                var apiKey = _configuration["CloudinarySettings:ApiKey"];
                var apiSecret = _configuration["CloudinarySettings:ApiSecret"];

                // Initialize Cloudinary account
                var cloudinaryAccount = new Account(cloudName, apiKey, apiSecret);
                var cloudinary = new Cloudinary(cloudinaryAccount);

                // Prepare the upload parameters
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(blogImage.FileName, blogImage.OpenReadStream()),
                    Folder = "Blogs", // Optional: Store blog images in a specific folder
                    UseFilename = true,
                    UniqueFilename = true
                };

                // Execute the upload
                var uploadResult = await cloudinary.UploadAsync(uploadParams);

                // Return the URL of the uploaded image if successful
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
}
