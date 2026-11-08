namespace NewsAgency.EndPoints.WebApi.Models;

public sealed class ApiResource<TData>
{
    public TData Data { get; init; }
    public Dictionary<string, ApiLink> Links { get; init; }
}