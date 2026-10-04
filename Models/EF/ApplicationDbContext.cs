using Microsoft.EntityFrameworkCore;
using NhaGiaKim.Models.Entity;

namespace NhaGiaKim.Models.EF;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<OrderStatus> OrderStatuses => Set<OrderStatus>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();
    public DbSet<Setting> Settings => Set<Setting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Khớp ON DELETE trong script SQL
        modelBuilder.Entity<Order>(e =>
        {
            e.HasOne(o => o.Status).WithMany(s => s.Orders)
                .HasForeignKey(o => o.StatusId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(o => o.Book).WithMany()
                .HasForeignKey(o => o.BookId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(o => o.Account).WithMany(a => a.Orders)
                .HasForeignKey(o => o.AccountId).OnDelete(DeleteBehavior.Restrict);
        });

        // Cột TIMESTAMP trong DB là không múi giờ -> khai báo rõ để Npgsql không dùng timestamptz
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            foreach (var prop in entityType.GetProperties()
                         .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?)))
                prop.SetColumnType("timestamp without time zone");

        modelBuilder.Entity<Feedback>()
            .HasOne(f => f.Book).WithMany(b => b.Feedbacks)
            .HasForeignKey(f => f.BookId).OnDelete(DeleteBehavior.Cascade);
    }
}
