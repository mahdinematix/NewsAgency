using FluentAssertions;
using NewsAgency.Domain.Tests.Unit.Builders;

namespace NewsAgency.Domain.Tests.Unit.CategoryTests;

public class CategoryCanRestoreTests
{
    private readonly CategoryTestBuilder _builder;

    public CategoryCanRestoreTests()
    {
        _builder = new CategoryTestBuilder();
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, true, false)]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    public void Should_Return_Correct_Result(
        bool isRemoved,
        bool parentIsRemoved,
        bool expectedResult)
    {
        // Arrange
        var category = _builder.Build();

        if (isRemoved)
        {
            category.Remove(hasActiveChildren: false);
        }

        // Act
        var result = category.CanRestore(parentIsRemoved);

        // Assert
        result.Should().Be(expectedResult);
    }
}