using NhaGiaKim.Models.Entity;

namespace NhaGiaKim.Tests;

public class OrderStatusTests
{
    [Theory]
    [InlineData(OrderStatusNames.Pending, OrderStatusNames.Confirmed)]
    [InlineData(OrderStatusNames.Pending, OrderStatusNames.Cancelled)]
    [InlineData(OrderStatusNames.Confirmed, OrderStatusNames.Shipping)]
    [InlineData(OrderStatusNames.Confirmed, OrderStatusNames.Cancelled)]
    [InlineData(OrderStatusNames.Shipping, OrderStatusNames.Completed)]
    [InlineData(OrderStatusNames.Shipping, OrderStatusNames.Cancelled)]
    public void CanMove_ValidTransitions_ReturnsTrue(string from, string to)
    {
        Assert.True(OrderStatusNames.CanMove(from, to));
    }

    [Theory]
    [InlineData(OrderStatusNames.Pending, OrderStatusNames.Shipping)]     // nhảy cóc
    [InlineData(OrderStatusNames.Pending, OrderStatusNames.Completed)]    // nhảy cóc
    [InlineData(OrderStatusNames.Confirmed, OrderStatusNames.Pending)]    // đi lùi
    [InlineData(OrderStatusNames.Completed, OrderStatusNames.Shipping)]   // đã hoàn thành
    [InlineData(OrderStatusNames.Completed, OrderStatusNames.Cancelled)]  // đã hoàn thành
    [InlineData(OrderStatusNames.Cancelled, OrderStatusNames.Pending)]    // đã hủy
    [InlineData(OrderStatusNames.Cancelled, OrderStatusNames.Confirmed)]  // đã hủy
    public void CanMove_InvalidTransitions_ReturnsFalse(string from, string to)
    {
        Assert.False(OrderStatusNames.CanMove(from, to));
    }

    [Fact]
    public void CanMove_UnknownStatus_ReturnsFalse()
    {
        Assert.False(OrderStatusNames.CanMove("Trạng thái lạ", OrderStatusNames.Confirmed));
    }

    [Theory]
    [InlineData(OrderStatusNames.Completed)]
    [InlineData(OrderStatusNames.Cancelled)]
    public void FinalStatuses_HaveNoNextStatus(string status)
    {
        Assert.Empty(OrderStatusNames.NextOf(status));
    }

    // Hủy đơn thì hoàn hàng về kho; các trạng thái khác thì không
    [Theory]
    [InlineData(OrderStatusNames.Cancelled, true)]
    [InlineData(OrderStatusNames.Confirmed, false)]
    [InlineData(OrderStatusNames.Shipping, false)]
    [InlineData(OrderStatusNames.Completed, false)]
    public void RestoresStock_OnlyWhenCancelled(string target, bool expected)
    {
        Assert.Equal(expected, OrderStatusNames.RestoresStock(target));
    }
}
