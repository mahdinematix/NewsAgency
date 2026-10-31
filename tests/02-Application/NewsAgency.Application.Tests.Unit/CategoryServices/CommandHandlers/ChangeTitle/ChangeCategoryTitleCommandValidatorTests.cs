using FluentValidation.TestHelper;
using NewsAgency.Application.CategoryServices.CommandHandlers.ChangeTitle;
using NewsAgency.Application.Contracts.Category.Commands;

namespace NewsAgency.Application.Tests.Unit.CategoryServices.CommandHandlers.ChangeTitle;

public class ChangeCategoryTitleCommandValidatorTests
{
    private readonly ChangeCategoryTitleCommandValidator _validator;

    public ChangeCategoryTitleCommandValidatorTests()
    {
        _validator = new ChangeCategoryTitleCommandValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var id = 1;
        var title = "Technology";
        var command = new ChangeCategoryTitleCommand(id, title);

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
        var title = "Technology";
        var command = new ChangeCategoryTitleCommand(id, title);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A")]
    public void Should_Have_Error_When_Title_Is_Empty_Or_Shorter_Than_Minimum(string? title)
    {
        // Arrange
        var id = 1;
        var command = new ChangeCategoryTitleCommand(id, title!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Exceeds_Maximum_Length()
    {
        // Arrange
        var id = 1;
        var title = new string('A', 51);
        var command = new ChangeCategoryTitleCommand(id, title);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }
}