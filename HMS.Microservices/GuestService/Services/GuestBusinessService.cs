using Microsoft.EntityFrameworkCore;
using GuestService.Data;
using GuestService.Models;

namespace GuestService.Services;

public class GuestBusinessService
{
    private readonly ApplicationDbContext _context;

    public GuestBusinessService(ApplicationDbContext context)
    {
        _context = context;
    }


     // Get guests methods 
    public async Task<List<GuestDto>> GetGuestsAsync()
    {
        var guests = await _context.Guests.ToListAsync();
        return guests.Select(g => new GuestDto
        {
            Id = g.Id,
            FirstName = g.FirstName,
            LastName = g.LastName,
            Email = g.Email,
            Phone = g.Phone
        }).ToList();
    }
    

    // get guests by id 
    public async Task<GuestDto?> GetGuestAsync(int id)
    {
        var guest = await _context.Guests.FindAsync(id);
        if (guest == null) return null;
        return new GuestDto
        {
            Id = guest.Id,
            FirstName = guest.FirstName,
            LastName = guest.LastName,
            Email = guest.Email,
            Phone = guest.Phone
        };
    }

     // creating the guests 
     public async Task<GuestDto> CreateGuestAsync(GuestDto guestDto)
    {
        var guest = new Guest
        {
            FirstName = guestDto.FirstName,
            LastName = guestDto.LastName,
            Email = guestDto.Email,
            Phone = guestDto.Phone
        };
        _context.Guests.Add(guest);
        await _context.SaveChangesAsync();
        guestDto.Id = guest.Id;
        return guestDto;
    }

    // updating the guests 
    public async Task<bool> UpdateGuestAsync(int id, GuestDto guestDto)
    {
        var guest = await _context.Guests.FindAsync(id);
        if (guest == null) return false;
        guest.FirstName = guestDto.FirstName;
        guest.LastName = guestDto.LastName;
        guest.Email = guestDto.Email;
        guest.Phone = guestDto.Phone;
        await _context.SaveChangesAsync();
        return true;
    }
 

      // delete the guests 
    public async Task<bool> DeleteGuestAsync(int id)
    {
        var guest = await _context.Guests.FindAsync(id);
        if (guest == null) return false;
        _context.Guests.Remove(guest);
        await _context.SaveChangesAsync();
        return true;
    }
    

    // search the guests 
    public async Task<List<GuestDto>> SearchGuestsAsync(string? email, string? phone, string? name)
    {
        var query = _context.Guests.AsQueryable();
        if (!string.IsNullOrEmpty(email))
            query = query.Where(g => g.Email.Contains(email));
        if (!string.IsNullOrEmpty(phone))
            query = query.Where(g => g.Phone.Contains(phone));
        if (!string.IsNullOrEmpty(name))
            query = query.Where(g => g.FirstName.Contains(name) || g.LastName.Contains(name));

        var guests = await query.ToListAsync();
        return guests.Select(g => new GuestDto
        {
            Id = g.Id,
            FirstName = g.FirstName,
            LastName = g.LastName,
            Email = g.Email,
            Phone = g.Phone
        }).ToList();
    }
}