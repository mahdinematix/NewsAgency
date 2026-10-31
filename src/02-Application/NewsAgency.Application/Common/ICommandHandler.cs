using MediatR;

namespace NewsAgency.Application.Common;

public interface ICommandHandler<TCommand, TResponse>
    : IRequestHandler<TCommand, TResponse>
    where TCommand : IRequest<TResponse>;