using FluentValidation;
using ReservationService.Models;

namespace ReservationService.Validators;

public class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
{
    public CreateReservationRequestValidator()
    {
        RuleFor(crr => crr.GuestId)
            .GreaterThan(0).WithMessage("Guest ID must be greater than 0.");

        RuleFor(crr => crr.RoomId)
            .GreaterThan(0).WithMessage("Room ID must be greater than 0.");

        RuleFor(crr => crr.CheckInDate)
            .NotEmpty().WithMessage("Check-in date is required.")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Check-in date cannot be in the past.");

        RuleFor(crr => crr.CheckOutDate)
            .NotEmpty().WithMessage("Check-out date is required.")
            .GreaterThan(crr => crr.CheckInDate).WithMessage("Check-out date must be after check-in date.");
    }
}

public class AvailabilityRequestValidator : AbstractValidator<AvailabilityRequest>
{
    public AvailabilityRequestValidator()
    {
        RuleFor(ar => ar.CheckIn)
            .NotEmpty().WithMessage("Check-in date is required.")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Check-in date cannot be in the past.");

        RuleFor(ar => ar.CheckOut)
            .NotEmpty().WithMessage("Check-out date is required.")
            .GreaterThan(ar => ar.CheckIn).WithMessage("Check-out date must be after check-in date.");

        RuleFor(ar => ar.Adults)
            .GreaterThan(0).WithMessage("Number of adults must be greater than 0.")
            .LessThanOrEqualTo(20).WithMessage("Number of adults cannot exceed 20.");

        RuleFor(ar => ar.Children)
            .GreaterThanOrEqualTo(0).WithMessage("Number of children must be 0 or greater.")
            .LessThanOrEqualTo(20).WithMessage("Number of children cannot exceed 20.");
    }
} 
