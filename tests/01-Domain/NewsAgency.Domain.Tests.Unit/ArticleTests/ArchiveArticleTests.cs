using FluentAssertions;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Domain.Tests.Unit.ArticleTests;

public class ArchiveArticleTests
{
    [Fact]
    public void Should_Archive_When_Status_Is_Published()
    {
        //arrange
        long id = 1;
        var title = new ArticleTitle("a".PadLeft(5, '-'));
        var content = new ArticleContent("a".PadLeft(10, '-'));
        long authorId = 1;
        long categoryId = 1;
        var article = new Article(id, title, content, authorId, categoryId);
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
        long id = 1;
        var title = new ArticleTitle("a".PadLeft(5, '-'));
        var content = new ArticleContent("a".PadLeft(10, '-'));
        long authorId = 1;
        long categoryId = 1;
        var article = new Article(id, title, content, authorId, categoryId);
        article.Publish();
        article.Archive();

        //act
        article.Archive();

        //assert
        article.Status.Should().Be(ArticleStatus.Archived);
    }

    [Fact]
    public void Should_Throw_Exception_When_Trying_Archive_When_Status_Is_Rejected()
    {
        //arrange
        long id = 1;
        var title = new ArticleTitle("a".PadLeft(5, '-'));
        var content = new ArticleContent("a".PadLeft(10, '-'));
        long authorId = 1;
        long categoryId = 1;
        var article = new Article(id, title, content, authorId, categoryId);
        article.Reject();

        //act
        Action archiveArticle = () => article.Archive();

        //assert
        archiveArticle.Should().ThrowExactly<DomainStateException>();
    }

    [Fact]
    public void Should_Throw_Exception_When_Trying_Archive_When_Status_Is_Draft()
    {
        //arrange
        long id = 1;
        var title = new ArticleTitle("a".PadLeft(5, '-'));
        var content = new ArticleContent("a".PadLeft(10, '-'));
        long authorId = 1;
        long categoryId = 1;
        var article = new Article(id, title, content, authorId, categoryId);

        //act
        Action archiveArticle = () => article.Archive();

        //assert
        archiveArticle.Should().ThrowExactly<DomainStateException>();
    }
}