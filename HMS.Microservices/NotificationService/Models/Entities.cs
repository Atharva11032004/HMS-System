using System.ComponentModel.DataAnnotations;

namespace NotificationService.Models;

public class NotificationRequest
{
    [Required]
    [StringLength(200)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Message { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string RecipientEmail { get; set; } = string.Empty;

    public string? Priority { get; set; } = "Normal"; // Normal, High, Low
}

public class ReservationConfirmationRequest
{
    [Required]
    [EmailAddress]
    public string GuestEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string GuestName { get; set; } = string.Empty;

    [Required]
    public int RoomId { get; set; }

    [Required]
    public DateTime CheckInDate { get; set; }

    [Required]
    public DateTime CheckOutDate { get; set; }

    [Required]
    public decimal TotalAmount { get; set; }
}

public class WelcomeEmailRequest
{
    [Required]
    [EmailAddress]
    public string UserEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string UserName { get; set; } = string.Empty;
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}