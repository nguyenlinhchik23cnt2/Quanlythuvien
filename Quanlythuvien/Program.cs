using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Quanlythuvien.Models;

var builder = WebApplication.CreateBuilder(args);

// =======================
// 🧱 Cấu hình Services
// =======================

builder.Services.AddControllersWithViews();

// Cho phép truy cập HttpContext trong các service
builder.Services.AddHttpContextAccessor();

// Cấu hình Session (dùng để lưu thông tin đăng nhập)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Kết nối database
var connectionString = builder.Configuration.GetConnectionString("DbConnet");
builder.Services.AddDbContext<QuanlythuvienDbContext>(options =>
    options.UseSqlServer(connectionString));

// Cấu hình xác thực cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LogoutPath = "/Auth/Logout";
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

var app = builder.Build();

// =======================
// ⚙️ Middleware pipeline
// =======================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Bật Session trước Authentication
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// =======================
// 🌐 Cấu hình route mặc định
// =======================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
