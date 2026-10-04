using FluentAssertions;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.Exceptions;
using NewsAgency.Domain.Tests.Unit.Builders;

namespace NewsAgency.Domain.Tests.Unit.ArticleTests;

public class RejectArticleTests
{
    private readonly ArticleTestBuilder _builder;

    public RejectArticleTests()
    {
        _builder = new ArticleTestBuilder();
    }
    [Fact]
    public void Should_Reject_When_Status_Is_Draft()
    {
        //arrange
        var article = _builder.Build();


        //act
        article.Reject();

        //assert
        article.Status.Should().Be(ArticleStatus.Rejected);
    }

    [Fact]
    public void Should_Not_Change_Status_When_Status_Is_Rejected_Already()
    {
        //arrange
        var article = _builder.Build();

        article.Reject();

        //act
        article.Reject();

        //assert
        article.Status.Should().Be(ArticleStatus.Rejected);
    }

    [Fact]
    public void Should_Throw_Exception_When_Trying_To_Reject_When_Status_Is_Archived()
    {
        //arrange
        var article = _builder.Build();

        article.Publish();
        article.Archive();

        //act
        Action rejectArticle = () => article.Reject();

        //assert
        rejectArticle.Should().ThrowExactly<DomainStateException>();
    }

    [Fact]
    public void Should_Throw_Exception_When_Trying_To_Reject_When_Status_Is_Published()
    {
        //arrange
        var article = _builder.Build();

        article.Publish();

        //act
        Action rejectArticle = () => article.Reject();

        //assert
        rejectArticle.Should().ThrowExactly<DomainStateException>();
    }
}