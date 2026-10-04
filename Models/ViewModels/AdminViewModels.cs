using NhaGiaKim.Models.Entity;

namespace NhaGiaKim.Models.ViewModels;

public class AdminOrdersViewModel
{
    public List<Order> Orders { get; set; } = new();
    public List<OrderStatus> Statuses { get; set; } = new();
    public Dictionary<long, int> Counts { get; set; } = new();
    public long? FilterStatusId { get; set; }

    // Thống kê tổng quan (không tính đơn đã hủy)
    public int TotalQuantity { get; set; }
    public decimal TotalRevenue { get; set; }
    public int FeedbackCount { get; set; }
}