using FluentValidation;

namespace Bluewave.Sales.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(p => p.CustomerName)
            .NotEmpty().WithMessage("O nome do cliente é obrigatório.")
            .MaximumLength(100).WithMessage("O nome não pode ter mais de 100 caracteres.");

        RuleFor(p => p.Items)
            .NotEmpty().WithMessage("O pedido deve conter pelo menos um item.");

        RuleForEach(p => p.Items).SetValidator(new OrderItemValidator());
    }
}

public class OrderItemValidator : AbstractValidator<OrderItemDto>
{
    public OrderItemValidator()
    {
        RuleFor(i => i.ProductId)
            .NotEmpty().WithMessage("O ID do produto é obrigatório.");

        RuleFor(i => i.ProductName)
            .NotEmpty().WithMessage("O nome do produto é obrigatório.");

        RuleFor(i => i.Quantity)
            .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");

        RuleFor(i => i.UnitPrice)
            .GreaterThan(0).WithMessage("O preço unitário deve ser maior que zero.");
    }
}