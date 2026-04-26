using Microsoft.EntityFrameworkCore;
using StaffService.Models;

namespace StaffService.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Staff> Staff { get; set; }
    public DbSet<Department> Departments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed data
        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "Management", Description = "Hotel management team" },
            new Department { Id = 2, Name = "Reception", Description = "Front desk and guest services" },
            new Department { Id = 3, Name = "Housekeeping", Description = "Room cleaning and maintenance" },
            new Department { Id = 4, Name = "Maintenance", Description = "Technical and facility maintenance" },
            new Department { Id = 5, Name = "Security", Description = "Hotel security personnel" }
        );

        modelBuilder.Entity<Staff>().HasData(
            new Staff { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@hotel.com", Phone = "123-456-7890", DepartmentId = 1, Role = "Owner", HireDate = new DateTime(2020, 1, 1), IsActive = true },
            new Staff { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@hotel.com", Phone = "123-456-7891", DepartmentId = 1, Role = "Manager", HireDate = new DateTime(2022, 1, 1), IsActive = true },
            new Staff { Id = 3, FirstName = "Bob", LastName = "Johnson", Email = "bob.johnson@hotel.com", Phone = "123-456-7892", DepartmentId = 2, Role = "Receptionist", HireDate = new DateTime(2023, 1, 1), IsActive = true }
        );
    }
}