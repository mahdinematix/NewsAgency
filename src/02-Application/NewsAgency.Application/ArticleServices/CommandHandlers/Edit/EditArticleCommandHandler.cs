using MediatR;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Article.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Domain.ArticleAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Application.ArticleServices.CommandHandlers.Edit;

public class EditArticleCommandHandler : IRequestHandler<EditArticleCommand , OperationResult>
{
    private readonly IArticleCommandRepository _commandRepository;
    private readonly IArticleQueryRepository _queryRepository;

    public EditArticleCommandHandler(IArticleCommandRepository commandRepository, IArticleQueryRepository queryRepository)
    {
        _commandRepository = commandRepository;
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult> Handle(EditArticleCommand request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult();

        if (request is null)
        {
            return operation.Failed(OperationErrorCode.Validation, ApplicationMessages.ValueMustNotBeNull);
        }

        try
        {
            var article = await _queryRepository.GetByAsync(request.Id, cancellationToken);

            if (article == null)
            {
                return operation.Failed(OperationErrorCode.NotFound, ApplicationMessages.RecordNotFound);
            }

            article.Edit(new ArticleTitle(request.Title), new ArticleContent(request.Content), request.AuthorId, request.CategoryId);
            await _commandRepository.SaveChangesAsync(cancellationToken);
            return operation.Succeeded();
        }
        catch (DomainStateException ex)
        {
            return operation.Failed(OperationErrorCode.LogicError, ex.Message);
        }
        catch (InvalidValueObjectStateException ex)
        {
            return operation.Failed(OperationErrorCode.Validation, ex.Message);
        }
    }
}