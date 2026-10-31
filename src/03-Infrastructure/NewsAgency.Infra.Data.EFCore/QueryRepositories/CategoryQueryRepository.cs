using Microsoft.EntityFrameworkCore;
using NewsAgency.Domain.CategoryAgg.Entities;
using NewsAgency.Domain.CategoryAgg.ValueObjects;

namespace NewsAgency.Infra.Data.EFCore.QueryRepositories;

public class CategoryQueryRepository : GenericQueryRepository<Category, long>
{
    public CategoryQueryRepository(NewsAgencyContext context) : base(context)
    {
    }

    public async Task<Category> GetByAsync(string title, CancellationToken cancellationToken)
    {
        var categoryTitle = CategoryTitle.FromString(title);

        return await Context.Categories
            .FirstOrDefaultAsync(x => x.Title == categoryTitle, cancellationToken);
    }


    public async Task<ICollection<Category>> GetChildrenByAsync(long parentId, CancellationToken cancellationToken)
    {
        return await Context.Categories.Where(x => x.ParentId == parentId).ToListAsync(cancellationToken);
    }


    public async Task<Category?> GetParentByAsync(long childId, CancellationToken cancellationToken)
    {
        return await (
            from child in Context.Categories
            join parent in Context.Categories
                on child.ParentId equals parent.Id
            where child.Id == childId
            select parent
        ).FirstOrDefaultAsync(cancellationToken);

    }

    public async Task<bool> HasActiveChildrenAsync(
        long parentId,
        CancellationToken cancellationToken)
    {
        return await Context.Categories
            .AnyAsync(
                child => child.ParentId == parentId
                         && !child.IsRemoved,
                cancellationToken);
    }

    public async Task<bool> IsParentRemovedAsync(
        long? parentId,
        CancellationToken cancellationToken)
    {
        if (!parentId.HasValue)
        {
            return false;
        }

        return await Context.Categories.AnyAsync(category => category.Id == parentId.Value && category.IsRemoved, cancellationToken);
    }
}