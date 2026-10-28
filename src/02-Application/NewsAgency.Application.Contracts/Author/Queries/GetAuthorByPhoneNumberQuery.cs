using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Author.Queries;

public record GetAuthorByPhoneNumberQuery(string PhoneNumber) : IQuery<OperationResult<AuthorViewModel>>;