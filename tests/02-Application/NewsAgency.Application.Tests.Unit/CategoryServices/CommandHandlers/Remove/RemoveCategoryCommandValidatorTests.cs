using FluentValidation.TestHelper;
using NewsAgency.Application.CategoryServices.CommandHandlers.Remove;
using NewsAgency.Application.Contracts.Category.Commands;

namespace NewsAgency.Application.Tests.Unit.CategoryServices.CommandHandlers.Remove;

public class RemoveCategoryCommandValidatorTests
{
    private readonly RemoveCategoryCommandValidator _validator;

    public RemoveCategoryCommandValidatorTests()
    {
        _validator = new RemoveCategoryCommandValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Id_Is_Valid()
    {
        // Arrange
        var id = 1;
        var command = new RemoveCategoryCommand(id);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_Id_Is_Not_Greater_Than_Zero(long id)
    {
        // Arrange
        var command = new RemoveCategoryCommand(id);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}