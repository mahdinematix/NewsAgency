using FluentAssertions;
using NewsAgency.Application.AuthorServices.QueryHandlers.GetByEmail;
using NewsAgency.Application.Contracts.Author.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.AuthorAgg.Repositories;
using NewsAgency.Domain.AuthorAgg.ValueObjects;
using NewsAgency.Domain.Tests.Unit.Factories;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace NewsAgency.Application.Tests.Unit.AuthorServices.QueryHandlers.GetByEmail;

public class GetAuthorByEmailQueryHandlerTests
{
    private readonly IAuthorQueryRepository _queryRepository;
    private readonly GetAuthorByEmailQueryHandler _queryHandler;
    private readonly AuthorTestBuilder _builder;

    public GetAuthorByEmailQueryHandlerTests()
    {
        _queryRepository = Substitute.For<IAuthorQueryRepository>();
        _queryHandler = new GetAuthorByEmailQueryHandler(_queryRepository);
        _builder = new AuthorTestBuilder();
    }
    [Fact]
    public async Task Should_Return_Success_Result_And_Author_When_Email_Passed_Properly()
    {
        //arrange
        string email = "TestEmail@test.test";
        var author = _builder.WithEmail(new Email(email)).Build();
        _queryRepository.GetByEmailAsync(email, Arg.Any<CancellationToken>()).Returns(author);


        //act
        var result = await _queryHandler.Handle(new GetAuthorByEmailQuery(email), CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        await _queryRepository.Received(1).GetByEmailAsync(email, Arg.Any<CancellationToken>());
        result.Data.Id.Should().Be(author.Id);
        result.Data.Email.Should().Be(author.Email.Value);
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
        await _queryRepository.DidNotReceive().GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Repository_Returns_Null()
    {
        //arrange
        string Email = "TestEmail@test.test";
        _queryRepository.GetByEmailAsync(Email, Arg.Any<CancellationToken>()).ReturnsNull();


        //act
        var result = await _queryHandler.Handle(new GetAuthorByEmailQuery(Email), CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        result.Data.Should().BeNull();
        await _queryRepository.Received(1).GetByEmailAsync(Email, Arg.Any<CancellationToken>());

    }
}