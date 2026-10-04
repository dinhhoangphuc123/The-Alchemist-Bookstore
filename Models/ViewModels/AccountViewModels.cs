using System.ComponentModel.DataAnnotations;

namespace NhaGiaKim.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
    public string Username { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    public bool RememberMe { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
    [StringLength(255, MinimumLength = 2, ErrorMessage = "Họ và tên từ 2 đến 255 ký tự")]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
    [RegularExpression(@"^[a-zA-Z0-9_]{4,50}$",
        ErrorMessage = "Tên đăng nhập 4–50 ký tự, chỉ gồm chữ không dấu, số và dấu gạch dưới")]
    public string Username { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(255)]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
    [Compare(nameof(Password), ErrorMessage = "Mật khẩu nhập lại không khớp")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = "";
}
