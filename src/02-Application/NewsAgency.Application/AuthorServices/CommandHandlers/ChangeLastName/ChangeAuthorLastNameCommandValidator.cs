using FluentValidation;
using NewsAgency.Application.Contracts.Author.Commands;

namespace NewsAgency.Application.AuthorServices.CommandHandlers.ChangeLastName;

public class ChangeAuthorLastNameCommandValidator
    : AbstractValidator<ChangeAuthorLastNameCommand>
{
    public ChangeAuthorLastNameCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("Author ID must be greater than zero.");

        RuleFor(command => command.LastName)
            .Cascade(CascadeMode.Stop)
            .Must(lastName => !string.IsNullOrWhiteSpace(lastName))
            .WithMessage("Last name is required.")
            .MinimumLength(2)
            .WithMessage("Last name must be at least 2 characters long.")
            .MaximumLength(50)
            .WithMessage("Last name cannot exceed 50 characters.");
    }
}