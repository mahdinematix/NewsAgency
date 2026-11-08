using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.EndPoints.WebApi.Models;

public sealed class ApiResponse<TData>
{
    public TData? Data { get; init; }

    public bool IsSuccess { get; init; }

    public string Message { get; init; }

    public OperationErrorCode ErrorCode { get; init; }

    public Dictionary<string, ApiLink> Links { get; init; }
}