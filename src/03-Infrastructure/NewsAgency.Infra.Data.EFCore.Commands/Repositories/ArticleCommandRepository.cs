using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Domain.ArticleAgg.ValueObjects;

namespace NewsAgency.Infra.Data.EFCore.Commands.Repositories;

public class ArticleCommandRepository : IArticleCommandRepository
{
    private readonly ArticleQueryRepository _articleQueryRepository;
    public ArticleCommandRepository(ArticleQueryRepository articleQueryRepository)
    {
        _articleQueryRepository = articleQueryRepository;
    }
    public List<Article> Articles = new List<Article>
    {
        new Article(1,new ArticleTitle("test"), new ArticleContent("teststests"),1,1)
    };

    

    public async Task Create(Article command, CancellationToken cancellationToken)
    {
        Articles.Add(command);
    }

    public async Task Delete(long id, CancellationToken cancellationToken)
    {
        var article = await _articleQueryRepository.GetByAsync(id);
        Articles.Remove(article);
    }

    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        
    }
}