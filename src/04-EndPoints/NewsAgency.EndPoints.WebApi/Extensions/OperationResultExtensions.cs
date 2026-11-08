using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.Models;

namespace NewsAgency.EndPoints.WebApi.Extensions;

public static class OperationResultExtensions
{
    public static IActionResult ToActionResult<TData>(
        this ControllerBase controller,
        OperationResult<TData> result,
        Dictionary<string, ApiLink>? links = null)
    {
        var response = new ApiResponse<TData>
        {
            Data = result.Data,
            IsSuccess = result.IsSuccess,
            Message = result.Message,
            ErrorCode = result.ErrorCode,
            Links = links ?? []
        };

        if (result.IsSuccess)
        {
            return controller.Ok(response);
        }

        var statusCode = result.ErrorCode switch
        {
            OperationErrorCode.Validation => 400,
            OperationErrorCode.NotFound => 404,
            OperationErrorCode.Conflict => 409,
            OperationErrorCode.LogicError => 422,

            _ => 500
        };

        return controller.StatusCode(statusCode, response);
    }
}