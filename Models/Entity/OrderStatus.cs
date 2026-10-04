using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhaGiaKim.Models.Entity;

[Table("order_status")]
public class OrderStatus
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("status_id")]
    public long StatusId { get; set; }

    [Column("status_name")] public string StatusName { get; set; } = "";
    [Column("description")] public string? Description { get; set; }
    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

/// <summary>Tên các trạng thái (khớp dữ liệu seed) và luật chuyển trạng thái.</summary>
public static class OrderStatusNames
{
    public const string Pending = "Chờ xác nhận";
    public const string Confirmed = "Đã xác nhận";
    public const string Shipping = "Đang giao hàng";
    public const string Completed = "Hoàn thành";
    public const string Cancelled = "Đã hủy";

    private static readonly Dictionary<string, string[]> Next = new()
    {
        [Pending] = new[] { Confirmed, Cancelled },
        [Confirmed] = new[] { Shipping, Cancelled },
        [Shipping] = new[] { Completed, Cancelled },
        [Completed] = Array.Empty<string>(),
        [Cancelled] = Array.Empty<string>(),
    };

    public static string[] NextOf(string current) =>
        Next.TryGetValue(current, out var list) ? list : Array.Empty<string>();

    public static bool CanMove(string from, string to) => NextOf(from).Contains(to);

    public static string ActionLabel(string target) => target switch
    {
        Confirmed => "Xác nhận đơn",
        Shipping => "Giao hàng",
        Completed => "Hoàn thành",
        Cancelled => "Hủy đơn",
        _ => target
    };

    public static string BadgeCss(string name) => name switch
    {
        Pending => "bg-[#F5F5F5] text-[#666666] border border-[#E5E5E5]",
        Confirmed => "bg-white text-[#111111] border border-[#111111]",
        Shipping => "bg-[#111111] text-white border border-[#111111]",
        Completed => "bg-[#F5F5F5] text-[#111111] border border-[#111111]",
        Cancelled => "bg-white text-[#999999] border border-[#E5E5E5] line-through",
        _ => "bg-[#F5F5F5] text-[#666666] border border-[#E5E5E5]"
    };
}
