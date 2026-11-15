using FluentAssertions;
using NewsAgency.Domain.Tests.Unit.Builders;

namespace NewsAgency.Domain.Tests.Unit.CategoryTests;

public class CategoryCanRemoveTests
{
    private readonly CategoryTestBuilder _builder;

    public CategoryCanRemoveTests()
    {
        _builder = new CategoryTestBuilder();
    }

    [Theory]
    [InlineData(false, false, true)]
    [InlineData(false, true, false)]
    [InlineData(true, false, false)]
    [InlineData(true, true, false)]
    public void Should_Return_Correct_Result(
        bool isRemoved,
        bool hasActiveChildren,
        bool expectedResult)
    {
        // Arrange
        var category = _builder.Build();

        if (isRemoved)
        {
            category.Remove(hasActiveChildren: false);
        }

        // Act
        var result = category.CanRemove(hasActiveChildren);

        // Assert
        result.Should().Be(expectedResult);
    }
}