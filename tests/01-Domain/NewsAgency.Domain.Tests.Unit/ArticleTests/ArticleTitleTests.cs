using FluentAssertions;
using NewsAgency.Domain.ArticleAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Domain.Tests.Unit.ArticleTests;

public class ArticleTitleTests
{
    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("123456789012345678901234567890123456789012345678901")]
    public void Should_Throws_Exception_When_Title_Is_Invalid(string inputValue)
    {
        //act
        Action articleTitle = () => new ArticleTitle(inputValue);

        //assert
        articleTitle.Should().ThrowExactly<InvalidValueObjectStateException>();
    }

}

