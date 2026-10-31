using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Article.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Utilities.Extensions;

namespace NewsAgency.Application.ArticleServices.QueryHandlers.GetById;

public class GetArticleByIdQueryHandler : IQueryHandler<GetArticleByIdQuery, OperationResult<ArticleViewModel>>
{
    private readonly IArticleQueryRepository _queryRepository;

    public GetArticleByIdQueryHandler(IArticleQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult<ArticleViewModel>> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult<ArticleViewModel>();

        if (request is null)
        {
            return operation.Failed(OperationErrorCode.Validation, ApplicationMessages.ValueMustNotBeNull);
        }

        var article = await _queryRepository.GetByAsync(request.Id, cancellationToken);

        if (article is null)
        {
            return operation.Failed(OperationErrorCode.NotFound, ApplicationMessages.RecordNotFound);
        }

        return operation.Succeeded(new ArticleViewModel
        {
            Title = article.Title.Value,
            Content = article.Content.Value,
            AuthorId = article.AuthorId,
            AuthorName = "",
            CategoryId = article.CategoryId,
            CategoryTitle = "",
            Status = article.Status.GetEnumDescription()
        });

    }
}