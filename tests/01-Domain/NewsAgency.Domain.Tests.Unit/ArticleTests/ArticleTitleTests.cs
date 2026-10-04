using FluentAssertions;
using NewsAgency.Domain.ArticleAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Domain.Tests.Unit.ArticleTests;

public class ArticleTitleTests
{

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
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(51)]
    public void Should_Throws_Exception_When_Title_Is_Invalid(int length)
    {
        //arrange
        string value = "A".PadLeft(length, '-');

        //act
        Action articleTitle = () => new ArticleTitle(value);

        //assert
        articleTitle.Should().ThrowExactly<InvalidValueObjectStateException>();
    }
}

