namespace ReservationService.Models;

public class Room
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int RoomTypeId { get; set; }
    public RoomType? RoomType { get; set; }
    public bool IsAvailable { get; set; } = true;
}