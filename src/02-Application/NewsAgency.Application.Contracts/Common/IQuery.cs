using MediatR;

namespace NewsAgency.Application.Contracts.Common;

public interface IQuery<TResponse> : IRequest<TResponse>;
