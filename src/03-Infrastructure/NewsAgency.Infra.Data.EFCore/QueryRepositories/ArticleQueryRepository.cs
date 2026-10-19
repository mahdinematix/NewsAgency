using Microsoft.EntityFrameworkCore;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Domain.ArticleAgg.ValueObjects;

namespace NewsAgency.Infra.Data.EFCore.QueryRepositories;

public class ArticleQueryRepository : IArticleQueryRepository
{
    private readonly NewsAgencyContext _context;

    public ArticleQueryRepository(NewsAgencyContext context)
    {
        _context = context;
    }

    public async Task<List<Article>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Articles.ToListAsync(cancellationToken);
    }

    public async Task<Article> GetByAsync(long id, CancellationToken cancellationToken)
    {
        return await _context.Articles.FindAsync(id, cancellationToken);
    }

    public async Task<Article> GetByAsync(string title, CancellationToken cancellationToken)
    {
        var articleTitle = ArticleTitle.FromString(title);

        return await _context.Articles
            .FirstOrDefaultAsync(x => x.Title == articleTitle, cancellationToken);
    }
}