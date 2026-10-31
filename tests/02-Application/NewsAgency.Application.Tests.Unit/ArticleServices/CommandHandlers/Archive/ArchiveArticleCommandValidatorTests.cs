using FluentValidation.TestHelper;
using NewsAgency.Application.ArticleServices.CommandHandlers.Archive;
using NewsAgency.Application.Contracts.Article.Commands;

namespace NewsAgency.Application.Tests.Unit.ArticleServices.CommandHandlers.Archive;

public class ArchiveArticleCommandValidatorTests
{
    private readonly ArchiveArticleCommandValidator _validator;

    public ArchiveArticleCommandValidatorTests()
    {
        _validator = new ArchiveArticleCommandValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Id_Is_Greater_Than_Zero()
    {
        // Arrange
        long id = 1;
        var command = new ArchiveArticleCommand(id);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(command => command.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_Id_Is_Not_Greater_Than_Zero(long id)
    {
        // Arrange
        var command = new ArchiveArticleCommand(id);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.Id);
    }
}