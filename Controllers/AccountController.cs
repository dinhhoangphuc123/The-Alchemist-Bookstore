using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NhaGiaKim.Models.Entity;
using NhaGiaKim.Models.ViewModels;
using NhaGiaKim.Services.Interfaces;

namespace NhaGiaKim.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accounts;

    public AccountController(IAccountService accounts) => _accounts = accounts;

    // ---------- Đăng nhập ----------
    [HttpGet("dang-nhap")]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost("dang-nhap"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(vm);

        var result = await _accounts.LoginAsync(vm.Username, vm.Password);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", result.Error == LoginError.Locked
                ? "Tài khoản đã bị khóa. Vui lòng liên hệ cửa hàng."
                : "Tên đăng nhập hoặc mật khẩu không đúng.");
            return View(vm);
        }

        await SignInAccountAsync(result.Account!, vm.RememberMe);
        return RedirectAfterLogin(result.Account!, returnUrl);
    }

    // ---------- Đăng ký (chỉ tạo tài khoản khách hàng) ----------
    [HttpGet("dang-ky")]
    public IActionResult Register(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new RegisterViewModel());
    }

    [HttpPost("dang-ky"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel vm, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (await _accounts.IsUsernameTakenAsync(vm.Username))
            ModelState.AddModelError(nameof(vm.Username), "Tên đăng nhập đã tồn tại");
        if (await _accounts.IsEmailTakenAsync(vm.Email))
            ModelState.AddModelError(nameof(vm.Email), "Email đã được sử dụng");

        if (!ModelState.IsValid) return View(vm);

        var acc = await _accounts.RegisterCustomerAsync(vm);

        await SignInAccountAsync(acc, false);
        return RedirectAfterLogin(acc, returnUrl);
    }

    // ---------- Đăng xuất ----------
    [HttpPost("dang-xuat"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }

    [HttpGet("tu-choi-truy-cap")]
    public IActionResult AccessDenied() => View();

    // ---------- Helpers (việc thuộc về HTTP/cookie nên ở lại controller) ----------
    private async Task SignInAccountAsync(Account acc, bool remember)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, acc.AccountId.ToString()),
            new(ClaimTypes.Name, acc.Username),
            new("FullName", string.IsNullOrWhiteSpace(acc.FullName) ? acc.Username : acc.FullName),
            new(ClaimTypes.Role, acc.Role)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = remember });
    }

    private IActionResult RedirectAfterLogin(Account acc, string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return acc.Role == AppRoles.Admin ? Redirect("/quan-tri/don-hang") : Redirect("/");
    }
}
