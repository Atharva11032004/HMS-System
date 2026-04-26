namespace ReservationService.Models;

public class Pricing
{
    public int Id { get; set; }
    public int RoomTypeId { get; set; }
    public RoomType? RoomType { get; set; }
    public decimal PricePerNight { get; set; }
    public DateTime EffectiveDate { get; set; }
}