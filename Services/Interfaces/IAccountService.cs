using NhaGiaKim.Models.Entity;
using NhaGiaKim.Models.ViewModels;

namespace NhaGiaKim.Services.Interfaces;

public enum LoginError { None, InvalidCredentials, Locked }

/// <summary>Kết quả đăng nhập: thành công thì có Account, thất bại thì có Error.</summary>
public record LoginResult(Account? Account, LoginError Error)
{
    public bool Succeeded => Account != null && Error == LoginError.None;
}

public interface IAccountService
{
    /// <summary>Kiểm tra username/mật khẩu và trạng thái khóa tài khoản.</summary>
    Task<LoginResult> LoginAsync(string username, string password);

    Task<bool> IsUsernameTakenAsync(string username);
    Task<bool> IsEmailTakenAsync(string email);

    /// <summary>Tạo tài khoản khách hàng mới (băm mật khẩu bằng PasswordHasher).</summary>
    Task<Account> RegisterCustomerAsync(RegisterViewModel vm);
}
