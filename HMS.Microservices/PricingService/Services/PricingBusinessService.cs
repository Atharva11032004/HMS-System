using Microsoft.EntityFrameworkCore;
using PricingService.Data;
using PricingService.Models;

namespace PricingService.Services;

public class PricingBusinessService
{
    private readonly ApplicationDbContext _context;

    public PricingBusinessService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // getting the pricings 
    public async Task<List<PricingDto>> GetPricingsAsync()
    {
        var pricings = await _context.Pricings
            .Include(p => p.RoomType)
            .ToListAsync();
        return pricings.Select(p => new PricingDto
        {
            Id = p.Id,
            RoomTypeId = p.RoomTypeId,
            RoomTypeName = p.RoomType!.Name,
            PricePerNight = p.PricePerNight,
            EffectiveDate = p.EffectiveDate
        }).ToList();
    }

    public async Task<PricingDto?> GetPricingAsync(int id)
    {
        var pricing = await _context.Pricings
            .Include(p => p.RoomType)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (pricing == null) return null;
        return new PricingDto
        {
            Id = pricing.Id,
            RoomTypeId = pricing.RoomTypeId,
            RoomTypeName = pricing.RoomType!.Name,
            PricePerNight = pricing.PricePerNight,
            EffectiveDate = pricing.EffectiveDate
        };
    }

    public async Task<PricingDto> CreatePricingAsync(PricingDto pricingDto)
    {
        var pricing = new Pricing
        {
            RoomTypeId = pricingDto.RoomTypeId,
            PricePerNight = pricingDto.PricePerNight,
            EffectiveDate = pricingDto.EffectiveDate
        };
        _context.Pricings.Add(pricing);
        await _context.SaveChangesAsync();
        pricingDto.Id = pricing.Id;
        return pricingDto;
    }

    public async Task<bool> UpdatePricingAsync(int id, PricingDto pricingDto)
    {
        var pricing = await _context.Pricings.FindAsync(id);
        if (pricing == null) return false;
        pricing.RoomTypeId = pricingDto.RoomTypeId;
        pricing.PricePerNight = pricingDto.PricePerNight;
        pricing.EffectiveDate = pricingDto.EffectiveDate;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePricingAsync(int id)
    {
        var pricing = await _context.Pricings.FindAsync(id);
        if (pricing == null) return false;
        _context.Pricings.Remove(pricing);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> GetQuoteAsync(QuoteRequest request)
    {
        var pricing = await _context.Pricings
            .Where(p => p.RoomTypeId == request.RoomTypeId && p.EffectiveDate <= request.CheckIn)
            .OrderByDescending(p => p.EffectiveDate)
            .FirstOrDefaultAsync();
        if (pricing == null) return 0;
        var nights = (request.CheckOut - request.CheckIn).Days;
        return pricing.PricePerNight * nights;
    }
}