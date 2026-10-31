using FluentValidation.TestHelper;
using NewsAgency.Application.AuthorServices.CommandHandlers.Create;
using NewsAgency.Application.Contracts.Author.Commands;

namespace NewsAgency.Application.Tests.Unit.AuthorServices.CommandHandlers.Create;

public class CreateAuthorCommandValidatorTests
{
    private readonly CreateAuthorCommandValidator _validator;

    public CreateAuthorCommandValidatorTests()
    {
        _validator = new CreateAuthorCommandValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var firstName = "Mahdi";
        var lastName = "Nemati";
        var email = "mahdi@example.com";
        var phoneNumber = "09123456789";
        var command = new CreateAuthorCommand(firstName, lastName, email, phoneNumber);

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
    public void Should_Have_Error_When_FirstName_Is_Empty_Or_Shorter_Than_Minimum(string? firstName)
    {
        // Arrange
        var command = new CreateAuthorCommand(firstName!, "Nemati", "mahdi@example.com", "09123456789");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Exceeds_Maximum_Length()
    {
        // Arrange
        var firstName = new string('A', 51);
        var command = new CreateAuthorCommand(firstName, "Nemati", "mahdi@example.com", "09123456789");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A")]
    public void Should_Have_Error_When_LastName_Is_Empty_Or_Shorter_Than_Minimum(string? lastName)
    {
        // Arrange
        var command = new CreateAuthorCommand("Mahdi", lastName!, "mahdi@example.com", "09123456789");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Exceeds_Maximum_Length()
    {
        // Arrange
        var lastName = new string('A', 51);
        var command = new CreateAuthorCommand("Mahdi", lastName, "mahdi@example.com", "09123456789");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Have_Error_When_Email_Is_Empty(string? email)
    {
        // Arrange
        var command = new CreateAuthorCommand("Mahdi", "Nemati", email!, "09123456789");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Exceeds_Maximum_Length()
    {
        // Arrange
        var email = $"{new string('a', 244)}@example.com";
        var command = new CreateAuthorCommand("Mahdi", "Nemati", email, "09123456789");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("test")]
    [InlineData("@example.com")]
    public void Should_Have_Error_When_Email_Format_Is_Invalid(string email)
    {
        // Arrange
        var command = new CreateAuthorCommand("Mahdi", "Nemati", email, "09123456789");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Have_Error_When_PhoneNumber_Is_Empty(string? phoneNumber)
    {
        // Arrange
        var command = new CreateAuthorCommand("Mahdi", "Nemati", "mahdi@example.com", phoneNumber!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("123456789")]
    [InlineData("123456789012345")]
    public void Should_Have_Error_When_PhoneNumber_Length_Is_Invalid(string phoneNumber)
    {
        // Arrange
        var command = new CreateAuthorCommand("Mahdi", "Nemati", "mahdi@example.com", phoneNumber);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("091234567A")]
    [InlineData("09123-56789")]
    [InlineData("+989123456789")]
    public void Should_Have_Error_When_PhoneNumber_Contains_NonDigit_Characters(string phoneNumber)
    {
        // Arrange
        var command = new CreateAuthorCommand("Mahdi", "Nemati", "mahdi@example.com", phoneNumber);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }
}