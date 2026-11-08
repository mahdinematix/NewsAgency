using NewsAgency.Application.Contracts.Author.Common;
using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Author.Queries;

public record GetAuthorByIdQuery(long Id) : IQuery<OperationResult<AuthorViewModel>>;