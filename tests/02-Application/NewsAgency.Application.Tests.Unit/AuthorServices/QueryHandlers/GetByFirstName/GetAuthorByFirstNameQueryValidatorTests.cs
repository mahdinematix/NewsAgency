using FluentValidation.TestHelper;
using NewsAgency.Application.AuthorServices.QueryHandlers.GetByFirstName;
using NewsAgency.Application.Contracts.Author.Queries;
using Xunit;

namespace NewsAgency.Application.Tests.Unit.AuthorServices.QueryHandlers.GetByFirstName;

public class GetAuthorByFirstNameQueryValidatorTests
{
    private readonly GetAuthorByFirstNameQueryValidator _validator;

    public GetAuthorByFirstNameQueryValidatorTests()
    {
        _validator = new GetAuthorByFirstNameQueryValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_FirstName_Is_Valid()
    {
        // Arrange
        var firstName = "Mahdi";
        var query = new GetAuthorByFirstNameQuery(firstName);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Have_Error_When_FirstName_Is_Empty(string? firstName)
    {
        // Arrange
        var query = new GetAuthorByFirstNameQuery(firstName!);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Is_Shorter_Than_Minimum_Length()
    {
        // Arrange
        var firstName = "A";
        var query = new GetAuthorByFirstNameQuery(firstName);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Exceeds_Maximum_Length()
    {
        // Arrange
        var firstName = new string('A', 51);
        var query = new GetAuthorByFirstNameQuery(firstName);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }
}