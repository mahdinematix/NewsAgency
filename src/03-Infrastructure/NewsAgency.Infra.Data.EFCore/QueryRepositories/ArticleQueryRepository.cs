using Microsoft.EntityFrameworkCore;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Domain.ArticleAgg.ValueObjects;

namespace NewsAgency.Infra.Data.EFCore.QueryRepositories;

public class ArticleQueryRepository : GenericQueryRepository<Article, long>, IArticleQueryRepository
{
    public ArticleQueryRepository(NewsAgencyContext context) : base(context)
    {
    }

    public async Task<Article?> GetByTitleAsync(
        ArticleTitle title,
        CancellationToken cancellationToken)
    {
        return await Context.Set<Article>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Title == title,
                cancellationToken);
    }
}