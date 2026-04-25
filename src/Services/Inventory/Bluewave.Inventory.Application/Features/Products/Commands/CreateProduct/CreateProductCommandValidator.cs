using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Application.Features.Products.Commands.CreateProduct;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator(IInventoryDbContext context)
    {
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3);

        RuleFor(x => x.CategoryId)
            .MustAsync(async (id, cancellation) => await context.Categories.AnyAsync(c => c.Id == id))
            .WithMessage("Category not found.");

        RuleFor(x => x.Sku)
            .NotEmpty()
            .MustAsync(async (sku, cancellation) =>
            {
                return !await context.Products.AnyAsync(p => p.Sku == sku, cancellation);
            })
            .WithMessage("A product with this SKU already exists.");

        RuleFor(x => x.StandardCost).GreaterThanOrEqualTo(0);
    }
}