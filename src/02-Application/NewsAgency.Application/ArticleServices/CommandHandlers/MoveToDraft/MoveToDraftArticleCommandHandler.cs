using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Article.Commands;
using NewsAgency.Application.Contracts.Article.Common;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Domain.Exceptions;
using NewsAgency.Utilities.Extensions;

namespace NewsAgency.Application.ArticleServices.CommandHandlers.MoveToDraft;

public class MoveToDraftArticleCommandHandler : ICommandHandler<MoveToDraftArticleCommand, OperationResult<ArticleViewModel>>
{
    private readonly IArticleCommandRepository _commandRepository;
    private readonly IArticleQueryRepository _queryRepository;

    public MoveToDraftArticleCommandHandler(IArticleCommandRepository commandRepository, IArticleQueryRepository queryRepository)
    {
        _commandRepository = commandRepository;
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult<ArticleViewModel>> Handle(MoveToDraftArticleCommand request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult<ArticleViewModel>();

        if (request is null)
        {
            return operation.Failed(OperationErrorCode.Validation, ApplicationMessages.ValueMustNotBeNull);
        }

        try
        {
            var article = await _queryRepository.GetByAsync(request.Id, cancellationToken);

            if (article is null)
            {
                return operation.Failed(OperationErrorCode.NotFound, ApplicationMessages.RecordNotFound);
            }

            article.MoveToDraft();
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
        catch (DomainStateException ex)
        {
            return operation.Failed(OperationErrorCode.LogicError, ex.Message);
        }
    }
}