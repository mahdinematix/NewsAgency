using FluentValidation.TestHelper;
using NewsAgency.Application.AuthorServices.QueryHandlers.GetByPhoneNumber;
using NewsAgency.Application.Contracts.Author.Queries;

namespace NewsAgency.Application.Tests.Unit.AuthorServices.QueryHandlers.GetByPhoneNumber;

public class GetAuthorByPhoneNumberQueryValidatorTests
{
    private readonly GetAuthorByPhoneNumberQueryValidator _validator;

    public GetAuthorByPhoneNumberQueryValidatorTests()
    {
        _validator = new GetAuthorByPhoneNumberQueryValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_PhoneNumber_Is_Valid()
    {
        // Arrange
        var phoneNumber = "09123456789";
        var query = new GetAuthorByPhoneNumberQuery(phoneNumber);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Have_Error_When_PhoneNumber_Is_Empty(string? phoneNumber)
    {
        // Arrange
        var query = new GetAuthorByPhoneNumberQuery(phoneNumber!);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("123456789")]
    [InlineData("123456789012345")]
    public void Should_Have_Error_When_PhoneNumber_Length_Is_Invalid(string phoneNumber)
    {
        // Arrange
        var query = new GetAuthorByPhoneNumberQuery(phoneNumber);

        // Act
        var result = _validator.TestValidate(query);

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
        var query = new GetAuthorByPhoneNumberQuery(phoneNumber);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }
}