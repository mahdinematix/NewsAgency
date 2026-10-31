using FluentValidation.TestHelper;
using NewsAgency.Application.ArticleServices.CommandHandlers.Edit;
using NewsAgency.Application.Contracts.Article.Commands;

namespace NewsAgency.Application.Tests.Unit.ArticleServices.CommandHandlers.Edit;

public class EditArticleCommandValidatorTests
{
    private readonly EditArticleCommandValidator _validator;

    public EditArticleCommandValidatorTests()
    {
        _validator = new EditArticleCommandValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Id_Is_Valid()
    {
        //arrange
        long id = 1;
        var command = CreateValidCommand() with { Id = id };

        //act
        var result = _validator.TestValidate(command);

        //assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_Id_Is_Invalid(long id)
    {
        //arrange
        var command = CreateValidCommand() with { Id = id };

        //act
        var result = _validator.TestValidate(command);

        //assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Title_Is_Valid()
    {
        //arrange
        var command = CreateValidCommand();

        //act
        var result = _validator.TestValidate(command);

        //assert
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A")]
    public void Should_Have_Error_When_Title_Is_Invalid(string? title)
    {
        //arrange
        var command = CreateValidCommand() with { Title = title! };

        //act
        var result = _validator.TestValidate(command);

        //assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Exceeds_Maximum_Length()
    {
        //arrange
        var command = CreateValidCommand() with
        {
            Title = new string('A', 51)
        };

        //act
        var result = _validator.TestValidate(command);

        //assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Content_Is_Valid()
    {
        //arrange
        var command = CreateValidCommand();

        //act
        var result = _validator.TestValidate(command);

        //assert
        result.ShouldNotHaveValidationErrorFor(x => x.Content);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("123456789")]
    public void Should_Have_Error_When_Content_Is_Invalid(string? content)
    {
        //arrange
        var command = CreateValidCommand() with { Content = content! };

        //act
        var result = _validator.TestValidate(command);

        //assert
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public void Should_Have_Error_When_Content_Exceeds_Maximum_Length()
    {
        //arrange
        var command = CreateValidCommand() with
        {
            Content = new string('A', 5001)
        };

        //act
        var result = _validator.TestValidate(command);

        //assert
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public void Should_Not_Have_Error_When_AuthorId_Is_Valid()
    {
        //arrange
        var command = CreateValidCommand();

        //act
        var result = _validator.TestValidate(command);

        //assert
        result.ShouldNotHaveValidationErrorFor(x => x.AuthorId);
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(-1L)]
    public void Should_Have_Error_When_AuthorId_Is_Invalid(long authorId)
    {
        //arrange
        var command = CreateValidCommand() with { AuthorId = authorId };

        //act
        var result = _validator.TestValidate(command);

        //assert
        result.ShouldHaveValidationErrorFor(x => x.AuthorId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_CategoryId_Is_Valid()
    {
        //arrange
        var command = CreateValidCommand();

        //act
        var result = _validator.TestValidate(command);

        //assert
        result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_CategoryId_Is_Invalid(long categoryId)
    {
        //arrange
        var command = CreateValidCommand() with { CategoryId = categoryId };

        //act
        var result = _validator.TestValidate(command);

        //assert
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    private static EditArticleCommand CreateValidCommand()
    {
        return new EditArticleCommand(
            Id: 1,
            Title: "Valid Title",
            Content: "This is valid article content.",
            AuthorId: 1,
            CategoryId: 1);
    }
}