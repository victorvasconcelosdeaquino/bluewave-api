using Bluewave.Inventory.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Inventory.Application.Features.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
{
    public CreateSupplierCommandValidator(IInventoryDbContext context)
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().MinimumLength(3).MaximumLength(150);

        RuleFor(x => x.TaxId)
            .NotEmpty().WithMessage("Tax ID / CNPJ is required.")
            .Matches(@"^\d+$").WithMessage("Tax ID must contain only numbers.");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("A valid email is required.");

        RuleFor(x => x.TaxId)
            .NotEmpty()
            .MustAsync(async (taxId, cancellation) =>
                !await context.Suppliers.AnyAsync(s => s.TaxId == taxId, cancellation))
            .WithMessage("A supplier with this Tax ID already exists.");

        RuleFor(x => x.Phone)
            .MaximumLength(20);
    }
}