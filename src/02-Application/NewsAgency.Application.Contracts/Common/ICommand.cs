using MediatR;

namespace NewsAgency.Application.Contracts.Common;

public interface ICommand : IRequest;
public interface ICommand<TResponse> : IRequest<TResponse>;