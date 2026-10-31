using System.Linq.Expressions;
using FluentAssertions;
using NewsAgency.Application.CategoryServices.CommandHandlers.Create;
using NewsAgency.Application.Contracts.Category.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Entities;
using NewsAgency.Domain.CategoryAgg.Repositories;
using NSubstitute;

namespace NewsAgency.Application.Tests.Unit.CategoryServices.CommandHandlers.Create;

public class CreateCategoryCommandHandlerTests
{
    private readonly ICategoryCommandRepository _commandRepository;
    private readonly ICategoryQueryRepository _queryRepository;
    private readonly CreateCategoryCommandHandler _commandHandler;

    public CreateCategoryCommandHandlerTests()
    {
        _commandRepository = Substitute.For<ICategoryCommandRepository>();
        _queryRepository = Substitute.For<ICategoryQueryRepository>();
        _commandHandler = new CreateCategoryCommandHandler(_commandRepository, _queryRepository);
    }

    [Fact]
    public async Task Should_Create_And_Return_Success_Result_When_Command_Is_Valid()
    {
        //arrange

        var command = SomeCreateCategoryCommand();

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();

        await _commandRepository.Received(1).CreateAsync(Arg.Any<Category>(), CancellationToken.None);
        await _commandRepository.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Should_Create_Root_Category_And_Return_Success_Result_When_ParentId_Is_Null()
    {
        // Arrange
        var command = new CreateCategoryCommand("Root Category", null);

        // Act
        var result = await _commandHandler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _commandRepository.Received(1).CreateAsync(
            Arg.Any<Category>(), CancellationToken.None);
        await _commandRepository.Received(1)
            .SaveChangesAsync(CancellationToken.None);
    }


    [Fact]
    public async Task Should_Pass_Real_Command_Data_To_Repository()
    {
        // arrange
        var command = SomeCreateCategoryCommand();

        // act
        await _commandHandler.Handle(command, CancellationToken.None);

        // assert
        await _commandRepository.Received(1).CreateAsync(
            Arg.Is<Category>(category =>
                category.Title.Value == command.Title &&
                category.ParentId == command.ParentId),
            CancellationToken.None);
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Request_Is_Null()
    {
        // act
        var result = await _commandHandler.Handle(
            null!,
            CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.Validation);
        await _commandRepository.DidNotReceive()
            .CreateAsync(
                Arg.Any<Category>(),
                Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(51)]
    public async Task Should_Return_Failed_Result_When_Title_Is_Invalid(int length)
    {
        //arrange
        var invalidTitle = "A".PadLeft(length, '-');
        var command = new CreateCategoryCommand(invalidTitle, 1);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.Validation);
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Category_To_Create_Title_Exist()
    {
        //arrange
        string sameTitle = "Same Category Title";
        var command1 = new CreateCategoryCommand(sameTitle,1);
        var command2 = new CreateCategoryCommand(sameTitle, 1);
        _queryRepository
            .Exists(
                Arg.Any<Expression<Func<Category, bool>>>(),
                Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(false),
                Task.FromResult(true));

        //act
        var result1 = await _commandHandler.Handle(command1, CancellationToken.None);
        var result2 = await _commandHandler.Handle(command2, CancellationToken.None);


        //assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeFalse();
        result2.ErrorCode.Should().Be(OperationErrorCode.Conflict);
        await _commandRepository.Received(1).CreateAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>());
        await _commandRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _queryRepository.Received(2)
            .Exists(
                Arg.Any<Expression<Func<Category, bool>>>(),
                Arg.Any<CancellationToken>());

    }

    private static CreateCategoryCommand SomeCreateCategoryCommand()
    {
        var command = new CreateCategoryCommand("New Title", 1);
        return command;
    }

}