using NhaGiaKim.Models.Entity;

namespace NhaGiaKim.Services.Interfaces;

public interface IBookService
{
    /// <summary>Website chỉ bán 1 cuốn: lấy cuốn đầu tiên trong bảng books.</summary>
    Task<Book?> GetMainBookAsync();
}
