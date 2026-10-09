using FluentAssertions;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;
using NewsAgency.Domain.Tests.Unit.Builders;
using NewsAgency.Domain.Tests.Unit.Factories;

namespace NewsAgency.Domain.Tests.Unit.ArticleTests;

public class EditArticleTests
{
    [Fact]
    public void Should_Edit_When_Status_Is_Draft()
    {
        //arrange
        var article = ArrangeArticleForEdit(out var title, out var content, out var authorId, out var categoryId);

        //act
        article.Edit(title, content, authorId, categoryId);

        //assert
        article.Title.Should().Be(title);
        article.Content.Should().Be(content);
        article.AuthorId.Should().Be(authorId);
        article.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public void Should_Edit_When_Status_Is_Rejected()
    {
        //arrange
        var article = ArrangeArticleForEdit(out var title, out var content, out var authorId, out var categoryId);
        article.Reject();

        //act
        article.Edit(title, content, authorId, categoryId);

        //assert
        article.Title.Should().Be(title);
        article.Content.Should().Be(content);
        article.AuthorId.Should().Be(authorId);
        article.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public void Should_Throw_Exception_When_Trying_To_Edit_When_Status_Is_Published()
    {
        //arrange
        var article = ArrangeArticleForEdit(out var title, out var content, out var authorId, out var categoryId);
        article.Publish();

        //act
        Action editArticle =() => article.Edit(title, content, authorId, categoryId);

        //assert
        editArticle.Should().ThrowExactly<DomainStateException>();
        article.Title.Should().NotBe(title);
        article.Content.Should().NotBe(content);
        article.AuthorId.Should().NotBe(authorId);
        article.CategoryId.Should().NotBe(categoryId);
    }

    [Fact]
    public void Should_Throw_Exception_When_Trying_To_Edit_When_Status_Is_Archived()
    {
        //arrange
        var article = ArrangeArticleForEdit(out var title, out var content, out var authorId, out var categoryId);
        article.Publish();
        article.Archive();

        //act
        Action editArticle = () => article.Edit(title, content, authorId, categoryId);

        //assert
        editArticle.Should().ThrowExactly<DomainStateException>();
        article.Title.Should().NotBe(title);
        article.Content.Should().NotBe(content);
        article.AuthorId.Should().NotBe(authorId);
        article.CategoryId.Should().NotBe(categoryId);

    }

    private Article ArrangeArticleForEdit(out ArticleTitle title, out ArticleContent content, out long authorId,
        out long categoryId)
    {
        var article = ArticleFactory.Create();
        title = new ArticleTitle("b".PadLeft(5, '-'));
        content = new ArticleContent("b".PadLeft(10, '-'));
        authorId = 2;
        categoryId = 2;
        return article;
    }

}
