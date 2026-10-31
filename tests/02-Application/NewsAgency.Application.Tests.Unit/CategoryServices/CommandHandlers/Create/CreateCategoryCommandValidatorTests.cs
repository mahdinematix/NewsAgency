using FluentValidation.TestHelper;
using NewsAgency.Application.CategoryServices.CommandHandlers.Create;
using NewsAgency.Application.Contracts.Category.Commands;

namespace NewsAgency.Application.Tests.Unit.CategoryServices.CommandHandlers.Create;

public class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _validator;

    public CreateCategoryCommandValidatorTests()
    {
        _validator = new CreateCategoryCommandValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var title = "Technology";
        long? parentId = 1;
        var command = new CreateCategoryCommand(title, parentId);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_ParentId_Is_Null()
    {
        // Arrange
        var title = "Technology";
        long? parentId = null;
        var command = new CreateCategoryCommand(title, parentId);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A")]
    public void Should_Have_Error_When_Title_Is_Empty_Or_Shorter_Than_Minimum(string? title)
    {
        // Arrange
        long? parentId = null;
        var command = new CreateCategoryCommand(title!, parentId);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Exceeds_Maximum_Length()
    {
        // Arrange
        var title = new string('A', 51);
        long? parentId = null;
        var command = new CreateCategoryCommand(title, parentId);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_ParentId_Is_Not_Greater_Than_Zero(long parentId)
    {
        // Arrange
        var title = "Technology";
        var command = new CreateCategoryCommand(title, parentId);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ParentId);
    }
}