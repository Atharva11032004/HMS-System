using FluentValidation;
using PricingService.Models;

namespace PricingService.Validators;

public class QuoteRequestValidator : AbstractValidator<QuoteRequest>
{
    public QuoteRequestValidator()
    {
        RuleFor(qr => qr.RoomTypeId)
            .GreaterThan(0).WithMessage("Room type ID must be greater than 0.");

        RuleFor(qr => qr.CheckIn)
            .NotEmpty().WithMessage("Check-in date is required.")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Check-in date cannot be in the past.");

        RuleFor(qr => qr.CheckOut)
            .NotEmpty().WithMessage("Check-out date is required.")
            .GreaterThan(qr => qr.CheckIn).WithMessage("Check-out date must be after check-in date.");

        RuleFor(qr => qr.Guests)
            .GreaterThan(0).WithMessage("Number of guests must be greater than 0.")
            .LessThanOrEqualTo(20).WithMessage("Number of guests cannot exceed 20.");
    }
}

public class PricingDtoValidator : AbstractValidator<PricingDto>
{
    public PricingDtoValidator()
    {
        RuleFor(pd => pd.RoomTypeId)
            .GreaterThan(0).WithMessage("Room type ID must be greater than 0.");

        RuleFor(pd => pd.PricePerNight)
            .GreaterThan(0).WithMessage("Price per night must be greater than 0.")
            .LessThanOrEqualTo(10000).WithMessage("Price per night cannot exceed 10,000.");

        RuleFor(pd => pd.EffectiveDate)
            .NotEmpty().WithMessage("Effective date is required.");
    }
}
