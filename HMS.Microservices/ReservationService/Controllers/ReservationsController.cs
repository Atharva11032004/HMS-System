using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservationService.Models;
using ReservationService.Services;
using Serilog;

namespace ReservationService.Controllers;

[ApiController]
[Route("reservations")]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly ReservationBusinessService _reservationService;

    public ReservationsController(ReservationBusinessService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        try
        {
            Log.Information("Creating reservation for GuestId: {GuestId}, RoomId: {RoomId}, CheckIn: {CheckIn}, CheckOut: {CheckOut}",
                request.GuestId, request.RoomId, request.CheckInDate, request.CheckOutDate);
            
            var result = await _reservationService.CreateReservationAsync(request);
            if (result == null) 
            {
                Log.Warning("Failed to create reservation - Guest not found or Room not available. GuestId: {GuestId}, RoomId: {RoomId}",
                    request.GuestId, request.RoomId);
                return BadRequest(new { Error = "Guest not found or Room not available for the selected dates" });
            }
            
            Log.Information("Reservation created successfully. ReservationId: {ReservationId}, GuestId: {GuestId}",
                result.Id, result.GuestId);
            return CreatedAtAction(nameof(GetReservation), new { id = result.Id }, result);
        }
     
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating reservation for GuestId: {GuestId}", request.GuestId);
            return StatusCode(500, new { Error = "An error occurred while creating the reservation", Details = ex.Message });
        }
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelReservation(int id)
    {
        var success = await _reservationService.CancelReservationAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReservation(int id)
    {
        var reservation = await _reservationService.GetReservationAsync(id);
        if (reservation == null) return NotFound();
        return Ok(reservation);
    }

    [HttpGet]
    public async Task<IActionResult> GetReservations([FromQuery] int? guestId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var reservations = await _reservationService.GetReservationsAsync(guestId, from, to);
        return Ok(reservations);
    }

    [HttpGet("availability")]
    public async Task<IActionResult> GetAvailability([FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut, [FromQuery] int adults, [FromQuery] int children)
    {
        var request = new AvailabilityRequest { CheckIn = checkIn, CheckOut = checkOut, Adults = adults, Children = children };
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var result = await _reservationService.GetAvailabilityAsync(request);
        return Ok(result);
    }
}