using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhaGiaKim.Models.Entity;

/// <summary>Tài khoản đăng nhập (bảng "admin"): cột role = admin | customer.</summary>
[Table("admin")]
public class Account
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("admin_id")]
    public long AccountId { get; set; }

    [Column("username")] public string Username { get; set; } = "";

    /// <summary>Lưu mật khẩu đã băm (PBKDF2), không lưu mật khẩu thô.</summary>
    [Column("password")] public string Password { get; set; } = "";

    [Column("full_name")] public string? FullName { get; set; }
    [Column("email")] public string? Email { get; set; }
    [Column("role")] public string Role { get; set; } = AppRoles.Customer;
    [Column("status")] public bool Status { get; set; } = true;

    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

public static class AppRoles
{
    public const string Admin = "admin";
    public const string Customer = "customer";
}
