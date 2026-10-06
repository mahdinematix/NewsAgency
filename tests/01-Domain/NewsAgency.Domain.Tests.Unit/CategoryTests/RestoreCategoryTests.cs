using FluentAssertions;
using NewsAgency.Domain.Tests.Unit.Builders;

namespace NewsAgency.Domain.Tests.Unit.CategoryTests;

public class RestoreCategoryTests
{
    private readonly CategoryTestBuilder _builder;

    public RestoreCategoryTests()
    {
        _builder = new CategoryTestBuilder();
    }

    [Fact]
    public void Should_Restore_Category()
    {
        //arrange
        var category = _builder.Build();
        category.Remove();

        //act
        category.Restore();

        //assert
        category.IsRemoved.Should().BeFalse();
    }

    [Fact]
    public void Should_Do_Nothing_When_Category_Is_Already_Not_Removed()
    {
        //arrange
        var category = _builder.Build();
        category.Remove();
        category.Restore();

        //act
        category.Restore();

        //assert
        category.IsRemoved.Should().BeFalse();
    }
}