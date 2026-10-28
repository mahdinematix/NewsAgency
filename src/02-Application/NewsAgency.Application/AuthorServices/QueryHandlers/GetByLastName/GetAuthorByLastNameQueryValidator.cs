using FluentValidation;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Author.Queries;

namespace NewsAgency.Application.AuthorServices.QueryHandlers.GetByLastName;

public class GetAuthorByLastNameQueryValidator : AbstractValidator<GetAuthorByLastNameQuery>
{
    public GetAuthorByLastNameQueryValidator()
    {
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage(ApplicationMessages.ValueMustNotBeNull)
            .MinimumLength(2)
            .WithMessage(ApplicationMessages.ValueFailureToObserveMinLength)
            .MaximumLength(50)
            .WithMessage(ApplicationMessages.ValueFailureToObserveMaxLength);
    }
}