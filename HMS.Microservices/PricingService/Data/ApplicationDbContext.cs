using Microsoft.EntityFrameworkCore;
using PricingService.Models;

namespace PricingService.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Pricing> Pricings { get; set; }
    public DbSet<RoomType> RoomTypes { get; set; }
}