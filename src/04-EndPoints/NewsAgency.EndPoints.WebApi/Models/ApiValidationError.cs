namespace NewsAgency.EndPoints.WebApi.Models;

public sealed record ApiValidationError(
    string PropertyName,
    string ErrorMessage);