using FluentValidation.TestHelper;
using NewsAgency.Application.AuthorServices.CommandHandlers.ChangeLastName;
using NewsAgency.Application.Contracts.Author.Commands;
using Xunit;

namespace NewsAgency.Application.Tests.Unit.AuthorServices.CommandHandlers.ChangeLastName;

public class ChangeAuthorLastNameCommandValidatorTests
{
    private readonly ChangeAuthorLastNameCommandValidator _validator;

    public ChangeAuthorLastNameCommandValidatorTests()
    {
        _validator = new ChangeAuthorLastNameCommandValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var id = 1;
        var lastName = "Nemati";
        var command = new ChangeAuthorLastNameCommand(id, lastName);

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
        var lastName = "Nemati";
        var command = new ChangeAuthorLastNameCommand(id, lastName);

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
    public void Should_Have_Error_When_LastName_Is_Empty_Or_Shorter_Than_Minimum(string? lastName)
    {
        // Arrange
        var id = 1;
        var command = new ChangeAuthorLastNameCommand(id, lastName!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Exceeds_Maximum_Length()
    {
        // Arrange
        var id = 1;
        var lastName = new string('A', 51);
        var command = new ChangeAuthorLastNameCommand(id, lastName);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }
}