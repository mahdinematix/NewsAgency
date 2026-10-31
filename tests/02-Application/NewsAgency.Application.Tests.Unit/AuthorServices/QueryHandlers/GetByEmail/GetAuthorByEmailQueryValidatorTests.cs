using FluentValidation.TestHelper;
using NewsAgency.Application.AuthorServices.QueryHandlers.GetByEmail;
using NewsAgency.Application.Contracts.Author.Queries;
using Xunit;

namespace NewsAgency.Application.Tests.Unit.AuthorServices.QueryHandlers.GetByEmail;

public class GetAuthorByEmailQueryValidatorTests
{
    private readonly GetAuthorByEmailQueryValidator _validator;

    public GetAuthorByEmailQueryValidatorTests()
    {
        _validator = new GetAuthorByEmailQueryValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Email_Is_Valid()
    {
        // Arrange
        var email = "mahdi@example.com";
        var query = new GetAuthorByEmailQuery(email);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Have_Error_When_Email_Is_Empty(string? email)
    {
        // Arrange
        var query = new GetAuthorByEmailQuery(email!);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Exceeds_Maximum_Length()
    {
        // Arrange
        var email = $"{new string('a', 244)}@example.com";
        var query = new GetAuthorByEmailQuery(email);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("test")]
    [InlineData("@example.com")]
    public void Should_Have_Error_When_Email_Format_Is_Invalid(string email)
    {
        // Arrange
        var query = new GetAuthorByEmailQuery(email);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
}