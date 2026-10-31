using FluentValidation;
using NewsAgency.Application.Contracts.Category.Commands;

namespace NewsAgency.Application.CategoryServices.CommandHandlers.ChangeTitle;

public class ChangeCategoryTitleCommandValidator
    : AbstractValidator<ChangeCategoryTitleCommand>
{
    public ChangeCategoryTitleCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("Category ID must be greater than zero.");

        RuleFor(command => command.Title)
            .Cascade(CascadeMode.Stop)
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage("Category title is required.")
            .MinimumLength(2)
            .WithMessage("Category title must be at least 2 characters long.")
            .MaximumLength(50)
            .WithMessage("Category title cannot exceed 50 characters.");
    }
}