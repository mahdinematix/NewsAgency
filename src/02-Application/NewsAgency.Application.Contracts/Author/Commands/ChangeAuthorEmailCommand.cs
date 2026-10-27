using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Author.Commands;

public record ChangeAuthorEmailCommand(long Id, string Email) : ICommand<OperationResult>;