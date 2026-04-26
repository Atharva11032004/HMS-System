using FluentValidation;
using InventoryService.Models;

namespace InventoryService.Validators;

public class CreateInventoryItemRequestValidator : AbstractValidator<CreateInventoryItemRequest>
{
    public CreateInventoryItemRequestValidator()
    {
        RuleFor(cii => cii.Name)
            .NotEmpty().WithMessage("Item name is required.")
            .Length(1, 200).WithMessage("Item name must be between 1 and 200 characters.");

        RuleFor(cii => cii.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(cii => cii.Category)
            .NotEmpty().WithMessage("Category is required.")
            .Length(1, 50).WithMessage("Category must be between 1 and 50 characters.");

        RuleFor(cii => cii.Supplier)
            .NotEmpty().WithMessage("Supplier is required.")
            .Length(1, 100).WithMessage("Supplier must be between 1 and 100 characters.");

        RuleFor(cii => cii.QuantityInStock)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity in stock must be 0 or greater.");

        RuleFor(cii => cii.MinimumStockLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum stock level must be 0 or greater.");

        RuleFor(cii => cii.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than 0.")
            .LessThanOrEqualTo(99999.99m).WithMessage("Unit price cannot exceed 99,999.99.");
    }
}

public class UpdateInventoryItemRequestValidator : AbstractValidator<UpdateInventoryItemRequest>
{
    public UpdateInventoryItemRequestValidator()
    {
        RuleFor(uii => uii.Name)
            .NotEmpty().WithMessage("Item name is required.")
            .Length(1, 200).WithMessage("Item name must be between 1 and 200 characters.");

        RuleFor(uii => uii.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(uii => uii.Category)
            .NotEmpty().WithMessage("Category is required.")
            .Length(1, 50).WithMessage("Category must be between 1 and 50 characters.");

        RuleFor(uii => uii.Supplier)
            .NotEmpty().WithMessage("Supplier is required.")
            .Length(1, 100).WithMessage("Supplier must be between 1 and 100 characters.");

        RuleFor(uii => uii.MinimumStockLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum stock level must be 0 or greater.");
    }
}
