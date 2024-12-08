using Final.net.Models;
using Microsoft.EntityFrameworkCore;
using Final.net.Areas.Admin.CategoryService;
using Final.net.Areas.Admin.ProductService;
using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Final.net.Services;
using Final.net.Areas.Admin.BlogService;

var builder = WebApplication.CreateBuilder(args);

// Register IHttpContextAccessor
// builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext with a single registration
builder.Services.AddDbContext<PizzaStoreContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Cloudinary settings
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddSingleton(x =>
{
    var cloudinarySettings = builder.Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
    var account = new Account(cloudinarySettings.CloudName, cloudinarySettings.ApiKey, cloudinarySettings.ApiSecret);
    return new Cloudinary(account);
});


builder.Services.AddTransient<EmailService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<BlogService>();

// Thêm cấu hình cho Session
// Thêm cấu hình cho Session
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;  // Đảm bảo cookie hoạt động đúng trên mọi nền tảng
});


builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Sign/SignIn";
        options.LogoutPath = "/Sign/Logout";
    });


// Add Cart service
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<CartService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Sử dụng Session trong ứng dụng
app.UseSession(); // Thêm dòng này để kích hoạt Session

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();




// Map area and default routes
app.MapAreaControllerRoute(
    name: "admin_category",
    areaName: "Admin",
    pattern: "admin/{controller=Category}/{action=Index}/{id?}");

app.MapAreaControllerRoute(
    name: "admin_blog",
    areaName: "Admin",
    pattern: "admin/{controller=Blog}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "blog",
    pattern: "blog/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "stores",
    pattern: "stores/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.Run();
