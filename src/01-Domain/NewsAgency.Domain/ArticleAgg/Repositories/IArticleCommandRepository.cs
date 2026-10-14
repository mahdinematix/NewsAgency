using NewsAgency.Domain.ArticleAgg.Entities;

namespace NewsAgency.Domain.ArticleAgg.Repositories;

public interface IArticleCommandRepository
{
    Task Create(Article command, CancellationToken cancellationToken);
    Task Delete(long id, CancellationToken cancellationToken);
    Task SaveChanges(CancellationToken cancellationToken);
}