using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Author.Commands;

public record ChangeAuthorPhoneNumberCommand(long Id, string PhoneNumber) : ICommand<OperationResult>;