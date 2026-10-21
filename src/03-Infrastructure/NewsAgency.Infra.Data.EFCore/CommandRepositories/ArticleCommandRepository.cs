using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.Repositories;

namespace NewsAgency.Infra.Data.EFCore.CommandRepositories;

public class ArticleCommandRepository : GenericCommandRepository<Article, long>, IArticleCommandRepository
{
    public ArticleCommandRepository(NewsAgencyContext context) : base(context)
    {
    }
    
}