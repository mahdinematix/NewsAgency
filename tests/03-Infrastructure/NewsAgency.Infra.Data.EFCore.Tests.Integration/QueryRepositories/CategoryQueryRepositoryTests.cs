using FluentAssertions;
using NewsAgency.Domain.CategoryAgg.Entities;
using NewsAgency.Domain.CategoryAgg.ValueObjects;
using NewsAgency.Domain.Tests.Unit.Builders;
using NewsAgency.Infra.Data.EFCore.CommandRepositories;
using NewsAgency.Infra.Data.EFCore.QueryRepositories;
using NewsAgency.Infra.Data.EFCore.Tests.Integration.ClassFixtures;
using System.Transactions;

namespace NewsAgency.Infra.Data.EFCore.Tests.Integration.QueryRepositories;

public class CategoryQueryRepositoryTests : IClassFixture<RealDatabaseFixture>
{
    private readonly CategoryQueryRepository _queryRepository;
    private readonly CategoryCommandRepository _commandRepository;
    private readonly CategoryTestBuilder _builder;

    public CategoryQueryRepositoryTests(RealDatabaseFixture fixture)
    {
        _queryRepository = new CategoryQueryRepository(fixture.Context);
        _commandRepository = new CategoryCommandRepository(fixture.Context);
        _builder = new CategoryTestBuilder();
    }


    [Fact]
    public async Task Should_Return_Category_By_Title()
    {
        //arrange
        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        CategoryTitle title = new("New Title");
        var category = _builder.WithTitle(title).Build();
        await _commandRepository.CreateAsync(category, CancellationToken.None);
        await _commandRepository.SaveChangesAsync(CancellationToken.None);


        //act
        var actual = await _queryRepository.GetByAsync(category.Title.Value, CancellationToken.None);

        //assert
        actual.Title.Should().Be(title);
    }

    [Fact]
    public async Task Should_Return_Null_When_Category_Title_Not_Found()
    {
        //arrange
        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        const string title = "NotFoundTitle";

        //act
        var actual = await _queryRepository.GetByAsync(title, CancellationToken.None);

        //assert
        actual.Should().BeNull();
    }

    [Fact]
    public async Task Should_Return_Children_By_ParentId()
    {
        // Arrange
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            TransactionScopeAsyncFlowOption.Enabled);

        var parent = _builder.WithParentId(null).Build();

        await _commandRepository.CreateAsync(parent, CancellationToken.None);
        await _commandRepository.SaveChangesAsync(CancellationToken.None);

        var parentId = parent.Id;

        var child = _builder.WithParentId(parentId).Build();

        await _commandRepository.CreateAsync(child, CancellationToken.None);
        await _commandRepository.SaveChangesAsync(CancellationToken.None);

        // Act
        var actual = await _queryRepository.GetChildrenByAsync(parentId, CancellationToken.None);

        // Assert
        actual.Should().Contain(category => category.Id == child.Id);
    }

    [Fact]
    public async Task Should_Return_Parent_By_Child_Id()
    {
        //arrange
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            TransactionScopeAsyncFlowOption.Enabled);
        var parent = _builder.WithParentId(null).Build();

        await _commandRepository.CreateAsync(parent, CancellationToken.None);
        await _commandRepository.SaveChangesAsync(CancellationToken.None);

        var parentId = parent.Id;

        var child = _builder.WithParentId(parentId).Build();

        await _commandRepository.CreateAsync(child, CancellationToken.None);
        await _commandRepository.SaveChangesAsync(CancellationToken.None);
        var childId = child.Id;

        //act
        var actual = await _queryRepository.GetParentByAsync(childId, CancellationToken.None);

        //assert
        actual.Should().Be(parent);
    }

    [Fact]
    public async Task Should_Return_Null_When_Category_Has_No_Parent()
    {
        // Arrange
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            TransactionScopeAsyncFlowOption.Enabled);

        var category = _builder.WithParentId(null).Build();

        await _commandRepository.CreateAsync(category, CancellationToken.None);
        await _commandRepository.SaveChangesAsync(CancellationToken.None);

        // Act
        var actual = await _queryRepository.GetParentByAsync(category.Id, CancellationToken.None);

        // Assert
        actual.Should().BeNull();
    }

    [Fact]
    public async Task Should_Return_True_When_Category_Has_An_Active_Child()
    {
        // Arrange
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            TransactionScopeAsyncFlowOption.Enabled);

        var parent = new Category(
            new CategoryTitle("Parent Category"));

        await _commandRepository.CreateAsync(
            parent,
            CancellationToken.None);

        await _commandRepository.SaveChangesAsync(
            CancellationToken.None);

        var child = new Category(
            new CategoryTitle("Child Category"),
            parent.Id);

        await _commandRepository.CreateAsync(
            child,
            CancellationToken.None);

        await _commandRepository.SaveChangesAsync(
            CancellationToken.None);

        // Act
        var result = await _queryRepository.HasActiveChildrenAsync(
            parent.Id,
            CancellationToken.None);

        // Assert
        result.Should().BeTrue();

    }

    [Fact]
    public async Task Should_Return_False_When_Parent_Category_Is_Active()
    {
        // Arrange
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            TransactionScopeAsyncFlowOption.Enabled);

        var parent = new Category(
            new CategoryTitle("Parent Category"));

        await _commandRepository.CreateAsync(
            parent,
            CancellationToken.None);

        await _commandRepository.SaveChangesAsync(
            CancellationToken.None);

        var child = new Category(
            new CategoryTitle("Child Category"),
            parent.Id);

        await _commandRepository.CreateAsync(
            child,
            CancellationToken.None);

        await _commandRepository.SaveChangesAsync(
            CancellationToken.None);

        // Act
        var result = await _queryRepository.IsParentRemovedAsync(
            child.ParentId,
            CancellationToken.None);

        // Assert
        result.Should().BeFalse();

    }

    [Fact]
    public async Task Should_Return_True_When_Parent_Category_Is_Removed()
    {
        // Arrange
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            TransactionScopeAsyncFlowOption.Enabled);

        var parent = new Category(
            new CategoryTitle("Parent Category"));

        await _commandRepository.CreateAsync(
            parent,
            CancellationToken.None);

        await _commandRepository.SaveChangesAsync(
            CancellationToken.None);

        var child = new Category(
            new CategoryTitle("Child Category"),
            parent.Id);

        await _commandRepository.CreateAsync(
            child,
            CancellationToken.None);

        await _commandRepository.SaveChangesAsync(
            CancellationToken.None);

        parent.Remove(false);

        await _commandRepository.SaveChangesAsync(
            CancellationToken.None);

        child.Remove(false);

        await _commandRepository.SaveChangesAsync(
            CancellationToken.None);

        // Act
        var result = await _queryRepository.IsParentRemovedAsync(
            child.ParentId,
            CancellationToken.None);

        // Assert
        result.Should().BeTrue();

    }

    [Fact]
    public async Task Should_Return_False_When_ParentId_Is_Null()
    {
        // Arrange
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            TransactionScopeAsyncFlowOption.Enabled);

        // Act
        var result = await _queryRepository.IsParentRemovedAsync(
            null,
            CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }
}