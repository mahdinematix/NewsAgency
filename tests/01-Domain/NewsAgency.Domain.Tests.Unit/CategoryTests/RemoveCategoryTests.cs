using FluentAssertions;
using NewsAgency.Domain.Exceptions;
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
    public void Should_Remove_Category_When_It_Has_No_Active_Children()
    {
        // Arrange
        var category = _builder.Build();

        // Act
        category.Remove(false);

        // Assert
        category.IsRemoved.Should().BeTrue();
    }

    [Fact]
    public void Should_Do_Nothing_When_Category_Is_Already_Removed()
    {
        // Arrange
        var category = _builder.Build();
        category.Remove(false);

        // Act
        category.Remove(false);

        // Assert
        category.IsRemoved.Should().BeTrue();
    }

    [Fact]
    public void Should_Throw_DomainStateException_When_Category_Has_Active_Children()
    {
        // Arrange
        var category = _builder.Build();

        // Act
        var act = () => category.Remove(hasActiveChildren: true);

        // Assert
        act.Should()
            .Throw<DomainStateException>();

        category.IsRemoved.Should().BeFalse();
    }
}