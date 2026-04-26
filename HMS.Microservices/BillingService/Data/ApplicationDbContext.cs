using Microsoft.EntityFrameworkCore;
using BillingService.Models;

namespace BillingService.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Bill> Bills { get; set; }
    public DbSet<BillLine> BillLines { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bill>()
            .HasMany(b => b.Lines)
            .WithOne()
            .HasForeignKey(bl => bl.BillId);

        modelBuilder.Entity<Payment>()
            .HasOne<Bill>()
            .WithMany()
            .HasForeignKey(p => p.BillId);
    }
}