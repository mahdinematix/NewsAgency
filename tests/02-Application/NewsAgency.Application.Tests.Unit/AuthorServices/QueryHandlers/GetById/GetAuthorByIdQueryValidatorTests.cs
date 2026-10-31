using FluentValidation.TestHelper;
using NewsAgency.Application.AuthorServices.QueryHandlers.GetById;
using NewsAgency.Application.Contracts.Author.Queries;

namespace NewsAgency.Application.Tests.Unit.AuthorServices.QueryHandlers.GetById;

public class GetAuthorByIdQueryValidatorTests
{
    private readonly GetAuthorByIdQueryValidator _validator;

    public GetAuthorByIdQueryValidatorTests()
    {
        _validator = new GetAuthorByIdQueryValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Id_Is_Valid()
    {
        // Arrange
        var id = 1;
        var query = new GetAuthorByIdQuery(id);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_Id_Is_Not_Greater_Than_Zero(long id)
    {
        // Arrange
        var query = new GetAuthorByIdQuery(id);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}