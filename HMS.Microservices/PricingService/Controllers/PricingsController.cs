using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PricingService.Models;
using PricingService.Services;

namespace PricingService.Controllers;

[ApiController]
[Route("pricings")]
[Authorize]
public class PricingsController : ControllerBase
{
    private readonly PricingBusinessService _pricingService;

    public PricingsController(PricingBusinessService pricingService)
    {
        _pricingService = pricingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPricings()
    {
        var pricings = await _pricingService.GetPricingsAsync();
        return Ok(pricings);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPricing(int id)
    {
        var pricing = await _pricingService.GetPricingAsync(id);
        if (pricing == null) return NotFound();
        return Ok(pricing);
    }

    [HttpPost]
    [Authorize(Roles = "Manager,Owner")]
    public async Task<IActionResult> CreatePricing([FromBody] PricingDto pricing)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var result = await _pricingService.CreatePricingAsync(pricing);
        return CreatedAtAction(nameof(GetPricing), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Manager,Owner")]
    public async Task<IActionResult> UpdatePricing(int id, [FromBody] PricingDto pricing)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var success = await _pricingService.UpdatePricingAsync(id, pricing);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager,Owner")]
    public async Task<IActionResult> DeletePricing(int id)
    {
        var success = await _pricingService.DeletePricingAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpGet("quote")]
    [AllowAnonymous]
    public async Task<IActionResult> GetQuote([FromQuery] int roomTypeId, [FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut, [FromQuery] int guests)
    {
        var request = new QuoteRequest { RoomTypeId = roomTypeId, CheckIn = checkIn, CheckOut = checkOut, Guests = guests };
        var quote = await _pricingService.GetQuoteAsync(request);
        return Ok(quote);
    }
}