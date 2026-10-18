using NewsAgency.Domain.ArticleAgg.Entities;

namespace NewsAgency.Domain.ArticleAgg.Repositories;

public interface IArticleQueryRepository
{
    Task<List<Article>> GetAllAsync();
    Task<Article> GetByAsync(long id);
    Task<Article> GetByAsync(string title);
}