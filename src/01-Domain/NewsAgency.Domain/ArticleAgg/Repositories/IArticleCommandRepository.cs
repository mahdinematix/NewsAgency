using NewsAgency.Domain.ArticleAgg.Entities;

namespace NewsAgency.Domain.ArticleAgg.Repositories;

public interface IArticleCommandRepository
{
    Task<long> CreateAsync(Article command, CancellationToken cancellationToken);
    Task Delete(long id, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}