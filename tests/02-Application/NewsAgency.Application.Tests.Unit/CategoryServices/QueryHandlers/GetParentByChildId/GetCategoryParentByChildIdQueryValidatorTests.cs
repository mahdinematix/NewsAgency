using FluentValidation.TestHelper;
using NewsAgency.Application.CategoryServices.QueryHandlers.GetParentByChildId;
using NewsAgency.Application.Contracts.Category.Queries;

namespace NewsAgency.Application.Tests.Unit.CategoryServices.QueryHandlers.GetParentByChildId;

public class GetCategoryParentByChildIdQueryValidatorTests
{
    private readonly GetCategoryParentByChildIdQueryValidator _validator;

    public GetCategoryParentByChildIdQueryValidatorTests()
    {
        _validator = new GetCategoryParentByChildIdQueryValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_ChildId_Is_Valid()
    {
        // Arrange
        var childId = 1;
        var query = new GetCategoryParentByChildIdQuery(childId);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_ChildId_Is_Not_Greater_Than_Zero(long childId)
    {
        // Arrange
        var query = new GetCategoryParentByChildIdQuery(childId);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ChildId);
    }
}