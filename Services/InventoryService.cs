using Microsoft.EntityFrameworkCore;
using NhaGiaKim.Models.EF;
using NhaGiaKim.Services.Interfaces;

namespace NhaGiaKim.Services;

public class InventoryService : IInventoryService
{
    public const int MaxStock = 100_000;

    private readonly ApplicationDbContext _db;

    public InventoryService(ApplicationDbContext db) => _db = db;

    public Task<int> GetStockAsync() =>
        _db.Books.AsNoTracking().OrderBy(b => b.BookId).Select(b => b.Stock).FirstOrDefaultAsync();

    public async Task<bool> IsInStockAsync() => await GetStockAsync() > 0;

    public async Task<bool> TryReserveAsync(long bookId, int quantity)
    {
        if (quantity <= 0) return false;

        // 1 câu UPDATE duy nhất, điều kiện stock >= quantity nằm trong WHERE nên không bị đua dữ liệu.
        var affected = await _db.Books
            .Where(b => b.BookId == bookId && b.Stock >= quantity)
            .ExecuteUpdateAsync(s => s
                .SetProperty(b => b.Stock, b => b.Stock - quantity)
                .SetProperty(b => b.UpdatedAt, DateTime.Now));

        return affected > 0;
    }

    public async Task ReleaseAsync(long bookId, int quantity)
    {
        if (quantity <= 0) return;

        await _db.Books
            .Where(b => b.BookId == bookId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(b => b.Stock, b => b.Stock + quantity)
                .SetProperty(b => b.UpdatedAt, DateTime.Now));
    }

    public async Task<OperationResult> SetStockAsync(int newStock)
    {
        if (newStock < 0)
            return new OperationResult(false, "Số lượng trong kho không được âm.");
        if (newStock > MaxStock)
            return new OperationResult(false, $"Số lượng trong kho tối đa {MaxStock:N0} cuốn.");

        var bookId = await _db.Books.OrderBy(b => b.BookId).Select(b => b.BookId).FirstOrDefaultAsync();
        if (bookId == 0)
            return new OperationResult(false, "Chưa có dữ liệu sách trong hệ thống.");

        await _db.Books
            .Where(b => b.BookId == bookId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(b => b.Stock, newStock)
                .SetProperty(b => b.UpdatedAt, DateTime.Now));

        return new OperationResult(true,
            newStock > 0
                ? $"Đã cập nhật kho: còn {newStock:N0} cuốn."
                : "Đã cập nhật kho: HẾT HÀNG, khách sẽ không đặt được sách.");
    }
}
