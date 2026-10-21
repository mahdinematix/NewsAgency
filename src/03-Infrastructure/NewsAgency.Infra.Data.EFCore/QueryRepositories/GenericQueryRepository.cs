using Microsoft.EntityFrameworkCore;
using NewsAgency.Domain.Common;
using NewsAgency.Domain.Common.Repositories;

namespace NewsAgency.Infra.Data.EFCore.QueryRepositories;

public class GenericQueryRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
{
    protected readonly NewsAgencyContext Context;

    public GenericQueryRepository(NewsAgencyContext context)
    {
        Context = context;
    }

    public async Task<List<TEntity>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await Context.Set<TEntity>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<TEntity> GetByAsync(
        TKey id,
        CancellationToken cancellationToken)
    {
        return await Context.Set<TEntity>()
            .FindAsync(new object[] { id }, cancellationToken);
    }
}
