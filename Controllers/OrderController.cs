using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaGiaKim.Models.EF;
using NhaGiaKim.Models.Entity;
using NhaGiaKim.Models.ViewModels;
using NhaGiaKim.Services;

namespace NhaGiaKim.Controllers;

// Chỉ khách hàng đã đăng nhập mới đặt hàng / xem đơn của mình
[Authorize(Roles = AppRoles.Customer)]
public class OrderController : Controller
{
    private readonly ApplicationDbContext _db;

    public OrderController(ApplicationDbContext db) => _db = db;

    private long CurrentAccountId => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // ---------- Form đặt hàng ----------
    [HttpGet("dat-sach")]
    public async Task<IActionResult> Create()
    {
        var book = await _db.GetMainBookAsync();
        if (book == null) return NotFound();

        return View(new OrderPageViewModel
        {
            Book = book,
            Form = new OrderFormViewModel { CustomerName = User.FindFirst("FullName")?.Value ?? "" }
        });
    }

    [HttpPost("dat-sach"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(Prefix = "Form")] OrderFormViewModel form)
    {
        var book = await _db.GetMainBookAsync();
        if (book == null) return NotFound();

        var page = new OrderPageViewModel { Book = book, Form = form };
        if (!ModelState.IsValid) return View(page);

        var pending = await _db.OrderStatuses
            .FirstOrDefaultAsync(s => s.StatusName == OrderStatusNames.Pending);
        if (pending == null)
        {
            ModelState.AddModelError("", "Hệ thống chưa cấu hình trạng thái đơn hàng. Vui lòng chạy file SQL seed.");
            return View(page);
        }

        // Tổng tiền luôn được tính ở server, không tin dữ liệu từ trình duyệt
        var order = new Order
        {
            OrderCode = "TMP" + Guid.NewGuid().ToString("N"),
            BookId = book.BookId,
            AccountId = CurrentAccountId,
            Quantity = form.Quantity,
            CustomerName = form.CustomerName.Trim(),
            Phone = form.Phone,
            PaymentMethod = form.PaymentMethod,
            Address = form.Address.Trim(),
            Note = string.IsNullOrWhiteSpace(form.Note) ? null : form.Note.Trim(),
            TotalAmount = book.SalePrice * form.Quantity,
            StatusId = pending.StatusId,
            OrderTime = DateTime.Now
        };

        // Mã đơn dạng ALG20260001 được sinh từ order_id nên cần lưu 2 bước trong 1 transaction
        await using var tx = await _db.Database.BeginTransactionAsync();
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        order.OrderCode = $"ALG{DateTime.Now.Year}{order.OrderId:D4}";
        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        TempData["OrderCode"] = order.OrderCode;
        TempData["OrderName"] = order.CustomerName;
        TempData["OrderQty"] = order.Quantity;
        TempData["OrderTotal"] = Vnd.Format(order.TotalAmount);
        return RedirectToAction(nameof(Create));
    }

    // ---------- Đơn hàng của tôi ----------
    [HttpGet("don-hang-cua-toi")]
    public async Task<IActionResult> My()
    {
        var accountId = CurrentAccountId;
        var orders = await _db.Orders.AsNoTracking()
            .Include(o => o.Status)
            .Include(o => o.Book)
            .Where(o => o.AccountId == accountId)
            .OrderByDescending(o => o.OrderTime)
            .ToListAsync();

        return View(orders);
    }
}
