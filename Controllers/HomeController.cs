using Microsoft.AspNetCore.Mvc;
using NhaGiaKim.Services.Interfaces;

namespace NhaGiaKim.Controllers;

public class HomeController : Controller
{
    private readonly IBookService _books;

    public HomeController(IBookService books) => _books = books;

    [HttpGet("")]
    public Task<IActionResult> Index() => BookPage();

    [HttpGet("noi-dung")]
    public Task<IActionResult> Story() => BookPage();

    [HttpGet("tac-gia")]
    public Task<IActionResult> Author() => BookPage();

    [HttpGet("thong-tin")]
    public Task<IActionResult> Info() => BookPage();

    private async Task<IActionResult> BookPage()
    {
        var book = await _books.GetMainBookAsync();
        if (book == null)
            return NotFound("Chưa có dữ liệu sách. Hãy chạy file Database/nhagiakim_update_seed.sql.");
        return View(book);
    }
}
