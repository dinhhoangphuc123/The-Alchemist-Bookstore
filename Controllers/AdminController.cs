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

        var vm = new AdminOrdersViewModel
        {
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

    // ---------- Duyệt đánh giá ----------
    [HttpGet("danh-gia")]
    public async Task<IActionResult> Feedbacks(string? status)
    {
        var query = _db.Feedbacks.AsNoTracking().AsQueryable();
        if (!string.IsNullOrEmpty(status))
            query = query.Where(f => f.Status == status);

        return View(new AdminFeedbacksViewModel
        {
            FilterStatus = status,
            Items = await query.OrderByDescending(f => f.CreatedAt).ToListAsync()
        });
    }

    [HttpPost("danh-gia/{id:long}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ModerateFeedback(long id, string status, string? filter)
    {
        var allowed = new[] { FeedbackStatus.Pending, FeedbackStatus.Approved, FeedbackStatus.Rejected };
        var fb = await _db.Feedbacks.FindAsync(id);

        if (fb == null || !allowed.Contains(status))
        {
            TempData["Error"] = "Không tìm thấy đánh giá hoặc trạng thái không hợp lệ.";
        }
        else
        {
            fb.Status = status;
            fb.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            TempData["Ok"] = "Đã cập nhật đánh giá.";
        }

        return RedirectToAction(nameof(Feedbacks), new { status = filter });
    }
}
