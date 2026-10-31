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

    public async Task<Article?> GetByAsync(string title, CancellationToken cancellationToken)
    {
        var articleTitle = ArticleTitle.FromString(title);

        return await Context.Articles
            .FirstOrDefaultAsync(x => x.Title == articleTitle, cancellationToken);
    }
}