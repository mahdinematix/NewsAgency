using FluentAssertions;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.Exceptions;
using NewsAgency.Domain.Tests.Unit.Builders;

namespace NewsAgency.Domain.Tests.Unit.ArticleTests;

public class PublishArticleTests
{
    private readonly ArticleTestBuilder _builder;

    public PublishArticleTests()
    {
        _builder = new ArticleTestBuilder();
    }

    [Fact]
    public void Should_Publish_When_Status_Is_Draft()
    {
        //arrange
        var article = _builder.Build();


        //act
        article.Publish();

        //assert
        article.Status.Should().Be(ArticleStatus.Published);
    }

    [Fact]
    public void Should_Not_Change_Status_When_Status_Is_Published_Already()
    {
        //arrange
        var article = _builder.Build();

        article.Publish();

        //act
        article.Publish();

        //assert
        article.Status.Should().Be(ArticleStatus.Published);
    }

    [Fact]
    public void Should_Throw_Exception_When_Trying_To_Publish_When_Status_Is_Rejected()
    {
        //arrange
        var article = _builder.Build();

        article.Reject();

        //act
        Action publishArticle = () => article.Publish();

        //assert
        publishArticle.Should().ThrowExactly<DomainStateException>();

    }

    [Fact]
    public void Should_Throw_Exception_When_Trying_To_Publish_When_Status_Is_Archived()
    {
        //arrange
        var article = _builder.Build();

        article.Publish();
        article.Archive();

        //act
        Action publishArticle = () => article.Publish();

        //assert
        publishArticle.Should().ThrowExactly<DomainStateException>();

    }

}