using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NhaGiaKim.Models.EF;
using NhaGiaKim.Models.Entity;
using NhaGiaKim.Services;
using NhaGiaKim.Services.Interfaces;

// Các cột TIMESTAMP (không múi giờ) trong DB ↔ DateTime của .NET
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<SiteSettingsService>();

// Tầng nghiệp vụ: Controller -> Service -> ApplicationDbContext
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAdminOrderService, AdminOrderService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddSingleton<IPasswordHasher<Account>, PasswordHasher<Account>>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/dang-nhap";
        options.AccessDeniedPath = "/tu-choi-truy-cap";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// Tạo sẵn 2 tài khoản demo (admin + khách hàng) nếu chưa có
using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedAccountsAsync(scope.ServiceProvider);
}
if (!app.Environment.IsProduction())
    app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
