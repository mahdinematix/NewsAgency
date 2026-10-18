using FluentAssertions;
using NewsAgency.Domain.ArticleAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Domain.Tests.Unit.ArticleTests;

public class ArticleTitleTests
{

    [Fact]
    public void Should_Return_ArticleTitle_Type_From_String()
    {
        //arrange
        string title = "test title";

        //act
        var articleTitle = ArticleTitle.FromString(title);

        //assert
        articleTitle.Should().BeOfType<ArticleTitle>();
        articleTitle.Value.Should().Be(title);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(50)]
    public void Should_Create_When_Value_Is_Valid(int length)
    {
        //arrange
        string value = "a".PadLeft(length, '-');

        //act
        var articleTitle = new ArticleTitle(value);

        //assert
        articleTitle.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(51)]
    public void Should_Throw_Exception_When_Title_Is_Invalid(int length)
    {
        //arrange
        string value = "A".PadLeft(length, '-');

        //act
        Action articleTitle = () => new ArticleTitle(value);

        //assert
        articleTitle.Should().ThrowExactly<InvalidValueObjectStateException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Throw_Exception_When_Title_Is_Null_Or_Whitespace(string? value)
    {
        //act
        Action articleTitle = () => new ArticleTitle(value);

        //assert
        articleTitle.Should().ThrowExactly<InvalidValueObjectStateException>();
    }
}

