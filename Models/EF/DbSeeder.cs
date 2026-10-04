using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NhaGiaKim.Models.Entity;

namespace NhaGiaKim.Models.EF;

public static class DbSeeder
{
    // Tài khoản demo — đổi mật khẩu trước khi đưa lên môi trường thật
    public const string AdminUser = "admin";
    public const string AdminPass = "Admin@123";
    public const string CustomerUser = "khachhang";
    public const string CustomerPass = "Khach@123";

    public static async Task SeedAccountsAsync(IServiceProvider sp)
    {
        var db = sp.GetRequiredService<ApplicationDbContext>();
        var hasher = sp.GetRequiredService<IPasswordHasher<Account>>();

        if (!await db.Accounts.AnyAsync(a => a.Username == AdminUser))
        {
            var admin = new Account
            {
                Username = AdminUser, FullName = "Quản trị viên",
                Email = "admin@nhagiakim.vn", Role = AppRoles.Admin
            };
            admin.Password = hasher.HashPassword(admin, AdminPass);
            db.Accounts.Add(admin);
        }

        if (!await db.Accounts.AnyAsync(a => a.Username == CustomerUser))
        {
            var customer = new Account
            {
                Username = CustomerUser, FullName = "Nguyễn Văn A",
                Email = "khachhang@example.com", Role = AppRoles.Customer
            };
            customer.Password = hasher.HashPassword(customer, CustomerPass);
            db.Accounts.Add(customer);
        }

        await db.SaveChangesAsync();
    }
}
