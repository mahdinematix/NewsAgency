using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Article.Commands;
using NewsAgency.Application.Contracts.Article.Common;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Domain.ArticleAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;
using NewsAgency.Utilities.Extensions;

namespace NewsAgency.Application.ArticleServices.CommandHandlers.Create;

public class CreateArticleCommandHandler : ICommandHandler<CreateArticleCommand, OperationResult<ArticleViewModel>>
{
    private readonly IArticleCommandRepository _commandRepository;
    private readonly IArticleQueryRepository _queryRepository;

    public CreateArticleCommandHandler(IArticleCommandRepository commandRepository, IArticleQueryRepository queryRepository)
    {
        _commandRepository = commandRepository;
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult<ArticleViewModel>> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult<ArticleViewModel>();

        if (request is null)
        {
            return operation.Failed(OperationErrorCode.Validation, ApplicationMessages.ValueMustNotBeNull);
        }

        try
        {
            var title = new ArticleTitle(request.Title);
            if (await _queryRepository.Exists(x => x.Title == title, cancellationToken))
            {
                return operation.Failed(OperationErrorCode.Conflict, ApplicationMessages.ValueCannotBeDuplicated);
            }
            var article = new Article(new ArticleTitle(request.Title), new ArticleContent(request.Content), request.AuthorId, request.CategoryId);
            await _commandRepository.CreateAsync(article, cancellationToken);
            await _commandRepository.SaveChangesAsync(cancellationToken);
            return operation.Succeeded(new ArticleViewModel
            {
                Id = article.Id,
                Title = article.Title.Value,
                Content = article.Content.Value,
                AuthorId = article.AuthorId,
                CategoryId = article.CategoryId,
                Status = article.Status.GetEnumDescription(),
                Actions = new ArticleActionsViewModel
                {
                    CanMoveToDraft = article.CanMoveToDraft,
                    CanArchive = article.CanArchive,
                    CanEdit = article.CanEdit,
                    CanPublish = article.CanPublish,
                    CanReject = article.CanReject
                }
            });
        }
        catch (InvalidValueObjectStateException ex)
        {
            return operation.Failed(OperationErrorCode.Validation, ex.Message);
        }
    }

}