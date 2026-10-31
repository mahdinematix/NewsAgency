using FluentValidation.TestHelper;
using NewsAgency.Application.ArticleServices.CommandHandlers.Publish;
using NewsAgency.Application.Contracts.Article.Commands;
using Xunit;

namespace NewsAgency.Application.Tests.Unit.ArticleServices.CommandHandlers.Publish;

public class PublishArticleCommandValidatorTests
{
    private readonly PublishArticleCommandValidator _validator;

    public PublishArticleCommandValidatorTests()
    {
        _validator = new PublishArticleCommandValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Id_Is_Valid()
    {
        // Arrange
        long id = 1;
        var command = new PublishArticleCommand(id);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_Id_Is_Not_Greater_Than_Zero(long id)
    {
        // Arrange
        var command = new PublishArticleCommand(id);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}