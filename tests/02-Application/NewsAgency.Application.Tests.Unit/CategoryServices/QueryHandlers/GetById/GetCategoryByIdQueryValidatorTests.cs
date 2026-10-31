using FluentValidation.TestHelper;
using NewsAgency.Application.CategoryServices.QueryHandlers.GetById;
using NewsAgency.Application.Contracts.Category.Queries;

namespace NewsAgency.Application.Tests.Unit.CategoryServices.QueryHandlers.GetById;

public class GetCategoryByIdQueryValidatorTests
{
    private readonly GetCategoryByIdQueryValidator _validator;

    public GetCategoryByIdQueryValidatorTests()
    {
        _validator = new GetCategoryByIdQueryValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Id_Is_Valid()
    {
        // Arrange
        var id = 1;
        var query = new GetCategoryByIdQuery(id);

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
        var query = new GetCategoryByIdQuery(id);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}