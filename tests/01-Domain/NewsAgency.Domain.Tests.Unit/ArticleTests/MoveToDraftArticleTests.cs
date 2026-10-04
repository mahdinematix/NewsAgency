using FluentAssertions;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.Exceptions;
using NewsAgency.Domain.Tests.Unit.Builders;

namespace NewsAgency.Domain.Tests.Unit.ArticleTests;

public class MoveToDraftArticleTests
{
    private readonly ArticleTestBuilder _builder;

    public MoveToDraftArticleTests()
    {
        _builder = new ArticleTestBuilder();
    }

    [Fact]
    public void Should_Move_To_Draft_When_Status_Is_Rejected()
    {
        //arrange
        var article = _builder.Build();
        article.Reject();

        //act
        article.MoveToDraft();

        //assert
        article.Status.Should().Be(ArticleStatus.Draft);
    }
    [Fact]
    public void Should_Not_Change_Status_When_Status_Is_Draft_Already()
    {
        //arrange
        var article = _builder.Build();


        //act
        article.MoveToDraft();

        //assert
        article.Status.Should().Be(ArticleStatus.Draft);
    }

    [Fact]
    public void Should_Move_To_Draft_When_Status_Is_Archived()
    {
        //arrange
        var article = _builder.Build();

        article.Publish();
        article.Archive();

        //act
        article.MoveToDraft();

        //assert
        article.Status.Should().Be(ArticleStatus.Draft);
    }

    [Fact]
    public void Should_Throw_Exception_When_Trying_To_Move_Draft_When_Status_Is_Published()
    {
        //arrange
        var article = _builder.Build();

        article.Publish();

        //act
        Action draftArticle = () => article.MoveToDraft();

        //assert
        draftArticle.Should().ThrowExactly<DomainStateException>();
    }
}