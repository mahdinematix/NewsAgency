namespace NewsAgency.Domain.Common.Repositories;

public interface IGenericCommandRepository<TEntity, TKey>
    where TEntity : BaseEntity
{
    Task CreateAsync(
        TEntity entity,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        TKey id,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}