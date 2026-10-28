using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Author.Queries;

public record GetAuthorByEmailQuery(string Email) : IQuery<OperationResult<AuthorViewModel>>;