using FluentValidation.TestHelper;
using NewsAgency.Application.AuthorServices.CommandHandlers.ChangeEmail;
using NewsAgency.Application.Contracts.Author.Commands;
using Xunit;

namespace NewsAgency.Application.Tests.Unit.AuthorServices.CommandHandlers.ChangeEmail;

public class ChangeAuthorEmailCommandValidatorTests
{
    private readonly ChangeAuthorEmailCommandValidator _validator;

    public ChangeAuthorEmailCommandValidatorTests()
    {
        _validator = new ChangeAuthorEmailCommandValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var id = 1;
        var email = "test@example.com";
        var command = new ChangeAuthorEmailCommand(id, email);

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
        var email = "test@example.com";
        var command = new ChangeAuthorEmailCommand(id, email);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Have_Error_When_Email_Is_Empty(string? email)
    {
        // Arrange
        var id = 1;
        var command = new ChangeAuthorEmailCommand(id, email!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Exceeds_Maximum_Length()
    {
        // Arrange
        var id = 1;
        var email = $"{new string('a', 244)}@example.com";
        var command = new ChangeAuthorEmailCommand(id, email);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("test@")]
    [InlineData("@example.com")]
    public void Should_Have_Error_When_Email_Format_Is_Invalid(string email)
    {
        // Arrange
        var id = 1;
        var command = new ChangeAuthorEmailCommand(id, email);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
}