using FluentAssertions;
using NewsAgency.Domain.Exceptions;
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
        category.Remove(false);

        //act
        category.Restore(false);

        //assert
        category.IsRemoved.Should().BeFalse();
    }

    [Fact]
    public void Should_Do_Nothing_When_Category_Is_Already_Not_Removed()
    {
        //arrange
        var category = _builder.Build();
        category.Remove(false);
        category.Restore(false);

        //act
        category.Restore(false);

        //assert
        category.IsRemoved.Should().BeFalse();
    }

    [Fact]
    public void Should_Throw_DomainStateException_When_Parent_Is_Removed()
    {
        // Arrange
        var category = _builder.Build();
        category.Remove(hasActiveChildren: false);

        // Act
        var act = () => category.Restore(parentIsRemoved: true);

        // Assert
        act.Should()
            .Throw<DomainStateException>();
    }
}