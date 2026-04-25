using Bluewave.Inventory.Application.Common.Interfaces;
using Bluewave.Inventory.Application.Features.ProductCategory.Commands.CreateProductCategory;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Bluewave.Inventory.Application.Features.ProductCategory.Commands.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator(IInventoryDbContext context)
    {
        RuleFor(x => x.Name)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.ParentId)
            .MustAsync(async (parentId, cancellation) =>
            {
                if (parentId == null || parentId == Guid.Empty) return true;
                return await context.Categories.AnyAsync(c => c.Id == parentId, cancellation);
            })
            .WithMessage("The specified parent category does not exist.");


    }

}

//TODO: split the validators into separate files for better organization, e.g., CreateCategoryCommandValidator and UpdateCategoryCommandValidator
public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator(IInventoryDbContext context)
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);

        RuleFor(x => x.ParentId)
            .Must((command, parentId) => parentId != command.Id)
            .WithMessage("A category cannot be its own parent.")
            .MustAsync(async (command, parentId, cancellation) =>
            {
                if (parentId == null || parentId == Guid.Empty) return true;

                var parentExists = await context.Categories.AnyAsync(c => c.Id == parentId, cancellation);
                if (!parentExists) return false;

                var isParentADescendant = await context.Categories
                    .AnyAsync(c => c.Id == parentId && c.ParentId == command.Id, cancellation);

                return !isParentADescendant;
            })
            .WithMessage("Circular reference detected: The parent category cannot be a sub-category of the current one.");
    }
}