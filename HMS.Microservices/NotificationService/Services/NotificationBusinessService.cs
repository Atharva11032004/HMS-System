using NotificationService.Models;
using Serilog;

namespace NotificationService.Services;

public class NotificationBusinessService
{
    private readonly EmailService _emailService;

    public NotificationBusinessService(EmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task<bool> SendNotificationAsync(NotificationRequest request)
    {
        Log.Information("Sending notification - Subject: {Subject}, To: {Recipient}, Priority: {Priority}, Message: {Message}",
            request.Subject, request.RecipientEmail, request.Priority, request.Message);

      
        var result = await _emailService.SendEmailAsync(request.RecipientEmail, request.Subject, request.Message, true);

        return result;
    }

    public async Task<bool> SendReservationConfirmationAsync(string guestEmail, string guestName, 
        int roomId, DateTime checkInDate, DateTime checkOutDate, decimal totalAmount)
    {
        return await _emailService.SendReservationConfirmationEmailAsync(guestEmail, guestName, 
            roomId, checkInDate, checkOutDate, totalAmount);
    }

    public async Task<bool> SendWelcomeEmailAsync(string userEmail, string userName)
    {
        return await _emailService.SendWelcomeEmailAsync(userEmail, userName);
    }
}