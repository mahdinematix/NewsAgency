using FluentAssertions;
using NewsAgency.Domain.CategoryAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Domain.Tests.Unit.CategoryTests;

public class CategoryTitleTests
{

    [Fact]
    public void Should_Return_CategoryTitle_Type_From_String()
    {
        //arrange
        string title = "test title";

        //act
        var categoryTitle = CategoryTitle.FromString(title);

        //assert
        categoryTitle.Should().BeOfType<CategoryTitle>();
        categoryTitle.Value.Should().Be(title);
    }


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

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Throw_Exception_When_Title_Is_Null_Or_Whitespace(string? value)
    {
        //act
        Action categoryTitle = () => new CategoryTitle(value);

        //assert
        categoryTitle.Should().ThrowExactly<InvalidValueObjectStateException>();
    }
}

