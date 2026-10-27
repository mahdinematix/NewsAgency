using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Article.Queries;

public record GetAllArticlesQuery : IQuery<OperationResult<List<ArticleViewModel>>>;