using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Article.Common;
using NewsAgency.Application.Contracts.Article.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Utilities.Extensions;

namespace NewsAgency.Application.ArticleServices.QueryHandlers.GetAll;

public class GetAllArticlesQueryHandler : IQueryHandler<GetAllArticlesQuery, OperationResult<List<ArticleViewModel>>>
{
    private readonly IArticleQueryRepository _queryRepository;

    public GetAllArticlesQueryHandler(IArticleQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult<List<ArticleViewModel>>> Handle(GetAllArticlesQuery request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult<List<ArticleViewModel>>();
        var articles = await _queryRepository.GetAllAsync(cancellationToken);

        if (articles is null)
        {
            return operation.Failed(OperationErrorCode.NotFound, ApplicationMessages.RecordNotFound);
        }

        return operation.Succeeded(articles.Select(x => new ArticleViewModel
        {
            Id = x.Id,
            Title = x.Title.Value,
            Content = x.Content.Value,
            AuthorId = x.AuthorId,
            CategoryId = x.CategoryId,
            Status = x.Status.GetEnumDescription(),
            Actions = new ArticleActionsViewModel
            {
                CanMoveToDraft = x.CanMoveToDraft,
                CanArchive = x.CanArchive,
                CanEdit = x.CanEdit,
                CanPublish = x.CanPublish,
                CanReject = x.CanReject
            }
        }).ToList());

    }
}