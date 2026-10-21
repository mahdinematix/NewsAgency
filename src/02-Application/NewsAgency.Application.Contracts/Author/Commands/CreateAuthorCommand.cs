using MediatR;
using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Author.Commands;

public record CreateAuthorCommand(string FirstName, string LastName, string Email, string PhoneNumber)
    : IRequest<OperationResult<long>>;

