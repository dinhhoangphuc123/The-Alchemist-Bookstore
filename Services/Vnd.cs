using System.Globalization;

namespace NhaGiaKim.Services;

public static class Vnd
{
    private static readonly CultureInfo Vi = new("vi-VN");

    /// <summary>129000 → "129.000 ₫"</summary>
    public static string Format(decimal amount) => amount.ToString("N0", Vi) + " ₫";
}
