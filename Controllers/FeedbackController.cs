using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaGiaKim.Models.EF;
using NhaGiaKim.Models.Entity;
using NhaGiaKim.Models.ViewModels;

namespace NhaGiaKim.Controllers;

public class FeedbackController : Controller
{
    private readonly ApplicationDbContext _db;

    public FeedbackController(ApplicationDbContext db) => _db = db;

    // Ai cũng xem được các đánh giá đã duyệt
    [HttpGet("danh-gia")]
    public async Task<IActionResult> Index()
    {
        var book = await _db.GetMainBookAsync();
        if (book == null) return NotFound();

        var form = new FeedbackFormViewModel { CustomerName = User.FindFirst("FullName")?.Value ?? "" };
        return View(await BuildPageAsync(book, form, showForm: false));
    }

    // Chỉ khách hàng đã đăng nhập mới gửi được; đánh giá hiển thị ngay, không qua duyệt
    [Authorize(Roles = AppRoles.Customer)]
    [HttpPost("danh-gia/gui"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(Prefix = "Form")] FeedbackFormViewModel form)
    {
        var book = await _db.GetMainBookAsync();
        if (book == null) return NotFound();

        if (!ModelState.IsValid)
            return View("Index", await BuildPageAsync(book, form, showForm: true));

        var content = form.Content.Trim();
        if (!content.StartsWith('“') && !content.StartsWith('"'))
            content = $"“{content}”";

        _db.Feedbacks.Add(new Feedback
        {
            BookId = book.BookId,
            CustomerName = form.CustomerName.Trim(),
            Rating = (short)form.Rating,
            Content = content,
            Status = FeedbackStatus.Approved,
            Badge = "Đánh giá mới từ bạn đọc"
        });
        await _db.SaveChangesAsync();

        TempData["Notice"] = "Cảm ơn bạn đã chia sẻ đánh giá về tác phẩm Nhà Giả Kim! Đánh giá của bạn đã được đăng bên dưới.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<FeedbackPageViewModel> BuildPageAsync(Book book, FeedbackFormViewModel form, bool showForm)
    {
        // Hiển thị toàn bộ đánh giá, không lọc theo trạng thái duyệt
        var all = _db.Feedbacks.AsNoTracking().Where(f => f.BookId == book.BookId);

        var count = await all.CountAsync();
        var average = count == 0 ? 0 : await all.AverageAsync(f => (double)f.Rating);
        var items = await all
            .OrderByDescending(f => f.CreatedAt).ThenBy(f => f.FeedbackId)
            .Take(50).ToListAsync();

        return new FeedbackPageViewModel
        {
            Book = book,
            Items = items,
            Average = average,
            Count = count,
            Form = form,
            ShowForm = showForm
        };
    }
}
