using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhaGiaKim.Models.Entity;

[Table("books")]
public class Book
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("book_id")]
    public long BookId { get; set; }

    [Column("name")] public string Name { get; set; } = "";
    [Column("category")] public string? Category { get; set; }
    [Column("title")] public string? Title { get; set; }
    [Column("subtitle")] public string? Subtitle { get; set; }
    [Column("author")] public string Author { get; set; } = "";

    [Column("price")] public decimal Price { get; set; }
    [Column("promotion")] public decimal? Promotion { get; set; }

    [Column("image")] public string? Image { get; set; }
    [Column("description")] public string? Description { get; set; }

    [Column("published_at")] public DateTime? PublishedAt { get; set; }
    [Column("press")] public string? Press { get; set; }
    [Column("file_remark")] public string? FileRemark { get; set; }

    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    /// <summary>Giá bán sau khuyến mãi, làm tròn đến nghìn đồng (169.000 ₫ giảm 23,67% → 129.000 ₫).</summary>
    [NotMapped]
    public decimal SalePrice
    {
        get
        {
            var promo = Promotion ?? 0m;
            if (promo <= 0) return Price;
            return Math.Round(Price * (1 - promo / 100m) / 1000m, 0, MidpointRounding.AwayFromZero) * 1000m;
        }
    }

    [NotMapped]
    public int DiscountPercent => (int)Math.Round(Promotion ?? 0m, MidpointRounding.AwayFromZero);
}
