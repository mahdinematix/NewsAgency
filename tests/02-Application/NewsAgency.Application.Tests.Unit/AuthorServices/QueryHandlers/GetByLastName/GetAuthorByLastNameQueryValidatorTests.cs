using FluentValidation.TestHelper;
using NewsAgency.Application.AuthorServices.QueryHandlers.GetByLastName;
using NewsAgency.Application.Contracts.Author.Queries;
using Xunit;

namespace NewsAgency.Application.Tests.Unit.AuthorServices.QueryHandlers.GetByLastName;

public class GetAuthorByLastNameQueryValidatorTests
{
    private readonly GetAuthorByLastNameQueryValidator _validator;

    public GetAuthorByLastNameQueryValidatorTests()
    {
        _validator = new GetAuthorByLastNameQueryValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_LastName_Is_Valid()
    {
        // Arrange
        var lastName = "Nemati";
        var query = new GetAuthorByLastNameQuery(lastName);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Have_Error_When_LastName_Is_Empty(string? lastName)
    {
        // Arrange
        var query = new GetAuthorByLastNameQuery(lastName!);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Is_Shorter_Than_Minimum_Length()
    {
        // Arrange
        var lastName = "A";
        var query = new GetAuthorByLastNameQuery(lastName);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Exceeds_Maximum_Length()
    {
        // Arrange
        var lastName = new string('A', 51);
        var query = new GetAuthorByLastNameQuery(lastName);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }
}