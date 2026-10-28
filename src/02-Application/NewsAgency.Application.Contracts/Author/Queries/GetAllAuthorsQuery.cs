using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Author.Queries;

public record GetAllAuthorsQuery : IQuery<OperationResult<List<AuthorViewModel>>>;