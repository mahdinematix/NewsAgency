using FluentValidation;
using NewsAgency.Application.Contracts.Category.Queries;

namespace NewsAgency.Application.CategoryServices.QueryHandlers.GetById;

public class GetCategoryByIdQueryValidator
    : AbstractValidator<GetCategoryByIdQuery>
{
    public GetCategoryByIdQueryValidator()
    {
        RuleFor(query => query.Id)
            .GreaterThan(0)
            .WithMessage("Category Id must be greater than zero.");
    }
}