using FluentValidation.TestHelper;
using NewsAgency.Application.CategoryServices.CommandHandlers.Restore;
using NewsAgency.Application.Contracts.Category.Commands;

namespace NewsAgency.Application.Tests.Unit.CategoryServices.CommandHandlers.Restore;

public class RestoreCategoryCommandValidatorTests
{
    private readonly RestoreCategoryCommandValidator _validator;

    public RestoreCategoryCommandValidatorTests()
    {
        _validator = new RestoreCategoryCommandValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Id_Is_Valid()
    {
        // Arrange
        var id = 1;
        var command = new RestoreCategoryCommand(id);

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
        var command = new RestoreCategoryCommand(id);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}