using FluentValidation;

namespace Bluewave.Inventory.Application.Features.Stock.Commands.ReceiveStock;

public class ReceiveStockCommandValidator : AbstractValidator<ReceiveStockCommand>
{
    public ReceiveStockCommandValidator()
    {
        RuleFor(v => v.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(v => v.WarehouseId)
            .NotEmpty().WithMessage("Warehouse ID is required.");

        RuleFor(v => v.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

        RuleFor(v => v.ReferenceDocument)
            .NotEmpty().WithMessage("Reference document (e.g., Invoice Number) is required.")
            .MaximumLength(50).WithMessage("Reference document must not exceed 50 characters.");
    }
}