using Microsoft.EntityFrameworkCore;
using NhaGiaKim.Models.EF;
using NhaGiaKim.Models.Entity;
using NhaGiaKim.Models.ViewModels;
using NhaGiaKim.Services.Interfaces;

namespace NhaGiaKim.Services;

public class AdminOrderService : IAdminOrderService
{
    private readonly ApplicationDbContext _db;

    public AdminOrderService(ApplicationDbContext db) => _db = db;

    public async Task<AdminOrdersViewModel> GetOrdersDashboardAsync(long? statusId)
    {
        IQueryable<Order> query = _db.Orders.AsNoTracking()
            .Include(o => o.Status)
            .Include(o => o.Book);

        if (statusId.HasValue)
            query = query.Where(o => o.StatusId == statusId.Value);

        // Thống kê không tính đơn đã hủy
        var valid = _db.Orders.Where(o => o.Status.StatusName != OrderStatusNames.Cancelled);

        return new AdminOrdersViewModel
        {
            TotalQuantity = await valid.SumAsync(o => o.Quantity),
            TotalRevenue = await valid.SumAsync(o => o.TotalAmount),
            FeedbackCount = await _db.Feedbacks.CountAsync(),
            FilterStatusId = statusId,
            Statuses = await _db.OrderStatuses.AsNoTracking().OrderBy(s => s.StatusId).ToListAsync(),
            Orders = await query.OrderByDescending(o => o.OrderTime).ToListAsync(),
            Counts = await _db.Orders.GroupBy(o => o.StatusId)
                .Select(g => new { g.Key, N = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.N)
        };
    }

    public async Task<StatusUpdateResult> UpdateStatusAsync(long orderId, long newStatusId)
    {
        var order = await _db.Orders.Include(o => o.Status).FirstOrDefaultAsync(o => o.OrderId == orderId);
        var target = await _db.OrderStatuses.FirstOrDefaultAsync(s => s.StatusId == newStatusId);

        if (order == null || target == null)
            return new StatusUpdateResult(false, "Không tìm thấy đơn hàng hoặc trạng thái.");

        if (!OrderStatusNames.CanMove(order.Status.StatusName, target.StatusName))
            return new StatusUpdateResult(false,
                $"Không thể chuyển đơn {order.OrderCode} từ “{order.Status.StatusName}” sang “{target.StatusName}”.");

        order.Status = target;
        await _db.SaveChangesAsync();
        return new StatusUpdateResult(true, $"Đơn {order.OrderCode} đã chuyển sang “{target.StatusName}”.");
    }
}
