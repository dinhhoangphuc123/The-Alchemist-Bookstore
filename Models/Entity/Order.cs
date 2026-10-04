using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhaGiaKim.Models.Entity;

[Table("orders")]
public class Order
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("order_id")]
    public long OrderId { get; set; }

    [Column("order_code")] public string OrderCode { get; set; } = "";
    [Column("quantity")] public int Quantity { get; set; }
    [Column("customer_name")] public string CustomerName { get; set; } = "";
    [Column("phone")] public string Phone { get; set; } = "";
    [Column("payment_method")] public string PaymentMethod { get; set; } = "COD";
    [Column("order_time")] public DateTime OrderTime { get; set; } = DateTime.Now;
    [Column("total_amount")] public decimal TotalAmount { get; set; }
    [Column("status_id")] public long StatusId { get; set; }

    // Các cột bổ sung (xem Database/nhagiakim_update_seed.sql)
    [Column("book_id")] public long BookId { get; set; }
    [Column("account_id")] public long AccountId { get; set; }
    [Column("address")] public string Address { get; set; } = "";
    [Column("note")] public string? Note { get; set; }

    public OrderStatus Status { get; set; } = null!;
    public Book Book { get; set; } = null!;
    public Account Account { get; set; } = null!;
}
