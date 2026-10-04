using NhaGiaKim.Models.Entity;
using NhaGiaKim.Models.ViewModels;

namespace NhaGiaKim.Services.Interfaces;

/// <summary>Dữ liệu điền sẵn cho form khi "Mua lại" từ đơn cũ.</summary>
public record ReorderData(OrderFormViewModel Form, string OrderCode);

/// <summary>Kết quả tạo đơn: thành công thì có Order, thất bại thì có ErrorMessage.</summary>
public record CreateOrderResult(Order? Order, string? ErrorMessage)
{
    public bool Succeeded => Order != null;
}

public interface IOrderService
{
    /// <summary>Chỉ lấy đơn thuộc về chính tài khoản đó; không có thì trả null.</summary>
    Task<ReorderData?> GetReorderDataAsync(long accountId, long orderId);

    /// <summary>Tính tổng tiền ở server, sinh mã đơn ALGyyyyNNNN trong 1 transaction.</summary>
    Task<CreateOrderResult> CreateOrderAsync(long accountId, Book book, OrderFormViewModel form);

    Task<List<Order>> GetOrdersOfAccountAsync(long accountId);
}
