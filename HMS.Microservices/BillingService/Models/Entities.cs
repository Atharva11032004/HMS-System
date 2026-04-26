namespace BillingService.Models;

public class Bill
{
    public int Id { get; set; }
    public int ReservationId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public List<BillLine> Lines { get; set; } = new();
}

public class BillLine
{
    public int Id { get; set; }
    public int BillId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class Payment
{
    public int Id { get; set; }
    public int BillId { get; set; }
    public decimal Amount { get; set; }
    public string CardNumber { get; set; } = string.Empty; 
    public DateTime PaidAt { get; set; } = DateTime.Now;
}

public class BillDto
{
    public int Id { get; set; }
    public int ReservationId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<BillLineDto> Lines { get; set; } = new();
}

public class BillLineDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class PaymentDto
{
    public int Id { get; set; }
    public int BillId { get; set; }
    public decimal Amount { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; }
}

public class CreateBillRequest
{
    public int ReservationId { get; set; }
    public List<BillLineDto> Lines { get; set; } = new();
}

public class CreatePaymentRequest
{
    public int BillId { get; set; }
    public decimal Amount { get; set; }
    public string CardNumber { get; set; } = string.Empty;
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}