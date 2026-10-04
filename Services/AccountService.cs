using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NhaGiaKim.Models.EF;
using NhaGiaKim.Models.Entity;
using NhaGiaKim.Models.ViewModels;
using NhaGiaKim.Services.Interfaces;

namespace NhaGiaKim.Services;

public class AccountService : IAccountService
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordHasher<Account> _hasher;

    public AccountService(ApplicationDbContext db, IPasswordHasher<Account> hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    public async Task<LoginResult> LoginAsync(string username, string password)
    {
        username = username.Trim();
        var acc = await _db.Accounts.FirstOrDefaultAsync(a => a.Username == username);

        if (acc == null ||
            _hasher.VerifyHashedPassword(acc, acc.Password, password) == PasswordVerificationResult.Failed)
            return new LoginResult(null, LoginError.InvalidCredentials);

        if (!acc.Status)
            return new LoginResult(null, LoginError.Locked);

        return new LoginResult(acc, LoginError.None);
    }

    public Task<bool> IsUsernameTakenAsync(string username) =>
        _db.Accounts.AnyAsync(a => a.Username == username.Trim());

    public Task<bool> IsEmailTakenAsync(string email)
    {
        var normalized = email.Trim().ToLower();
        return _db.Accounts.AnyAsync(a => a.Email != null && a.Email.ToLower() == normalized);
    }

    public async Task<Account> RegisterCustomerAsync(RegisterViewModel vm)
    {
        var acc = new Account
        {
            Username = vm.Username.Trim(),
            FullName = vm.FullName.Trim(),
            Email = vm.Email.Trim().ToLower(),
            Role = AppRoles.Customer
        };
        acc.Password = _hasher.HashPassword(acc, vm.Password);

        _db.Accounts.Add(acc);
        await _db.SaveChangesAsync();
        return acc;
    }
}
