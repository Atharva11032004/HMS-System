using Microsoft.EntityFrameworkCore;
using InventoryService.Models;

namespace InventoryService.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<InventoryItem> InventoryItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        modelBuilder.Entity<InventoryItem>().HasData(
            new InventoryItem { Id = 1, Name = "Towels", Description = "Bath towels", Category = "Linens", Supplier = "TowelCo", QuantityInStock = 100, MinimumStockLevel = 20, UnitPrice = 5.00m, LastRestocked = new DateTime(2024, 1, 1), IsActive = true },
            new InventoryItem { Id = 2, Name = "Toilet Paper", Description = "Bathroom tissue", Category = "Bathroom Supplies", Supplier = "PaperPlus", QuantityInStock = 200, MinimumStockLevel = 50, UnitPrice = 2.50m, LastRestocked = new DateTime(2024, 1, 1), IsActive = true },
            new InventoryItem { Id = 3, Name = "Coffee", Description = "Ground coffee beans", Category = "Beverages", Supplier = "CoffeeWorld", QuantityInStock = 50, MinimumStockLevel = 10, UnitPrice = 8.00m, LastRestocked = new DateTime(2024, 1, 1), IsActive = true },
            new InventoryItem { Id = 4, Name = "Cleaning Supplies", Description = "Multi-purpose cleaner", Category = "Cleaning", Supplier = "CleanCorp", QuantityInStock = 30, MinimumStockLevel = 5, UnitPrice = 12.00m, LastRestocked = new DateTime(2024, 1, 1), IsActive = true }
        );
    }
}