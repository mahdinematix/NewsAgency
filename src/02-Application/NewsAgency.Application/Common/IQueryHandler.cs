using MediatR;

namespace NewsAgency.Application.Common;

public interface IQueryHandler<TQuery, TResponse>
    : IRequestHandler<TQuery, TResponse>
    where TQuery : IRequest<TResponse>;