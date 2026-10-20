using MediatR;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Article.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Utilities.Extensions;

namespace NewsAgency.Application.ArticleServices.QueryHandlers;

public class GetAllArticlesQueryHandler : IRequestHandler<GetAllArticlesQuery, OperationResult<List<ArticleViewModel>>>
{
    private readonly IArticleQueryRepository _queryRepository;

    public GetAllArticlesQueryHandler(IArticleQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult<List<ArticleViewModel>>> Handle(GetAllArticlesQuery request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult<List<ArticleViewModel>>();
        if (request is null)
        {
            return operation.Failed(ApplicationMessages.ValueMustNotBeNull);
        }
        var articles = await _queryRepository.GetAllAsync(cancellationToken);

        if (articles is null)
        {
            return operation.Failed(ApplicationMessages.RecordNotFound);
        }

        return operation.Succeeded(articles.Select(x => new ArticleViewModel
        {
            Title = x.Title.Value,
            Content = x.Content.Value,
            AuthorId = x.AuthorId,
            AuthorName = "",
            CategoryId = x.CategoryId,
            CategoryTitle = "",
            Status = x.Status.GetEnumDescription()
        }).ToList());

    }
}