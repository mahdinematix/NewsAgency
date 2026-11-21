using FluentAssertions;
using NewsAgency.Application.Contracts.Article.Commands;
using NewsAgency.Application.Contracts.Article.Common;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.EndPoints.WebApi.Models;
using NewsAgency.EndPoints.WebApi.Models.Articles;
using NewsAgency.EndPoints.WebApi.Tests.Integration.Infrastructure;
using NewsAgency.Utilities.Extensions;
using RESTFulSense.Clients;
using NewsAgency.Application.Contracts.Author.Commands;
using NewsAgency.Application.Contracts.Category.Commands;

namespace NewsAgency.EndPoints.WebApi.Tests.Integration;

[Collection("IntegrationTests")]
public class ArticlesControllerTests
{
    private readonly TestDatabaseFixture _database;
    private readonly RESTFulApiFactoryClient _restClient;

    private const string _path = "api/Articles";

    public ArticlesControllerTests(TestDatabaseFixture database)
    {
        _database = database;
        _restClient = database.RestClient;
    }

    [Fact]
    public async Task Should_Return_All_Articles()
    {
        // Arrange
        await _database.ResetAsync();

        var (authorId, categoryId) = await CreateArticleDependenciesAsync();


        var command = CreateValidArticleCommand(authorId, categoryId);

        var created = await _restClient.PostContentAsync<
            CreateArticleCommand,
            ApiResponse<long>>(
            _path,
            command);

        // Act
        var actual = await _restClient.GetContentAsync<
            ApiResponse<List<ApiResource<ArticleViewModel>>>>(_path);

        // Assert
        actual.IsSuccess.Should().BeTrue();

        actual.Data.Should().NotBeNull();

        actual.Data.Should().ContainSingle(article =>
            article.Data.Id == created.Data);
    }

    [Fact]
    public async Task Should_Create_New_Article()
    {
        // Arrange
        await _database.ResetAsync();

        var (authorId, categoryId) = await CreateArticleDependenciesAsync();

        var command = CreateValidArticleCommand(authorId, categoryId);

        // Act
        var actual = await _restClient.PostContentAsync<
            CreateArticleCommand,
            ApiResponse<long>>(
                _path,
                command);

        var Article = await _restClient.GetContentAsync<
            ApiResponse<ArticleViewModel>>(
                $"{_path}/{actual.Data}");

        // Assert
        actual.IsSuccess.Should().BeTrue();

        Article.IsSuccess.Should().BeTrue();

        Article.Data.Id.Should().Be(actual.Data);

        Article.Data.Title.Should().Be(command.Title);

        Article.Data.Content.Should().Be(command.Content);

        Article.Data.CategoryId.Should().Be(command.CategoryId);

        Article.Data.AuthorId.Should().Be(command.AuthorId);
    }

    [Fact]
    public async Task Should_Get_An_Article_By_Id()
    {
        //arrange
        await _database.ResetAsync();

        var (authorId, categoryId) = await CreateArticleDependenciesAsync();

        var command = CreateValidArticleCommand(authorId, categoryId);

        var created = await _restClient.PostContentAsync<
            CreateArticleCommand,
            ApiResponse<long>>(
            _path,
            command);


        //act
        var actual = await _restClient.GetContentAsync<
            ApiResponse<ArticleViewModel>>(
            $"{_path}/{created.Data}");

        //assert
        actual.IsSuccess.Should().BeTrue();

        actual.Data.Id.Should().Be(created.Data);

        actual.Data.Title.Should().Be(command.Title);

        actual.Data.Content.Should().Be(command.Content);

        actual.Data.AuthorId.Should().Be(command.AuthorId);

        actual.Data.CategoryId.Should().Be(command.CategoryId);
    }

