using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Author.Queries;

public record GetAuthorByFirstNameQuery(string FirstName) : IQuery<OperationResult<AuthorViewModel>>;