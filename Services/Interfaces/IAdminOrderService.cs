using NhaGiaKim.Models.ViewModels;

namespace NhaGiaKim.Services.Interfaces;

public record StatusUpdateResult(bool Success, string Message);

public interface IAdminOrderService
{
    /// <summary>Danh sách đơn (lọc theo trạng thái) + thống kê số cuốn / doanh thu / đánh giá.</summary>
    Task<AdminOrdersViewModel> GetOrdersDashboardAsync(long? statusId);

    /// <summary>Đổi trạng thái đơn, chỉ cho phép đi theo luồng hợp lệ.</summary>
    Task<StatusUpdateResult> UpdateStatusAsync(long orderId, long newStatusId);
}
