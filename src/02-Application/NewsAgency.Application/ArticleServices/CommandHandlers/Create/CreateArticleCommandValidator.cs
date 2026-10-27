using FluentValidation;
using NewsAgency.Application.Contracts.Article.Commands;

namespace NewsAgency.Application.ArticleServices.CommandHandlers.Create;

public class CreateArticleCommandValidator
    : AbstractValidator<CreateArticleCommand>
{
    public CreateArticleCommandValidator()
    {
        RuleFor(command => command.Title)
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage("Article title is required.")
            .MinimumLength(2)
            .WithMessage("Article title must contain at least 2 characters.")
            .MaximumLength(50)
            .WithMessage("Article title cannot exceed 50 characters.");

        RuleFor(command => command.Content)
            .Must(content => !string.IsNullOrWhiteSpace(content))
            .WithMessage("Article content is required.")
            .MinimumLength(10)
            .WithMessage("Article content must contain at least 10 characters.")
            .MaximumLength(5000)
            .WithMessage("Article content cannot exceed 5000 characters.");

        RuleFor(command => command.AuthorId)
            .GreaterThan(0)
            .WithMessage("Author ID must be greater than zero.");

        RuleFor(command => command.CategoryId)
            .GreaterThan(0)
            .WithMessage("Category ID must be greater than zero.");
    }
}