using NewsAgency.Application.Contracts.Author.Common;
using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Author.Queries;

public record GetAuthorByLastNameQuery(string LastName) : IQuery<OperationResult<AuthorViewModel>>;