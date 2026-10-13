using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Domain.ArticleAgg.ValueObjects;

namespace NewsAgency.Infra.Data.EFCore.Commands.Repositories;

public class ArticleQueryRepository : IArticleQueryRepository
{
    public List<Article> Articles = new List<Article>
    {
        new Article(1,new ArticleTitle("test"), new ArticleContent("teststests"),1,1)
    };

    public async Task<List<Article>> GetAllAsync()
    {
        return Articles;
    }

    public async Task<Article> GetByAsync(long id)
    {
        return Articles.FirstOrDefault(x=>x.Id == id);
    }

}