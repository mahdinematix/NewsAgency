using NewsAgency.Domain.CategoryAgg.Entities;
using NewsAgency.Domain.Common.Repositories;

namespace NewsAgency.Domain.CategoryAgg.Repositories;

public interface ICategoryQueryRepository : IGenericQueryRepository<Category, long>
{
    Task<Category> GetByAsync(string title, CancellationToken cancellationToken);
    Task<ICollection<Category>> GetChildrenByAsync(long parentId, CancellationToken cancellationToken);
    Task<Category?> GetParentByAsync(long childId, CancellationToken cancellationToken);
    Task<bool> HasActiveChildrenAsync(
        long parentId,
        CancellationToken cancellationToken);
    Task<bool> IsParentRemovedAsync(
        long? parentId,
        CancellationToken cancellationToken);
}