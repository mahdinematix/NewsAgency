using FluentValidation.TestHelper;
using NewsAgency.Application.AuthorServices.CommandHandlers.ChangePhoneNumber;
using NewsAgency.Application.Contracts.Author.Commands;
using Xunit;

namespace NewsAgency.Application.Tests.Unit.AuthorServices.CommandHandlers.ChangePhoneNumber;

public class ChangeAuthorPhoneNumberCommandValidatorTests
{
    private readonly ChangeAuthorPhoneNumberCommandValidator _validator;

    public ChangeAuthorPhoneNumberCommandValidatorTests()
    {
        _validator = new ChangeAuthorPhoneNumberCommandValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var id = 1;
        var phoneNumber = "09123456789";
        var command = new ChangeAuthorPhoneNumberCommand(id, phoneNumber);

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
        var phoneNumber = "09123456789";
        var command = new ChangeAuthorPhoneNumberCommand(id, phoneNumber);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Have_Error_When_PhoneNumber_Is_Empty(string? phoneNumber)
    {
        // Arrange
        var id = 1;
        var command = new ChangeAuthorPhoneNumberCommand(id, phoneNumber!);

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
        var id = 1;
        var command = new ChangeAuthorPhoneNumberCommand(id, phoneNumber);

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
        var id = 1;
        var command = new ChangeAuthorPhoneNumberCommand(id, phoneNumber);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }
}