using FluentValidation;
using NewsAgency.Application.Contracts.Author.Queries;

namespace NewsAgency.Application.AuthorServices.QueryHandlers.GetById;

public class GetAuthorByIdQueryValidator
    : AbstractValidator<GetAuthorByIdQuery>
{
    public GetAuthorByIdQueryValidator()
    {
        RuleFor(query => query.Id)
            .GreaterThan(0)
            .WithMessage("Author Id must be greater than zero.");
    }
}