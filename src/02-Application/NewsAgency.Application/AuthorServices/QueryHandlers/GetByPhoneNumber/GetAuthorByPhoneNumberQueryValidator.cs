using FluentValidation;
using NewsAgency.Application.Contracts.Author.Queries;

namespace NewsAgency.Application.AuthorServices.QueryHandlers.GetByPhoneNumber;

public class GetAuthorByPhoneNumberQueryValidator
    : AbstractValidator<GetAuthorByPhoneNumberQuery>
{
    public GetAuthorByPhoneNumberQueryValidator()
    {
        RuleFor(query => query.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .Must(phoneNumber => !string.IsNullOrWhiteSpace(phoneNumber))
            .WithMessage("Phone number is required.")
            .MinimumLength(10)
            .WithMessage("Phone number must be at least 10 digits long.")
            .MaximumLength(14)
            .WithMessage("Phone number cannot exceed 14 digits.")
            .Matches(@"^\d+$")
            .WithMessage("Phone number must contain only digits.");
    }
}