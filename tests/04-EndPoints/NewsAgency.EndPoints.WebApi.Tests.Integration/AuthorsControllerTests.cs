using FluentAssertions;
using NewsAgency.Application.Contracts.Author.Commands;
using NewsAgency.Application.Contracts.Author.Common;
using NewsAgency.EndPoints.WebApi.Models;
using NewsAgency.EndPoints.WebApi.Models.Authors;
using NewsAgency.EndPoints.WebApi.Tests.Integration.Infrastructure;
using RESTFulSense.Clients;

namespace NewsAgency.EndPoints.WebApi.Tests.Integration;

[Collection("IntegrationTests")]
public class AuthorsControllerTests
{
    private readonly TestDatabaseFixture _database;
    private readonly RESTFulApiFactoryClient _restClient;

    private const string _path = "api/Authors";

    public AuthorsControllerTests(TestDatabaseFixture database)
    {
        _database = database;
        _restClient = database.RestClient;
    }

    [Fact]
    public async Task Should_Return_All_Authors()
    {
        // Arrange
        await _database.ResetAsync();

        var command = CreateValidAuthorCommand();

        var created = await _restClient.PostContentAsync<
            CreateAuthorCommand,
            ApiResponse<long>>(
                _path,
                command);

        // Act
        var actual = await _restClient.GetContentAsync<
            ApiResponse<List<ApiResource<AuthorViewModel>>>>(_path);

        // Assert
        actual.IsSuccess.Should().BeTrue();

        actual.Data.Should().NotBeNull();

        actual.Data.Should().ContainSingle(author =>
            author.Data.Id == created.Data);
    }

    [Fact]
    public async Task Should_Create_New_Author()
    {
        // Arrange
        await _database.ResetAsync();

        var command = CreateValidAuthorCommand();

        // Act
        var actual = await _restClient.PostContentAsync<
            CreateAuthorCommand,
            ApiResponse<long>>(
                _path,
                command);

        var author = await _restClient.GetContentAsync<
            ApiResponse<AuthorViewModel>>(
                $"{_path}/{actual.Data}");

        // Assert
        actual.IsSuccess.Should().BeTrue();

        author.IsSuccess.Should().BeTrue();

        author.Data.Id.Should().Be(actual.Data);

        author.Data.FirstName.Should().Be(command.FirstName);

        author.Data.LastName.Should().Be(command.LastName);

        author.Data.Email.Should().Be(command.Email);

        author.Data.PhoneNumber.Should().Be(command.PhoneNumber);
    }

    [Fact]
    public async Task Should_Get_An_Author_By_Id()
    {
        //arrange
        await _database.ResetAsync();

        var command = CreateValidAuthorCommand();

        var created = await _restClient.PostContentAsync<
            CreateAuthorCommand,
            ApiResponse<long>>(
            _path,
            command);

        //act
        var actual = await _restClient.GetContentAsync<
            ApiResponse<AuthorViewModel>>(
            $"{_path}/{created.Data}");

        //assert
        actual.IsSuccess.Should().BeTrue();

        actual.Data.Id.Should().Be(created.Data);

        actual.Data.FirstName.Should().Be(command.FirstName);

        actual.Data.LastName.Should().Be(command.LastName);

        actual.Data.Email.Should().Be(command.Email);

        actual.Data.PhoneNumber.Should().Be(command.PhoneNumber);
    }

    [Fact]
    public async Task Should_Get_An_Author_By_FirstName()
    {
        //arrange
        await _database.ResetAsync();

        var command = CreateValidAuthorCommand();

        var created = await _restClient.PostContentAsync<
            CreateAuthorCommand,
            ApiResponse<long>>(
            _path,
            command);

        //act
        var actual = await _restClient.GetContentAsync<
            ApiResponse<AuthorViewModel>>(
            $"{_path}/by-first-name?firstName={command.FirstName}");

        //assert
        actual.IsSuccess.Should().BeTrue();

        actual.Data.Id.Should().Be(created.Data);

        actual.Data.FirstName.Should().Be(command.FirstName);

        actual.Data.LastName.Should().Be(command.LastName);

        actual.Data.Email.Should().Be(command.Email);

        actual.Data.PhoneNumber.Should().Be(command.PhoneNumber);
    }

    [Fact]
    public async Task Should_Get_An_Author_By_LastName()
    {
        //arrange
        await _database.ResetAsync();

        var command = CreateValidAuthorCommand();

        var created = await _restClient.PostContentAsync<
            CreateAuthorCommand,
            ApiResponse<long>>(
            _path,
            command);

        //act
        var actual = await _restClient.GetContentAsync<
            ApiResponse<AuthorViewModel>>(
            $"{_path}/by-last-name?lastName={command.LastName}");

        //assert
        actual.IsSuccess.Should().BeTrue();

        actual.Data.Id.Should().Be(created.Data);

        actual.Data.FirstName.Should().Be(command.FirstName);

        actual.Data.LastName.Should().Be(command.LastName);

        actual.Data.Email.Should().Be(command.Email);

        actual.Data.PhoneNumber.Should().Be(command.PhoneNumber);
    }

    [Fact]
    public async Task Should_Get_An_Author_By_Email()
    {
        //arrange
        await _database.ResetAsync();

        var command = CreateValidAuthorCommand();

        var created = await _restClient.PostContentAsync<
            CreateAuthorCommand,
            ApiResponse<long>>(
            _path,
            command);

        //act
        var actual = await _restClient.GetContentAsync<
            ApiResponse<AuthorViewModel>>(
            $"{_path}/by-email?email={command.Email}");

        //assert
        actual.IsSuccess.Should().BeTrue();

        actual.Data.Id.Should().Be(created.Data);

        actual.Data.FirstName.Should().Be(command.FirstName);

        actual.Data.LastName.Should().Be(command.LastName);

        actual.Data.Email.Should().Be(command.Email);

        actual.Data.PhoneNumber.Should().Be(command.PhoneNumber);
    }

