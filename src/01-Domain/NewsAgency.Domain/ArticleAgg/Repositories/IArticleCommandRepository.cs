using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.Common.Repositories;

namespace NewsAgency.Domain.ArticleAgg.Repositories;

public interface IArticleCommandRepository : IGenericCommandRepository<Article, long>
{
}