using BillingService.Data;
using BillingService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BillingService.Services;

public class BillingBusinessService
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClientService _httpClient;
    private readonly IConfiguration _configuration;

    public BillingBusinessService(ApplicationDbContext context, HttpClientService httpClient, IConfiguration configuration)
    {
        _context = context;
        _httpClient = httpClient;
        _configuration = configuration;
    }
    

    // creating the bill 
    public async Task<BillDto> CreateBillAsync(CreateBillRequest request)
    {
        var bill = new Bill
        {
            ReservationId = request.ReservationId,
            TotalAmount = request.Lines.Sum(l => l.Amount),
            Lines = request.Lines.Select(l => new BillLine
            {
                Description = l.Description,
                Amount = l.Amount
            }).ToList()
        };

        _context.Bills.Add(bill);
        await _context.SaveChangesAsync();

        return new BillDto
        {
            Id = bill.Id,
            ReservationId = bill.ReservationId,
            TotalAmount = bill.TotalAmount,
            CreatedAt = bill.CreatedAt,
            Lines = bill.Lines.Select(l => new BillLineDto
            {
                Id = l.Id,
                Description = l.Description,
                Amount = l.Amount
            }).ToList()
        };
    }
     
    // get all the bills 
    public async Task<List<BillDto>> GetAllBillsAsync()
    {
        var bills = await _context.Bills
            .Include(b => b.Lines)
            .ToListAsync();

        return bills.Select(b => new BillDto
        {
            Id = b.Id,
            ReservationId = b.ReservationId,
            TotalAmount = b.TotalAmount,
            CreatedAt = b.CreatedAt,
            Lines = b.Lines.Select(l => new BillLineDto
            {
                Id = l.Id,
                Description = l.Description,
                Amount = l.Amount
            }).ToList()
        }).ToList();
    }
    
    // get bills by id 
    public async Task<BillDto?> GetBillAsync(int id)
    {
        var bill = await _context.Bills
            .Include(b => b.Lines)
            .FirstOrDefaultAsync(b => b.Id == id);
        if (bill == null) return null;

        return new BillDto
        {
            Id = bill.Id,
            ReservationId = bill.ReservationId,
            TotalAmount = bill.TotalAmount,
            CreatedAt = bill.CreatedAt,
            Lines = bill.Lines.Select(l => new BillLineDto
            {
                Id = l.Id,
                Description = l.Description,
                Amount = l.Amount
            }).ToList()
        };
    }
    
    // adding the bill lines 
    public async Task<BillLineDto> AddBillLineAsync(int billId, BillLineDto lineDto)
    {
        var bill = await _context.Bills.FindAsync(billId);
        if (bill == null) throw new Exception("Bill not found");

        var line = new BillLine
        {
            BillId = billId,
            Description = lineDto.Description,
            Amount = lineDto.Amount
        };

        _context.BillLines.Add(line);
        bill.TotalAmount += line.Amount;
        await _context.SaveChangesAsync();

        lineDto.Id = line.Id;
        return lineDto;
    }
    
    // creating the payments 
    public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentRequest request)
    {
        var bill = await _context.Bills.FindAsync(request.BillId);
        if (bill == null) throw new Exception("Bill not found");

        var payment = new Payment
        {
            BillId = request.BillId,
            Amount = request.Amount,
            CardNumber = MaskCardNumber(request.CardNumber)
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

       
        var notificationServiceUrl = _configuration["ServiceUrls:NotificationService"] ?? "http://localhost:5022";
        var emailBody = GeneratePaymentConfirmationEmail(payment.Amount, payment.PaidAt);
        await _httpClient.PostAsync<object>($"{notificationServiceUrl}/api/Notifications/notify", new { 
            Subject = "Payment Received - HMS", 
            Message = emailBody, 
            RecipientEmail = "guest@example.com" 
        });

        return new PaymentDto
        {
            Id = payment.Id,
            BillId = payment.BillId,
            Amount = payment.Amount,
            CardNumber = payment.CardNumber,
            PaidAt = payment.PaidAt
        };
    }

    private string MaskCardNumber(string cardNumber)
    {
        if (cardNumber.Length < 4) return cardNumber;
        return "****" + cardNumber[^4..];
    }

    private string GeneratePaymentConfirmationEmail(decimal amount, DateTime paidAt)
    {
        return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Payment Confirmation</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            margin: 0;
            padding: 20px;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            background: white;
            border-radius: 12px;
            box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
            overflow: hidden;
        }}
        .header {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 40px 20px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 28px;
            font-weight: 600;
        }}
        .header p {{
            margin: 10px 0 0 0;
            font-size: 14px;
            opacity: 0.9;
        }}
        .content {{
            padding: 40px 30px;
        }}
        .success-icon {{
            text-align: center;
            margin-bottom: 20px;
        }}
        .success-icon svg {{
            width: 60px;
            height: 60px;
            fill: #10b981;
        }}
        .message {{
            text-align: center;
            margin-bottom: 30px;
        }}
        .message h2 {{
            color: #10b981;
            margin: 0 0 10px 0;
            font-size: 24px;
        }}
        .message p {{
            color: #6b7280;
            margin: 0;
            font-size: 16px;
        }}
        .details {{
            background: #f9fafb;
            border-left: 4px solid #667eea;
            padding: 20px;
            border-radius: 6px;
            margin-bottom: 30px;
        }}
        .detail-row {{
            display: flex;
            justify-content: space-between;
            margin-bottom: 12px;
            font-size: 15px;
        }}
        .detail-row:last-child {{
            margin-bottom: 0;
        }}
        .detail-label {{
            color: #6b7280;
            font-weight: 500;
        }}
        .detail-value {{
            color: #1f2937;
            font-weight: 600;
        }}
        .amount {{
            font-size: 28px;
            color: #10b981;
            font-weight: bold;
        }}
        .footer {{
            background: #f9fafb;
            padding: 20px 30px;
            text-align: center;
            border-top: 1px solid #e5e7eb;
            font-size: 12px;
            color: #6b7280;
        }}
        .reference {{
            background: white;
            border: 1px dashed #d1d5db;
            padding: 10px;
            border-radius: 4px;
            margin-top: 15px;
            word-break: break-all;
            font-size: 12px;
            color: #4b5563;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>HMS - Hotel Management System</h1>
            <p>Payment Confirmation</p>
        </div>
        <div class=""content"">
            <div class=""success-icon"">
                <svg viewBox=""0 0 24 24"" xmlns=""http://www.w3.org/2000/svg"">
                    <path d=""M9 16.17L4.83 12l-1.42 1.41L9 19 21 7l-1.41-1.41L9 16.17z""/>
                </svg>
            </div>
            <div class=""message"">
                <h2>Payment Successful!</h2>
                <p>Your payment has been processed successfully</p>
            </div>
            <div class=""details"">
                <div class=""detail-row"">
                    <span class=""detail-label"">Amount Paid:</span>
                    <span class=""amount"">${amount:F2}</span>
                </div>
                <div class=""detail-row"">
                    <span class=""detail-label"">Payment Date:</span>
                    <span class=""detail-value"">{paidAt:MMMM dd, yyyy}</span>
                </div>
                <div class=""detail-row"">
                    <span class=""detail-label"">Payment Time:</span>
                    <span class=""detail-value"">{paidAt:hh:mm tt}</span>
                </div>
                <div class=""detail-row"">
                    <span class=""detail-label"">Status:</span>
                    <span class=""detail-value"" style=""color: #10b981;"">✓ Confirmed</span>
                </div>
                <div class=""reference"">
                    <strong>Transaction Reference:</strong> TXN{DateTime.Now:yyyyMMddHHmmss}
                </div>
            </div>
        </div>
        <div class=""footer"">
            <p>Thank you for your payment. If you have any questions, please contact our support team.</p>
            <p style=""margin-top: 10px; color: #9ca3af;"">© 2026 Hotel Management System. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
    }
}