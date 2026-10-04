using Microsoft.EntityFrameworkCore;
using NhaGiaKim.Models.EF;
using NhaGiaKim.Models.Entity;
using NhaGiaKim.Models.ViewModels;
using NhaGiaKim.Services.Interfaces;

namespace NhaGiaKim.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _db;

    public OrderService(ApplicationDbContext db) => _db = db;

    public async Task<ReorderData?> GetReorderDataAsync(long accountId, long orderId)
    {
        // Chỉ lấy đơn thuộc về chính người đang đăng nhập
        var old = await _db.Orders.AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrderId == orderId && o.AccountId == accountId);
        if (old == null) return null;

        var form = new OrderFormViewModel
        {
            Quantity = old.Quantity,
            CustomerName = old.CustomerName,
            Phone = old.Phone,
            PaymentMethod = old.PaymentMethod,
            Address = old.Address,
            Note = old.Note
        };
        return new ReorderData(form, old.OrderCode);
    }

    public async Task<CreateOrderResult> CreateOrderAsync(long accountId, Book book, OrderFormViewModel form)
    {
        var pending = await _db.OrderStatuses
            .FirstOrDefaultAsync(s => s.StatusName == OrderStatusNames.Pending);
        if (pending == null)
            return new CreateOrderResult(null,
                "Hệ thống chưa cấu hình trạng thái đơn hàng. Vui lòng chạy file SQL seed.");

        // Tổng tiền luôn được tính ở server, không tin dữ liệu từ trình duyệt
        var order = new Order
        {
            OrderCode = "TMP" + Guid.NewGuid().ToString("N"),
            BookId = book.BookId,
            AccountId = accountId,
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

        return new CreateOrderResult(order, null);
    }

    public Task<List<Order>> GetOrdersOfAccountAsync(long accountId) =>
        _db.Orders.AsNoTracking()
            .Include(o => o.Status)
            .Include(o => o.Book)
            .Where(o => o.AccountId == accountId)
            .OrderByDescending(o => o.OrderTime)
            .ToListAsync();
}
