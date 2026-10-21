using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.Common.Repositories;

namespace NewsAgency.Domain.ArticleAgg.Repositories;

public interface IArticleQueryRepository : IGenericQueryRepository<Article, long>
{
    Task<Article> GetByAsync(string title, CancellationToken cancellationToken);
}