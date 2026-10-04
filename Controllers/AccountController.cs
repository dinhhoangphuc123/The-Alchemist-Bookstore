using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaGiaKim.Models.EF;
using NhaGiaKim.Models.Entity;
using NhaGiaKim.Models.ViewModels;

namespace NhaGiaKim.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordHasher<Account> _hasher;

    public AccountController(ApplicationDbContext db, IPasswordHasher<Account> hasher)
    {
        _db = db;
        _hasher = hasher;
    }

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

        var username = vm.Username.Trim();
        var acc = await _db.Accounts.FirstOrDefaultAsync(a => a.Username == username);

        if (acc == null ||
            _hasher.VerifyHashedPassword(acc, acc.Password, vm.Password) == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
            return View(vm);
        }

        if (!acc.Status)
        {
            ModelState.AddModelError("", "Tài khoản đã bị khóa. Vui lòng liên hệ cửa hàng.");
            return View(vm);
        }

        await SignInAccountAsync(acc, vm.RememberMe);
        return RedirectAfterLogin(acc, returnUrl);
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

        var username = vm.Username.Trim();
        var email = vm.Email.Trim().ToLower();

        if (await _db.Accounts.AnyAsync(a => a.Username == username))
            ModelState.AddModelError(nameof(vm.Username), "Tên đăng nhập đã tồn tại");
        if (await _db.Accounts.AnyAsync(a => a.Email != null && a.Email.ToLower() == email))
            ModelState.AddModelError(nameof(vm.Email), "Email đã được sử dụng");

        if (!ModelState.IsValid) return View(vm);

        var acc = new Account
        {
            Username = username,
            FullName = vm.FullName.Trim(),
            Email = email,
            Role = AppRoles.Customer
        };
        acc.Password = _hasher.HashPassword(acc, vm.Password);

        _db.Accounts.Add(acc);
        await _db.SaveChangesAsync();

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

    // ---------- Helpers ----------
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
