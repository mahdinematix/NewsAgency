using FluentAssertions;
using NewsAgency.Domain.Tests.Unit.Builders;
using NewsAgency.Infra.Data.EFCore.Commands.Repositories;

namespace NewsAgency.Infra.Data.EFCore.Commands.Tests.Unit.CommandRepositories;

public class ArticleCommandRepositoryTests
{
    private readonly ArticleTestBuilder _articleBuilder;
    private readonly ArticleCommandRepository _commandRepository;
    private readonly ArticleQueryRepository _queryRepository;

    public ArticleCommandRepositoryTests()
    {
        _commandRepository = new ArticleCommandRepository();
        _queryRepository = new ArticleQueryRepository();
        _articleBuilder = new ArticleTestBuilder();
    }
    [Fact]
    public void Create_Should_Add_An_Article_Into_Database()
    {
        //arrange
        var article = _articleBuilder.Build();
        


        //act
        _commandRepository.Create(article);

        //assert
        _commandRepository.Articles.Should().ContainEquivalentOf(article);
    }

    [Fact]
    public void Should_Delete_Article_By_Id()
    {
        //arrange
        long id = 6;
        var article = _articleBuilder.Build();
        _commandRepository.Create(article);

        //act
        _commandRepository.Delete(id);
        var actual = _queryRepository.GetAllAsync();

        //assert
        actual.Should().NotContain(article);
    }


}

