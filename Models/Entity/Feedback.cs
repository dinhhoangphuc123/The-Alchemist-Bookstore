using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhaGiaKim.Models.Entity;

[Table("feedback")]
public class Feedback
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("feedback_id")]
    public long FeedbackId { get; set; }

    [Column("book_id")] public long BookId { get; set; }
    [Column("customer_name")] public string CustomerName { get; set; } = "";
    [Column("rating")] public short Rating { get; set; }
    [Column("content")] public string? Content { get; set; }
    [Column("status")] public string Status { get; set; } = FeedbackStatus.Pending;

    // Cột bổ sung: nhãn nhỏ cạnh tên người đánh giá (vd "Đã mua bản bìa cứng")
    [Column("badge")] public string? Badge { get; set; }

    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public Book Book { get; set; } = null!;
}

public static class FeedbackStatus
{
    public const string Pending = "pending";
    public const string Approved = "approved";
    public const string Rejected = "rejected";
}
