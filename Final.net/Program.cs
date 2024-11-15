using Final.net.Models;
using Final.net.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Razor;

var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddDbContext<PizzaStoreContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("PizzeStoreContext") ?? throw new InvalidOperationException("Connection string 'PizzeStoreContext' not found.")));

builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.AreaViewLocationFormats.Clear();
    options.AreaViewLocationFormats.Add("/MyAreas/{2}/Views/{1}/{0}.cshtml");
    options.AreaViewLocationFormats.Add("/MyAreas/{2}/Views/Shared/{0}.cshtml");
    options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
});


// Add services to the container.
builder.Services.AddControllersWithViews();

// Đăng ký DBContext
builder.Services.AddDbContext<PizzaStoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Đăng ký dịch vụ services
builder.Services.AddScoped<UserService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


// app.MapControllerRoute(
//     name: "MyArea",
//     pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");

// app.MapAreaControllerRoute(
//     name: "MyAreaPayments",
//     areaName: "Admin",
//     pattern: "admin/{controller=Payments}/{action=Index}/{id?}");
// app.UseEndpoints(endpoints =>
//    {
//        endpoints.MapControllerRoute(
//            name: "admin",
//            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

//        endpoints.MapControllerRoute(
//            name: "default",
//            pattern: "{controller=Home}/{action=Index}/{id?}");
//    });

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
