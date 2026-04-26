using FluentValidation;
using GuestService.Models;

namespace GuestService.Validators;

public class GuestValidator : AbstractValidator<Guest>
{
    public GuestValidator()
    {
        RuleFor(g => g.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .Length(1, 100).WithMessage("First name must be between 1 and 100 characters.");

        RuleFor(g => g.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .Length(1, 100).WithMessage("Last name must be between 1 and 100 characters.");

        RuleFor(g => g.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(g => g.Phone)
            .NotEmpty().WithMessage("Phone is required.")
            .Matches(@"^\+?[\d\s\-()]{7,}$").WithMessage("Phone must be a valid phone number.");
    }
}

public class GuestDtoValidator : AbstractValidator<GuestDto>
{
    public GuestDtoValidator()
    {
        RuleFor(g => g.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .Length(1, 100).WithMessage("First name must be between 1 and 100 characters.");

        RuleFor(g => g.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .Length(1, 100).WithMessage("Last name must be between 1 and 100 characters.");

        RuleFor(g => g.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(g => g.Phone)
            .NotEmpty().WithMessage("Phone is required.")
            .Matches(@"^\+?[\d\s\-()]{7,}$").WithMessage("Phone must be a valid phone number.");
    }
}
