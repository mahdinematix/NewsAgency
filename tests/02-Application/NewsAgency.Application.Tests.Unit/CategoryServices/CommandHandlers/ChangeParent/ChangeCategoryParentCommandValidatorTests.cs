using FluentValidation.TestHelper;
using NewsAgency.Application.CategoryServices.CommandHandlers.ChangeParent;
using NewsAgency.Application.Contracts.Category.Commands;

namespace NewsAgency.Application.Tests.Unit.CategoryServices.CommandHandlers.ChangeParent;

public class ChangeCategoryParentCommandValidatorTests
{
    private readonly ChangeCategoryParentCommandValidator _validator;

    public ChangeCategoryParentCommandValidatorTests()
    {
        _validator = new ChangeCategoryParentCommandValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var command = new ChangeCategoryParentCommand(
            1,
            2);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_ParentId_Is_Null()
    {
        // Arrange
        var command = new ChangeCategoryParentCommand(
            1,
            null);

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
        var command = new ChangeCategoryParentCommand(
            id,
            2);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_ParentId_Is_Not_Greater_Than_Zero(long parentId)
    {
        // Arrange
        var command = new ChangeCategoryParentCommand(
            1,
            parentId);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ParentId);
    }
}