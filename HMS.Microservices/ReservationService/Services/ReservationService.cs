using Microsoft.EntityFrameworkCore;
using ReservationService.Data;
using ReservationService.Models;

namespace ReservationService.Services;

public class ReservationBusinessService
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClientService _httpClient;
    private readonly IConfiguration _configuration;

    public ReservationBusinessService(ApplicationDbContext context, HttpClientService httpClient, IConfiguration configuration)
    {
        _context = context;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<ReservationDto?> CreateReservationAsync(CreateReservationRequest request)
    {
        var roomServiceUrl = _configuration["ServiceUrls:RoomService"] ?? "http://localhost:5187";
        var pricingServiceUrl = _configuration["ServiceUrls:PricingService"] ?? "http://localhost:5032";
        var notificationServiceUrl = _configuration["ServiceUrls:NotificationService"] ?? "http://localhost:5022";
        var guestServiceUrl = _configuration["ServiceUrls:GuestService"] ?? "http://localhost:5281";
        var billingServiceUrl = _configuration["ServiceUrls:BillingService"] ?? "http://localhost:5114";

        try
        {
            // Verify guest exists in GuestService
            GuestDto? guestResponse = null;
            try
            {
                guestResponse = await _httpClient.GetAsync<GuestDto>($"{guestServiceUrl}/guests/{request.GuestId}");
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, "Failed to fetch guest {GuestId} from GuestService", request.GuestId);
            }
            if (guestResponse == null)
            {
                Serilog.Log.Warning("Guest {GuestId} not found in GuestService", request.GuestId);
                return null;
            }

            // Check room availability
            AvailabilityResponse? availability = null;
            try
            {
                availability = await _httpClient.GetAsync<AvailabilityResponse>($"{roomServiceUrl}/rooms/available?checkIn={request.CheckInDate:yyyy-MM-dd}&checkOut={request.CheckOutDate:yyyy-MM-dd}&adults=1&children=0");
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, "Failed to fetch availability from RoomService");
            }

            // If availability check fails or room not in available list, try to get room directly
            RoomDto? selectedRoom = availability?.AvailableRooms?.FirstOrDefault(r => r.Id == request.RoomId);
            if (selectedRoom == null)
            {
                Serilog.Log.Warning("Room {RoomId} not found in availability. Available rooms: {Rooms}",
                    request.RoomId,
                    string.Join(",", availability?.AvailableRooms?.Select(r => r.Id.ToString()) ?? Array.Empty<string>()));
                return null;
            }

            var quote = await _httpClient.GetAsync<decimal>($"{pricingServiceUrl}/pricings/quote?roomTypeId=1&checkIn={request.CheckInDate:yyyy-MM-dd}&checkOut={request.CheckOutDate:yyyy-MM-dd}&guests=2");

            try
            {
                await _httpClient.PostAsync<object>($"{roomServiceUrl}/rooms/block", new { RoomId = request.RoomId, CheckIn = request.CheckInDate, CheckOut = request.CheckOutDate });
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, "Failed to block room {RoomId} — continuing with reservation creation", request.RoomId);
            }

            var reservation = new Reservation
            {
                GuestId = request.GuestId,
                RoomId = request.RoomId,
                RoomNumber = selectedRoom.RoomNumber,
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                TotalAmount = quote,
                Status = "Confirmed"
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            try
            {
                await _httpClient.PostAsync<object>($"{billingServiceUrl}/bills", new
                {
                    ReservationId = reservation.Id,
                    Lines = new[]
                    {
                        new { Description = $"Room charges ({reservation.CheckInDate:dd MMM} - {reservation.CheckOutDate:dd MMM})", Amount = reservation.TotalAmount }
                    }
                });
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, "Failed to auto-create bill for ReservationId: {ReservationId}", reservation.Id);
            }

            if (!string.IsNullOrEmpty(guestResponse.Email))
            {
                var guestName = guestResponse.FirstName + " " + guestResponse.LastName;
                await _httpClient.PostAsync<object>(
                    $"{notificationServiceUrl}/api/notifications/reservation-confirmation",
                    new {
                        GuestEmail = guestResponse.Email,
                        GuestName = guestName,
                        RoomId = reservation.RoomId,
                        CheckInDate = reservation.CheckInDate,
                        CheckOutDate = reservation.CheckOutDate,
                        TotalAmount = reservation.TotalAmount
                    });
            }

            return new ReservationDto
            {
                Id = reservation.Id,
                GuestId = reservation.GuestId,
                RoomId = reservation.RoomId,
                RoomNumber = reservation.RoomNumber,
                CheckInDate = reservation.CheckInDate,
                CheckOutDate = reservation.CheckOutDate,
                TotalAmount = reservation.TotalAmount,
                Status = reservation.Status
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error creating reservation. GuestService: {guestServiceUrl}, RoomService: {roomServiceUrl}, PricingService: {pricingServiceUrl}. {ex.Message}", ex);
        }
    }

    public async Task<bool> CancelReservationAsync(int id)
    {
        var roomServiceUrl = _configuration["ServiceUrls:RoomService"] ?? "http://localhost:5187";

        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null) return false;

        reservation.Status = "Cancelled";
        await _context.SaveChangesAsync();

        await _httpClient.PostAsync<object>($"{roomServiceUrl}/rooms/free", new { RoomId = reservation.RoomId });

        return true;
    }

    public async Task<ReservationDto?> GetReservationAsync(int id)
    {
        var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id);
        if (reservation == null) return null;

        return new ReservationDto
        {
            Id = reservation.Id,
            GuestId = reservation.GuestId,
            GuestName = $"Guest {reservation.GuestId}",
            RoomId = reservation.RoomId,
            RoomNumber = reservation.RoomNumber,
            CheckInDate = reservation.CheckInDate,
            CheckOutDate = reservation.CheckOutDate,
            TotalAmount = reservation.TotalAmount,
            Status = reservation.Status
        };
    }

    public async Task<List<ReservationDto>> GetReservationsAsync(int? guestId, DateTime? from, DateTime? to)
    {
        var query = _context.Reservations.AsQueryable();

        if (guestId.HasValue)
            query = query.Where(r => r.GuestId == guestId);

        if (from.HasValue)
            query = query.Where(r => r.CheckInDate >= from);

        if (to.HasValue)
            query = query.Where(r => r.CheckOutDate <= to);

        var reservations = await query.ToListAsync();

        return reservations.Select(r => new ReservationDto
        {
            Id = r.Id,
            GuestId = r.GuestId,
            GuestName = $"Guest {r.GuestId}",
            RoomId = r.RoomId,
            RoomNumber = r.RoomNumber,
            CheckInDate = r.CheckInDate,
            CheckOutDate = r.CheckOutDate,
            TotalAmount = r.TotalAmount,
            Status = r.Status
        }).ToList();
    }

    public async Task<AvailabilityResponse> GetAvailabilityAsync(AvailabilityRequest request)
    {
        var roomServiceUrl = _configuration["ServiceUrls:RoomService"] ?? "http://localhost:5187";
        return await _httpClient.GetAsync<AvailabilityResponse>($"{roomServiceUrl}/rooms/available?checkIn={request.CheckIn:yyyy-MM-dd}&checkOut={request.CheckOut:yyyy-MM-dd}&adults={request.Adults}&children={request.Children}") ?? new AvailabilityResponse();
    }
}
