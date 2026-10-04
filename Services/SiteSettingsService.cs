using Microsoft.EntityFrameworkCore;
using NhaGiaKim.Models.EF;

namespace NhaGiaKim.Services;

/// <summary>Đọc bảng settings (1 lần / request), có giá trị mặc định nếu thiếu key.</summary>
public class SiteSettingsService
{
    private readonly ApplicationDbContext _db;
    private Dictionary<string, string?>? _cache;

    public SiteSettingsService(ApplicationDbContext db) => _db = db;

    public async Task<string> GetAsync(string key, string fallback = "")
    {
        _cache ??= await _db.Settings.AsNoTracking()
            .ToDictionaryAsync(s => s.SettingKey, s => s.SettingValue);

        return _cache.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : fallback;
    }
}
