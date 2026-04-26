using FluentValidation;
using RoomService.Models;

namespace RoomService.Validators;

public class RoomValidator : AbstractValidator<Room>
{
    public RoomValidator()
    {
        RuleFor(r => r.RoomNumber)
            .NotEmpty().WithMessage("Room number is required.")
            .Length(1, 50).WithMessage("Room number must be between 1 and 50 characters.");

        RuleFor(r => r.RoomTypeId)
            .GreaterThan(0).WithMessage("Room type ID must be greater than 0.");

        RuleFor(r => r.IsAvailable)
            .NotNull().WithMessage("IsAvailable must be specified.");
    }
}

public class RoomTypeValidator : AbstractValidator<RoomType>
{
    public RoomTypeValidator()
    {
        RuleFor(rt => rt.Name)
            .NotEmpty().WithMessage("Room type name is required.")
            .Length(1, 100).WithMessage("Room type name must be between 1 and 100 characters.");

        RuleFor(rt => rt.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(rt => rt.MaxOccupancy)
            .GreaterThan(0).WithMessage("Maximum occupancy must be greater than 0.")
            .LessThanOrEqualTo(10).WithMessage("Maximum occupancy cannot exceed 10 guests.");
    }
}

public class BlockRequestValidator : AbstractValidator<BlockRequest>
{
    public BlockRequestValidator()
    {
        RuleFor(br => br.RoomId)
            .GreaterThan(0).WithMessage("Room ID must be greater than 0.");

        RuleFor(br => br.CheckIn)
            .NotEmpty().WithMessage("Check-in date is required.")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Check-in date cannot be in the past.");

        RuleFor(br => br.CheckOut)
            .NotEmpty().WithMessage("Check-out date is required.")
            .GreaterThan(br => br.CheckIn).WithMessage("Check-out date must be after check-in date.");
    }
}
