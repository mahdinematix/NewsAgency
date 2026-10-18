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

    public async Task<List<Article>> GetAllAsync()
    {
        return await _context.Articles.ToListAsync();
    }

    public async Task<Article> GetByAsync(long id)
    {
        return await _context.Articles.FindAsync(id);
    }

    public async Task<Article> GetByAsync(string title)
    {
        var articleTitle = ArticleTitle.FromString(title);

        return await _context.Articles
            .FirstOrDefaultAsync(x => x.Title == articleTitle);
    }
}