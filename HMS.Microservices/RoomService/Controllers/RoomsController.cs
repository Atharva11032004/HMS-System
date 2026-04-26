using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomService.Models;
using RoomService.Services;

namespace RoomService.Controllers;

[ApiController]
[Route("rooms")]
[Authorize]
public class RoomsController : ControllerBase
{
    private readonly RoomBusinessService _roomService;

    public RoomsController(RoomBusinessService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRooms()
    {
        var rooms = await _roomService.GetRoomsAsync();
        return Ok(rooms);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoom(int id)
    {
        var room = await _roomService.GetRoomAsync(id);
        if (room == null) return NotFound();
        return Ok(room);
    }

    [HttpPost]
    [Authorize(Roles = "Manager,Owner")]
    public async Task<IActionResult> CreateRoom([FromBody] Room room)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var result = await _roomService.CreateRoomAsync(room);
        return CreatedAtAction(nameof(GetRoom), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Manager,Owner")]
    public async Task<IActionResult> UpdateRoom(int id, [FromBody] Room room)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        try
        {
            var success = await _roomService.UpdateRoomAsync(id, room);
            if (!success) return NotFound(new { Error = "Room not found" });
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager,Owner")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var success = await _roomService.DeleteRoomAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpGet("available")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAvailable([FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut, [FromQuery] int adults = 1, [FromQuery] int children = 0)
    {
        var rooms = await _roomService.GetAvailableRoomsAsync(checkIn, checkOut, adults, children);
        var response = new AvailabilityResponse { AvailableRooms = rooms };
        return Ok(response);
    }

    [HttpPost("block")]
    [AllowAnonymous]
    public async Task<IActionResult> BlockRoom([FromBody] BlockRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        try
        {
            await _roomService.BlockRoomAsync(request.RoomId, request.CheckIn, request.CheckOut);
            return Ok(new { Message = "Room blocked successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("free")]
    public async Task<IActionResult> FreeRoom([FromBody] int roomId)
    {
        await _roomService.FreeRoomAsync(roomId);
        return Ok();
    }
}

[ApiController]
[Route("roomtypes")]
[Authorize]
public class RoomTypesController : ControllerBase
{
    private readonly RoomBusinessService _roomService; 



    public RoomTypesController(RoomBusinessService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRoomTypes()
    {
        var roomTypes = await _roomService.GetRoomTypesAsync();
        return Ok(roomTypes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoomType(int id)
    {
        var roomType = await _roomService.GetRoomTypeAsync(id);
        if (roomType == null) return NotFound();
        return Ok(roomType); 
    }

    [HttpPost]
    [Authorize(Roles = "Manager,Owner")]
    public async Task<IActionResult> CreateRoomType([FromBody] RoomType roomType)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var result = await _roomService.CreateRoomTypeAsync(roomType);
        return CreatedAtAction(nameof(GetRoomType), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Manager,Owner")]
    public async Task<IActionResult> UpdateRoomType(int id, [FromBody] RoomType roomType)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var success = await _roomService.UpdateRoomTypeAsync(id, roomType);
        if (!success) return NotFound(new { Error = "Room type not found" });
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager,Owner")]
    public async Task<IActionResult> DeleteRoomType(int id)
    {
        var success = await _roomService.DeleteRoomTypeAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}