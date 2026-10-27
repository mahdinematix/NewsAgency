using FluentValidation;
using NewsAgency.Application.Contracts.Author.Commands;

namespace NewsAgency.Application.AuthorServices.CommandHandlers.ChangeEmail;

public class ChangeAuthorEmailCommandValidator
    : AbstractValidator<ChangeAuthorEmailCommand>
{
    public ChangeAuthorEmailCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("Author ID must be greater than zero.");

        RuleFor(command => command.Email)
            .Cascade(CascadeMode.Stop)
            .Must(email => !string.IsNullOrWhiteSpace(email))
            .WithMessage("Email is required.")
            .MaximumLength(255)
            .WithMessage("Email cannot exceed 255 characters.")
            .EmailAddress()
            .WithMessage("Email format is invalid.");
    }
}