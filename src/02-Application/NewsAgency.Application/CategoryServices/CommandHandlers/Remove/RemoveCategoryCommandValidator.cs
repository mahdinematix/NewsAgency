using FluentValidation;
using NewsAgency.Application.Contracts.Category.Commands;

namespace NewsAgency.Application.CategoryServices.CommandHandlers.Remove;

public class RemoveCategoryCommandValidator
    : AbstractValidator<RemoveCategoryCommand>
{
    public RemoveCategoryCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("Category ID must be greater than zero.");
    }
}