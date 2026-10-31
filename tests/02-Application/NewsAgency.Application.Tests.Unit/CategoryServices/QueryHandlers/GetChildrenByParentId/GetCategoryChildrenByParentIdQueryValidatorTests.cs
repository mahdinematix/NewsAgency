using FluentValidation.TestHelper;
using NewsAgency.Application.CategoryServices.QueryHandlers.GetChildrenByParentId;
using NewsAgency.Application.Contracts.Category.Queries;
using Xunit;

namespace NewsAgency.Application.Tests.Unit.CategoryServices.QueryHandlers.GetChildrenByParentId;

public class GetCategoryChildrenByParentIdQueryValidatorTests
{
    private readonly GetCategoryChildrenByParentIdQueryValidator _validator;

    public GetCategoryChildrenByParentIdQueryValidatorTests()
    {
        _validator = new GetCategoryChildrenByParentIdQueryValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_ParentId_Is_Valid()
    {
        // Arrange
        var parentId = 1;
        var query = new GetCategoryChildrenByParentIdQuery(parentId);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_ParentId_Is_Not_Greater_Than_Zero(long parentId)
    {
        // Arrange
        var query = new GetCategoryChildrenByParentIdQuery(parentId);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ParentId);
    }
}