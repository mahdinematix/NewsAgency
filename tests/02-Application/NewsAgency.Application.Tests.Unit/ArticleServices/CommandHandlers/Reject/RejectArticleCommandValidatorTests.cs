using FluentValidation.TestHelper;
using NewsAgency.Application.ArticleServices.CommandHandlers.Reject;
using NewsAgency.Application.Contracts.Article.Commands;
using Xunit;

namespace NewsAgency.Application.Tests.Unit.ArticleServices.CommandHandlers.Reject;

public class RejectArticleCommandValidatorTests
{
    private readonly RejectArticleCommandValidator _validator;

    public RejectArticleCommandValidatorTests()
    {
        _validator = new RejectArticleCommandValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Id_Is_Valid()
    {
        // Arrange
        var id = 1;
        var command = new RejectArticleCommand(id);

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
        var command = new RejectArticleCommand(id);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}