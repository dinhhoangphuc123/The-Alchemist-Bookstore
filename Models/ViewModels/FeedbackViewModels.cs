using System.ComponentModel.DataAnnotations;
using NhaGiaKim.Models.Entity;

namespace NhaGiaKim.Models.ViewModels;

public class FeedbackFormViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
    [StringLength(255, MinimumLength = 2, ErrorMessage = "Họ và tên từ 2 đến 255 ký tự")]
    public string CustomerName { get; set; } = "";

    [Range(1, 5, ErrorMessage = "Số sao từ 1 đến 5")]
    public int Rating { get; set; } = 5;

    [Required(ErrorMessage = "Vui lòng nhập nội dung đánh giá")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Nội dung đánh giá cần từ 10 đến 1000 ký tự")]
    public string Content { get; set; } = "";
}

public class FeedbackPageViewModel
{
    public Book Book { get; set; } = null!;
    public List<Feedback> Items { get; set; } = new();
    public double Average { get; set; }
    public int Count { get; set; }
    public FeedbackFormViewModel Form { get; set; } = new();
    public bool ShowForm { get; set; }
}
