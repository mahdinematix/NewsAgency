using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.ValueObjects;
using NewsAgency.Domain.Common.Repositories;

namespace NewsAgency.Domain.ArticleAgg.Repositories;

public interface IArticleQueryRepository : IGenericQueryRepository<Article, long>
{
    Task<Article?> GetByTitleAsync(
        ArticleTitle title,
        CancellationToken cancellationToken);
}