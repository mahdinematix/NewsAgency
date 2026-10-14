using FluentAssertions;
using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Tests.Unit
{
    public class OperationResultTests
    {
        [Fact]
        public void Should_Set_IsSuccess_To_True_When_Succeeded()
        {
            // Arrange
            var operation = new OperationResult();

            // Act
            operation.Succeeded();

            // Assert
            operation.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Should_Set_IsSuccess_To_False_When_Failed()
        {
            // Arrange
            var operation = new OperationResult();

            // Act
            operation.Failed();

            // Assert
            operation.IsSuccess.Should().BeFalse();
        }
    }
}
