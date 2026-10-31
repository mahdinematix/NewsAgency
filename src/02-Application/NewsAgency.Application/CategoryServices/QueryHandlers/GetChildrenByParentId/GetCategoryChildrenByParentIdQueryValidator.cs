using FluentValidation;
using NewsAgency.Application.Contracts.Category.Queries;

namespace NewsAgency.Application.CategoryServices.QueryHandlers.GetChildrenByParentId;

public class GetCategoryChildrenByParentIdQueryValidator
    : AbstractValidator<GetCategoryChildrenByParentIdQuery>
{
    public GetCategoryChildrenByParentIdQueryValidator()
    {
        RuleFor(query => query.ParentId)
            .GreaterThan(0)
            .WithMessage("Parent category ID must be greater than zero.");
    }
}