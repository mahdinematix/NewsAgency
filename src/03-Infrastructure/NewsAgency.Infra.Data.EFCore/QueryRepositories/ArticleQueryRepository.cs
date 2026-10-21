using Microsoft.EntityFrameworkCore;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Domain.ArticleAgg.ValueObjects;

namespace NewsAgency.Infra.Data.EFCore.QueryRepositories;

public class ArticleQueryRepository : GenericQueryRepository<Article, long>, IArticleQueryRepository
{
    private readonly NewsAgencyContext _context;

    public ArticleQueryRepository(NewsAgencyContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Article> GetByAsync(string title, CancellationToken cancellationToken)
    {
        var articleTitle = ArticleTitle.FromString(title);

        return await _context.Articles
            .FirstOrDefaultAsync(x => x.Title == articleTitle, cancellationToken);
    }
}