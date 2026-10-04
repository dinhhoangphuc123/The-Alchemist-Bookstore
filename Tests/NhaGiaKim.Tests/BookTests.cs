using NhaGiaKim.Models.Entity;

namespace NhaGiaKim.Tests;

public class BookTests
{
    // ---------- Giá bán ----------
    [Fact]
    public void SalePrice_NoPromotion_ReturnsOriginalPrice()
    {
        var book = new Book { Price = 169_000m, Promotion = null };
        Assert.Equal(169_000m, book.SalePrice);
    }

    [Fact]
    public void SalePrice_ZeroPromotion_ReturnsOriginalPrice()
    {
        var book = new Book { Price = 169_000m, Promotion = 0m };
        Assert.Equal(169_000m, book.SalePrice);
    }

    [Fact]
    public void SalePrice_Promotion_RoundsToNearestThousand()
    {
        // 169.000 giảm 23,67% ≈ 128.998 -> làm tròn đến nghìn = 129.000
        var book = new Book { Price = 169_000m, Promotion = 23.67m };
        Assert.Equal(129_000m, book.SalePrice);
    }

    [Fact]
    public void DiscountPercent_RoundsToInteger()
    {
        var book = new Book { Price = 169_000m, Promotion = 23.67m };
        Assert.Equal(24, book.DiscountPercent);
    }

    // ---------- Tồn kho ----------
    [Theory]
    [InlineData(10, true)]
    [InlineData(1, true)]
    [InlineData(0, false)]
    public void InStock_DependsOnStock(int stock, bool expected)
    {
        Assert.Equal(expected, new Book { Stock = stock }.InStock);
    }

    [Theory]
    [InlineData(10, 1, true)]    // còn 10, mua 1
    [InlineData(10, 10, true)]   // mua đúng số còn lại
    [InlineData(10, 11, false)]  // mua nhiều hơn số còn
    [InlineData(0, 1, false)]    // hết hàng
    [InlineData(10, 0, false)]   // số lượng 0 không hợp lệ
    [InlineData(10, -3, false)]  // số lượng âm không hợp lệ
    public void CanFulfill_ChecksQuantityAgainstStock(int stock, int quantity, bool expected)
    {
        Assert.Equal(expected, new Book { Stock = stock }.CanFulfill(quantity));
    }
}
