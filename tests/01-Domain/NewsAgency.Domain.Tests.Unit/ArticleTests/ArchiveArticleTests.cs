using FluentAssertions;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.Exceptions;
using NewsAgency.Domain.Tests.Unit.Factories;

namespace NewsAgency.Domain.Tests.Unit.ArticleTests;

public class ArchiveArticleTests
{
    [Fact]
    public void Should_Archive_When_Status_Is_Published()
    {
        //arrange
        var article = ArticleFactory.Create();
        article.Publish();

        //act
        article.Archive();

        //assert
        article.Status.Should().Be(ArticleStatus.Archived);
    }

    [Fact]
    public void Should_Not_Change_Status_When_Status_Is_Archived_Already()
    {
        //arrange
        var article = ArticleFactory.Create();
        article.Publish();
        article.Archive();

        //act
        article.Archive();

        //assert
        article.Status.Should().Be(ArticleStatus.Archived);
    }

    [Fact]
    public void Should_Throw_Exception_When_Trying_To_Archive_When_Status_Is_Rejected()
    {
        //arrange
        var article = ArticleFactory.Create();
        article.Reject();

        //act
        Action archiveArticle = () => article.Archive();

        //assert
        archiveArticle.Should().ThrowExactly<DomainStateException>();
    }

    [Fact]
    public void Should_Throw_Exception_When_Trying_To_Archive_When_Status_Is_Draft()
    {
        //arrange
        var article = ArticleFactory.Create();


        //act
        Action archiveArticle = () => article.Archive();

        //assert
        archiveArticle.Should().ThrowExactly<DomainStateException>();
    }
}