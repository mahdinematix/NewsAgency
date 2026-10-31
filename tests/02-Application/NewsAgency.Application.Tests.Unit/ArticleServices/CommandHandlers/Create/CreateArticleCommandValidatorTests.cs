using FluentValidation.TestHelper;
using NewsAgency.Application.ArticleServices.CommandHandlers.Create;
using NewsAgency.Application.Contracts.Article.Commands;
using Xunit;

namespace NewsAgency.Application.Tests.Unit.ArticleServices.CommandHandlers.Create;

public class CreateArticleCommandValidatorTests
{
    private readonly CreateArticleCommandValidator _validator;

    public CreateArticleCommandValidatorTests()
    {
        _validator = new CreateArticleCommandValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Title_Is_Valid()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A")]
    public void Should_Have_Error_When_Title_Is_Invalid(string? title)
    {
        // Arrange
        var command = CreateValidCommand() with { Title = title! };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = CreateValidCommand() with { Title = new string('A', 51) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Content_Is_Valid()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Content);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("123456789")]
    public void Should_Have_Error_When_Content_Is_Invalid(string? content)
    {
        // Arrange
        var command = CreateValidCommand() with { Content = content! };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public void Should_Have_Error_When_Content_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = CreateValidCommand() with { Content = new string('A', 5001) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Ids_Are_Valid()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.AuthorId);
        result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-1, 1)]
    [InlineData(1, 0)]
    [InlineData(1, -1)]
    public void Should_Have_Error_When_An_Id_Is_Invalid(long authorId, long categoryId)
    {
        // Arrange
        var command = CreateValidCommand() with
        {
            AuthorId = authorId,
            CategoryId = categoryId
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        if (authorId <= 0)
            result.ShouldHaveValidationErrorFor(x => x.AuthorId);
        else
            result.ShouldNotHaveValidationErrorFor(x => x.AuthorId);

        if (categoryId <= 0)
            result.ShouldHaveValidationErrorFor(x => x.CategoryId);
        else
            result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
    }

    private static CreateArticleCommand CreateValidCommand()
    {
        return new CreateArticleCommand(
            Title: "Valid Title",
            Content: "This is valid article content.",
            AuthorId: 1,
            CategoryId: 1);
    }
}