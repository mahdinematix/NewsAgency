using MediatR;
using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Author.Commands;

public record ChangeAuthorFirstNameCommand(long Id, string FirstName) : IRequest<OperationResult>;