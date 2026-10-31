using FluentValidation;
using NewsAgency.Application.Contracts.Category.Queries;

namespace NewsAgency.Application.CategoryServices.QueryHandlers.GetByTitle;

public class GetCategoryByTitleQueryValidator
    : AbstractValidator<GetCategoryByTitleQuery>
{
    public GetCategoryByTitleQueryValidator()
    {
        RuleFor(query => query.Title)
            .Cascade(CascadeMode.Stop)
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage("Category title is required.")
            .MinimumLength(2)
            .WithMessage("Category title must be at least 2 characters long.")
            .MaximumLength(50)
            .WithMessage("Category title cannot exceed 50 characters.");
    }
}