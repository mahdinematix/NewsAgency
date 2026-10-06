using FluentAssertions;
using NewsAgency.Domain.Tests.Unit.Builders;

namespace NewsAgency.Domain.Tests.Unit.CategoryTests;

public class RemoveCategoryTests
{
    private readonly CategoryTestBuilder _builder;

    public RemoveCategoryTests()
    {
        _builder = new CategoryTestBuilder();
    }
    [Fact]
    public void Should_Remove_Category()
    {
         //arrange
         var category = _builder.Build();

        //act
        category.Remove();

        //assert
        category.IsRemoved.Should().BeTrue();
    }

    [Fact]
    public void Should_Do_Nothing_When_Category_Is_Already_Removed()
    {
        //arrange
        var category = _builder.Build();
        category.Remove();

        //act
        category.Remove();

        //assert
        category.IsRemoved.Should().BeTrue();
    }
}