    [Fact]
    public async Task Should_Get_An_Article_By_Title()
    {
        //arrange
        await _database.ResetAsync();

        var authorCommand = CreateValidAuthorCommand();

        var (authorId, categoryId) = await CreateArticleDependenciesAsync();

        var command = CreateValidArticleCommand(authorId, categoryId);

        var created = await _restClient.PostContentAsync<
            CreateArticleCommand,
            ApiResponse<long>>(
            _path,
            command);

        //act
        var actual = await _restClient.GetContentAsync<
            ApiResponse<ArticleViewModel>>(
            $"{_path}/by-title?title={command.Title}");

        //assert
        actual.IsSuccess.Should().BeTrue();

        actual.Data.Id.Should().Be(created.Data);

        actual.Data.Title.Should().Be(command.Title);

        actual.Data.Content.Should().Be(command.Content);

        actual.Data.AuthorId.Should().Be(command.AuthorId);

        actual.Data.CategoryId.Should().Be(command.CategoryId);
    }
    [Fact]
    public async Task Should_Edit_Article()
    {
        // Arrange
        await _database.ResetAsync();

        var (authorId, categoryId) = await CreateArticleDependenciesAsync();

        var command = CreateValidArticleCommand(authorId, categoryId);

        var created = await _restClient.PostContentAsync<
            CreateArticleCommand,
            ApiResponse<long>>(
            _path,
            command);

        var request = new EditArticleRequest
        {
            Title = "Test Title Edited",
            Content = "Test Article Content Edited",
            AuthorId = authorId,
            CategoryId = categoryId
        };

        // Act
        var actual = await _restClient.PutContentAsync<
            EditArticleRequest,
            ApiResponse<ArticleViewModel>>(
            $"{_path}/{created.Data}",
            request);

        // Assert
        actual.IsSuccess.Should().BeTrue();

        actual.Data.Id.Should().Be(created.Data);

        actual.Data.Title.Should().Be(request.Title);

        actual.Data.Content.Should().Be(request.Content);

        actual.Data.CategoryId.Should().Be(categoryId);

        actual.Data.AuthorId.Should().Be(authorId);
    }

    [Fact]
    public async Task Should_Publish_Article()
    {
        //arrange
        await _database.ResetAsync();

        var (authorId, categoryId) = await CreateArticleDependenciesAsync();

        var command = CreateValidArticleCommand(authorId, categoryId);

        var created = await _restClient.PostContentAsync<
            CreateArticleCommand,
            ApiResponse<long>>(
            _path,
            command);

        //act
        var actual = await _restClient.PostContentAsync<long,
            ApiResponse<ArticleViewModel>>(
            $"{_path}/{created.Data}/publish", created.Data);

        //assert
        actual.IsSuccess.Should().BeTrue();

        actual.Data.Status.Should().Be(ArticleStatus.Published.GetEnumDescription());
    }

    

    private static CreateArticleCommand CreateValidArticleCommand(long authorId,
        long categoryId)
    {
        return new CreateArticleCommand("Test Title", "Test Article Content", authorId, categoryId);
    }

    private static CreateAuthorCommand CreateValidAuthorCommand()
    {
        return new CreateAuthorCommand(
            "Mahdi",
            "Nemati",
            "Mahdin@gmail.com",
            "0098956574574");
    }

    private static CreateCategoryCommand CreateValidCategoryCommand()
    {
        return new CreateCategoryCommand(
            "test title",null);
    }


    private async Task<(long AuthorId, long CategoryId)>
        CreateArticleDependenciesAsync()
    {
        var authorCommand = CreateValidAuthorCommand();

        var createdAuthor = await _restClient.PostContentAsync<
            CreateAuthorCommand,
            ApiResponse<long>>(
            "api/Authors",
            authorCommand);

        var categoryCommand = CreateValidCategoryCommand();

        var createdCategory = await _restClient.PostContentAsync<
            CreateCategoryCommand,
            ApiResponse<long>>(
            "api/Categories",
            categoryCommand);

        return (
            createdAuthor.Data,
            createdCategory.Data
        );
    }
}