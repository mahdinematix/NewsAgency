using FluentValidation;
using NewsAgency.Application.Contracts.Category.Commands;

namespace NewsAgency.Application.CategoryServices.CommandHandlers.Create;

public class CreateCategoryCommandValidator
    : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(command => command.Title)
            .Cascade(CascadeMode.Stop)
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage("Category title is required.")
            .MinimumLength(2)
            .WithMessage("Category title must be at least 2 characters long.")
            .MaximumLength(50)
            .WithMessage("Category title cannot exceed 50 characters.");
        RuleFor(command => command.ParentId)
            .GreaterThan(0)
            .When(command => command.ParentId.HasValue)
            .WithMessage("Parent category ID must be greater than zero.");
    }
}