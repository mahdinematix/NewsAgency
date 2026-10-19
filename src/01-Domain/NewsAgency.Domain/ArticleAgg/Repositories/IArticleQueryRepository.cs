using NewsAgency.Domain.ArticleAgg.Entities;

namespace NewsAgency.Domain.ArticleAgg.Repositories;

public interface IArticleQueryRepository
{
    Task<List<Article>> GetAllAsync(CancellationToken cancellationToken);
    Task<Article> GetByAsync(long id, CancellationToken cancellationToken);
    Task<Article> GetByAsync(string title, CancellationToken cancellationToken);
}