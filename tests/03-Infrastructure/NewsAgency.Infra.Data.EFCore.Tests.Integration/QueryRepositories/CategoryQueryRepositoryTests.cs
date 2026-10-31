using FluentAssertions;
using NewsAgency.Domain.CategoryAgg.Entities;
using NewsAgency.Domain.CategoryAgg.ValueObjects;
using NewsAgency.Domain.Tests.Unit.Builders;
using NewsAgency.Infra.Data.EFCore.CommandRepositories;
using NewsAgency.Infra.Data.EFCore.QueryRepositories;
using NewsAgency.Infra.Data.EFCore.Tests.Integration.ClassFixtures;

namespace NewsAgency.Infra.Data.EFCore.Tests.Integration.QueryRepositories;

public class CategoryQueryRepositoryTests
    : IClassFixture<RealDatabaseFixture>
{
    private readonly RealDatabaseFixture _fixture;
    private readonly CategoryTestBuilder _builder;

    public CategoryQueryRepositoryTests(RealDatabaseFixture fixture)
    {
        _fixture = fixture;
        _builder = new CategoryTestBuilder();
    }

    [Fact]
    public async Task Should_Return_Category_By_Title()
    {
        // Arrange
        await using var context = _fixture.CreateContext();

        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var queryRepository =
            new CategoryQueryRepository(context);

        var commandRepository =
            new CategoryCommandRepository(context);

        CategoryTitle title = new("New Title");

        var category = _builder
            .WithTitle(title)
            .Build();

        await commandRepository.CreateAsync(
            category,
            CancellationToken.None);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        // Act
        var actual = await queryRepository.GetByAsync(
            category.Title.Value,
            CancellationToken.None);

        // Assert
        actual.Title.Should().Be(title);

        // Teardown
        await transaction.RollbackAsync();
    }

    [Fact]
    public async Task Should_Return_Null_When_Category_Title_Not_Found()
    {
        // Arrange
        await using var context = _fixture.CreateContext();

        var queryRepository =
            new CategoryQueryRepository(context);

        const string title = "NotFoundTitle";

        // Act
        var actual = await queryRepository.GetByAsync(
            title,
            CancellationToken.None);

        // Assert
        actual.Should().BeNull();
    }

    [Fact]
    public async Task Should_Return_Children_By_ParentId()
    {
        // Arrange
        await using var context = _fixture.CreateContext();

        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var queryRepository =
            new CategoryQueryRepository(context);

        var commandRepository =
            new CategoryCommandRepository(context);

        var parent = _builder
            .WithParentId(null)
            .Build();

        await commandRepository.CreateAsync(
            parent,
            CancellationToken.None);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        var parentId = parent.Id;

        var child = _builder
            .WithParentId(parentId)
            .Build();

        await commandRepository.CreateAsync(
            child,
            CancellationToken.None);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        // Act
        var actual = await queryRepository.GetChildrenByAsync(
            parentId,
            CancellationToken.None);

        // Assert
        actual.Should().Contain(category => category.Id == child.Id);

        // Teardown
        await transaction.RollbackAsync();
    }

    [Fact]
    public async Task Should_Return_Parent_By_Child_Id()
    {
        // Arrange
        await using var context = _fixture.CreateContext();

        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var queryRepository =
            new CategoryQueryRepository(context);

        var commandRepository =
            new CategoryCommandRepository(context);

        var parent = _builder
            .WithParentId(null)
            .Build();

        await commandRepository.CreateAsync(
            parent,
            CancellationToken.None);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        var parentId = parent.Id;

        var child = _builder
            .WithParentId(parentId)
            .Build();

        await commandRepository.CreateAsync(
            child,
            CancellationToken.None);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        var childId = child.Id;

        // Act
        var actual = await queryRepository.GetParentByAsync(
            childId,
            CancellationToken.None);

        // Assert
        actual.Should().Be(parent);

        // Teardown
        await transaction.RollbackAsync();
    }

    [Fact]
    public async Task Should_Return_Null_When_Category_Has_No_Parent()
    {
        // Arrange
        await using var context = _fixture.CreateContext();

        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var queryRepository =
            new CategoryQueryRepository(context);

        var commandRepository =
            new CategoryCommandRepository(context);

        var category = _builder
            .WithParentId(null)
            .Build();

        await commandRepository.CreateAsync(
            category,
            CancellationToken.None);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        // Act
        var actual = await queryRepository.GetParentByAsync(
            category.Id,
            CancellationToken.None);

        // Assert
        actual.Should().BeNull();

        // Teardown
        await transaction.RollbackAsync();
    }

    [Fact]
    public async Task Should_Return_True_When_Category_Has_An_Active_Child()
    {
        // Arrange
        await using var context = _fixture.CreateContext();

        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var queryRepository =
            new CategoryQueryRepository(context);

        var commandRepository =
            new CategoryCommandRepository(context);

        var parent = new Category(
            new CategoryTitle("Parent Category"));

        await commandRepository.CreateAsync(
            parent,
            CancellationToken.None);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        var child = new Category(
            new CategoryTitle("Child Category"),
            parent.Id);

        await commandRepository.CreateAsync(
            child,
            CancellationToken.None);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        // Act
        var result = await queryRepository.HasActiveChildrenAsync(
            parent.Id,
            CancellationToken.None);

        // Assert
        result.Should().BeTrue();

        // Teardown
        await transaction.RollbackAsync();
    }

    [Fact]
    public async Task Should_Return_False_When_Parent_Category_Is_Active()
    {
        // Arrange
        await using var context = _fixture.CreateContext();

        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var queryRepository =
            new CategoryQueryRepository(context);

        var commandRepository =
            new CategoryCommandRepository(context);

        var parent = new Category(
            new CategoryTitle("Parent Category"));

        await commandRepository.CreateAsync(
            parent,
            CancellationToken.None);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        var child = new Category(
            new CategoryTitle("Child Category"),
            parent.Id);

        await commandRepository.CreateAsync(
            child,
            CancellationToken.None);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        // Act
        var result = await queryRepository.IsParentRemovedAsync(
            child.ParentId,
            CancellationToken.None);

        // Assert
        result.Should().BeFalse();

        // Teardown
        await transaction.RollbackAsync();
    }

    [Fact]
    public async Task Should_Return_True_When_Parent_Category_Is_Removed()
    {
        // Arrange
        await using var context = _fixture.CreateContext();

        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var queryRepository =
            new CategoryQueryRepository(context);

        var commandRepository =
            new CategoryCommandRepository(context);

        var parent = new Category(
            new CategoryTitle("Parent Category"));

        await commandRepository.CreateAsync(
            parent,
            CancellationToken.None);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        var child = new Category(
            new CategoryTitle("Child Category"),
            parent.Id);

        await commandRepository.CreateAsync(
            child,
            CancellationToken.None);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        parent.Remove(false);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        child.Remove(false);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        // Act
        var result = await queryRepository.IsParentRemovedAsync(
            child.ParentId,
            CancellationToken.None);

        // Assert
        result.Should().BeTrue();

        // Teardown
        await transaction.RollbackAsync();
    }

    [Fact]
    public async Task Should_Return_False_When_ParentId_Is_Null()
    {
        // Arrange
        await using var context = _fixture.CreateContext();

        var queryRepository =
            new CategoryQueryRepository(context);

        // Act
        var result = await queryRepository.IsParentRemovedAsync(
            null,
            CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }
}