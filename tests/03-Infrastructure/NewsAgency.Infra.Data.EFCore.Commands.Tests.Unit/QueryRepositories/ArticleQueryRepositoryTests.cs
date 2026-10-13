using FluentAssertions;
using NewsAgency.Domain.Tests.Unit.Builders;
using NewsAgency.Infra.Data.EFCore.Commands.Repositories;

namespace NewsAgency.Infra.Data.EFCore.Commands.Tests.Unit.QueryRepositories;

public class ArticleQueryRepositoryTests
{
    private readonly ArticleQueryRepository _queryRepository;
    private readonly ArticleCommandRepository _commandRepository;
    private readonly ArticleTestBuilder _articleBuilder;
    public ArticleQueryRepositoryTests()
    {
        _queryRepository = new ArticleQueryRepository();
        _commandRepository = new ArticleCommandRepository();
        _articleBuilder = new ArticleTestBuilder();
    }

    [Fact]
    public void GetAll_Should_Return_List_Of_Articles()
    {

        //act
        var articles = _queryRepository.GetAllAsync();


        //assert
        articles.Should().HaveCountGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void Should_Get_An_Article_By_Id()
    {
        //arrange
        int id = 5;
        var article = _articleBuilder.WithId(id).Build();
        _commandRepository.Create(article);

        //act
        var actual = _queryRepository.GetByAsync(id);


        //assert
        actual.Should().Be(article);

    }

    [Fact]
    public void Should_Return_Null_When_Id_Does_Not_Exists()
    {
        //arrange
        int id = 67;

        //act
        var actual = _queryRepository.GetByAsync(id);


        //assert
        actual.Should().BeNull();

    }
}

