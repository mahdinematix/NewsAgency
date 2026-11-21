using Microsoft.AspNetCore.Diagnostics;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Application.Exceptions;
using NewsAgency.EndPoints.WebApi.Models;

namespace NewsAgency.EndPoints.WebApi.ExceptionHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            _logger.LogError(
                exception,
                "An unexpected exception occurred while processing the request.");

            return false;
        }

        _logger.LogInformation(
            "Request validation failed with {ErrorCount} errors.",
            validationException.Errors.Count);

        var response = new ApiResponse<object?>
        {
            Data = null,
            IsSuccess = false,
            Message = validationException.Message,
            ErrorCode = OperationErrorCode.Validation,
            Links = new Dictionary<string, ApiLink>(),
            Errors = validationException.Errors
                .Select(error => new ApiValidationError(
                    error.PropertyName,
                    error.ErrorMessage))
                .ToList()
        };

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(
            response,
            cancellationToken);

        return true;
    }
}