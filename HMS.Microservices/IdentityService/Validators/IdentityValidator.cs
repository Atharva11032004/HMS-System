using FluentValidation;
using IdentityService.Models;

namespace IdentityService.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(lr => lr.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(lr => lr.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
    }
}

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(rr => rr.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(rr => rr.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            .Must(HaveUpperAndLowerCase).WithMessage("Password must contain both uppercase and lowercase letters.")
            .Must(HaveNumberOrSpecialChar).WithMessage("Password must contain at least one number or special character.");

        RuleFor(rr => rr.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(r => new[] { "Owner", "Manager", "Receptionist" }.Contains(r))
            .WithMessage("Role must be one of: Owner, Manager, Receptionist.");
    }

    private static bool HaveUpperAndLowerCase(string password)
    {
        return password.Any(char.IsUpper) && password.Any(char.IsLower);
    }

    private static bool HaveNumberOrSpecialChar(string password)
    {
        return password.Any(char.IsDigit) || password.Any(c => !char.IsLetterOrDigit(c));
    }
}

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(rtr => rtr.Token)
            .NotEmpty().WithMessage("Token is required.");

        RuleFor(rtr => rtr.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}
