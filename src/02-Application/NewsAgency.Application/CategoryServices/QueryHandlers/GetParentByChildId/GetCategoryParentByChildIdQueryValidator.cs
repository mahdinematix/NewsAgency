using FluentValidation;
using NewsAgency.Application.Contracts.Category.Queries;

namespace NewsAgency.Application.CategoryServices.QueryHandlers.GetParentByChildId;

public class GetCategoryParentByChildIdQueryValidator
    : AbstractValidator<GetCategoryParentByChildIdQuery>
{
    public GetCategoryParentByChildIdQueryValidator()
    {
        RuleFor(query => query.ChildId)
            .GreaterThan(0)
            .WithMessage("Child category ID must be greater than zero.");
    }
}