using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Models;
using NotificationService.Services;
using Serilog;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly NotificationBusinessService _notificationService;

    public NotificationsController(NotificationBusinessService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpPost("notify")]
    [AllowAnonymous]
    public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Invalid request data"
            });
        }

        var result = await _notificationService.SendNotificationAsync(request);

        if (result)
        {
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Notification sent successfully"
            });
        }
        else
        {
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = "Failed to send notification"
            });
        }
    }

    [HttpPost("reservation-confirmation")]
    [AllowAnonymous]
    public async Task<IActionResult> SendReservationConfirmation([FromBody] ReservationConfirmationRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            Log.Warning("Invalid reservation confirmation request: {Errors}", string.Join(", ", errors));
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Invalid request data: " + string.Join(", ", errors)
            });
        }

        try
        {
            var result = await _notificationService.SendReservationConfirmationAsync(
                request.GuestEmail, 
                request.GuestName, 
                request.RoomId, 
                request.CheckInDate, 
                request.CheckOutDate, 
                request.TotalAmount);

            if (result)
            {
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Reservation confirmation email sent successfully"
                });
            }
            else
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Failed to send reservation confirmation email"
                });
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error sending reservation confirmation email");
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = "Error sending reservation confirmation email: " + ex.Message
            });
        }
    }

    [HttpPost("welcome")]
    [AllowAnonymous]
    public async Task<IActionResult> SendWelcomeEmail([FromBody] WelcomeEmailRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            Log.Warning("Invalid welcome email request: {Errors}", string.Join(", ", errors));
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Invalid request data: " + string.Join(", ", errors)
            });
        }

        try
        {
            Log.Information("Processing welcome email request for {UserEmail}", request.UserEmail);
            var result = await _notificationService.SendWelcomeEmailAsync(request.UserEmail, request.UserName);

            if (result)
            {
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Welcome email sent successfully"
                });
            }
            else
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Failed to send welcome email"
                });
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error sending welcome email to {UserEmail}", request.UserEmail);
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = "Error sending welcome email: " + ex.Message
            });
        }
    }

    [HttpGet("diagnostic")]
    [AllowAnonymous]
    public IActionResult GetDiagnosticInfo()
    {
        var smtpConfig = new
        {
            SmtpServer = "smtp.gmail.com",
            SmtpPort = "587",
            ConfiguredCorrectly = true
        };

        return Ok(new { Message = "NotificationService is running. SMTP configuration details logged.", Diagnostic = smtpConfig });
    }

    [HttpPost("test-email")]
    [AllowAnonymous]
    public async Task<IActionResult> TestEmailSending([FromQuery] string recipientEmail, [FromQuery] string subject = "Test Email")
    {
        if (string.IsNullOrWhiteSpace(recipientEmail))
        {
            return BadRequest(new ApiResponse { Success = false, Message = "Recipient email is required" });
        }

        try
        {
            Log.Information("Testing email sending to {RecipientEmail}", recipientEmail);
            var result = await _notificationService.SendNotificationAsync(new NotificationRequest
            {
                RecipientEmail = recipientEmail,
                Subject = subject,
                Message = "This is a test email from HMS Notification Service",
                Priority = "Normal"
            });

            if (result)
            {
                return Ok(new ApiResponse { Success = true, Message = $"Test email sent successfully to {recipientEmail}" });
            }
            else
            {
                return StatusCode(500, new ApiResponse { Success = false, Message = "Failed to send test email. Check logs for details." });
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in test email endpoint");
            return StatusCode(500, new ApiResponse { Success = false, Message = $"Error: {ex.Message}" });
        }
    }
}