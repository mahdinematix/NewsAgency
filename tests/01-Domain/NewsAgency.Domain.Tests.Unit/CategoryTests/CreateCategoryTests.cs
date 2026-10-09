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
        long id = 1;
        Title title = new(new string('a',2));

        //act
        var category = new Category(id, title, parentId);

        //assert
        category.Id.Should().Be(id);
        category.Title.Should().Be(title);
        category.ParentId.Should().Be(parentId);
        category.Articles.Should().BeEmpty();
    }

    [Fact]
    public void Should_Throw_Exception_When_Category_Is_Its_Own_Parent()
    {
       //act
        Action createCategory = () => _builder.WithParentId(1).Build();

        //assert
        createCategory.Should().ThrowExactly<DomainStateException>();
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

