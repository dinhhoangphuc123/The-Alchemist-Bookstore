using NhaGiaKim.Services;

namespace NhaGiaKim.Tests;

public class VndTests
{
    [Theory]
    [InlineData(129_000, "129.000 ₫")]
    [InlineData(169_000, "169.000 ₫")]
    [InlineData(0, "0 ₫")]
    [InlineData(1_290_000, "1.290.000 ₫")]
    public void Format_UsesDotThousandsSeparatorAndDongSign(decimal amount, string expected)
    {
        Assert.Equal(expected, Vnd.Format(amount));
    }
}
