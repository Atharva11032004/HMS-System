namespace PricingService.Models;

public class Pricing
{
    public int Id { get; set; }
    public int RoomTypeId { get; set; }
    public RoomType? RoomType { get; set; }
    public decimal PricePerNight { get; set; }
    public DateTime EffectiveDate { get; set; }
}

public class RoomType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class PricingDto
{
    public int Id { get; set; }
    public int RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public DateTime EffectiveDate { get; set; }
}

public class QuoteRequest
{
    public int RoomTypeId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int Guests { get; set; }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}