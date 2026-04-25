using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Application.Features.Warehouses.Commands.CreateWarehouse;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

public class CreateWarehouseCommandValidator : AbstractValidator<CreateWarehouseCommand>
{
    public CreateWarehouseCommandValidator(IInventoryDbContext context)
    {
        RuleFor(x => x.Code).NotEmpty().Length(3, 10)
            .WithMessage("Warehouse code must be between 3 and 10 characters.");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Code)
            .NotEmpty()
            .MustAsync(async (code, cancellation) =>
                !await context.Warehouses.AnyAsync(w => w.Code == code, cancellation))
            .WithMessage("A warehouse with this code already exists.");
    }
}