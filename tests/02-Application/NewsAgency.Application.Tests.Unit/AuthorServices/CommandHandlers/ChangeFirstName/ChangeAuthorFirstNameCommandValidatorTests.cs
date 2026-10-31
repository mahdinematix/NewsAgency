using FluentValidation.TestHelper;
using NewsAgency.Application.AuthorServices.CommandHandlers.ChangeFirstName;
using NewsAgency.Application.Contracts.Author.Commands;
using Xunit;

namespace NewsAgency.Application.Tests.Unit.AuthorServices.CommandHandlers.ChangeFirstName;

public class ChangeAuthorFirstNameCommandValidatorTests
{
    private readonly ChangeAuthorFirstNameCommandValidator _validator;

    public ChangeAuthorFirstNameCommandValidatorTests()
    {
        _validator = new ChangeAuthorFirstNameCommandValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var id = 1;
        var firstName = "Mahdi";
        var command = new ChangeAuthorFirstNameCommand(id, firstName);

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
        var firstName = "Mahdi";
        var command = new ChangeAuthorFirstNameCommand(id, firstName);

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
    public void Should_Have_Error_When_FirstName_Is_Empty_Or_Shorter_Than_Minimum(string? firstName)
    {
        // Arrange
        var id = 1;
        var command = new ChangeAuthorFirstNameCommand(id, firstName!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Exceeds_Maximum_Length()
    {
        // Arrange
        var id = 1;
        var firstName = new string('A', 51);
        var command = new ChangeAuthorFirstNameCommand(id, firstName);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }
}