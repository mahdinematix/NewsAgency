using FluentAssertions;
using NewsAgency.Domain.CategoryAgg.Entities;
using NewsAgency.Domain.CategoryAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;
using NewsAgency.Domain.Tests.Unit.Builders;

namespace NewsAgency.Domain.Tests.Unit.CategoryTests;

public class CreateCategoryTests
{
    private readonly CategoryTestBuilder _builder;

    public CreateCategoryTests()
    {
        _builder = new CategoryTestBuilder();
    }
    [Theory]
    [InlineData(null)]
    [InlineData(2L)]
    public void Should_Create_When_Data_Is_Valid(long? parentId)
    {
        //arrange
        CategoryTitle categoryTitle = new(new string('a',2));

        //act
        var category = new Category(categoryTitle, parentId);

        //assert
        category.CategoryTitle.Should().Be(categoryTitle);
        category.ParentId.Should().Be(parentId);
        category.Articles.Should().BeEmpty();
        category.Children.Should().BeEmpty();
    }

    [Fact]
    public void Should_Create_With_As_Not_Removed()
    {
        //act
        var category = _builder.Build();

        //assert
        category.IsRemoved.Should().BeFalse();
    }
}

