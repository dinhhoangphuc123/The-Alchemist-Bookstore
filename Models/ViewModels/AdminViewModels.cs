using NhaGiaKim.Models.Entity;

namespace NhaGiaKim.Models.ViewModels;

public class AdminOrdersViewModel
{
    public List<Order> Orders { get; set; } = new();
    public List<OrderStatus> Statuses { get; set; } = new();
    public Dictionary<long, int> Counts { get; set; } = new();
    public long? FilterStatusId { get; set; }
}

public class AdminFeedbacksViewModel
{
    public List<Feedback> Items { get; set; } = new();
    public string? FilterStatus { get; set; }
}
