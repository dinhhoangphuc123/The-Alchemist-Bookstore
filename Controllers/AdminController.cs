using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NhaGiaKim.Models.Entity;
using NhaGiaKim.Services.Interfaces;

namespace NhaGiaKim.Controllers;

[Authorize(Roles = AppRoles.Admin)]
[Route("quan-tri")]
public class AdminController : Controller
{
    private readonly IAdminOrderService _adminOrders;
    private readonly IInventoryService _inventory;

    public AdminController(IAdminOrderService adminOrders, IInventoryService inventory)
    {
        _adminOrders = adminOrders;
        _inventory = inventory;
    }

    // ---------- Quản lý đơn hàng ----------
    [HttpGet("don-hang")]
    public async Task<IActionResult> Orders(long? statusId)
        => View(await _adminOrders.GetOrdersDashboardAsync(statusId));

    // Xác nhận / giao hàng / hoàn thành / hủy — luật chuyển trạng thái nằm trong service
    [HttpPost("don-hang/{id:long}/trang-thai"), ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(long id, long newStatusId, long? statusId)
    {
        var result = await _adminOrders.UpdateStatusAsync(id, newStatusId);
        TempData[result.Success ? "Ok" : "Error"] = result.Message;

        return RedirectToAction(nameof(Orders), new { statusId });
    }

    // Admin nhập lại số lượng còn trong kho
    [HttpPost("kho"), ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStock(int stock, long? statusId)
    {
        var result = await _inventory.SetStockAsync(stock);
        TempData[result.Success ? "Ok" : "Error"] = result.Message;

        return RedirectToAction(nameof(Orders), new { statusId });
    }
}
