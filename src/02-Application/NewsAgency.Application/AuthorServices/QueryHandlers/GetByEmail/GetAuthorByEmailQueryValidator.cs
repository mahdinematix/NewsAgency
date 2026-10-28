using FluentValidation;
using NewsAgency.Application.Contracts.Author.Queries;

namespace NewsAgency.Application.AuthorServices.QueryHandlers.GetByEmail;

public class GetAuthorByEmailQueryValidator : AbstractValidator<GetAuthorByEmailQuery>
{
    public GetAuthorByEmailQueryValidator()
    {
        RuleFor(query => query.Email)
            .Cascade(CascadeMode.Stop)
            .Must(email => !string.IsNullOrWhiteSpace(email))
            .WithMessage("Email is required.")
            .MaximumLength(255)
            .WithMessage("Email cannot exceed 255 characters.")
            .EmailAddress()
            .WithMessage("Email format is invalid.");
    }

}