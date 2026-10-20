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
        operation.Succeeded(123);

        // Act
        operation.Failed("Article creation failed.");

        // Assert
        operation.Data.Should().Be(default(long));
    }
}
