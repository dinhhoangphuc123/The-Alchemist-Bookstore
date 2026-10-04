using Microsoft.EntityFrameworkCore;
using NhaGiaKim.Models.EF;
using NhaGiaKim.Models.Entity;
using NhaGiaKim.Models.ViewModels;
using NhaGiaKim.Services.Interfaces;

namespace NhaGiaKim.Services;

public class FeedbackService : IFeedbackService
{
    private readonly ApplicationDbContext _db;

    public FeedbackService(ApplicationDbContext db) => _db = db;

    public async Task<FeedbackPageViewModel> BuildPageAsync(Book book, FeedbackFormViewModel form, bool showForm)
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

    public async Task CreateAsync(Book book, FeedbackFormViewModel form)
    {
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
    }
}
