using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaGiaKim.Models.EF;
using NhaGiaKim.Models.Entity;
using NhaGiaKim.Models.ViewModels;

namespace NhaGiaKim.Controllers;

[Authorize(Roles = AppRoles.Admin)]
[Route("quan-tri")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _db;

    public AdminController(ApplicationDbContext db) => _db = db;

    // ---------- Quản lý đơn hàng ----------
    [HttpGet("don-hang")]
    public async Task<IActionResult> Orders(long? statusId)
    {
        IQueryable<Order> query = _db.Orders.AsNoTracking()
            .Include(o => o.Status)
            .Include(o => o.Book);

        if (statusId.HasValue)
            query = query.Where(o => o.StatusId == statusId.Value);

        var valid = _db.Orders.Where(o => o.Status.StatusName != OrderStatusNames.Cancelled);

        var vm = new AdminOrdersViewModel
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
        return View(vm);
    }

    // Xác nhận / giao hàng / hoàn thành / hủy — chỉ cho phép đi theo luồng hợp lệ
    [HttpPost("don-hang/{id:long}/trang-thai"), ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(long id, long newStatusId, long? statusId)
    {
        var order = await _db.Orders.Include(o => o.Status).FirstOrDefaultAsync(o => o.OrderId == id);
        var target = await _db.OrderStatuses.FirstOrDefaultAsync(s => s.StatusId == newStatusId);

        if (order == null || target == null)
        {
            TempData["Error"] = "Không tìm thấy đơn hàng hoặc trạng thái.";
        }
        else if (!OrderStatusNames.CanMove(order.Status.StatusName, target.StatusName))
        {
            TempData["Error"] = $"Không thể chuyển đơn {order.OrderCode} từ “{order.Status.StatusName}” sang “{target.StatusName}”.";
        }
        else
        {
            order.Status = target;
            await _db.SaveChangesAsync();
            TempData["Ok"] = $"Đơn {order.OrderCode} đã chuyển sang “{target.StatusName}”.";
        }

        return RedirectToAction(nameof(Orders), new { statusId });
    }
}