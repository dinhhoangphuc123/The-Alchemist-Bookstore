using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NhaGiaKim.Models.Entity;
using NhaGiaKim.Models.ViewModels;
using NhaGiaKim.Services.Interfaces;

namespace NhaGiaKim.Controllers;

public class FeedbackController : Controller
{
    private readonly IBookService _books;
    private readonly IFeedbackService _feedbacks;

    public FeedbackController(IBookService books, IFeedbackService feedbacks)
    {
        _books = books;
        _feedbacks = feedbacks;
    }

    // Ai cũng xem được các đánh giá
    [HttpGet("danh-gia")]
    public async Task<IActionResult> Index()
    {
        var book = await _books.GetMainBookAsync();
        if (book == null) return NotFound();

        var form = new FeedbackFormViewModel { CustomerName = User.FindFirst("FullName")?.Value ?? "" };
        return View(await _feedbacks.BuildPageAsync(book, form, showForm: false));
    }

    // Chỉ khách hàng đã đăng nhập mới gửi được; đánh giá hiển thị ngay, không qua duyệt
    [Authorize(Roles = AppRoles.Customer)]
    [HttpPost("danh-gia/gui"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(Prefix = "Form")] FeedbackFormViewModel form)
    {
        var book = await _books.GetMainBookAsync();
        if (book == null) return NotFound();

        if (!ModelState.IsValid)
            return View("Index", await _feedbacks.BuildPageAsync(book, form, showForm: true));

        await _feedbacks.CreateAsync(book, form);

        TempData["Notice"] = "Cảm ơn bạn đã chia sẻ đánh giá về tác phẩm Nhà Giả Kim! Đánh giá của bạn đã được đăng bên dưới.";
        return RedirectToAction(nameof(Index));
    }
}
