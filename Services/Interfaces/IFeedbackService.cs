using NhaGiaKim.Models.Entity;
using NhaGiaKim.Models.ViewModels;

namespace NhaGiaKim.Services.Interfaces;

public interface IFeedbackService
{
    Task<FeedbackPageViewModel> BuildPageAsync(Book book, FeedbackFormViewModel form, bool showForm);

    /// <summary>Lưu đánh giá mới (hiển thị ngay, không qua duyệt).</summary>
    Task CreateAsync(Book book, FeedbackFormViewModel form);
}
