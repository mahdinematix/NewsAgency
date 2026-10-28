using FluentAssertions;
using NewsAgency.Application.AuthorServices.QueryHandlers.GetById;
using NewsAgency.Application.Contracts.Author.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.AuthorAgg.Repositories;
using NewsAgency.Domain.Tests.Unit.Factories;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace NewsAgency.Application.Tests.Unit.AuthorServices.QueryHandlers.GetById;
    public class GetAuthorByIdQueryHandlerTests
    {
        private readonly IAuthorQueryRepository _queryRepository;
        private readonly GetAuthorByIdQueryHandler _queryHandler;
        private readonly AuthorTestBuilder _builder;

        public GetAuthorByIdQueryHandlerTests()
        {
            _queryRepository = Substitute.For<IAuthorQueryRepository>();
            _queryHandler = new GetAuthorByIdQueryHandler(_queryRepository);
            _builder = new AuthorTestBuilder();
        }


        [Fact]
        public async Task Should_Return_Success_Result_And_Author_When_Pass_Title()
        {
            // Arrange
            var author = _builder.Build();

            _queryRepository.GetByAsync(author.Id, Arg.Any<CancellationToken>()).Returns(author);

            // Act
            var result = await _queryHandler.Handle(new GetAuthorByIdQuery(author.Id), CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
        result.Data.Id.Should().Be(author.Id);
        result.Data.FirstName.Should().Be(author.FirstName.Value);
        result.Data.LastName.Should().Be(author.LastName.Value);
        result.Data.PhoneNumber.Should().Be(author.PhoneNumber.Value);
        result.Data.Email.Should().Be(author.Email.Value);
        await _queryRepository.Received(1).GetByAsync(Arg.Any<long>(), Arg.Any<CancellationToken>());

        }

        [Fact]
        public async Task Should_Return_Failed_Result_When_Request_Is_Null()
        {
            //act
            var result = await _queryHandler.Handle(null, CancellationToken.None);

            //assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.Validation);
        result.Data.Should().BeNull();
            await _queryRepository.DidNotReceive().GetByAsync(Arg.Any<long>(), Arg.Any<CancellationToken>());

        }

        [Fact]
        public async Task Should_Return_Failed_Result_When_Repository_Returns_Null()
        {
            //arrange
            var author = _builder.Build();
            _queryRepository.GetByAsync(author.Id, Arg.Any<CancellationToken>()).ReturnsNull();


            //act
            var result = await _queryHandler.Handle(new GetAuthorByIdQuery(author.Id), CancellationToken.None);

            //assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        result.Data.Should().BeNull();
            await _queryRepository.Received(1).GetByAsync(Arg.Any<long>(), Arg.Any<CancellationToken>());

        }

    }
