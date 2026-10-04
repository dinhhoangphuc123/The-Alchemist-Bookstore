using System.ComponentModel.DataAnnotations;
using NhaGiaKim.Models.Entity;

namespace NhaGiaKim.Models.ViewModels;

public class OrderFormViewModel
{
    [Range(1, 50, ErrorMessage = "Số lượng từ 1 đến 50 cuốn")]
    public int Quantity { get; set; } = 1;

    [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
    [StringLength(255, MinimumLength = 2, ErrorMessage = "Họ và tên từ 2 đến 255 ký tự")]
    public string CustomerName { get; set; } = "";

    // Đúng 10 chữ số, bắt đầu bằng 0
    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [RegularExpression(@"^0\d{9}$",
        ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số và bắt đầu bằng số 0")]
    public string Phone { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
    [RegularExpression("^(COD|Banking)$", ErrorMessage = "Phương thức thanh toán không hợp lệ")]
    public string PaymentMethod { get; set; } = "COD";

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Địa chỉ cần từ 10 đến 500 ký tự (số nhà, đường, phường/xã, quận/huyện, tỉnh/thành)")]
    public string Address { get; set; } = "";

    [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
    public string? Note { get; set; }
}

public class OrderPageViewModel
{
    public Book Book { get; set; } = null!;
    public OrderFormViewModel Form { get; set; } = new();
}
