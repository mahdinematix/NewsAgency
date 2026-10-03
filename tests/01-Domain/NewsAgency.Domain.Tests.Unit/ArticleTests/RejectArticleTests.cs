using FluentAssertions;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Domain.Tests.Unit.ArticleTests;

public class RejectArticleTests
{
    [Fact]
    public void Should_Can_Reject_When_Status_Is_Draft()
    {
        //arrange
        long id = 1;
        var title = new ArticleTitle("a".PadLeft(5, '-'));
        var content = new ArticleContent("a".PadLeft(10, '-'));
        long authorId = 1;
        long categoryId = 1;
        var article = new Article(id, title, content, authorId, categoryId);

        //act
        article.Reject();

        //assert
        article.Status.Should().Be(ArticleStatus.Rejected);
    }

    [Fact]
    public void Should_Not_Change_Status_When_Status_Is_Rejected_Already()
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
        article.Reject();

        //assert
        article.Status.Should().Be(ArticleStatus.Rejected);
    }

    [Fact]
    public void Should_Throw_Exception_When_Trying_Reject_When_Status_Is_Archived()
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
        Action rejectArticle = () => article.Reject();

        //assert
        rejectArticle.Should().ThrowExactly<DomainStateException>();
    }

    [Fact]
    public void Should_Throw_Exception_When_Trying_Reject_When_Status_Is_Published()
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
        Action rejectArticle = () => article.Reject();

        //assert
        rejectArticle.Should().ThrowExactly<DomainStateException>();
    }
}