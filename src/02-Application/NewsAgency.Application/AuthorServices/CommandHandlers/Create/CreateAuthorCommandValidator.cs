using FluentValidation;
using NewsAgency.Application.Contracts.Author.Commands;

namespace NewsAgency.Application.AuthorServices.CommandHandlers.Create;

public class CreateAuthorCommandValidator : AbstractValidator<CreateAuthorCommand>
{
    public CreateAuthorCommandValidator()
    {
        RuleFor(command => command.FirstName)
            .Cascade(CascadeMode.Stop)
            .Must(firstName => !string.IsNullOrWhiteSpace(firstName))
            .WithMessage("First name is required.")
            .MinimumLength(2)
            .WithMessage("First name must be at least 2 characters long.")
            .MaximumLength(50)
            .WithMessage("First name cannot exceed 50 characters.");

        RuleFor(command => command.LastName)
            .Cascade(CascadeMode.Stop)
            .Must(lastName => !string.IsNullOrWhiteSpace(lastName))
            .WithMessage("Last name is required.")
            .MinimumLength(2)
            .WithMessage("Last name must be at least 2 characters long.")
            .MaximumLength(50)
            .WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(command => command.Email)
            .Cascade(CascadeMode.Stop)
            .Must(email => !string.IsNullOrWhiteSpace(email))
            .WithMessage("Email is required.")
            .MaximumLength(255)
            .WithMessage("Email cannot exceed 255 characters.")
            .EmailAddress()
            .WithMessage("Email format is invalid.");

        RuleFor(command => command.PhoneNumber)
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