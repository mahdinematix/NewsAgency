using FluentAssertions;
using NewsAgency.Application.Contracts.Category.Commands;
using NewsAgency.Application.Contracts.Category.Common;
using NewsAgency.EndPoints.WebApi.Models;
using NewsAgency.EndPoints.WebApi.Models.Categories;
using NewsAgency.EndPoints.WebApi.Tests.Integration.Infrastructure;
using RESTFulSense.Clients;

namespace NewsAgency.EndPoints.WebApi.Tests.Integration;

[Collection("IntegrationTests")]
public class CategoriesControllerTests
{
    private readonly TestDatabaseFixture _database;
    private readonly RESTFulApiFactoryClient _restClient;

    private const string _path = "api/Categories";

    public CategoriesControllerTests(TestDatabaseFixture database)
    {
        _database = database;
        _restClient = database.RestClient;
    }

    [Fact]
    public async Task Should_Return_All_Categories()
    {
        // Arrange
        await _database.ResetAsync();

        var command = CreateValidCategoryCommand();

        var created = await _restClient.PostContentAsync<
            CreateCategoryCommand,
            ApiResponse<long>>(
                _path,
                command);

        // Act
        var actual = await _restClient.GetContentAsync<
            ApiResponse<List<ApiResource<CategoryViewModel>>>>(_path);

        // Assert
        actual.IsSuccess.Should().BeTrue();
        actual.Data.Should().NotBeNull();
        actual.Data.Should().ContainSingle(category => category.Data.Id == created.Data);
    }

    [Fact]
    public async Task Should_Create_New_Category()
    {
        // Arrange
        await _database.ResetAsync();

        var command = CreateValidCategoryCommand();

        // Act
        var actual = await _restClient.PostContentAsync<
            CreateCategoryCommand,
            ApiResponse<long>>(
                _path,
                command);

        var Category = await _restClient.GetContentAsync<
            ApiResponse<CategoryViewModel>>(
                $"{_path}/{actual.Data}");

        // Assert
        actual.IsSuccess.Should().BeTrue();
        Category.Data.Id.Should().Be(actual.Data);
        Category.Data.Title.Should().Be(command.Title);
        Category.Data.ParentId.Should().Be(command.ParentId);
    }

    [Fact]
    public async Task Should_Get_A_Category_By_Id()
    {
        //arrange
        await _database.ResetAsync();

        var command = CreateValidCategoryCommand();

        var created = await _restClient.PostContentAsync<
            CreateCategoryCommand,
            ApiResponse<long>>(
            _path,
            command);

        //act
        var actual = await _restClient.GetContentAsync<
            ApiResponse<CategoryViewModel>>(
            $"{_path}/{created.Data}");

        //assert
        actual.IsSuccess.Should().BeTrue();
        actual.Data.Id.Should().Be(created.Data);
        actual.Data.Title.Should().Be(command.Title);
        actual.Data.ParentId.Should().Be(command.ParentId);
    }

    [Fact]
    public async Task Should_Get_A_Category_By_Title()
    {
        //arrange
        await _database.ResetAsync();

        var command = CreateValidCategoryCommand();

        var created = await _restClient.PostContentAsync<
            CreateCategoryCommand,
            ApiResponse<long>>(
            _path,
            command);

        //act
        var actual = await _restClient.GetContentAsync<
            ApiResponse<CategoryViewModel>>(
            $"{_path}/by-title?title={command.Title}");

        //assert
        actual.IsSuccess.Should().BeTrue();
        actual.Data.Id.Should().Be(created.Data);
        actual.Data.Title.Should().Be(command.Title);
        actual.Data.ParentId.Should().Be(command.ParentId);
    }

    [Fact]
    public async Task Should_Get_Parent_By_Child_Id()
    {
        // Arrange
        await _database.ResetAsync();

        // Create Parent Category
        var parentCommand = CreateValidCategoryCommand();

        var createdParent = await _restClient.PostContentAsync<
            CreateCategoryCommand,
            ApiResponse<long>>(
            _path,
            parentCommand);

        // Create Child Category
        var childCommand = new CreateCategoryCommand(
            "Child Category",
            createdParent.Data);

        var createdChild = await _restClient.PostContentAsync<
            CreateCategoryCommand,
            ApiResponse<long>>(
            _path,
            childCommand);

        // Act
        var actual = await _restClient.GetContentAsync<
            ApiResponse<CategoryViewModel>>(
            $"{_path}/{createdChild.Data}/parent");
        var children = await _restClient.GetContentAsync<
            ApiResponse<List<ApiResource<CategoryViewModel>>>>(
            $"{_path}/{createdParent.Data}/children");

        // Assert
        actual.IsSuccess.Should().BeTrue();
        actual.Data.Id.Should().Be(createdParent.Data);
        actual.Data.Title.Should().Be(parentCommand.Title);
    }

