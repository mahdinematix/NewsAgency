using NewsAgency.Domain.AuthorAgg.Entities;
using NewsAgency.Domain.AuthorAgg.Repositories;

namespace NewsAgency.Infra.Data.EFCore.CommandRepositories;

public class AuthorCommandRepository : GenericCommandRepository<Author, long> , IAuthorCommandRepository
{
    public AuthorCommandRepository(NewsAgencyContext context):base(context)
    {
    }
}

