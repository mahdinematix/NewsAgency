using FluentValidation;
using NewsAgency.Application.Contracts.Category.Commands;

namespace NewsAgency.Application.CategoryServices.CommandHandlers.Restore;

public class RestoreCategoryCommandValidator
    : AbstractValidator<RestoreCategoryCommand>
{
    public RestoreCategoryCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("Parent category ID must be greater than zero.");
    }
}