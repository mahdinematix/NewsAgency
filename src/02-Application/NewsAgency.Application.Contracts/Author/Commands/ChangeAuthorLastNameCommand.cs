using MediatR;
using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Author.Commands;

public record ChangeAuthorLastNameCommand(long Id, string LastName) : IRequest<OperationResult>;