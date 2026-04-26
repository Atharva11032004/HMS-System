using Microsoft.EntityFrameworkCore;
using RoomService.Models;

namespace RoomService.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<RoomType> RoomTypes { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<RoomCalendar> RoomCalendars { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>()
            .HasOne(r => r.RoomType)
            .WithMany()
            .HasForeignKey(r => r.RoomTypeId);

        modelBuilder.Entity<RoomCalendar>()
            .HasOne(rc => rc.Room)
            .WithMany()
            .HasForeignKey(rc => rc.RoomId);
    }
}