using NewsAgency.Application.Contracts.Author.Common;
using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Author.Commands;

public record ChangeAuthorFirstNameCommand(long Id, string FirstName) : ICommand<OperationResult<AuthorViewModel>>;