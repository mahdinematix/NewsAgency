using FluentValidation.TestHelper;
using NewsAgency.Application.ArticleServices.QueryHandlers.GetById;
using NewsAgency.Application.Contracts.Article.Queries;
using Xunit;

namespace NewsAgency.Application.Tests.Unit.ArticleServices.QueryHandlers.GetById;

public class GetArticleByIdQueryValidatorTests
{
    private readonly GetArticleByIdQueryValidator _validator;

    public GetArticleByIdQueryValidatorTests()
    {
        _validator = new GetArticleByIdQueryValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Id_Is_Valid()
    {
        // Arrange
        var id = 1;
        var query = new GetArticleByIdQuery(id);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_Id_Is_Not_Greater_Than_Zero(long id)
    {
        // Arrange
        var query = new GetArticleByIdQuery(id);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}