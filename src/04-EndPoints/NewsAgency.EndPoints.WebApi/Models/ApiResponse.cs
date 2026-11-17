using NewsAgency.Application.Contracts.Common;
using System.Text.Json.Serialization;

namespace NewsAgency.EndPoints.WebApi.Models;

public sealed class ApiResponse<TData>
{
    public TData? Data { get; init; }

    public bool IsSuccess { get; init; }

    public string Message { get; init; }

    public OperationErrorCode ErrorCode { get; init; }

    public Dictionary<string, ApiLink> Links { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyCollection<ApiValidationError>? Errors { get; init; }
}