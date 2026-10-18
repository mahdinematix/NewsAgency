using FluentAssertions;
using NewsAgency.Domain.CategoryAgg.Entities;
using NewsAgency.Domain.CategoryAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;
using NewsAgency.Domain.Tests.Unit.Builders;

namespace NewsAgency.Domain.Tests.Unit.CategoryTests;

public class EditCategoryTests
{
    private readonly CategoryTestBuilder _builder;

    public EditCategoryTests()
    {
        _builder = new CategoryTestBuilder();
    }
    [Fact]
    public void Should_Change_Title_When_Value_Is_Valid()
    {
        //arrange
        CategoryTitle oldCategoryTitle = new(new string('a', 2));
        CategoryTitle newCategoryTitle = new(new string('b', 2));
        var category = _builder.WithTitle(oldCategoryTitle).Build();

        //act
        category.ChangeTitle(newCategoryTitle);

        //assert
        category.CategoryTitle.Should().Be(newCategoryTitle);
    }

    [Theory]
    [InlineData(3L)]
    [InlineData(null)]
    public void Should_Change_Parent_When_Value_Is_Valid(long? newparentId)
    {
        //arrange
        long oldparentId = 2;
        var category = _builder.WithParentId(oldparentId).Build();

        //act
        category.ChangeParent(newparentId);

        //assert
        category.ParentId.Should().Be(newparentId);
    }
}

