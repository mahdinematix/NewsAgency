using FluentValidation;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Author.Queries;

namespace NewsAgency.Application.AuthorServices.QueryHandlers.GetByFirstName;

public class GetAuthorByFirstNameQueryValidator : AbstractValidator<GetAuthorByFirstNameQuery>
{
    public GetAuthorByFirstNameQueryValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage(ApplicationMessages.ValueMustNotBeNull)
            .MinimumLength(2)
            .WithMessage(ApplicationMessages.ValueFailureToObserveMinLength)
            .MaximumLength(50)
            .WithMessage(ApplicationMessages.ValueFailureToObserveMaxLength);
    }
}