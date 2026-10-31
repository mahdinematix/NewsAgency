using NewsAgency.Domain.Common;

namespace NewsAgency.Infra.Data.EFCore.CommandRepositories;

public class GenericCommandRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
{
    protected readonly NewsAgencyContext Context;

    public GenericCommandRepository(NewsAgencyContext context)
    {
        Context = context;
    }

    public async Task CreateAsync(
        TEntity entity,
        CancellationToken cancellationToken)
    {
        await Context.Set<TEntity>()
            .AddAsync(entity, cancellationToken);


    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        await Context.SaveChangesAsync(cancellationToken);
    }
}