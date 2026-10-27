using System.Linq.Expressions;

namespace NewsAgency.Domain.Common.Repositories;

public interface IGenericQueryRepository<TEntity, TKey>
    where TEntity : BaseEntity
{
    Task<List<TEntity>?> GetAllAsync(
        CancellationToken cancellationToken);

    Task<TEntity>? GetByAsync(
        TKey id,
        CancellationToken cancellationToken);

    public Task<bool> Exists(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken);
}