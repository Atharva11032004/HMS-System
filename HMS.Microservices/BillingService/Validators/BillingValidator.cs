using FluentValidation;
using BillingService.Models;

namespace BillingService.Validators;

public class CreateBillRequestValidator : AbstractValidator<CreateBillRequest>
{
    public CreateBillRequestValidator()
    {
        RuleFor(cbr => cbr.ReservationId)
            .GreaterThan(0).WithMessage("Reservation ID must be greater than 0.");

        RuleFor(cbr => cbr.Lines)
            .NotEmpty().WithMessage("Bill must have at least one line item.");

        RuleForEach(cbr => cbr.Lines)
            .SetValidator(new BillLineDtoValidator());
    }
}

public class BillLineDtoValidator : AbstractValidator<BillLineDto>
{
    public BillLineDtoValidator()
    {
        RuleFor(bl => bl.Description)
            .NotEmpty().WithMessage("Description is required.")
            .Length(1, 500).WithMessage("Description must be between 1 and 500 characters.");

        RuleFor(bl => bl.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0.")
            .LessThanOrEqualTo(999999.99m).WithMessage("Amount cannot exceed 999,999.99.");
    }
}

public class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequest>
{
    public CreatePaymentRequestValidator()
    {
        RuleFor(cpr => cpr.BillId)
            .GreaterThan(0).WithMessage("Bill ID must be greater than 0.");

        RuleFor(cpr => cpr.Amount)
            .GreaterThan(0).WithMessage("Payment amount must be greater than 0.")
            .LessThanOrEqualTo(999999.99m).WithMessage("Payment amount cannot exceed 999,999.99.");

        RuleFor(cpr => cpr.CardNumber)
            .NotEmpty().WithMessage("Card number is required.")
            .Matches(@"^\d{4}$").WithMessage("Card number should be a masked 4-digit representation.");
    }
}
