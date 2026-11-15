using FluentValidation;
using NewsAgency.Application.Contracts.Category.Commands;

namespace NewsAgency.Application.CategoryServices.CommandHandlers.ChangeParent;

public class ChangeCategoryParentCommandValidator
    : AbstractValidator<ChangeCategoryParentCommand>
{
    public ChangeCategoryParentCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("Category ID must be greater than zero.");

        RuleFor(command => command.ParentId)
            .Must(parentId => !parentId.HasValue || parentId.Value > 0)
            .WithMessage("Parent category ID must be greater than zero.");
    }
}