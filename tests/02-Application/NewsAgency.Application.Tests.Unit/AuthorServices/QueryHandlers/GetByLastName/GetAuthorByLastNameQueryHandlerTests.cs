using FluentAssertions;
using NewsAgency.Application.AuthorServices.QueryHandlers.GetByLastName;
using NewsAgency.Application.Contracts.Author.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.AuthorAgg.Repositories;
using NewsAgency.Domain.AuthorAgg.ValueObjects;
using NewsAgency.Domain.Tests.Unit.Builders;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace NewsAgency.Application.Tests.Unit.AuthorServices.QueryHandlers.GetByLastName;

public class GetAuthorByLastNameQueryHandlerTests
{
    private readonly IAuthorQueryRepository _queryRepository;
    private readonly GetAuthorByLastNameQueryHandler _queryHandler;
    private readonly AuthorTestBuilder _builder;

    public GetAuthorByLastNameQueryHandlerTests()
    {
        _queryRepository = Substitute.For<IAuthorQueryRepository>();
        _queryHandler = new GetAuthorByLastNameQueryHandler(_queryRepository);
        _builder = new AuthorTestBuilder();
    }
    [Fact]
    public async Task Should_Return_Success_Result_And_Author_When_LastName_Passed_Properly()
    {
        //arrange
        string lastName = "Test LastName";
        var author = _builder.WithLastName(new LastName(lastName)).Build();
        _queryRepository.GetByLastNameAsync(lastName, Arg.Any<CancellationToken>()).Returns(author);


        //act
        var result = await _queryHandler.Handle(new GetAuthorByLastNameQuery(lastName), CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        await _queryRepository.Received(1).GetByLastNameAsync(lastName, Arg.Any<CancellationToken>());
        result.Data.Id.Should().Be(author.Id);
        result.Data.FirstName.Should().Be(author.FirstName.Value);
        result.Data.LastName.Should().Be(author.LastName.Value);
        result.Data.PhoneNumber.Should().Be(author.PhoneNumber.Value);
        result.Data.Email.Should().Be(author.Email.Value);
    }


    [Fact]
    public async Task Should_Return_Failed_Result_When_Request_Is_Null()
    {
        //act
        var result = await _queryHandler.Handle(null!, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.Validation);
        result.Data.Should().BeNull();
        await _queryRepository.DidNotReceive().GetByLastNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Repository_Returns_Null()
    {
        //arrange
        string lastname = "Test LastName";
        _queryRepository.GetByLastNameAsync(lastname, Arg.Any<CancellationToken>()).ReturnsNull();


        //act
        var result = await _queryHandler.Handle(new GetAuthorByLastNameQuery(lastname), CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        result.Data.Should().BeNull();
        await _queryRepository.Received(1).GetByLastNameAsync(lastname, Arg.Any<CancellationToken>());

    }
}

