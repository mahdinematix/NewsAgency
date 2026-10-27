using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Article.Queries;

public record GetArticleByIdQuery(long Id) : IQuery<OperationResult<ArticleViewModel>>;