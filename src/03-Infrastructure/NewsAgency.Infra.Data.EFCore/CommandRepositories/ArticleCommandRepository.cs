using NewsAgency.Application.Contracts.Article.Repositories;
using NewsAgency.Domain.ArticleAgg.Entities;

namespace NewsAgency.Infra.Data.EFCore.CommandRepositories;

public class ArticleCommandRepository : IArticleCommandRepository
{
    private readonly NewsAgencyContext _context;

    public ArticleCommandRepository(NewsAgencyContext context)
    {
        _context = context;
    }

    public async Task<long> CreateAsync(Article command, CancellationToken cancellationToken)
    {
        await _context.AddAsync(command, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        return command.Id;
    }

    public async Task Delete(long id, CancellationToken cancellationToken)
    {
        var article = await _context.Articles.FindAsync(id, cancellationToken);
        _context.Remove(article);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}