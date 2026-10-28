using FluentValidation;
using NewsAgency.Application.Contracts.Author.Commands;

namespace NewsAgency.Application.AuthorServices.CommandHandlers.ChangeFirstName;

public class ChangeAuthorFirstNameCommandValidator
    : AbstractValidator<ChangeAuthorFirstNameCommand>
{
    public ChangeAuthorFirstNameCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("Author ID must be greater than zero.");

        RuleFor(command => command.FirstName)
            .Cascade(CascadeMode.Stop)
            .Must(firstName => !string.IsNullOrWhiteSpace(firstName))
            .WithMessage("First name is required.")
            .MinimumLength(2)
            .WithMessage("First name must be at least 2 characters long.")
            .MaximumLength(50)
            .WithMessage("First name cannot exceed 50 characters.");
    }
}