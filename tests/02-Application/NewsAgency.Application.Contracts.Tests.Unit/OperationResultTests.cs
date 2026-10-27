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
            var errorCode = OperationErrorCode.NotFound;

            // Act
            operation.Failed(errorCode);

            // Assert
            operation.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public void Should_Throw_Exception_When_ErrorCode_Set_None_When_Operation_Is_Failed()
        {
            // Arrange
            var operation = new OperationResult();
            var errorCode = OperationErrorCode.NoError;

            // Act
            Action operationFailed = () => operation.Failed(errorCode);

            // Assert
            operationFailed.Should().ThrowExactly<ArgumentOutOfRangeException>();
        }
    }
}
