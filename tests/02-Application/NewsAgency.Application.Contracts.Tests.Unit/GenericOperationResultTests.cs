using FluentAssertions;
using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Tests.Unit;

public class GenericOperationResultTests
{
    [Fact]
    public void Should_Set_Data_When_Succeeded()
    {
        // Arrange
        var operation = new OperationResult<long>();
        long articleId = 123;

        // Act
        operation.Succeeded(articleId);

        // Assert
        operation.Data.Should().Be(articleId);
    }

    [Fact]
    public void Should_Clear_Data_When_Failed()
    {
        // Arrange
        var operation = new OperationResult<long>();
        long articleId = 123;
        var errorCode = OperationErrorCode.NotFound;

        operation.Succeeded(articleId, "Article created.");

        // Act
        operation.Failed(
            errorCode,
            "Article creation failed.");

        // Assert
        operation.IsSuccess.Should().BeFalse();
        operation.Data.Should().Be(default);
    }
}
