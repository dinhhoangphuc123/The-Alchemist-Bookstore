namespace NhaGiaKim.Services.Interfaces;

public record OperationResult(bool Success, string Message);

/// <summary>Quản lý tồn kho của cuốn sách duy nhất.</summary>
public interface IInventoryService
{
    /// <summary>Số cuốn còn trong kho (0 nếu chưa có sách).</summary>
    Task<int> GetStockAsync();

    Task<bool> IsInStockAsync();

    /// <summary>
    /// Trừ kho nguyên tử: chỉ trừ khi còn đủ hàng (UPDATE ... WHERE stock >= qty).
    /// Trả false nếu không đủ, kho không bao giờ bị âm kể cả khi nhiều người đặt cùng lúc.
    /// </summary>
    Task<bool> TryReserveAsync(long bookId, int quantity);

    /// <summary>Trả hàng về kho (khi đơn bị hủy).</summary>
    Task ReleaseAsync(long bookId, int quantity);

    /// <summary>Admin đặt lại số lượng tồn kho.</summary>
    Task<OperationResult> SetStockAsync(int newStock);
}