    [Fact]
    public async Task Should_Get_An_Author_By_PhoneNumber()
    {
        //arrange
        await _database.ResetAsync();

        var command = CreateValidAuthorCommand();

        var created = await _restClient.PostContentAsync<
            CreateAuthorCommand,
            ApiResponse<long>>(
            _path,
            command);

        //act
        var actual = await _restClient.GetContentAsync<
            ApiResponse<AuthorViewModel>>(
            $"{_path}/by-phone-number?phonenumber={command.PhoneNumber}");

        //assert
        actual.IsSuccess.Should().BeTrue();

        actual.Data.Id.Should().Be(created.Data);

        actual.Data.FirstName.Should().Be(command.FirstName);

        actual.Data.LastName.Should().Be(command.LastName);

        actual.Data.Email.Should().Be(command.Email);

        actual.Data.PhoneNumber.Should().Be(command.PhoneNumber);
    }

    [Fact]
    public async Task Should_Change_FirstName()
    {
        //arrange
        await _database.ResetAsync();

        var command = CreateValidAuthorCommand();

        var created = await _restClient.PostContentAsync<
            CreateAuthorCommand,
            ApiResponse<long>>(
            _path,
            command);

        var request = new ChangeAuthorFirstNameRequest
        {
            FirstName = "Reza"
        };

        //act
        var actual = await _restClient.PutContentAsync<
            ChangeAuthorFirstNameRequest,
            ApiResponse<AuthorViewModel>>(
            $"{_path}/{created.Data}/first-name",
            request);

        //assert
        actual.IsSuccess.Should().BeTrue();

        actual.Data.Id.Should().Be(created.Data);

        actual.Data.FirstName.Should().Be(request.FirstName);

        actual.Data.LastName.Should().Be(command.LastName);

        actual.Data.Email.Should().Be(command.Email);

        actual.Data.PhoneNumber.Should().Be(command.PhoneNumber);
    }

    [Fact]
    public async Task Should_Change_LastName()
    {
        //arrange
        await _database.ResetAsync();

        var command = CreateValidAuthorCommand();

        var created = await _restClient.PostContentAsync<
            CreateAuthorCommand,
            ApiResponse<long>>(
            _path,
            command);

        var request = new ChangeAuthorLastNameRequest
        {
            LastName = "Ahmadi"
        };

        //act
        var actual = await _restClient.PutContentAsync<
            ChangeAuthorLastNameRequest,
            ApiResponse<AuthorViewModel>>(
            $"{_path}/{created.Data}/last-name",
            request);

        //assert
        actual.IsSuccess.Should().BeTrue();

        actual.Data.Id.Should().Be(created.Data);

        actual.Data.FirstName.Should().Be(command.FirstName);

        actual.Data.LastName.Should().Be(request.LastName);

        actual.Data.Email.Should().Be(command.Email);

        actual.Data.PhoneNumber.Should().Be(command.PhoneNumber);
    }

    [Fact]
    public async Task Should_Change_Email()
    {
        //arrange
        await _database.ResetAsync();

        var command = CreateValidAuthorCommand();

        var created = await _restClient.PostContentAsync<
            CreateAuthorCommand,
            ApiResponse<long>>(
            _path,
            command);

        var request = new ChangeAuthorEmailRequest()
        {
            Email = "rezaahmadi@gmail.com"
        };

        //act
        var actual = await _restClient.PutContentAsync<
            ChangeAuthorEmailRequest,
            ApiResponse<AuthorViewModel>>(
            $"{_path}/{created.Data}/email",
            request);

        //assert
        actual.IsSuccess.Should().BeTrue();

        actual.Data.Id.Should().Be(created.Data);

        actual.Data.FirstName.Should().Be(command.FirstName);

        actual.Data.LastName.Should().Be(command.LastName);

        actual.Data.Email.Should().Be(request.Email);

        actual.Data.PhoneNumber.Should().Be(command.PhoneNumber);
    }

    [Fact]
    public async Task Should_Change_PhoneNumber()
    {
        //arrange
        await _database.ResetAsync();

        var command = CreateValidAuthorCommand();

        var created = await _restClient.PostContentAsync<
            CreateAuthorCommand,
            ApiResponse<long>>(
            _path,
            command);

        var request = new ChangeAuthorPhoneNumberRequest()
        {
            PhoneNumber = "00989635874112"
        };

        //act
        var actual = await _restClient.PutContentAsync<
            ChangeAuthorPhoneNumberRequest,
            ApiResponse<AuthorViewModel>>(
            $"{_path}/{created.Data}/phone-number",
            request);

        //assert
        actual.IsSuccess.Should().BeTrue();

        actual.Data.Id.Should().Be(created.Data);

        actual.Data.FirstName.Should().Be(command.FirstName);

        actual.Data.LastName.Should().Be(command.LastName);

        actual.Data.Email.Should().Be(command.Email);

        actual.Data.PhoneNumber.Should().Be(request.PhoneNumber);
    }

    private static CreateAuthorCommand CreateValidAuthorCommand()
    {
        return new CreateAuthorCommand(
            "Mahdi",
            "Nemati",
            "Mahdin@gmail.com",
            "0098956574574");
    }
}