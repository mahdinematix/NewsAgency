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
        CategoryTitle oldTitle = new(new string('a', 2));
        CategoryTitle newTitle = new(new string('b', 2));
        var category = _builder.WithTitle(oldTitle).Build();

        //act
        category.ChangeTitle(newTitle);

        //assert
        category.Title.Should().Be(newTitle);
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

    [Fact]
    public void Should_Throw_Exception_When_Category_Is_Its_Own_Parent()
    {
        //arrange
        long oldparentId = 2;
        long newparentId = 1;
        var category = _builder.WithParentId(oldparentId).Build();

        //act
        Action changeParent = ()=>category.ChangeParent(newparentId);

        //assert
        changeParent.Should().ThrowExactly<DomainStateException>();
    }
}

