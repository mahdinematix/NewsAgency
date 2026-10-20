namespace NewsAgency.Application.Contracts.Article.Repositories;

public interface IArticleCommandRepository
{
    Task<long> CreateAsync(Domain.ArticleAgg.Entities.Article command, CancellationToken cancellationToken);
    Task Delete(long id, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}