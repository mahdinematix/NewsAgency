using FluentValidation.TestHelper;
using NewsAgency.Application.ArticleServices.QueryHandlers.GetByTitle;
using NewsAgency.Application.Contracts.Article.Queries;

namespace NewsAgency.Application.Tests.Unit.ArticleServices.QueryHandlers.GetByTitle;

public class GetArticleByTitleQueryValidatorTests
{
    private readonly GetArticleByTitleQueryValidator _validator;

    public GetArticleByTitleQueryValidatorTests()
    {
        _validator = new GetArticleByTitleQueryValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Title_Is_Valid()
    {
        // Arrange
        var title = "Valid Article Title";
        var query = new GetArticleByTitleQuery(title);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A")]
    public void Should_Have_Error_When_Title_Is_Empty_Or_Shorter_Than_Minimum(string? title)
    {
        // Arrange
        var query = new GetArticleByTitleQuery(title!);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Exceeds_Maximum_Length()
    {
        // Arrange
        var title = new string('A', 51);
        var query = new GetArticleByTitleQuery(title);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }
}