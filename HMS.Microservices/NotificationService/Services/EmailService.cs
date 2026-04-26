using System.Net;
using System.Net.Mail;
using Serilog;

namespace NotificationService.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _senderEmail;
    private readonly string _senderPassword;
    private readonly bool _enableSSL;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        _smtpServer = _configuration["Smtp:Server"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(_configuration["Smtp:Port"] ?? "587");
        _senderEmail = _configuration["Smtp:SenderEmail"] ?? "";
        _senderPassword = _configuration["Smtp:SenderPassword"] ?? "";
        _enableSSL = bool.Parse(_configuration["Smtp:EnableSSL"] ?? "true");

        Log.Information("EmailService initialized - Server: {SmtpServer}, Port: {SmtpPort}, Sender: {SenderEmail}", 
            _smtpServer, _smtpPort, _senderEmail);
    }

    public async Task<bool> SendEmailAsync(string recipientEmail, string subject, string body, bool isHtml = true)
    {
        try
        {
            
            if (string.IsNullOrWhiteSpace(_senderEmail))
            {
                Log.Error("Sender email is not configured");
                return false;
            }

            if (string.IsNullOrWhiteSpace(_senderPassword))
            {
                Log.Error("Sender password is not configured");
                return false;
            }

            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                Log.Error("Recipient email is empty");
                return false;
            }

            Log.Information("Sending email to {RecipientEmail} via SMTP server {SmtpServer}:{SmtpPort}", 
                recipientEmail, _smtpServer, _smtpPort);

            using (var client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.EnableSsl = _enableSSL;
                client.Credentials = new NetworkCredential(_senderEmail, _senderPassword);
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(_senderEmail, "HMS - Hotel Management System");
                    mailMessage.To.Add(recipientEmail);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = isHtml;

                    await client.SendMailAsync(mailMessage);

                    Log.Information("Email sent successfully to {RecipientEmail} with subject {Subject}", 
                        recipientEmail, subject);
                    return true;
                }
            }
        }
        catch (SmtpException smtpEx)
        {
            Log.Error(smtpEx, "SMTP Error sending email to {RecipientEmail}. Status: {StatusCode}, Message: {SmtpMessage}", 
                recipientEmail, smtpEx.StatusCode, smtpEx.Message);
            return false;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to send email to {RecipientEmail} with subject {Subject}. Error: {ErrorMessage}", 
                recipientEmail, subject, ex.Message);
            return false;
        }
    }

    public async Task<bool> SendReservationConfirmationEmailAsync(string guestEmail, string guestName, 
        int roomId, DateTime checkInDate, DateTime checkOutDate, decimal totalAmount)
    {
        if (string.IsNullOrWhiteSpace(guestEmail))
        {
            Log.Warning("Guest email is empty for reservation confirmation");
            return false;
        }

        var subject = "Reservation Confirmed - HMS Hotel Management System";
        var body = $@"
            <html>
               <body style='font-family: Arial, Helvetica, sans-serif; background-color: #f4f4f4; padding: 20px; margin: 0;'>
    <table style='width: 100%; max-width: 650px; margin: auto; background: #ffffff; border-radius: 8px; box-shadow: 0 4px 10px rgba(0,0,0,0.08); overflow: hidden;'>
        
        <!-- Header -->
        <tr>
            <td style='background: #2c3e50; padding: 20px; text-align: center; color: #ffffff;'>
                <h1 style='margin: 0; font-size: 26px; letter-spacing: 1px;'>HMS Hotel Management System</h1>
            </td>
        </tr>

        <!-- Body -->
        <tr>
            <td style='padding: 25px 30px; color: #333333;'>

                <h2 style='color: #2c3e50; margin-top: 0;'>Reservation Confirmation</h2>

                <p style='font-size: 16px; line-height: 1.6;'>
                    Dear <strong>{guestName}</strong>,
                </p>

                <p style='font-size: 16px; line-height: 1.6;'>
                    We are pleased to inform you that your reservation has been successfully confirmed.
                </p>

                <hr style='border: none; height: 1px; background: #e0e0e0; margin: 25px 0;' />

                <h3 style='color: #2c3e50;'>Reservation Details</h3>

                <table style='font-size: 15px; width: 100%; background: #f9f9f9; border-radius: 6px;'>
                    <tr>
                        <td style='padding: 6px; width: 40%;'><strong>Room ID:</strong></td>
                        <td style='padding: 6px;'>{roomId}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px; width: 40%;'><strong>Check-in Date:</strong></td>
                        <td style='padding: 6px;'>{checkInDate:dd-MM-yyyy}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px; width: 40%;'><strong>Check-out Date:</strong></td>
                        <td style='padding: 6px;'>{checkOutDate:dd-MM-yyyy}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px; width: 40%;'><strong>Total Amount:</strong></td>
                        <td style='padding: 6px;'>$ {totalAmount:F2}</td>
                    </tr>
                </table>

                <hr style='border: none; height: 1px; background: #e0e0e0; margin: 25px 0;' />

                <p style='font-size: 16px; line-height: 1.6;'>
                    We look forward to welcoming you and ensuring a pleasant stay.
                </p>

                <p style='font-size: 16px; line-height: 1.6; margin-top: 30px;'>
                    Warm regards,<br/>
                    <strong>HMS Hotel Management System</strong>
                </p>

            </td>
        </tr>

        <!-- Footer -->
        <tr>
            <td style='background: #ecf0f1; padding: 15px; text-align: center; font-size: 13px; color: #7f8c8d;'>
                This is an automated message. Please do not reply.
            </td>
        </tr>

    </table>
</body>
            </html>";

        return await SendEmailAsync(guestEmail, subject, body, true);
    }

    public async Task<bool> SendWelcomeEmailAsync(string userEmail, string userName)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
        {
            Log.Warning("User email is empty for welcome email");
            return false;
        }

        var subject = "Welcome to HMS - Hotel Management System";
        var body = $@"
            <html>
    <body style='font-family: Arial, Helvetica, sans-serif; background-color: #f4f4f4; padding: 20px; margin: 0;'>
        
        <table style='width: 100%; max-width: 650px; margin: auto; background: #ffffff; border-radius: 8px; box-shadow: 0 4px 10px rgba(0,0,0,0.08); overflow: hidden;'>
            
            <!-- Header -->
            <tr>
                <td style='background: #2c3e50; padding: 20px; text-align: center; color: #ffffff;'>
                    <h2 style='margin: 0; font-size: 26px;'>Welcome to HMS!</h2>
                </td>
            </tr>

            <!-- Body -->
            <tr>
                <td style='padding: 25px 30px; color: #333333;'>

                    <p style='font-size: 16px; line-height: 1.6;'>
                        Hello <strong>{userName}</strong>,
                    </p>

                    <p style='font-size: 16px; line-height: 1.6;'>
                        Your account has been successfully registered. Welcome to the Hotel Management System!
                    </p>

                    <hr style='border: none; height: 1px; background: #e0e0e0; margin: 25px 0;' />

                    <p style='font-size: 16px; line-height: 1.6;'><strong>Account Information:</strong></p>

                    <ul style='font-size: 15px; background: #f9f9f9; border-radius: 6px; padding: 15px; list-style: none;'>
                        <li style='margin-bottom: 8px;'><strong>Email:</strong> {userEmail}</li>
                    </ul>

                    <hr style='border: none; height: 1px; background: #e0e0e0; margin: 25px 0;' />

                    <p style='font-size: 16px; line-height: 1.6;'>
                        You can now log in to your account and start making reservations.
                    </p>

                    <p style='font-size: 16px; line-height: 1.6;'>
                        If you have any questions, please don't hesitate to contact our support team.
                    </p>

                    <p style='font-size: 16px; line-height: 1.6; margin-top: 30px;'>
                        Best regards,<br/>
                        <strong>HMS Hotel Management System</strong>
                    </p>

                </td>
            </tr>

            <!-- Footer -->
            <tr>
                <td style='background: #ecf0f1; padding: 15px; text-align: center; font-size: 13px; color: #7f8c8d;'>
                    This is an automated message. Please do not reply.
                </td>
            </tr>

        </table>

    </body>
</html>";

        return await SendEmailAsync(userEmail, subject, body, true);
    }
}