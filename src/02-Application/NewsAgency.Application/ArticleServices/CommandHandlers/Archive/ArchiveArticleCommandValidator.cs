using FluentValidation;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Article.Commands;

namespace NewsAgency.Application.ArticleServices.CommandHandlers.Archive;

public class ArchiveArticleCommandValidator
    : AbstractValidator<ArchiveArticleCommand>
{
    public ArchiveArticleCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("Article Id must be greater than zero.");
    }
}