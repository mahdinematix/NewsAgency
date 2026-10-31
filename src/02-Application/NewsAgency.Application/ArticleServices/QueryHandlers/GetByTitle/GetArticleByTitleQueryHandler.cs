using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Article.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Utilities.Extensions;

namespace NewsAgency.Application.ArticleServices.QueryHandlers.GetByTitle;

public class GetArticleByTitleQueryHandler : IQueryHandler<GetArticleByTitleQuery, OperationResult<ArticleViewModel>>
{
    private readonly IArticleQueryRepository _queryRepository;

    public GetArticleByTitleQueryHandler(IArticleQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult<ArticleViewModel>> Handle(GetArticleByTitleQuery request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult<ArticleViewModel>();

        if (request is null)
        {
            return operation.Failed(OperationErrorCode.Validation, ApplicationMessages.ValueMustNotBeNull);
        }

        var article = await _queryRepository.GetByAsync(request.Title, cancellationToken);

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