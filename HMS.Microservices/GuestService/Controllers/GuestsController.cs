using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuestService.Models;
using GuestService.Services;

namespace GuestService.Controllers;

[ApiController]
[Route("guests")]
[Authorize]
public class GuestsController : ControllerBase
{
    private readonly GuestBusinessService _guestService;

    public GuestsController(GuestBusinessService guestService)
    {
        _guestService = guestService;
    }
    

    // get route 
    [HttpGet]
    public async Task<IActionResult> GetGuests()
    {
        var guests = await _guestService.GetGuestsAsync();
        return Ok(guests);
    }
   

    // get guest by id route 
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGuest(int id)
    {
        var guest = await _guestService.GetGuestAsync(id);
        if (guest == null) return NotFound();
        return Ok(guest);
    }
    
    // create guest route 
    [HttpPost]
    public async Task<IActionResult> CreateGuest([FromBody] GuestDto guest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var result = await _guestService.CreateGuestAsync(guest);
        return CreatedAtAction(nameof(GetGuest), new { id = result.Id }, result);
    }
    
    // update guest route 
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGuest(int id, [FromBody] GuestDto guest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var success = await _guestService.UpdateGuestAsync(id, guest);
        if (!success) return NotFound();
        return NoContent();
    }
    
    // delete guest route 
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGuest(int id)
    {
        var success = await _guestService.DeleteGuestAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
    

    // searching guest route 
    [HttpGet("search")]
    public async Task<IActionResult> SearchGuests([FromQuery] string? email, [FromQuery] string? phone, [FromQuery] string? name)
    {
        var guests = await _guestService.SearchGuestsAsync(email, phone, name);
        return Ok(guests);
    }
}