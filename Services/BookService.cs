using Microsoft.EntityFrameworkCore;
using NhaGiaKim.Models.EF;
using NhaGiaKim.Models.Entity;
using NhaGiaKim.Services.Interfaces;

namespace NhaGiaKim.Services;

public class BookService : IBookService
{
    private readonly ApplicationDbContext _db;

    public BookService(ApplicationDbContext db) => _db = db;

    public Task<Book?> GetMainBookAsync() =>
        _db.Books.AsNoTracking().OrderBy(b => b.BookId).FirstOrDefaultAsync();
}
