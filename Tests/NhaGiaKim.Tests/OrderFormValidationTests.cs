using System.ComponentModel.DataAnnotations;
using NhaGiaKim.Models.ViewModels;

namespace NhaGiaKim.Tests;

public class OrderFormValidationTests
{
    private static OrderFormViewModel ValidForm() => new()
    {
        Quantity = 1,
        CustomerName = "Nguyễn Văn A",
        Phone = "0912345678",
        PaymentMethod = "COD",
        Address = "12 Nguyễn Huệ, Quận 1, TP. Hồ Chí Minh"
    };

    private static List<ValidationResult> Validate(OrderFormViewModel form)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(form, new ValidationContext(form), results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void ValidForm_HasNoErrors()
    {
        Assert.Empty(Validate(ValidForm()));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(51)]
    public void Quantity_OutOfRange_IsInvalid(int quantity)
    {
        var form = ValidForm();
        form.Quantity = quantity;
        Assert.Contains(Validate(form), r => r.MemberNames.Contains(nameof(form.Quantity)));
    }

    [Theory]
    [InlineData("0912345678", true)]
    [InlineData("912345678", false)]    // thiếu số 0 đầu
    [InlineData("091234567", false)]    // thiếu 1 chữ số
    [InlineData("09123456789", false)]  // thừa 1 chữ số
    [InlineData("09123abcde", false)]   // có chữ
    public void Phone_MustBeTenDigitsStartingWithZero(string phone, bool valid)
    {
        var form = ValidForm();
        form.Phone = phone;
        var hasPhoneError = Validate(form).Any(r => r.MemberNames.Contains(nameof(form.Phone)));
        Assert.Equal(!valid, hasPhoneError);
    }

    [Theory]
    [InlineData("COD", true)]
    [InlineData("Banking", true)]
    [InlineData("Momo", false)]
    public void PaymentMethod_OnlyCodOrBanking(string method, bool valid)
    {
        var form = ValidForm();
        form.PaymentMethod = method;
        var hasError = Validate(form).Any(r => r.MemberNames.Contains(nameof(form.PaymentMethod)));
        Assert.Equal(!valid, hasError);
    }

    [Fact]
    public void Address_TooShort_IsInvalid()
    {
        var form = ValidForm();
        form.Address = "Hà Nội";
        Assert.Contains(Validate(form), r => r.MemberNames.Contains(nameof(form.Address)));
    }
}
