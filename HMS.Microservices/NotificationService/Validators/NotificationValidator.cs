using FluentValidation;
using NotificationService.Models;

namespace NotificationService.Validators;

public class NotificationRequestValidator : AbstractValidator<NotificationRequest>
{
    public NotificationRequestValidator()
    {
        RuleFor(nr => nr.Subject)
            .NotEmpty().WithMessage("Subject is required.")
            .Length(1, 200).WithMessage("Subject must be between 1 and 200 characters.");

        RuleFor(nr => nr.Message)
            .NotEmpty().WithMessage("Message is required.")
            .Length(1, 2000).WithMessage("Message must be between 1 and 2000 characters.");

        RuleFor(nr => nr.RecipientEmail)
            .NotEmpty().WithMessage("Recipient email is required.")
            .EmailAddress().WithMessage("Recipient email must be a valid email address.");

        RuleFor(nr => nr.Priority)
            .Must(p => string.IsNullOrEmpty(p) || new[] { "Low", "Normal", "High" }.Contains(p))
            .WithMessage("Priority must be one of: Low, Normal, High.");
    }
}
