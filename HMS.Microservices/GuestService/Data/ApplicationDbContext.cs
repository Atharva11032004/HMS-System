using Microsoft.EntityFrameworkCore;
using GuestService.Models;

namespace GuestService.Data;
 

// connecting with the db . 
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Guest> Guests { get; set; }
}