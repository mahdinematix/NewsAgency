using FluentValidation.TestHelper;
using NewsAgency.Application.CategoryServices.QueryHandlers.GetByTitle;
using NewsAgency.Application.Contracts.Category.Queries;

namespace NewsAgency.Application.Tests.Unit.CategoryServices.QueryHandlers.GetByTitle;

public class GetCategoryByTitleQueryValidatorTests
{
    private readonly GetCategoryByTitleQueryValidator _validator;

    public GetCategoryByTitleQueryValidatorTests()
    {
        _validator = new GetCategoryByTitleQueryValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Title_Is_Valid()
    {
        // Arrange
        var title = "Technology";
        var query = new GetCategoryByTitleQuery(title);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A")]
    public void Should_Have_Error_When_Title_Is_Empty_Or_Shorter_Than_Minimum(string? title)
    {
        // Arrange
        var query = new GetCategoryByTitleQuery(title!);

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
        var query = new GetCategoryByTitleQuery(title);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }
}