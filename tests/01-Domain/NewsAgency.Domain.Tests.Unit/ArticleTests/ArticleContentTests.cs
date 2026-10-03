using FluentAssertions;
using NewsAgency.Domain.ArticleAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Domain.Tests.Unit.ArticleTests;

public class ArticleContentTests
{
    [Theory]
    [InlineData(10)]
    [InlineData(5000)]
    public void Should_Create_When_Value_Is_Valid(int length)
    {
        //arrange
        var value = "a".PadLeft(length, '-');

        //act
        var articleContent = new ArticleContent(value);

        //assert
        articleContent.Value.Should().Be(value);
    }


    [Fact]
    public void Should_Throw_Exception_When_Value_Is_Empty()
    {
        //arrange
        var value = string.Empty;

        //act
        Action articleContent = () => new ArticleContent(value);

        //assert
        articleContent.Should().ThrowExactly<InvalidValueObjectStateException>();
    }

    [Theory]
    [InlineData(9)]
    [InlineData(5001)]
    public void Should_Throw_Exception_When_Value_Length_Is_Invalid(int length)
    {
        //arrange
        var value = "a".PadLeft(length,'-');

        //act
        Action articleContent = () => new ArticleContent(value);

        //assert
        articleContent.Should().ThrowExactly<InvalidValueObjectStateException>();
    }
}

