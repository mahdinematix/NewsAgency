using FluentAssertions;
using NewsAgency.Domain.ArticleAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Domain.Tests.Unit.ArticleTests;

public class ArticleContentTests
{

    [Fact]
    public void Should_Return_ArticleContent_Type_From_String()
    {
        //arrange
        string title = "test content";

        //act
        var articleContent = ArticleContent.FromString(title);

        //assert
        articleContent.Should().BeOfType<ArticleContent>();
        articleContent.Value.Should().Be(title);
    }
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

