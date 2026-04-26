using FluentValidation;
using StaffService.Models;

namespace StaffService.Validators;

public class CreateStaffRequestValidator : AbstractValidator<CreateStaffRequest>
{
    public CreateStaffRequestValidator()
    {
        RuleFor(csr => csr.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .Length(1, 100).WithMessage("First name must be between 1 and 100 characters.");

        RuleFor(csr => csr.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .Length(1, 100).WithMessage("Last name must be between 1 and 100 characters.");

        RuleFor(csr => csr.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(csr => csr.Phone)
            .Matches(@"^\+?[\d\s\-()]{7,}$")
            .When(csr => !string.IsNullOrEmpty(csr.Phone))
            .WithMessage("Phone must be a valid phone number.");

        RuleFor(csr => csr.DepartmentId)
            .GreaterThan(0).WithMessage("Department ID must be greater than 0.");

        RuleFor(csr => csr.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(r => new[] { "Staff", "Manager", "Owner" }.Contains(r))
            .WithMessage("Role must be one of: Staff, Manager, Owner.");
    }
}

public class UpdateStaffRequestValidator : AbstractValidator<UpdateStaffRequest>
{
    public UpdateStaffRequestValidator()
    {
        RuleFor(usr => usr.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .Length(1, 100).WithMessage("First name must be between 1 and 100 characters.");

        RuleFor(usr => usr.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .Length(1, 100).WithMessage("Last name must be between 1 and 100 characters.");

        RuleFor(usr => usr.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(usr => usr.Phone)
            .Matches(@"^\+?[\d\s\-()]{7,}$")
            .When(usr => !string.IsNullOrEmpty(usr.Phone))
            .WithMessage("Phone must be a valid phone number.");

        RuleFor(usr => usr.DepartmentId)
            .GreaterThan(0).WithMessage("Department ID must be greater than 0.");

        RuleFor(usr => usr.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(r => new[] { "Staff", "Manager", "Owner" }.Contains(r))
            .WithMessage("Role must be one of: Staff, Manager, Owner.");
    }
}
