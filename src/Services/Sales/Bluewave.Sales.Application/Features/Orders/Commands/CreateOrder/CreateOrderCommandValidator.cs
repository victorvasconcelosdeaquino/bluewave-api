using FluentValidation;

namespace Bluewave.Sales.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(p => p.CustomerName)
            .NotEmpty().WithMessage("Client name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters.")
            .MaximumLength(100).WithMessage("Name cannot be longer than 100 characters.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("An order must have at least one item.");

        RuleForEach(p => p.Items).SetValidator(new OrderItemValidator());
    }
}

public class OrderItemValidator : AbstractValidator<OrderItemDto>
{
    public OrderItemValidator()
    {
        RuleFor(i => i.ProductId).NotEmpty().WithMessage("Product ID is required.");
        RuleFor(i => i.ProductName).NotEmpty().WithMessage("Product name is required.");
        RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        RuleFor(i => i.UnitPrice).GreaterThan(0).WithMessage("Unit price must be greater than zero.");
    }
}