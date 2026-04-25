using Bluewave.Inventory.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Inventory.Application.Features.MeasurementUnits.Commands.CreateMeasurementUnit;

public class CreateMeasurementUnitCommandValidator : AbstractValidator<CreateMeasurementUnitCommand>
{
    public CreateMeasurementUnitCommandValidator(IInventoryDbContext context)
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Unit code is required.")
            .MaximumLength(5).WithMessage("Code must be short (e.g., KG, UN, PCS).")
            .Matches(@"^[A-Z0-9]+$").WithMessage("Code must be uppercase alphanumeric.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .MustAsync(async (code, cancellation) =>
                !await context.MeasurementUnits.AnyAsync(m => m.Code == code, cancellation))
            .WithMessage("Unit code already exists.");

        RuleFor(x => x.Name)
            .NotEmpty().MaximumLength(50);
    }
}