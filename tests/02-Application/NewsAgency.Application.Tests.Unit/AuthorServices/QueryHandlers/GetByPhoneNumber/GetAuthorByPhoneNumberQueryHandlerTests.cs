using FluentAssertions;
using NewsAgency.Application.AuthorServices.QueryHandlers.GetByPhoneNumber;
using NewsAgency.Application.Contracts.Author.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.AuthorAgg.Repositories;
using NewsAgency.Domain.AuthorAgg.ValueObjects;
using NewsAgency.Domain.Tests.Unit.Builders;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace NewsAgency.Application.Tests.Unit.AuthorServices.QueryHandlers.GetByPhoneNumber;

public class GetAuthorByPhoneNumberQueryHandlerTests
{
    private readonly IAuthorQueryRepository _queryRepository;
    private readonly GetAuthorByPhoneNumberQueryHandler _queryHandler;
    private readonly AuthorTestBuilder _builder;

    public GetAuthorByPhoneNumberQueryHandlerTests()
    {
        _queryRepository = Substitute.For<IAuthorQueryRepository>();
        _queryHandler = new GetAuthorByPhoneNumberQueryHandler(_queryRepository);
        _builder = new AuthorTestBuilder();
    }
    [Fact]
    public async Task Should_Return_Success_Result_And_Author_When_PhoneNumber_Passed_Properly()
    {
        //arrange
        string PhoneNumber = "00989389966885";
        var author = _builder.WithPhoneNumber(new PhoneNumber(PhoneNumber)).Build();
        _queryRepository.GetByPhoneNumberAsync(PhoneNumber, Arg.Any<CancellationToken>()).Returns(author);


        //act
        var result = await _queryHandler.Handle(new GetAuthorByPhoneNumberQuery(PhoneNumber), CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        await _queryRepository.Received(1).GetByPhoneNumberAsync(PhoneNumber, Arg.Any<CancellationToken>());
        result.Data.Id.Should().Be(author.Id);
        result.Data.PhoneNumber.Should().Be(author.PhoneNumber.Value);
        result.Data.LastName.Should().Be(author.LastName.Value);
        result.Data.PhoneNumber.Should().Be(author.PhoneNumber.Value);
        result.Data.PhoneNumber.Should().Be(author.PhoneNumber.Value);
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
        await _queryRepository.DidNotReceive().GetByPhoneNumberAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Repository_Returns_Null()
    {
        //arrange
        string PhoneNumber = "TestPhoneNumber@test.test";
        _queryRepository.GetByPhoneNumberAsync(PhoneNumber, Arg.Any<CancellationToken>()).ReturnsNull();


        //act
        var result = await _queryHandler.Handle(new GetAuthorByPhoneNumberQuery(PhoneNumber), CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        result.Data.Should().BeNull();
        await _queryRepository.Received(1).GetByPhoneNumberAsync(PhoneNumber, Arg.Any<CancellationToken>());

    }
}