    [Fact]
    public async Task Should_Get_Children_By_Parent_Id()
    {
        // Arrange
        await _database.ResetAsync();

        var parentCommand = CreateValidCategoryCommand();

        var createdParent = await _restClient.PostContentAsync<
            CreateCategoryCommand,
            ApiResponse<long>>(
            _path,
            parentCommand);

        var firstChildCommand = new CreateCategoryCommand(
            "First Child Category",
            createdParent.Data);

        var createdFirstChild = await _restClient.PostContentAsync<
            CreateCategoryCommand,
            ApiResponse<long>>(
            _path,
            firstChildCommand);

        var secondChildCommand = new CreateCategoryCommand(
            "Second Child Category",
            createdParent.Data);

        var createdSecondChild = await _restClient.PostContentAsync<
            CreateCategoryCommand,
            ApiResponse<long>>(
            _path,
            secondChildCommand);

        // Act
        var actual = await _restClient.GetContentAsync<
            ApiResponse<List<ApiResource<CategoryViewModel>>>>(
            $"{_path}/{createdParent.Data}/children");

        // Assert
        actual.IsSuccess.Should().BeTrue();
        actual.Data.Should().HaveCount(2);

        actual.Data.Should().ContainSingle(child =>
            child.Data.Id == createdFirstChild.Data &&
            child.Data.Title == firstChildCommand.Title &&
            child.Data.ParentId == createdParent.Data);

        actual.Data.Should().ContainSingle(child =>
            child.Data.Id == createdSecondChild.Data &&
            child.Data.Title == secondChildCommand.Title &&
            child.Data.ParentId == createdParent.Data);
    }

    [Fact]
    public async Task Should_Change_Title()
    {
        //arrange
        await _database.ResetAsync();

        var command = CreateValidCategoryCommand();

        var created = await _restClient.PostContentAsync<
            CreateCategoryCommand,
            ApiResponse<long>>(
            _path,
            command);

        var request = new ChangeCategoryTitleRequest
        {
            Title = "test title changed"
        };

        //act
        var actual = await _restClient.PutContentAsync<
            ChangeCategoryTitleRequest,
            ApiResponse<CategoryViewModel>>(
            $"{_path}/{created.Data}/title",
            request);

        //assert
        actual.IsSuccess.Should().BeTrue();
        actual.Data.Id.Should().Be(created.Data);
        actual.Data.Title.Should().Be(request.Title);
        actual.Data.ParentId.Should().Be(command.ParentId);
    }

    [Fact]
    public async Task Should_Change_Parent()
    {
        // Arrange
        await _database.ResetAsync();

        var firstParentCommand = new CreateCategoryCommand(
            "First Parent Category",
            null);

        var firstParent = await _restClient.PostContentAsync<
            CreateCategoryCommand,
            ApiResponse<long>>(
            _path,
            firstParentCommand);

        var secondParentCommand = new CreateCategoryCommand(
            "Second Parent Category",
            null);

        var secondParent = await _restClient.PostContentAsync<
            CreateCategoryCommand,
            ApiResponse<long>>(
            _path,
            secondParentCommand);

        var childCommand = new CreateCategoryCommand(
            "Child Category",
            firstParent.Data);

        var createdChild = await _restClient.PostContentAsync<
            CreateCategoryCommand,
            ApiResponse<long>>(
            _path,
            childCommand);

        var request = new ChangeCategoryParentRequest
        {
            ParentId = secondParent.Data
        };

        // Act
        var actual = await _restClient.PutContentAsync<
            ChangeCategoryParentRequest,
            ApiResponse<CategoryViewModel>>(
            $"{_path}/{createdChild.Data}/parent",
            request);

        // Assert
        actual.IsSuccess.Should().BeTrue();
        actual.Data.Id.Should().Be(createdChild.Data);
        actual.Data.Title.Should().Be(childCommand.Title);
        actual.Data.ParentId.Should().Be(secondParent.Data);
    }

    [Fact]
    public async Task Should_Remove_Category()
    {
        // Arrange
        await _database.ResetAsync();

        var command = CreateValidCategoryCommand();

        var created = await _restClient.PostContentAsync<
            CreateCategoryCommand,
            ApiResponse<long>>(
            _path,
            command);

        // Act
        var actual = await _restClient.DeleteContentAsync<ApiResponse<CategoryViewModel>>($"{_path}/{created.Data}");

        // Assert
        actual.IsSuccess.Should().BeTrue();
        actual.Data.Id.Should().Be(created.Data);
        actual.Data.IsRemoved.Should().BeTrue();
        actual.Data.Actions.CanRemove.Should().BeFalse();
        actual.Data.Actions.CanRestore.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Restore_Category()
    {
        // Arrange
        await _database.ResetAsync();

        var command = CreateValidCategoryCommand();

        var created = await _restClient.PostContentAsync<
            CreateCategoryCommand,
            ApiResponse<long>>(
            _path,
            command);

        var removed = await _restClient.DeleteContentAsync<
            ApiResponse<CategoryViewModel>>(
            $"{_path}/{created.Data}");

        removed.Data.IsRemoved.Should().BeTrue();

        // Act
        var actual =
            await _restClient.PostContentAsync<ApiResponse<CategoryViewModel>>($"{_path}/{created.Data}/restore", null);

        // Assert
        actual.IsSuccess.Should().BeTrue();
        actual.Data.Id.Should().Be(created.Data);
        actual.Data.IsRemoved.Should().BeFalse();
        actual.Data.Actions.CanRemove.Should().BeTrue();
        actual.Data.Actions.CanRestore.Should().BeFalse();
    }

    private static CreateCategoryCommand CreateValidCategoryCommand()
    {
        return new CreateCategoryCommand(
            "test title",
            null);
    }
}