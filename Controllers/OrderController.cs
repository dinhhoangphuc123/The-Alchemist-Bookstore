using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NhaGiaKim.Models.Entity;
using NhaGiaKim.Models.ViewModels;
using NhaGiaKim.Services;
using NhaGiaKim.Services.Interfaces;

namespace NhaGiaKim.Controllers;

// Chỉ khách hàng đã đăng nhập mới đặt hàng / xem đơn của mình
[Authorize(Roles = AppRoles.Customer)]
public class OrderController : Controller
{
    private readonly IBookService _books;
    private readonly IOrderService _orders;

    public OrderController(IBookService books, IOrderService orders)
    {
        _books = books;
        _orders = orders;
    }

    private long CurrentAccountId => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // ---------- Form đặt hàng ----------
    [HttpGet("dat-sach")]
    public async Task<IActionResult> Create(long? from)
    {
        var book = await _books.GetMainBookAsync();
        if (book == null) return NotFound();

        var form = new OrderFormViewModel { CustomerName = User.FindFirst("FullName")?.Value ?? "" };

        // "Mua lại": điền sẵn thông tin từ đơn cũ (service chỉ lấy đơn của chính người đăng nhập)
        if (from.HasValue)
        {
            var reorder = await _orders.GetReorderDataAsync(CurrentAccountId, from.Value);
            if (reorder != null)
            {
                form = reorder.Form;
                ViewData["ReorderCode"] = reorder.OrderCode;
            }
        }

        return View(new OrderPageViewModel { Book = book, Form = form });
    }

    [HttpPost("dat-sach"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(Prefix = "Form")] OrderFormViewModel form)
    {
        var book = await _books.GetMainBookAsync();
        if (book == null) return NotFound();

        var page = new OrderPageViewModel { Book = book, Form = form };
        if (!ModelState.IsValid) return View(page);

        var result = await _orders.CreateOrderAsync(CurrentAccountId, book, form);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", result.ErrorMessage!);
            return View(page);
        }

        var order = result.Order!;
        TempData["OrderCode"] = order.OrderCode;
        TempData["OrderName"] = order.CustomerName;
        TempData["OrderQty"] = order.Quantity;
        TempData["OrderTotal"] = Vnd.Format(order.TotalAmount);
        return RedirectToAction(nameof(Create));
    }

    // ---------- Đơn hàng của tôi ----------
    [HttpGet("don-hang-cua-toi")]
    public async Task<IActionResult> My()
        => View(await _orders.GetOrdersOfAccountAsync(CurrentAccountId));
}
