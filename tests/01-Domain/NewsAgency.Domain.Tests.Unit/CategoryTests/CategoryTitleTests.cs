using FluentAssertions;
using NewsAgency.Domain.CategoryAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Domain.Tests.Unit.CategoryTests;

public class CategoryTitleTests
{
    [Theory]
    [InlineData(2)]
    [InlineData(50)]
    public void Should_Create_When_Value_Length_Is_Valid(int length)
    {
        //arrange
        string value = new string('a',length);

        //act
        var categoryTitle = new CategoryTitle(value);

        //assert
        categoryTitle.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(51)]
    public void Should_Throw_Exception_When_Value_Length_Is_Invalid(int length)
    {
        //arrange
        string value = "A".PadLeft(length, '-');

        //act
        Action categoryTitle = () => new CategoryTitle(value);

        //assert
        categoryTitle.Should().ThrowExactly<InvalidValueObjectStateException>();
    }
}

