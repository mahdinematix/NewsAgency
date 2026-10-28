using FluentValidation;
using NewsAgency.Application.Contracts.Author.Commands;

namespace NewsAgency.Application.AuthorServices.CommandHandlers.ChangePhoneNumber;

public class ChangeAuthorPhoneNumberCommandValidator
    : AbstractValidator<ChangeAuthorPhoneNumberCommand>
{
    public ChangeAuthorPhoneNumberCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("Author ID must be greater than zero.");

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