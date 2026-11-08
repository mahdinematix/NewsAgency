using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.Extensions;
using NewsAgency.EndPoints.WebApi.Models;

namespace NewsAgency.EndPoints.WebApi.Tests.Unit.Extensions;

public class OperationResultExtensionsTests
{
    private readonly TestController _controller = new();

    private sealed class TestController : ControllerBase
    {
    }

    [Fact]
    public void Should_Return_Ok_With_Data_And_Links_When_Operation_Succeeds()
    {
        // Arrange
        const string expectedData = "Test Data";

        var operation = new OperationResult<string>()
            .Succeeded(expectedData);

        var links = new Dictionary<string, ApiLink>
        {
            ["self"] = new("/api/test/1", "GET"),
            ["collection"] = new("/api/test", "GET")
        };

        // Act
        var actionResult = _controller.ToActionResult(operation, links);

        // Assert
        var okResult = actionResult.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        okResult.StatusCode.Should().Be(200);

        var response = okResult.Value.Should()
            .BeOfType<ApiResponse<string>>()
            .Subject;

        response.IsSuccess.Should().BeTrue();
        response.Data.Should().Be(expectedData);
        response.Links.Should().BeEquivalentTo(links);
    }

    [Fact]
    public void Should_Return_Empty_Links_When_No_Links_Are_Provided()
    {
        // Arrange
        var operation = new OperationResult<string>()
            .Succeeded("Test Data");

        // Act
        var actionResult = _controller.ToActionResult(operation);

        // Assert
        var okResult = actionResult.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        var response = okResult.Value.Should()
            .BeOfType<ApiResponse<string>>()
            .Subject;

        response.Links.Should().NotBeNull();
        response.Links.Should().BeEmpty();
    }

    [Theory]
    [InlineData(OperationErrorCode.Validation, 400)]
    [InlineData(OperationErrorCode.NotFound, 404)]
    [InlineData(OperationErrorCode.Conflict, 409)]
    [InlineData(OperationErrorCode.LogicError, 422)]
    [InlineData((OperationErrorCode)999, 500)]
    public void Should_Return_Correct_Http_Status_Code_For_Error_Code(
        OperationErrorCode errorCode,
        int expectedStatusCode)
    {
        // Arrange
        var operation = new OperationResult<string>()
            .Failed(errorCode);

        // Act
        var actionResult = _controller.ToActionResult(operation);

        // Assert
        var objectResult = actionResult.Should()
            .BeOfType<ObjectResult>()
            .Subject;

        objectResult.StatusCode.Should().Be(expectedStatusCode);
        var response = objectResult.Value.Should()
            .BeOfType<ApiResponse<string>>()
            .Subject;

        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();
        response.ErrorCode.Should().Be(errorCode);
        response.Links.Should().BeEmpty();
    }
}